using System;
using System.Collections;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

public class Outfit : MonoBehaviour
{
	[Serializable]
	public struct ColliderData
	{
		[Tooltip("The pivot offset of the collider/hitbox")]
		public Vector3 Center;

		[Tooltip("Capsule/Sphere Collider: The radius of the collider\nSphere type Hitbox: The radius of the sphere\nBox type Hitbox: Half the x and z values of the box's size")]
		public float Radius;

		[Tooltip("Capsule Collider: The height of the collider\nSphere Collider/Hitbox: Unused\nBox Hitbox: The y value of the box's size")]
		public float Height;
	}

	[Header("Containers")]
	[SerializeField]
	private Transform _meleeContainer;

	[SerializeField]
	private Transform _rightHandContainer;

	[SerializeField]
	[FormerlySerializedAs("_leftHandMeleeContainer")]
	private Transform _leftHandContainer;

	[SerializeField]
	private Transform _backpackContainer;

	[SerializeField]
	private Transform _hatContainer;

	[SerializeField]
	private Transform _abilityEmenationPoint;

	[Header("Weapons and Aiming")]
	[SerializeField]
	private Transform _weaponPositioner;

	[SerializeField]
	private AimPointHandler _aimPointHandler;

	[SerializeField]
	private LookAtIK _ikController;

	[Header("Physics")]
	[SerializeField]
	private ColliderData _movementColliderData;

	[SerializeField]
	private ColliderData _hurtBoxColliderData;

	[SerializeField]
	private ColliderData _headHurtBoxColliderData;

	[SerializeField]
	private BoltHitbox _proximityHitbox;

	[SerializeField]
	private BoltHitbox[] _hitboxes;

	[Header("Audio")]
	[SerializeField]
	private AudioClip _landAudio;

	[SerializeField]
	private AudioClip _walkAudio;

	[SerializeField]
	private AudioClip _emoteAudio;

	[Header("Visuals")]
	[SerializeField]
	private GameObject _modelRoot;

	[SerializeField]
	private SkinnedMeshRenderer _mesh;

	private ObjectFader _objectFader;

	protected OutfitProfile _profile;

	public readonly FiringWeapon[] Weapons = new FiringWeapon[4];

	private Coroutine _deathFadeRoutine;

	public AimPointHandler AimPointHandler => _aimPointHandler;

	public ColliderData MovementColliderData => _movementColliderData;

	public ColliderData HurtBoxColliderData => _hurtBoxColliderData;

	public ColliderData HeadHurtBoxColliderData => _headHurtBoxColliderData;

	public BoltHitbox ProximityHitbox => _proximityHitbox;

	public BoltHitbox[] Hitboxes => _hitboxes;

	public AudioClip LandAudio => _landAudio;

	public AudioClip WalkAudio => _walkAudio;

	public AudioClip EmoteAudio => _emoteAudio;

	public SkinnedMeshRenderer Mesh => _mesh;

	public GameObject ModelRoot => _modelRoot;

	public LookAtIK IKController => _ikController;

	public Transform BackpackContainer => _backpackContainer;

	public Transform HatContainer => _hatContainer;

	public Transform MeleeContainer => _meleeContainer;

	public Transform RightHandContainer => _rightHandContainer;

	public Transform LeftHandContainer => _leftHandContainer;

	public Transform AbilityEmenationPoint => _abilityEmenationPoint;

	public OutfitProfile Profile
	{
		get
		{
			return _profile;
		}
		set
		{
			_profile = value;
		}
	}

	public MeleeWeapon MeleeWeapon { get; private set; }

	public Backpack Backpack { get; private set; }

	public Hat Hat { get; private set; }

	protected virtual void Awake()
	{
		_objectFader = GetComponent<ObjectFader>();
	}

	protected virtual IEnumerator Start()
	{
		SetFadeLevel(0f);
		yield return null;
		SetFadeLevel(1f);
	}

	public virtual void ReleaseEquippedItems()
	{
		if (MeleeWeapon != null)
		{
			Addressables.ReleaseInstance(MeleeWeapon.gameObject);
			MeleeWeapon = null;
		}
		for (int i = 0; i < Weapons.Length; i++)
		{
			if (Weapons[i] != null)
			{
				Addressables.ReleaseInstance(Weapons[i].gameObject);
				Weapons[i] = null;
			}
		}
		if (Backpack != null)
		{
			Addressables.ReleaseInstance(Backpack.gameObject);
			Backpack = null;
		}
		if (Hat != null)
		{
			Addressables.ReleaseInstance(Hat.gameObject);
			Hat = null;
		}
	}

	public virtual void EquipMeleeWeapon(WeaponProfile profile, Action<bool> operationCompleted)
	{
		if (profile != null && profile.ItemType != ItemType.meleeWeapon)
		{
			operationCompleted?.Invoke(obj: false);
		}
		else if (MeleeWeapon != null && profile == null)
		{
			Addressables.ReleaseInstance(MeleeWeapon.gameObject);
			MeleeWeapon = null;
			operationCompleted?.Invoke(obj: false);
		}
		else if (profile == null)
		{
			operationCompleted?.Invoke(obj: false);
		}
		else
		{
			StartCoroutine(InstantiateMeleeWeaponRoutine(profile, operationCompleted));
		}
	}

