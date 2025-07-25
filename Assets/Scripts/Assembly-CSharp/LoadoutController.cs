using System;
using System.Collections;
using BSCore;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class LoadoutController : MonoBehaviour
{
	[Inject]
	private ProfileManager _profileManager;

	[SerializeField]
	private Transform _outfitContainer;

	public readonly WeaponProfile[] WeaponProfiles = new WeaponProfile[4];

	private PlayerAnimationController _animationController;

	private IWeaponController _weaponController;

	private IPlayerController _playerController;

	public bool IsInitialized { get; private set; }

	public OutfitProfile OutfitProfile { get; private set; }

	public BackpackProfile BackpackProfile { get; private set; }

	public HatProfile HatProfile { get; private set; }

	public EquipmentProfile MajorEquipmentSlotProfile { get; private set; }

	public EquipmentProfile MinorEquipmentSlotProfile { get; private set; }

	public WeaponProfile MeleeWeaponProfile { get; private set; }

	public bool HasOutfit => Outfit != null;

	public bool HasMeleeWeapon => MeleeWeaponProfile != null;

	public Outfit Outfit { get; private set; }

	public Backpack Backpack
	{
		get
		{
			if (!(Outfit != null))
			{
				return null;
			}
			return Outfit.Backpack;
		}
	}

	public Hat Hat
	{
		get
		{
			if (!(Outfit != null))
			{
				return null;
			}
			return Outfit.Hat;
		}
	}

	public FiringWeapon[] Weapons
	{
		get
		{
			if (!(Outfit != null))
			{
				return null;
			}
			return Outfit.Weapons;
		}
	}

	public MeleeWeapon MeleeWeapon
	{
		get
		{
			if (!(Outfit != null))
			{
				return null;
			}
			return Outfit.MeleeWeapon;
		}
	}

	public Transform BackpackContainer
	{
		get
		{
			if (!(Outfit != null))
			{
				return null;
			}
			return Outfit.BackpackContainer;
		}
	}

	public bool MeleeWeaponHidden { get; private set; }

	private event Action<HitInfo> _hit;

	public event Action<HitInfo> Hit
	{
		add
		{
			_hit += value;
		}
		remove
		{
			_hit -= value;
		}
	}

	private event Action _outfitEquipped;

	public event Action OutfitEquipped
	{
		add
		{
			_outfitEquipped += value;
		}
		remove
		{
			_outfitEquipped -= value;
		}
	}

	protected void RaiseHit(HitInfo hitInfo)
	{
		this._hit?.Invoke(hitInfo);
	}

	protected void RaiseOutfitEquipped()
	{
		this._outfitEquipped?.Invoke();
	}

	[Inject]
	private void Construct()
	{
		IsInitialized = true;
	}

	private void Awake()
	{
		_animationController = GetComponentInParent<PlayerAnimationController>();
		_animationController.AnimationControllerUpdated += OnAnimationControllerUpdated;
		_weaponController = GetComponent<IWeaponController>();
		_playerController = GetComponent<IPlayerController>();
	}

	public void SetLoadout(Loadout loadout, Action<bool> operationCompleted)
	{
		SetLoadout(LoadoutData.FromLoadout(loadout), operationCompleted);
	}

	public void SetLoadout(LoadoutData loadoutData, Action<bool> operationCompleted)
	{
		Debug.Log("[LoadoutController] Setting loadout");
		EquipMeleeWeapon(loadoutData.MeleeWeapon, setProfileOnly: true);
		for (int i = 0; i < loadoutData.Weapons.Length; i++)
		{
			EquipWeapon(i, loadoutData.Weapons[i], setProfileOnly: true);
		}
		EquipBackpack(loadoutData.Backpack, setProfileOnly: true);
		EquipHat(loadoutData.Hat, setProfileOnly: true);
		EquipOutfit(loadoutData.Outfit, operationCompleted);
		EquipMajorEquipmentSlot(loadoutData.MajorEquipmentSlot);
		EquipMinorEquipmentSlot(loadoutData.MinorEquipmentSlot);
	}

	public void EquipOutfit(string outfitId, Action<bool> operationCompleted)
	{
		OutfitProfile = _profileManager.GetById<OutfitProfile>(outfitId);
		if (OutfitProfile == null)
		{
			Debug.LogError("[LoadoutController] Failed to get outfit profile for outfit with id - " + outfitId);
			operationCompleted?.Invoke(obj: false);
		}
		else
		{
			StartCoroutine(InstantiateOutfitRoutine(operationCompleted));
		}
	}

	private IEnumerator InstantiateOutfitRoutine(Action<bool> operationCompleted)
	{
		AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(OutfitProfile.Id, _outfitContainer);
		yield return handle;
		if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
		{
			if (Outfit != null)
			{
				if (Outfit.Profile.HeroClass != OutfitProfile.HeroClass)
				{
					Outfit.ReleaseEquippedItems();
				}
				Addressables.ReleaseInstance(Outfit.gameObject);
				Outfit = null;
			}
			Outfit = handle.Result.GetComponent<Outfit>();
			ToogleModelVisible(visible: false);
			Outfit.Profile = OutfitProfile;
			_animationController.SetOutfit(Outfit);
			_playerController.InitializeCharacter(Outfit);
			EquipMeleeWeapon(MeleeWeaponProfile?.Id);
			for (int i = 0; i < WeaponProfiles.Length; i++)
			{
				EquipWeapon(i, WeaponProfiles[i]?.Id);
			}
			EquipBackpack(BackpackProfile?.Id);
			EquipHat(HatProfile?.Id);
			RaiseOutfitEquipped();
			operationCompleted?.Invoke(obj: true);
		}
		else
		{
			Debug.LogError("[LoadoutController] Failed to instantiate outfit with id - " + OutfitProfile.Id);
			operationCompleted?.Invoke(obj: false);
		}
	}

	private void OnAnimationControllerUpdated()
	{
		ToogleModelVisible(visible: true);
	}

	private void ToogleModelVisible(bool visible)
	{
		if (Outfit != null && UIPrefabManager.sceneLoad == "MainMenu")
		{
			if (Outfit.ModelRoot != null)
			{
				Outfit.ModelRoot.gameObject.SetActive(visible);
			}
			else
			{
				Outfit.Mesh.gameObject.SetActive(visible);
			}
		}
	}

	public WeaponProfile EquipMeleeWeapon(string weaponId, bool setProfileOnly = false, Action<bool> operationCompleted = null)
	{
		Debug.Log("[LoadoutController] Equipping Melee Weapon - (" + weaponId + ")");
		MeleeWeaponProfile = _profileManager.GetById<WeaponProfile>(weaponId);
		if (setProfileOnly)
		{
			return MeleeWeaponProfile;
		}
		if (HasOutfit && (MeleeWeaponProfile == null || MeleeWeaponProfile.HeroClass == Outfit.Profile.HeroClass))
		{
			Outfit.EquipMeleeWeapon(MeleeWeaponProfile, delegate(bool didEquip)
			{
				OnMeleeWeaponEquipCompleted(didEquip, operationCompleted);
			});
		}
		return MeleeWeaponProfile;
	}

	private void OnMeleeWeaponEquipCompleted(bool didEquip, Action<bool> operationCompleted)
	{
		if (didEquip && _weaponController != null && Outfit.MeleeWeapon != null)
		{
			Outfit.MeleeWeapon.GetServerFrame = _weaponController.GetServerFrame;
			Outfit.MeleeWeapon.Hit += RaiseHit;
		}
		_weaponController.SetMeleeWeapon(OutfitProfile.HeroClass, MeleeWeaponProfile);
		operationCompleted?.Invoke(didEquip);
	}

	public void UpdateMeleeWeaponDisplayState(bool shouldDisplay)
	{
		if (MeleeWeapon != null)
		{
			MeleeWeapon.gameObject.SetActive(shouldDisplay);
			MeleeWeaponHidden = !shouldDisplay;
		}
	}

	public WeaponProfile EquipWeapon(int index, string weaponId, bool setProfileOnly = false, Action<int, bool> operationCompleted = null)
	{
		if ((string.IsNullOrEmpty(weaponId) && WeaponProfiles[index] == null) || (WeaponProfiles[index] != null && WeaponProfiles[index].Id == weaponId && HasOutfit && Weapons[index] != null))
		{
			operationCompleted?.Invoke(index, arg2: false);
			return null;
		}
		WeaponProfile byId = _profileManager.GetById<WeaponProfile>(weaponId);
		WeaponProfiles[index] = byId;
		if (setProfileOnly)
		{
			operationCompleted?.Invoke(index, arg2: true);
			return byId;
		}
		if (Outfit != null && _weaponController != null)
		{
			Outfit.EquipWeapon(byId, index, delegate(int i, bool didEquip)
			{
				OnWeaponEquipCompleted(i, didEquip, operationCompleted);
			});
		}
		return byId;
	}

	private void OnWeaponEquipCompleted(int index, bool didEquip, Action<int, bool> operationCompleted)
	{
		if (didEquip)
		{
			Outfit.Weapons[index].GetServerFrame = _weaponController.GetServerFrame;
			Outfit.Weapons[index].Hit += RaiseHit;
			operationCompleted?.Invoke(index, arg2: true);
		}
		else
		{
			operationCompleted?.Invoke(index, arg2: false);
		}
	}

	public void EquipBackpack(string backpackId, bool setProfileOnly = false)
	{
		BackpackProfile = _profileManager.GetById<BackpackProfile>(backpackId);
		if (!setProfileOnly && Outfit != null)
		{
			Outfit.EquipBackpack(BackpackProfile);
		}
	}

	public void EquipHat(string hatId, bool setProfileOnly = false)
	{
		HatProfile = _profileManager.GetById<HatProfile>(hatId);
		if (!setProfileOnly && Outfit != null)
		{
			Outfit.EquipHat(HatProfile);
		}
	}

	public void EquipMajorEquipmentSlot(string equipmentId)
	{
		MajorEquipmentSlotProfile = _profileManager.GetById<EquipmentProfile>(equipmentId);
	}

	public void EquipMinorEquipmentSlot(string equipmentId)
	{
		MinorEquipmentSlotProfile = _profileManager.GetById<EquipmentProfile>(equipmentId);
	}

	public float TryGetModifiedHealth()
	{
		float num = OutfitProfile.HeroClassProfile.Health;
		if (MajorEquipmentSlotProfile != null && MajorEquipmentSlotProfile.Type == EquipmentType.Health)
		{
			num = MajorEquipmentSlotProfile.GetMajorModifiedValue(num);
		}
		if (MinorEquipmentSlotProfile != null && MinorEquipmentSlotProfile.Type == EquipmentType.Health)
		{
			num = MinorEquipmentSlotProfile.GetMinorModifiedValue(num);
		}
		return num;
	}

	private void OnDestroy()
	{
		_animationController.AnimationControllerUpdated -= OnAnimationControllerUpdated;
		ReleaseAll();
	}

	private void ReleaseAll()
	{
		if (Outfit != null)
		{
			Outfit.ReleaseEquippedItems();
			Addressables.ReleaseInstance(Outfit.gameObject);
		}
	}
}
