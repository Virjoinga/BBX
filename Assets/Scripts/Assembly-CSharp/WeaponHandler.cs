using System;
using System.Collections;
using System.Collections.Generic;
using BSCore;
using UnityEngine;
using Zenject;

public class WeaponHandler : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private Transform _meleeMovementVFX;

	public Func<float> GetServerTime;

	public readonly Dictionary<int, WeaponStateAtFrame> _weaponStatesByFrame = new Dictionary<int, WeaponStateAtFrame>();

	private LoadoutController _loadoutController;

	private PlayerAnimationController _animationController;

	private IPlayerController _playerController;

	private Coroutine _meleeCoroutine;

	private Coroutine _switchMeleeCoroutine;

	private Coroutine _switchWeaponCoroutine;

	private DataStoreFloat _zoomSensitivity;

	private float _chargeValueCache;

	public bool IsUsingMelee => _meleeCoroutine != null;

	public WeaponProfile ActiveWeaponProfile
	{
		get
		{
			if (ActiveWeaponIndex < 0)
			{
				return null;
			}
			return _loadoutController.WeaponProfiles[ActiveWeaponIndex];
		}
	}

	public bool IsTransitioningWeapon { get; private set; }

	public bool IsTransitioningMelee { get; private set; }

	public bool HasActiveWeapon
	{
		get
		{
			if (ActiveWeaponIndex >= 0 && !_transitioningToWeapon && _loadoutController.HasOutfit && _loadoutController.Weapons[ActiveWeaponIndex] != null)
			{
				return ActiveWeaponProfile != null;
			}
			return false;
		}
	}

	public FiringWeapon ActiveWeapon
	{
		get
		{
			if (!HasActiveWeapon)
			{
				return null;
			}
			return _loadoutController.Weapons[ActiveWeaponIndex];
		}
	}

	public int ActiveWeaponIndex { get; private set; } = -1;

	public bool HasActiveMelee { get; private set; }

	private bool _transitioningToWeapon => _switchWeaponCoroutine != null;

	private event Action<WeaponProfile.MeleeOptionData> MeleeAnimationTriggered
	{
		add
		{
			_loadoutController.MeleeWeapon.MeleeAnimationTriggered += value;
		}
		remove
		{
			_loadoutController.MeleeWeapon.MeleeAnimationTriggered -= value;
		}
	}

	private void Awake()
	{
		_loadoutController = GetComponent<LoadoutController>();
		_animationController = GetComponent<PlayerAnimationController>();
		_playerController = GetComponentInParent<IPlayerController>();
	}

	private void Start()
	{
		_meleeMovementVFX.gameObject.SetActive(value: false);
	}

	public void SetZoomSensitivity(DataStoreFloat zoomSensitivity)
	{
		_zoomSensitivity = zoomSensitivity;
	}

	public void TrackWeaponState(int serverFrame, int trimFrame, int ammoRemaining, int nextFireTime, bool isReloading)
	{
		_weaponStatesByFrame.Add(serverFrame, new WeaponStateAtFrame(this, ActiveWeapon, ammoRemaining, nextFireTime, isReloading));
		if (_weaponStatesByFrame.ContainsKey(trimFrame))
		{
			_weaponStatesByFrame.Remove(trimFrame);
		}
	}

	public WeaponStateAtFrame WeaponStateAtFrame(int frame)
	{
		_weaponStatesByFrame.TryGetValue(frame, out var value);
		return value;
	}

	public void OnOutfitSet()
	{
		ActiveWeaponIndex = -1;
		_meleeMovementVFX.transform.localPosition = _loadoutController.Outfit.HurtBoxColliderData.Center;
		_meleeMovementVFX.transform.localScale = Vector3.one * _loadoutController.Outfit.HurtBoxColliderData.Height * 0.6f;
	}

	public void DeployWeapon(int index, Action onComplete = null)
	{
		if (_loadoutController.HasOutfit && (ActiveWeaponIndex != index || _transitioningToWeapon))
		{
			if (!IndexIsValid(index) && index >= 0)
			{
				Debug.LogError($"[LoadoutController] Tried to deploy slot {index}, but range is 0-{_loadoutController.Weapons.Length - 1} only");
			}
			if (_switchWeaponCoroutine != null)
			{
				StopCoroutine(_switchWeaponCoroutine);
			}
			_switchWeaponCoroutine = StartCoroutine(SwitchWeaponRoutine(index, onComplete));
		}
	}

	public void StowWeapon(Action onComplete = null)
	{
		if (IndexIsValid(ActiveWeaponIndex))
		{
			if (_switchWeaponCoroutine != null)
			{
				StopCoroutine(_switchWeaponCoroutine);
			}
			_switchWeaponCoroutine = StartCoroutine(SwitchWeaponRoutine(-1, onComplete));
		}
	}

	public void DrawMeleeWeapon(Action onComplete = null)
	{
		if (_switchMeleeCoroutine != null)
		{
			StopCoroutine(_switchMeleeCoroutine);
		}
		_animationController.SetMeleeWeapon(_loadoutController.OutfitProfile.HeroClass, _loadoutController.MeleeWeaponProfile);
		_switchMeleeCoroutine = StartCoroutine(SwitchMeleeRoutine(isVisible: true, onComplete));
		HasActiveMelee = true;
	}

	public void SheatheMeleeWeapon(Action onComplete = null)
	{
		if (_switchMeleeCoroutine != null)
		{
			StopCoroutine(_switchMeleeCoroutine);
		}
		_animationController.SetMeleeWeapon(_loadoutController.OutfitProfile.HeroClass, null);
		_switchMeleeCoroutine = StartCoroutine(SwitchMeleeRoutine(isVisible: false, onComplete));
		HasActiveMelee = false;
	}

	public void DisableHandGunLayer()
	{
		_animationController.DisableLayer(2);
	}

	public bool CanDeployIndex(int index)
	{
		if (index != ActiveWeaponIndex)
		{
			return IndexIsValid(index);
		}
		return false;
	}

	public bool CanStow(int index)
	{
		if (IndexIsValid(index))
		{
			return index == ActiveWeaponIndex;
		}
		return false;
	}

	public bool HandleIronSightMode(bool enableIronsight, ref float cameraVertical, ref float cameraHorizontal)
	{
		if (!HasActiveWeapon || !ActiveWeaponProfile.Zoom.CanZoom)
		{
			enableIronsight = false;
		}
		if (enableIronsight)
		{
			float value = _zoomSensitivity.Value;
			cameraVertical *= value;
			cameraHorizontal *= value;
		}
		return enableIronsight;
	}

	public void MockFire(Vector3 position, Vector3 forward, int lagFrames)
	{
		if (_loadoutController.HasOutfit && HasActiveWeapon && !_transitioningToWeapon)
		{
			ActiveWeapon.LaunchPointFire(position, ref forward, lagFrames, 0, isMock: true);
		}
	}

	public FireResults TryFire(bool fireHeld, int serverFrame, int shotCount)
	{
		if (!_loadoutController.HasOutfit || !HasActiveWeapon || _transitioningToWeapon)
		{
			return FireResults.Empty;
		}
		return ActiveWeapon.Fire(fireHeld, _loadoutController.Outfit.AimPointHandler.AimPosition, serverFrame, shotCount);
	}

	public bool TryUseMelee(bool shouldCancelFire, WeaponProfile.MeleeOptionData meleeOption, Action onMeleeComplete)
	{
		if (_loadoutController.HasMeleeWeapon && !IsUsingMelee)
		{
			if (shouldCancelFire && HasActiveWeapon)
			{
				ActiveWeapon.CancelFire();
			}
			_meleeCoroutine = StartCoroutine(UseMeleeRoutine(meleeOption, onMeleeComplete));
			return true;
		}
		return false;
	}

	public void ClearChargeValueCache()
	{
		_chargeValueCache = 0f;
	}

	public void TrySendChargeValueUpdatedSignal()
	{
		if (HasActiveWeapon && ActiveWeaponProfile.Charge.CanCharge)
		{
			ChargingWeapon chargingWeapon = ActiveWeapon as ChargingWeapon;
			if (chargingWeapon.ChargeValue != _chargeValueCache)
			{
				_signalBus.Fire(new ChargeValueUpdatedSignal(ActiveWeaponIndex, chargingWeapon.ChargeValue));
				_chargeValueCache = chargingWeapon.ChargeValue;
			}
		}
		else if (_chargeValueCache > 0f)
		{
			_chargeValueCache = 0f;
			_signalBus.Fire(new ChargeValueUpdatedSignal(-1, 0f));
		}
	}

	public WeaponProfile.MeleeOptionData GetMeleeOptionFromInput(Vector2 movementInput, Vector3 currentVelocity, bool isGrounded)
	{
		if (movementInput == Vector2.zero && currentVelocity.magnitude >= 0.5f)
		{
			Vector3 vector = base.transform.InverseTransformDirection(currentVelocity);
			movementInput = new Vector2(vector.x, vector.z);
		}
		MeleeWeapon.Direction direction = MeleeWeapon.Direction.Standing;
		Debug.Log($"[WeaponHandler] Input ({movementInput}) -> {direction}");
		return _loadoutController.MeleeWeaponProfile.Melee.GetOptionByDirection(direction);
	}

	private IEnumerator UseMeleeRoutine(WeaponProfile.MeleeOptionData meleeOption, Action onComplete)
	{
		if (meleeOption.MovementDirection != MeleeWeapon.Direction.Standing)
		{
			_meleeMovementVFX.gameObject.SetActive(value: true);
			_meleeMovementVFX.forward = meleeOption.GetVelocity(base.transform).normalized;
		}
		if (meleeOption.LiftWeapon)
		{
			_playerController.AimPointHandler.LiftWeapon();
		}
		yield return _loadoutController.MeleeWeapon.Swing(meleeOption);
		_meleeCoroutine = null;
		onComplete?.Invoke();
		if (meleeOption.LiftWeapon)
		{
			_playerController.AimPointHandler.LowerWeapon();
		}
		if (meleeOption.MovementDirection != MeleeWeapon.Direction.Standing)
		{
			_meleeMovementVFX.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator SwitchMeleeRoutine(bool isVisible, Action onComplete)
	{
		IsTransitioningMelee = true;
		FiringWeapon weapon = null;
		bool hasActiveWeapon = IndexIsValid(ActiveWeaponIndex);
		if (hasActiveWeapon)
		{
			weapon = _loadoutController.Weapons[ActiveWeaponIndex];
		}
		if (_loadoutController.Backpack != null && (!hasActiveWeapon || (weapon != null && weapon.AnimationController.BackpackOperation != WeaponAnimationController.BackpackOp.OpenWhileActive)))
		{
			_loadoutController.Backpack.AnimationController.Open(_playerController.IsLocal);
		}
		if (HasActiveMelee || (!HasActiveMelee && isVisible))
		{
			AnimatorStateInfo currentAnimatorStateInfo = _animationController.GetCurrentAnimatorStateInfo(0);
			int num = Animator.StringToHash("MeleeRetract");
			if (!currentAnimatorStateInfo.IsName("MeleeRetract"))
			{
				Debug.Log($"animatorStateInfo.shortNameHash = {currentAnimatorStateInfo.shortNameHash} - hash = {num}");
				_animationController.SetMeleeExtend(extended: true);
				_loadoutController.MeleeWeapon.IsTriggerMeleeExtended = true;
				yield return new WaitForSeconds(0.3f);
			}
			if (!isVisible)
			{
				_loadoutController.UpdateMeleeWeaponDisplayState(shouldDisplay: false);
			}
		}
		if (isVisible)
		{
			_loadoutController.UpdateMeleeWeaponDisplayState(shouldDisplay: true);
			yield return new WaitForSeconds(0.3f);
		}
		if (_loadoutController.Backpack != null && (!hasActiveWeapon || (weapon != null && weapon.AnimationController.BackpackOperation != WeaponAnimationController.BackpackOp.OpenWhileActive)))
		{
			_loadoutController.Backpack.AnimationController.Close();
		}
		IsTransitioningMelee = false;
		onComplete?.Invoke();
	}

	private IEnumerator SwitchWeaponRoutine(int deployIndex, Action onComplete)
	{
		IsTransitioningWeapon = true;
		FiringWeapon weapon = null;
		bool hasActiveWeapon = IndexIsValid(ActiveWeaponIndex);
		if (hasActiveWeapon)
		{
			weapon = _loadoutController.Weapons[ActiveWeaponIndex];
		}
		if (_loadoutController.Backpack != null && (!hasActiveWeapon || weapon.AnimationController.BackpackOperation != WeaponAnimationController.BackpackOp.OpenWhileActive))
		{
			_loadoutController.Backpack.AnimationController.Open(_playerController.IsLocal);
		}
		if (hasActiveWeapon && ActiveWeaponIndex != deployIndex)
		{
			weapon.AnimationController.IsExtended = false;
			if (weapon.GetWeaponType() == EWeaponType.HandGun)
			{
				_animationController.SetMeleeExtend(extended: true);
				_loadoutController.MeleeWeapon.IsTriggerMeleeExtended = true;
				if (IndexIsValid(deployIndex) && _loadoutController.Weapons[deployIndex].GetWeaponType() == EWeaponType.HandGun)
				{
					weapon.AnimationController.PlayerAnimationController.StopCoroutineHandGunTransitionLayerWeight();
				}
			}
			yield return new WaitForSeconds(0.22f);
			if (weapon != null)
			{
				weapon.gameObject.SetActive(value: false);
			}
		}
		else if (IsTransitioningMelee)
		{
			yield return new WaitForSeconds(0.22f);
		}
		ActiveWeaponIndex = deployIndex;
		hasActiveWeapon = IndexIsValid(ActiveWeaponIndex);
		if (hasActiveWeapon)
		{
			weapon = _loadoutController.Weapons[ActiveWeaponIndex];
			if (!UIPrefabManager.IsMainMenuScene())
			{
				weapon.gameObject.SetActive(value: true);
				weapon.AnimationController.IsExtended = true;
				weapon.AnimationController.SetProperties(weapon.Profile);
				yield return new WaitForSeconds(0.22f);
			}
			else if (weapon.AnimationController.PlayerAnimationController.GetCurrentAnimatorStateInfo(0).IsName("LookAtOutfit"))
			{
				yield return new MaxWaitUntil(() => weapon != null && !weapon.AnimationController.PlayerAnimationController.GetCurrentAnimatorStateInfo(0).IsName("LookAtOutfit"), 3f);
				if (weapon.GetWeaponType() == EWeaponType.HandGun)
				{
					weapon.gameObject.SetActive(value: true);
					weapon.AnimationController.IsExtended = true;
					weapon.AnimationController.SetProperties(weapon.Profile);
					yield return new WaitForSeconds(0.22f);
				}
				else
				{
					weapon.gameObject.SetActive(value: true);
					weapon.AnimationController.IsExtended = true;
				}
			}
			else if (weapon.GetWeaponType() == EWeaponType.HandGun)
			{
				weapon.gameObject.SetActive(value: true);
				weapon.AnimationController.IsExtended = true;
				weapon.AnimationController.SetProperties(weapon.Profile);
				yield return new WaitForSeconds(0.2f);
			}
			else
			{
				weapon.gameObject.SetActive(value: true);
				weapon.AnimationController.IsExtended = true;
				yield return new WaitForSeconds(0.22f);
			}
		}
		if (_loadoutController.Backpack != null && (!hasActiveWeapon || weapon.AnimationController.BackpackOperation != WeaponAnimationController.BackpackOp.OpenWhileActive))
		{
			_loadoutController.Backpack.AnimationController.Close();
		}
		IsTransitioningWeapon = false;
		onComplete?.Invoke();
		_switchWeaponCoroutine = null;
	}

	private bool IndexIsValid(int index)
	{
		if (index >= 0 && index < _loadoutController.WeaponProfiles.Length)
		{
			return _loadoutController.Weapons[index] != null;
		}
		return false;
	}
}