	private IEnumerator InstantiateMeleeWeaponRoutine(WeaponProfile profile, Action<bool> operationCompleted)
	{
		AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(profile.Id, _meleeContainer);
		yield return handle;
		if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
		{
			if (MeleeWeapon != null)
			{
				Addressables.ReleaseInstance(MeleeWeapon.gameObject);
				MeleeWeapon = null;
			}
			MeleeWeapon = handle.Result.GetComponent<MeleeWeapon>();
			MeleeWeapon.Profile = profile;
			MeleeWeapon.name = profile.Id;
			MeleeWeapon.gameObject.SetActive(value: false);
			operationCompleted?.Invoke(obj: true);
		}
		else
		{
			operationCompleted?.Invoke(obj: false);
		}
	}

	public virtual void EquipWeapon(WeaponProfile profile, int index, Action<int, bool> operationCompleted)
	{
		if (index < 0 || index >= Weapons.Length)
		{
			Debug.LogError($"[Outfit] Tried to equip weapon to slot {index}, but the range is 0-{Weapons.Length - 1} only");
			operationCompleted?.Invoke(index, arg2: false);
		}
		else if (profile != null && profile.ItemType == ItemType.meleeWeapon)
		{
			operationCompleted?.Invoke(index, arg2: false);
		}
		else if (Weapons[index] != null && profile == null)
		{
			Addressables.ReleaseInstance(Weapons[index].gameObject);
			Weapons[index] = null;
			operationCompleted?.Invoke(index, arg2: false);
		}
		else if (profile == null)
		{
			operationCompleted?.Invoke(index, arg2: false);
		}
		else
		{
			StartCoroutine(InstantiateWeaponRoutine(profile, index, operationCompleted));
		}
	}

	private IEnumerator InstantiateWeaponRoutine(WeaponProfile profile, int index, Action<int, bool> operationCompleted)
	{
		Transform parent = ((profile.WeaponType == EWeaponType.HandGun.ToString()) ? _rightHandContainer : _backpackContainer);
		AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(profile.Id, parent);
		yield return handle;
		if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
		{
			if (Weapons[index] != null)
			{
				Addressables.ReleaseInstance(Weapons[index].gameObject);
				Weapons[index] = null;
			}
			FiringWeapon component = handle.Result.GetComponent<FiringWeapon>();
			component.Profile = profile;
			component.name = profile.Id;
			component.AnimationController.SetAimTarget(AimPointHandler.AimPoint);
			component.AnimationController.SetPositioner(_weaponPositioner);
			if (UIPrefabManager.IsMainMenuScene())
			{
				component.gameObject.SetActive(value: false);
			}
			Weapons[index] = component;
			operationCompleted?.Invoke(index, arg2: true);
		}
		else
		{
			operationCompleted?.Invoke(index, arg2: false);
		}
	}

	public virtual void EquipBackpack(BackpackProfile profile)
	{
		if (Backpack != null && profile == null)
		{
			Addressables.ReleaseInstance(Backpack.gameObject);
			Backpack = null;
		}
		else if (profile != null && profile.HeroClass == _profile.HeroClass)
		{
			StartCoroutine(InstantiateBackpackRoutine(profile));
		}
	}

	private IEnumerator InstantiateBackpackRoutine(BackpackProfile profile)
	{
		AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(profile.Id, _backpackContainer);
		yield return handle;
		if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
		{
			if (Backpack != null)
			{
				Addressables.ReleaseInstance(Backpack.gameObject);
				Backpack = null;
			}
			Backpack = handle.Result.GetComponent<Backpack>();
			Backpack.Profile = profile;
			Backpack.name = profile.Id;
		}
	}

	public virtual void EquipHat(HatProfile profile)
	{
		if (Hat != null && profile == null)
		{
			Addressables.ReleaseInstance(Hat.gameObject);
			Hat = null;
		}
		else if (profile != null && profile.HeroClass == _profile.HeroClass)
		{
			StartCoroutine(InstantiateHatRoutine(profile));
		}
	}

	private IEnumerator InstantiateHatRoutine(HatProfile profile)
	{
		AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(profile.Id, _hatContainer);
		yield return handle;
		if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
		{
			if (Hat != null)
			{
				Addressables.ReleaseInstance(Hat.gameObject);
				Hat = null;
			}
			Hat = handle.Result.GetComponent<Hat>();
			Hat.Profile = profile;
			Hat.name = profile.Id;
		}
	}

	public virtual void OnRespawn()
	{
		if (_deathFadeRoutine != null)
		{
			StopCoroutine(_deathFadeRoutine);
			_deathFadeRoutine = null;
		}
		_objectFader.SetFadeLevel(1f);
	}

	public virtual void OnDeath()
	{
		if (_deathFadeRoutine != null)
		{
			StopCoroutine(_deathFadeRoutine);
			_deathFadeRoutine = null;
		}
		_deathFadeRoutine = StartCoroutine(FadeAfterDeath());
	}

	public virtual void SetFadeLevel(float value)
	{
		_objectFader.SetFadeLevel(value);
	}

	private IEnumerator FadeAfterDeath()
	{
		float seconds = 3f;
		if (ClientTeamDeathMatchGameModeEntity.HasTDMGameMode)
		{
			seconds = ClientTeamDeathMatchGameModeEntity.TDMGameMode.MinRespawnTime - 1f;
		}
		yield return new WaitForSeconds(seconds);
		float step = 1f;
		for (float t = 0f; t < 1f; t += Time.deltaTime * step)
		{
			_objectFader.SetFadeLevel(Mathf.SmoothStep(1f, 0f, t));
			yield return null;
		}
		_objectFader.SetFadeLevel(0f);
	}
}
