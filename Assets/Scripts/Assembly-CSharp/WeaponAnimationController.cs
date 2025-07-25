using System;
using System.Collections;
using System.Linq;
using RootMotion.FinalIK;
using UnityEngine;

public class WeaponAnimationController : BaseAnimationController
{
	public enum Parameter
	{
		IsFiring = 0,
		IsReloading = 1,
		IsCharging = 2,
		IsExtended = 3,
		Emote = 4,
		Idle = 5,
		Stunned = 6,
		ShootSpeed = 7,
		ReloadSpeed = 8
	}

	public enum BackpackOp
	{
		Standard = 0,
		OpenForReload = 1,
		OpenWhileActive = 2
	}

	public enum PlaysOn
	{
		WeaponOnly = 0,
		BodyOnly = 1,
		Both = 2
	}

	[SerializeField]
	private BackpackOp _backpackOperation;

	[SerializeField]
	private bool _opensBackpackOnReload;

	[SerializeField]
	private PlaysOn _playsOn;

	[SerializeField]
	private float _shootSpeedOverride;

	[SerializeField]
	private FiringWeaponBodyAnimationData _bodyAnimation;

	private PlayerAnimationController.Parameter _bodyFiringParameter;

	private PlayerAnimationController.Parameter _bodyChargingParameter;

	private AimIK _aimIKController;

	private LimbIK _limbIKController;

	private Coroutine _ikAimingTransitionCoroutine;

	private Coroutine _ikPositioningTransitionCoroutine;

	private float _shootSpeed = 1f;

	private float _reloadSpeed = 1f;

	private float _initialPositionerPositionWeight;

	private float _initialPositionerRotationWeight;

	private bool _isExtended;

	private bool _isReloading;

	public bool IsReloading
	{
		set
		{
			if (_isReloading != value)
			{
				_isReloading = value;
				SetBool(Parameter.IsReloading, value);
				if (HasBodyAnimation)
				{
					PlayerAnimationController.SetBool(PlayerAnimationController.Parameter.WeaponReloading, value);
				}
				this._reloadingStateChanged?.Invoke(value);
			}
		}
	}

	public bool IsFiring
	{
		set
		{
			SetBool(Parameter.IsFiring, value);
			if (HasBodyAnimation)
			{
				PlayerAnimationController.Parameter parameter = PlayerAnimationController.Parameter.WeaponFiring;
				if (_bodyAnimation.Section == PlayerAnimationController.SectionLayer.FullBody)
				{
					parameter = ((_bodyAnimation.WeaponType == ItemType.primaryWeapon) ? PlayerAnimationController.Parameter.FullBodyPrimaryWeaponFiring : PlayerAnimationController.Parameter.FullBodySecondaryWeaponFiring);
				}
				else if (_bodyAnimation.Section == PlayerAnimationController.SectionLayer.UpperBody)
				{
					parameter = ((_bodyAnimation.WeaponType == ItemType.primaryWeapon) ? PlayerAnimationController.Parameter.UpperBodyPrimaryWeaponFiring : PlayerAnimationController.Parameter.UpperBodySecondaryWeaponFiring);
				}
				PlayerAnimationController.SetBool(parameter, value);
			}
		}
	}

	public bool IsCharging
	{
		set
		{
			SetBool(Parameter.IsCharging, value);
			if (HasBodyAnimation)
			{
				PlayerAnimationController.Parameter parameter = PlayerAnimationController.Parameter.WeaponCharging;
				if (_bodyAnimation.Section == PlayerAnimationController.SectionLayer.FullBody)
				{
					parameter = ((_bodyAnimation.WeaponType == ItemType.primaryWeapon) ? PlayerAnimationController.Parameter.FullBodyPrimaryWeaponCharging : PlayerAnimationController.Parameter.FullBodySecondaryWeaponCharging);
				}
				else if (_bodyAnimation.Section == PlayerAnimationController.SectionLayer.UpperBody)
				{
					parameter = ((_bodyAnimation.WeaponType == ItemType.primaryWeapon) ? PlayerAnimationController.Parameter.UpperBodyPrimaryWeaponCharging : PlayerAnimationController.Parameter.UpperBodySecondaryWeaponCharging);
				}
				PlayerAnimationController.SetBool(parameter, value);
			}
		}
	}

