using System;
using System.Collections;
using BSCore;
using RootMotion.FinalIK;
using UnityEngine;
using Zenject;

public class MenuLoadoutManager
{
	private UserManager _userManager;

	private LoadoutData _currentLoadout = new LoadoutData
	{
		Weapons = new string[4]
	};

	private PlayerProfile _playerProfile;

	public LoadoutData CurrentLoadout => _playerProfile.LoadoutManager.GetEquippedLoadout();

	public HeroClass CurrentHeroClass => _playerProfile.LoadoutManager.EquippedHeroClass;

	private event Action _currentLoadoutChanged;

	public event Action CurrentLoadoutChanged
	{
		add
		{
			_currentLoadoutChanged += value;
		}
		remove
		{
			_currentLoadoutChanged -= value;
		}
	}

	[Inject]
	public MenuLoadoutManager(UserManager userManager)
	{
		_userManager = userManager;
		_playerProfile = _userManager.CurrentUser;
		_currentLoadout = CurrentLoadout;
	}

	public void SaveLoadoutToServer()
	{
		_playerProfile.LoadoutManager.SetLoadout(_currentLoadout);
		_playerProfile.LoadoutManager.SaveLoadouts();
	}

	public void SetCurrentLoadout()
	{
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			if (MonoBehaviourSingleton<GearUpProcessingOverlayController>.IsInstantiated)
			{
				MonoBehaviourSingleton<GearUpProcessingOverlayController>.Instance.SetOverlayState(isActive: true);
			}
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController.SetLoadout(_currentLoadout, OnLoadoutSet);
		}
	}

	private void OnLoadoutSet(bool didSet)
	{
		for (int i = 0; i < _currentLoadout.Weapons.Length; i++)
		{
			if (!string.IsNullOrEmpty(_currentLoadout.Weapons[i]))
			{
				MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.StartCoroutine(DeployWeaponWhenReady(i));
				break;
			}
		}
		SetOutfitLookAt();
	}

	private IEnumerator DeployWeaponWhenReady(int index)
	{
		yield return new WaitUntil(() => MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController.Weapons[index] != null);
		MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.WeaponHandler.DeployWeapon(index);
		if (MonoBehaviourSingleton<GearUpProcessingOverlayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<GearUpProcessingOverlayController>.Instance.SetOverlayState(isActive: false);
		}
	}

	public void EquipItem(ItemType itemType, string itemId)
	{
		switch (itemType)
		{
		case ItemType.outfit:
			SetOutfit(itemId);
			break;
		case ItemType.primaryWeapon:
			SetWeapon(0, itemId);
			break;
		case ItemType.secondaryWeapon:
			SetWeapon(1, itemId);
			break;
		case ItemType.meleeWeapon:
			SetMeleeWeapon(itemId);
			break;
		case ItemType.backpack:
			SetBackpack(itemId);
			break;
		case ItemType.hat:
			SetHat(itemId);
			break;
		default:
			Debug.LogError("No Case to equip item type: " + itemType);
			break;
		}
	}

	public void EquipEquipmentInMajorSlot(string equipmentId)
	{
		_currentLoadout.MajorEquipmentSlot = equipmentId;
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController.EquipMajorEquipmentSlot(equipmentId);
		}
	}

	public void EquipEquipmentInMinorSlot(string equipmentId)
	{
		_currentLoadout.MinorEquipmentSlot = equipmentId;
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController.EquipMinorEquipmentSlot(equipmentId);
		}
	}

	public void DisplayMeleeWeapon()
	{
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.WeaponHandler.DrawMeleeWeapon();
		}
	}

	public void HideMeleeWeapon()
	{
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.WeaponHandler.SheatheMeleeWeapon();
		}
	}

	public void DisableHandGunLayer()
	{
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.WeaponHandler.DisableHandGunLayer();
		}
	}

	public void PresentBack()
	{
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.PresentBack();
		}
	}

	public void PresentFront()
	{
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.PresentFront();
		}
	}

	private void SetOutfit(string outfitId)
	{
		_currentLoadout.Outfit = outfitId;
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController.SetLoadout(_currentLoadout, SetupAfterLoadoutOutfitReady);
		}
	}

	private void SetupAfterLoadoutOutfitReady(bool didSet)
	{
		MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.StartCoroutine(WaitForFullOutfitLoad());
	}

	private IEnumerator WaitForFullOutfitLoad()
	{
		LoadoutController lc = MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController;
		yield return new WaitUntil(() => lc.Weapons[0] != null && lc.Weapons[0].Profile != null && lc.Weapons[0].Profile.Id == _currentLoadout.Weapons[0] && lc.Weapons[1] != null && lc.Weapons[1].Profile != null && lc.Weapons[1].Profile.Id == _currentLoadout.Weapons[1] && lc.HasMeleeWeapon && lc.MeleeWeapon.Profile != null && lc.MeleeWeapon.Profile.Id == _currentLoadout.MeleeWeapon);
		SetOutfitLookAt();
		HideMeleeWeapon();
		DisableHandGunLayer();
		if (MonoBehaviourSingleton<GearUpProcessingOverlayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<GearUpProcessingOverlayController>.Instance.SetOverlayState(isActive: false);
		}
	}

	private void SetOutfitLookAt()
	{
		LookAtIK componentInChildren = MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController.GetComponentInChildren<LookAtIK>();
		if (componentInChildren != null)
		{
			componentInChildren.solver.target = Camera.main.transform;
			componentInChildren.solver.IKPositionWeight = 1f;
			componentInChildren.solver.bodyWeight = 0f;
			componentInChildren.solver.headWeight = 0.75f;
			componentInChildren.solver.eyesWeight = 0.6f;
		}
	}

	private void SetMeleeWeapon(string weaponId)
	{
		_currentLoadout.MeleeWeapon = weaponId;
		if (!MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			return;
		}
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.WeaponHandler.HasActiveMelee)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.WeaponHandler.SheatheMeleeWeapon(delegate
			{
				MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController.EquipMeleeWeapon(weaponId, setProfileOnly: false, MeleeWeaponEquipCompleted);
			});
		}
		else
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController.EquipMeleeWeapon(weaponId, setProfileOnly: false, MeleeWeaponEquipCompleted);
		}
	}

	private void MeleeWeaponEquipCompleted(bool didSet)
	{
		MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.WeaponHandler.DrawMeleeWeapon();
	}

	private void SetWeapon(int index, string weaponId)
	{
		_currentLoadout.Weapons[index] = weaponId;
		if (!MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			return;
		}
		if (MonoBehaviourSingleton<GearUpProcessingOverlayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<GearUpProcessingOverlayController>.Instance.SetOverlayState(isActive: true);
		}
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.WeaponHandler.HasActiveWeapon)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.WeaponHandler.StowWeapon(delegate
			{
				MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController.EquipWeapon(index, weaponId, setProfileOnly: false, WeaponEquipCompleted);
			});
		}
		else
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController.EquipWeapon(index, weaponId, setProfileOnly: false, WeaponEquipCompleted);
		}
	}

	private void WeaponEquipCompleted(int index, bool didSet)
	{
		MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.WeaponHandler.DeployWeapon(index);
		if (MonoBehaviourSingleton<GearUpProcessingOverlayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<GearUpProcessingOverlayController>.Instance.SetOverlayState(isActive: false);
		}
	}

	private void SetBackpack(string backpackId)
	{
		_currentLoadout.Backpack = backpackId;
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController.EquipBackpack(backpackId);
		}
	}

	private void SetHat(string hatId)
	{
		_currentLoadout.Hat = hatId;
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController.EquipHat(hatId);
		}
	}

	public void SetHeroClass(HeroClass heroClass)
	{
		if (heroClass == CurrentHeroClass)
		{
			return;
		}
		_playerProfile.LoadoutManager.SetLoadout(_currentLoadout);
		_playerProfile.LoadoutManager.SetEquippedHeroClass(heroClass);
		_currentLoadout = CurrentLoadout;
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			if (MonoBehaviourSingleton<GearUpProcessingOverlayController>.IsInstantiated)
			{
				MonoBehaviourSingleton<GearUpProcessingOverlayController>.Instance.SetOverlayState(isActive: true);
			}
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController.SetLoadout(_currentLoadout, SetupAfterLoadoutOutfitReady);
		}
		this._currentLoadoutChanged?.Invoke();
	}

	public void SetHeroClassSlot(LoadoutSlot loadoutSlot)
	{
		_playerProfile.LoadoutManager.SetLoadout(_currentLoadout);
		_playerProfile.LoadoutManager.SetEquippedSlot(loadoutSlot);
		_currentLoadout = CurrentLoadout;
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			if (MonoBehaviourSingleton<GearUpProcessingOverlayController>.IsInstantiated)
			{
				MonoBehaviourSingleton<GearUpProcessingOverlayController>.Instance.SetOverlayState(isActive: true);
			}
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.LoadoutController.SetLoadout(_currentLoadout, SetupAfterLoadoutOutfitReady);
		}
		this._currentLoadoutChanged?.Invoke();
	}

	public void DeployWeapon(int index)
	{
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.WeaponHandler.DeployWeapon(index);
		}
	}

	public void StowWeapon()
	{
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.WeaponHandler.StowWeapon();
		}
	}
}