	public bool IsExtended
	{
		get
		{
			return _isExtended;
		}
		set
		{
			_isExtended = value;
			SetBool(Parameter.IsExtended, value);
			if (HasBodyAnimation)
			{
				if (_bodyAnimation.Section == PlayerAnimationController.SectionLayer.LeftArm || _bodyAnimation.Section == PlayerAnimationController.SectionLayer.RightArm)
				{
					PlayerAnimationController.SetArmAnimActiveState(value, _bodyAnimation.Section);
				}
				else if (_bodyAnimation.Section == PlayerAnimationController.SectionLayer.HandGun)
				{
					PlayerAnimationController.SetHandGunAnimActiveState(value);
				}
				if (!value)
				{
					IsReloading = false;
					IsFiring = false;
					IsCharging = false;
				}
			}
			this._extendedStateChanged?.Invoke(value);
		}
	}

	public BackpackOp BackpackOperation => _backpackOperation;

	public bool OpensBackPackOnReload => _opensBackpackOnReload;

	public bool HasBodyAnimation
	{
		get
		{
			if (_playsOn != PlaysOn.WeaponOnly)
			{
				return _bodyAnimation != null;
			}
			return false;
		}
	}

	public FiringWeaponBodyAnimationData BodyAnimation => _bodyAnimation;

	public PlayerAnimationController PlayerAnimationController { get; private set; }

	private event Action<bool> _extendedStateChanged;

	public event Action<bool> ExtendedStateChanged
	{
		add
		{
			_extendedStateChanged += value;
		}
		remove
		{
			_extendedStateChanged -= value;
		}
	}

	private event Action<bool> _reloadingStateChanged;

	public event Action<bool> ReloadingStateChanged
	{
		add
		{
			_reloadingStateChanged += value;
		}
		remove
		{
			_reloadingStateChanged -= value;
		}
	}

	public void SetHandGunAnimActiveAfterState(int layer, string stateName)
	{
		if (_bodyAnimation.Section == PlayerAnimationController.SectionLayer.HandGun)
		{
			StartCoroutine(CorSetHandGunAnimActiveAfterState(layer, stateName));
		}
	}

	public IEnumerator CorSetHandGunAnimActiveAfterState(int layer, string stateName)
	{
		float maxTimeCheck = 3f;
		do
		{
			bool flag = false;
			if (PlayerAnimationController.GetCurrentAnimatorStateInfo(layer).IsName(stateName))
			{
				flag = true;
			}
			if (!flag)
			{
				break;
			}
			yield return new WaitForSeconds(0.1f);
			maxTimeCheck -= 0.1f;
		}
		while (!(maxTimeCheck <= 0f));
		yield return null;
		PlayerAnimationController.SetHandGunAnimActiveState(extended: true);
	}

	protected override void Awake()
	{
		base.Awake();
		_aimIKController = GetComponentInChildren<AimIK>();
		_limbIKController = GetComponent<LimbIK>();
		IPlayerController componentInParent = GetComponentInParent<IPlayerController>();
		if (componentInParent == null)
		{
			Debug.LogError("WeaponAnimationController playerController = null");
		}
		else
		{
			PlayerAnimationController = componentInParent.AnimationController;
		}
		base.gameObject.SetActive(value: false);
	}

	private void OnEnable()
	{
		SetFloat(Parameter.ShootSpeed, _shootSpeed);
		SetFloat(Parameter.ReloadSpeed, _reloadSpeed);
	}

	public void SetProperties(WeaponProfile profile)
	{
		if (_animator == null)
		{
			CacheAnimator();
		}
		if (_playsOn != PlaysOn.WeaponOnly)
		{
			SetupBodyAnimations();
			_bodyFiringParameter = GetFiringAnimParam(_bodyAnimation.WeaponType, _bodyAnimation.Section);
			_bodyChargingParameter = GetChargingAnimParam(_bodyAnimation.WeaponType, _bodyAnimation.Section);
		}
		if (_shootSpeedOverride > 0f)
		{
			_shootSpeed = _shootSpeedOverride;
		}
		else
		{
			AnimationClip animationClip = _animator.runtimeAnimatorController.animationClips.FirstOrDefault((AnimationClip c) => c.name == "Shoot");
			if (animationClip != null)
			{
				_shootSpeed = animationClip.length / profile.Cooldown;
			}
		}
		SetFloat(Parameter.ShootSpeed, _shootSpeed);
		AnimationClip animationClip2 = _animator.runtimeAnimatorController.animationClips.FirstOrDefault((AnimationClip c) => c.name == "Reload");
		if (animationClip2 != null)
		{
			_reloadSpeed = animationClip2.length / (profile.Reload.Time - profile.Reload.Delay);
		}
		SetFloat(Parameter.ReloadSpeed, _reloadSpeed);
	}

	public void SetAimTarget(Transform target)
	{
		if (_aimIKController != null)
		{
			_aimIKController.solver.target = target;
		}
	}

	public void SetPositioner(Transform target)
	{
		if (_limbIKController != null)
		{
			_limbIKController.solver.target = target;
			_initialPositionerPositionWeight = _limbIKController.solver.IKPositionWeight;
			_initialPositionerRotationWeight = _limbIKController.solver.IKRotationWeight;
			_limbIKController.solver.IKPositionWeight = 0f;
			_limbIKController.solver.IKRotationWeight = 0f;
		}
	}

	public void SetAnimationStates(WeaponState state)
	{
		SetAnimationStates(state.IsFiring, state.IsReloading, state.FireResults.IsCharging);
	}

	public void SetAnimationStates(bool isFiring, bool isReloading, bool isCharging)
	{
		if (_playsOn != PlaysOn.BodyOnly)
		{
			SetBool(Parameter.IsFiring, isFiring);
			SetBool(Parameter.IsReloading, isReloading);
			SetBool(Parameter.IsCharging, isCharging);
		}
		if (_playsOn != PlaysOn.WeaponOnly)
		{
			if (_bodyAnimation.Section == PlayerAnimationController.SectionLayer.LeftArm || _bodyAnimation.Section == PlayerAnimationController.SectionLayer.RightArm || _bodyAnimation.Section == PlayerAnimationController.SectionLayer.HandGun)
			{
				PlayerAnimationController.SetBool(PlayerAnimationController.Parameter.WeaponFiring, isFiring);
				PlayerAnimationController.SetBool(PlayerAnimationController.Parameter.WeaponReloading, isReloading);
				PlayerAnimationController.SetBool(PlayerAnimationController.Parameter.WeaponCharging, isCharging);
			}
			else
			{
				PlayerAnimationController.SetBool(_bodyFiringParameter, isFiring);
			}
		}
	}

	public void EnableAiming()
	{
		if (_aimIKController != null && _aimIKController.solver.IKPositionWeight < 1f)
		{
			TransitionAiming(0f, 1f);
		}
		if (_limbIKController != null && _limbIKController.solver.IKPositionWeight < _initialPositionerPositionWeight)
		{
			TransitionPositioning(0f, _initialPositionerPositionWeight, 0f, _initialPositionerRotationWeight);
		}
	}

	public void DisableAiming()
	{
		if ((bool)_aimIKController && _aimIKController.solver.IKPositionWeight > 0f)
		{
			TransitionAiming(1f, 0f);
		}
		if (_limbIKController != null && _limbIKController.solver.IKPositionWeight > 0f)
		{
			TransitionPositioning(_initialPositionerPositionWeight, 0f, _initialPositionerRotationWeight, 0f);
		}
	}

	private void SetupBodyAnimations()
	{
		if (_playsOn != PlaysOn.WeaponOnly)
		{
			PlayerAnimationController.SetFiringWeaponBodyAnimations(_bodyAnimation);
		}
	}

	private void TransitionAiming(float start, float end, float duration = 0.125f)
	{
		if (_ikAimingTransitionCoroutine != null)
		{
			StopCoroutine(_ikAimingTransitionCoroutine);
		}
		_ikAimingTransitionCoroutine = StartCoroutine(TransitionAimingRoutine(start, end, duration));
	}

	private IEnumerator TransitionAimingRoutine(float start, float end, float duration = 0.125f)
	{
		yield return this.PerformOverDuration(duration, onTick, smooth: true);
		void onTick(float t)
		{
			_aimIKController.solver.IKPositionWeight = Mathf.Lerp(start, end, t);
		}
	}

	private void TransitionPositioning(float pStart, float pEnd, float rStart, float rEnd, float duration = 0.125f)
	{
		if (_ikPositioningTransitionCoroutine != null)
		{
			StopCoroutine(_ikPositioningTransitionCoroutine);
		}
		_ikPositioningTransitionCoroutine = StartCoroutine(TransitionPositioningRoutine(pStart, pEnd, rStart, rEnd, duration));
	}

	private IEnumerator TransitionPositioningRoutine(float pStart, float pEnd, float rStart, float rEnd, float duration = 0.125f)
	{
		yield return this.PerformOverDuration(duration, onTick, smooth: true);
		void onTick(float t)
		{
			_limbIKController.solver.IKPositionWeight = Mathf.Lerp(pStart, pEnd, t);
			_limbIKController.solver.IKRotationWeight = Mathf.Lerp(rStart, rEnd, t);
		}
	}

	private PlayerAnimationController.Parameter GetFiringAnimParam(ItemType itemType, PlayerAnimationController.SectionLayer section)
	{
		switch (itemType)
		{
		case ItemType.primaryWeapon:
			switch (section)
			{
			case PlayerAnimationController.SectionLayer.FullBody:
				return PlayerAnimationController.Parameter.FullBodyPrimaryWeaponFiring;
			case PlayerAnimationController.SectionLayer.UpperBody:
				return PlayerAnimationController.Parameter.UpperBodyPrimaryWeaponFiring;
			}
			break;
		case ItemType.secondaryWeapon:
			switch (section)
			{
			case PlayerAnimationController.SectionLayer.FullBody:
				return PlayerAnimationController.Parameter.FullBodySecondaryWeaponFiring;
			case PlayerAnimationController.SectionLayer.UpperBody:
				return PlayerAnimationController.Parameter.UpperBodySecondaryWeaponFiring;
			}
			break;
		}
		return PlayerAnimationController.Parameter.WeaponFiring;
	}

	private PlayerAnimationController.Parameter GetChargingAnimParam(ItemType itemType, PlayerAnimationController.SectionLayer section)
	{
		switch (itemType)
		{
		case ItemType.primaryWeapon:
			switch (section)
			{
			case PlayerAnimationController.SectionLayer.FullBody:
				return PlayerAnimationController.Parameter.FullBodyPrimaryWeaponCharging;
			case PlayerAnimationController.SectionLayer.UpperBody:
				return PlayerAnimationController.Parameter.UpperBodyPrimaryWeaponCharging;
			}
			break;
		case ItemType.secondaryWeapon:
			switch (section)
			{
			case PlayerAnimationController.SectionLayer.FullBody:
				return PlayerAnimationController.Parameter.FullBodySecondaryWeaponCharging;
			case PlayerAnimationController.SectionLayer.UpperBody:
				return PlayerAnimationController.Parameter.UpperBodySecondaryWeaponCharging;
			}
			break;
		}
		return PlayerAnimationController.Parameter.WeaponCharging;
	}
}
