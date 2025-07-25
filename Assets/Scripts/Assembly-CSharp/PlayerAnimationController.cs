using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class PlayerAnimationController : BaseAnimationController
{
	public enum Parameter
	{
		Speed = 0,
		StrafeSpeed = 1,
		VerticalSpeed = 2,
		TurnSpeed = 3,
		Hugged = 4,
		BreakAway = 5,
		IsDead = 6,
		Win = 7,
		Lose = 8,
		Respawn = 9,
		Emote = 10,
		SpawnDive = 12,
		SpawnLandStart = 13,
		SpawnLandFinish = 14,
		MeleeSpeedMultiplier = 15,
		Blink = 16,
		IdleSpecial1 = 17,
		IdleSpecial2 = 18,
		IdleSpecial3 = 19,
		LookAtOutfit = 20,
		FullBodyPrimaryWeaponFiring = 21,
		FullBodySecondaryWeaponFiring = 22,
		UseMeleeStanding = 23,
		UseMeleeForward = 24,
		UseMeleeLeft = 25,
		UseMeleeRight = 26,
		UseMeleeBackward = 27,
		UseMeleeDownward = 28,
		UseMeleeDownwardAttack = 29,
		MeleeInCooldown = 30,
		LeftArmPrepares = 31,
		RightArmPrepares = 32,
		WeaponFiring = 33,
		WeaponCharging = 34,
		WeaponReloading = 35,
		WeaponExtended = 36,
		UpperBodyPrimaryWeaponFiring = 37,
		UpperBodySecondaryWeaponFiring = 38,
		FullBodyPrimaryWeaponCharging = 39,
		FullBodySecondaryWeaponCharging = 40,
		UpperBodyPrimaryWeaponCharging = 41,
		UpperBodySecondaryWeaponCharging = 42,
		MeleeExtended = 43
	}

	public enum SectionLayer
	{
		FullBody = 0,
		UpperBody = 1,
		LeftArm = 2,
		RightArm = 3,
		HandGun = 4
	}

	public enum EBattleAnimatorLayer
	{
		BaseLayer = 0,
		WeaponLayer = 1,
		LeftArmLayer = 2,
		RightArmLayer = 3,
		HeadLeftLayer = 4,
		HeadRightLayer = 5,
		HandGunLayer = 6
	}

	public enum EMainMenuAnimatorLayer
	{
		BaseLayer = 0,
		Eyes = 1,
		HandGunLayer = 2
	}

	private LookAtIK _ikController;

	private Coroutine _ikLookAtTransitionCoroutine;

	private Coroutine _layerWeightTransitionCoroutine;

	private bool _applyRootMotionCache;

	private bool _lookAtIKDisabled;

	private Coroutine _armLayerWeightTransitionCoroutine;

	private Coroutine _handGunLayerWeightTransitionCoroutine;

	private readonly Dictionary<string, AnimationClip> _weaponBodyAnimationOverrides = new Dictionary<string, AnimationClip>();

	public float Speed
	{
		set
		{
			SetFloat(Parameter.Speed, value);
		}
	}

	public float StrafeSpeed
	{
		set
		{
			SetFloat(Parameter.StrafeSpeed, value);
		}
	}

	public bool Hugged
	{
		set
		{
			SetBool(Parameter.Hugged, value);
		}
	}

	public bool IsDead
	{
		set
		{
			SetBool(Parameter.IsDead, value);
		}
	}

	public AnimatorCullingMode CullingMode
	{
		set
		{
			_animator.cullingMode = value;
		}
	}

	public bool ApplyRootMotion
	{
		set
		{
			_applyRootMotionCache = value;
			if (_animator != null)
			{
				_animator.applyRootMotion = _applyRootMotionCache;
			}
		}
	}

	private event Action _animationControllerUpdated;

	public event Action AnimationControllerUpdated
	{
		add
		{
			_animationControllerUpdated += value;
		}
		remove
		{
			_animationControllerUpdated -= value;
		}
	}

	private event Action<bool> _upperBodyAnimationStateUpdated;

	public event Action<bool> UpperBodyAnimationStateUpdated
	{
		add
		{
			_upperBodyAnimationStateUpdated += value;
		}
		remove
		{
			_upperBodyAnimationStateUpdated -= value;
		}
	}

	protected void RaiseAnimationControllerUpdated()
	{
		if (_animator != null)
		{
			this._animationControllerUpdated?.Invoke();
		}
	}

	public void RaiseUpperBodyAnimationStateUpdated(bool isActive)
	{
		this._upperBodyAnimationStateUpdated?.Invoke(isActive);
	}

	protected override void Awake()
	{
		base.Awake();
		_ikController = GetComponentInChildren<LookAtIK>();
	}

	public AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layer)
	{
		return _animator.GetCurrentAnimatorClipInfo(layer);
	}

	public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layer)
	{
		return _animator.GetCurrentAnimatorStateInfo(layer);
	}

	public void SetOutfit(Outfit outfit)
	{
		_ikController = outfit.IKController;
		_animator = outfit.GetComponent<Animator>();
		if (_animator == null)
		{
			Debug.LogError("[PlayerAnimationController] Failed to get Animator Component on gameobject - " + outfit.name);
			return;
		}
		Debug.Log("[PlayerAnimationController] Got Animator for Outfit: " + outfit.Profile.Name);
		_animator.applyRootMotion = _applyRootMotionCache;
		_weaponBodyAnimationOverrides.Clear();
	}

	public void SetAnimationOverride(string clipName, AnimationClip newOverrideClip)
	{
		if (_weaponBodyAnimationOverrides.ContainsKey(clipName))
		{
			if (newOverrideClip != _weaponBodyAnimationOverrides[clipName])
			{
				_weaponBodyAnimationOverrides[clipName] = newOverrideClip;
			}
		}
		else
		{
			_weaponBodyAnimationOverrides.Add(clipName, newOverrideClip);
		}
		if (_animator != null)
		{
			StartCoroutine(AnimationOverride(clipName, newOverrideClip));
		}
	}

	private IEnumerator AnimationOverride(string clipName, AnimationClip newOverrideClip)
	{
		yield return new MaxWaitUntil(() => _animator != null && _animator.runtimeAnimatorController != null, 5f);
		if (!(_animator != null) || !(_animator.runtimeAnimatorController != null))
		{
			yield break;
		}
		AnimationClip[] animationClips = _animator.runtimeAnimatorController.animationClips;
		AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
		List<KeyValuePair<AnimationClip, AnimationClip>> list = new List<KeyValuePair<AnimationClip, AnimationClip>>();
		for (int num = 0; num < animationClips.Length; num++)
		{
			AnimationClip animationClip = animatorOverrideController.animationClips[num];
			AnimationClip value = animationClips[num];
			if (animationClip.name == clipName)
			{
				value = newOverrideClip;
			}
			list.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, value));
		}
		animatorOverrideController.ApplyOverrides(list);
		_animator.runtimeAnimatorController = animatorOverrideController;
	}

	public void EnableLayers(int[] layerIndexes)
	{
		TransitionLayerWeight(layerIndexes, 0f, 1f);
	}

	public void DisableLayers(int[] layerIndexes, float duration = 0.125f)
	{
		TransitionLayerWeight(layerIndexes, 1f, 0f, duration);
	}

	public float[] GetArrLayerWeight(int[] layerIndexes)
	{
		float[] array = new float[layerIndexes.Length];
		for (int i = 0; i < layerIndexes.Length; i++)
		{
			array[i] = _animator.GetLayerWeight(layerIndexes[i]);
		}
		return array;
	}

	public void EnableLayer(int layerIndex)
	{
		TransitionLayerWeight(new int[1] { layerIndex }, 0f, 1f);
	}

	public void DisableLayer(int layerIndex)
	{
		TransitionLayerWeight(new int[1] { layerIndex }, 1f, 0f);
	}

	public void DisableLookAt()
	{
		if (_ikController != null && _ikController.solver.IKPositionWeight > 0f)
		{
			TransitionIKLookAt(1f, 0f);
		}
	}

	public void EnableLookAt()
	{
		if (_ikController != null && _ikController.solver.IKPositionWeight <= 0f)
		{
			TransitionIKLookAt(0f, 1f);
		}
	}

	public void SetMeleeWeapon(HeroClass heroClass, WeaponProfile weaponProfile)
	{
		string weaponType = weaponProfile?.AnimationType ?? "noWeapon";
		AnimatorOverrideController animatorOverrideController = AnimationOverrideManager.Request(heroClass.ToString(), weaponType);
		_animator.runtimeAnimatorController = animatorOverrideController;
		AnimationClip[] animationClips = animatorOverrideController.animationClips;
		foreach (AnimationClip animationClip in animationClips)
		{
			switch (animationClip.name)
			{
			case "BackwardAttack":
				SetMeleeSpeedParam("MeleeBackwardSpeed", animationClip, weaponProfile.Melee.Backward);
				break;
			case "RightAttack":
				SetMeleeSpeedParam("MeleeRightSpeed", animationClip, weaponProfile.Melee.Right);
				break;
			case "LeftAttack":
				SetMeleeSpeedParam("MeleeLeftSpeed", animationClip, weaponProfile.Melee.Left);
				break;
			case "ForwardAttack":
				SetMeleeSpeedParam("MeleeForwardSpeed", animationClip, weaponProfile.Melee.Forward);
				break;
			case "StandingAttack":
				SetMeleeSpeedParam("MeleeStandingSpeed", animationClip, weaponProfile.Melee.Standing);
				break;
			}
		}
		RaiseAnimationControllerUpdated();
		foreach (KeyValuePair<string, AnimationClip> weaponBodyAnimationOverride in _weaponBodyAnimationOverrides)
		{
			SetAnimationOverride(weaponBodyAnimationOverride.Key, weaponBodyAnimationOverride.Value);
		}
	}

	public void SetFiringWeaponBodyAnimations(FiringWeaponBodyAnimationData data)
	{
		if (data != null)
		{
			switch (data.Section)
			{
			case SectionLayer.LeftArm:
			case SectionLayer.RightArm:
				SetArmAnimation(data);
				break;
			case SectionLayer.UpperBody:
				SetUpperBodyAnimation(data);
				break;
			case SectionLayer.HandGun:
				SetHandGunAnimation(data);
				break;
			default:
				SetFullBodyAnimation(data);
				break;
			}
		}
	}

	private void SetFullBodyAnimation(FiringWeaponBodyAnimationData data)
	{
		if (data.WeaponType == ItemType.primaryWeapon)
		{
			SetAnimationOverride("FullBodyPrimaryWeaponCharge", data.Prepare);
			SetAnimationOverride("FullBodyPrimaryWeaponFire", data.Shoot);
		}
		else
		{
			SetAnimationOverride("FullBodySecondaryWeaponCharge", data.Prepare);
			SetAnimationOverride("FullBodySecondaryWeapon", data.Shoot);
		}
	}

	private void SetUpperBodyAnimation(FiringWeaponBodyAnimationData data)
	{
		if (data.WeaponType == ItemType.primaryWeapon)
		{
			SetAnimationOverride("UpperBodyPrimaryWeaponCharge", data.Prepare);
			SetAnimationOverride("UpperBodyPrimaryWeaponFire", data.Shoot);
		}
		else
		{
			SetAnimationOverride("UpperBodySecondaryWeaponCharge", data.Prepare);
			SetAnimationOverride("UpperBodySecondaryWeaponFire", data.Shoot);
		}
	}

	private void SetArmAnimation(FiringWeaponBodyAnimationData data)
	{
		string text = ((data.Section == SectionLayer.LeftArm) ? "Left" : "Right");
		Parameter parameter = ((data.Section == SectionLayer.LeftArm) ? Parameter.LeftArmPrepares : Parameter.RightArmPrepares);
		SetBool(parameter, data.PrepareBeforeShoot);
		SetAnimationOverride(text + "ArmIdle", data.Idle);
		SetAnimationOverride(text + "ArmPrepare", data.Prepare);
		SetAnimationOverride(text + "ArmShoot", data.Shoot);
		SetAnimationOverride(text + "ArmReload", data.Reload);
		SetAnimationOverride(text + "ArmExtend", data.Extend);
		SetAnimationOverride(text + "ArmRetract", data.Retract);
		if (data.IncludeHead)
		{
			SetAnimationOverride("Head" + text + "ArmIdle", data.Idle);
			SetAnimationOverride("Head" + text + "ArmPrepare", data.Prepare);
			SetAnimationOverride("Head" + text + "ArmShoot", data.Shoot);
			SetAnimationOverride("Head" + text + "ArmReload", data.Reload);
			SetAnimationOverride("Head" + text + "ArmExtend", data.Extend);
			SetAnimationOverride("Head" + text + "ArmRetract", data.Retract);
		}
	}

	private void SetHandGunAnimation(FiringWeaponBodyAnimationData data)
	{
		SetAnimationOverride("HandGunIdle", data.Idle);
		SetAnimationOverride("HandGunPrepare", data.Prepare);
		SetAnimationOverride("HandGunShoot", data.Shoot);
		SetAnimationOverride("HandGunReload", data.Reload);
		SetAnimationOverride("HandGunExtend", data.Extend);
		SetAnimationOverride("HandGunRetract", data.Retract);
		SetAnimationOverride("HandGunBackward", data.Backward);
		SetAnimationOverride("HandGunForward", data.Forward);
	}

	public void SetArmAnimActiveState(bool extended, SectionLayer arm)
	{
		if (extended)
		{
			SetBool(Parameter.WeaponExtended, value: true);
			TransitionArmLayerWeight(new int[2]
			{
				(int)arm,
				(int)(arm + 2)
			}, 0f, 1f);
		}
		else
		{
			SetBool(Parameter.WeaponExtended, value: false);
			TransitionArmLayerWeight(new int[2]
			{
				(int)arm,
				(int)(arm + 2)
			}, 1f, 0f, 0.125f, 0.7f);
		}
	}

	public void SetMeleeExtend(bool extended)
	{
		SetTrigger(Parameter.MeleeExtended);
	}

	public bool GetMeleeExtend()
	{
		return GetBool(Parameter.MeleeExtended);
	}

	public void SetHandGunAnimActiveState(bool extended)
	{
		if (extended)
		{
			SetBool(Parameter.WeaponExtended, value: true);
			if (UIPrefabManager.IsMainMenuScene())
			{
				float layerWeight = _animator.GetLayerWeight(2);
				TransitionHandGunLayerWeight(new int[1] { 2 }, layerWeight, 1f);
			}
			else
			{
				float layerWeight2 = _animator.GetLayerWeight(6);
				TransitionHandGunLayerWeight(new int[1] { 6 }, layerWeight2, 1f);
			}
		}
		else
		{
			SetBool(Parameter.WeaponExtended, value: false);
			if (UIPrefabManager.IsMainMenuScene())
			{
				float layerWeight3 = _animator.GetLayerWeight(2);
				TransitionHandGunLayerWeight(new int[1] { 2 }, layerWeight3, 0f, 0.125f, 0.32f);
			}
			else
			{
				float layerWeight4 = _animator.GetLayerWeight(6);
				TransitionHandGunLayerWeight(new int[1] { 6 }, layerWeight4, 0f, 0.125f, 0.32f);
			}
		}
	}

	public void SetWeaponExtended(bool value)
	{
		SetBool(Parameter.WeaponExtended, value);
	}

	private void SetMeleeSpeedParam(string paramName, AnimationClip clip, WeaponProfile.MeleeOptionData meleeOption)
	{
		if (meleeOption.AnimationSpeedMultiplier > 0f)
		{
			_animator.SetFloat(paramName, meleeOption.AnimationSpeedMultiplier);
		}
	}

	public void SetAnimationStates(PlayerMotorState state)
	{
		SetFloat(Parameter.Speed, state.Speed);
		SetFloat(Parameter.StrafeSpeed, state.StrafeSpeed);
		if (state.IsGrounded)
		{
			SetFloat(Parameter.VerticalSpeed, 0f);
		}
		else
		{
			SetFloat(Parameter.VerticalSpeed, state.Velocity.y);
		}
		SetFloat(Parameter.TurnSpeed, state.TurnSpeed);
	}

	public float GetEmoteLength()
	{
		AnimationClip[] animationClips = _animator.runtimeAnimatorController.animationClips;
		foreach (AnimationClip animationClip in animationClips)
		{
			if (animationClip.name.StartsWith("Emote"))
			{
				return animationClip.length;
			}
		}
		return 2f;
	}

	public void Dive()
	{
		SetTrigger(Parameter.SpawnDive);
	}

	public void StartDiveLand()
	{
		SetTrigger(Parameter.SpawnLandStart);
	}

	public void FinishDiveLand()
	{
		SetTrigger(Parameter.SpawnLandFinish);
	}

	public void SetMenuLookAtByRotation(float angle)
	{
		if (!_lookAtIKDisabled)
		{
			float num = Mathf.Abs(angle);
			if (num > 90f)
			{
				_ikController.solver.IKPositionWeight = Mathf.InverseLerp(135f, 90f, num);
			}
			else if (_ikController.solver.IKPositionWeight < 1f)
			{
				_ikController.solver.IKPositionWeight = 1f;
			}
		}
	}

	public void StopCoroutineHandGunTransitionLayerWeight()
	{
		if (_handGunLayerWeightTransitionCoroutine != null)
		{
			StopCoroutine(_handGunLayerWeightTransitionCoroutine);
		}
	}

	private void TransitionIKLookAt(float start, float end, float duration = 0.125f)
	{
		if (_ikLookAtTransitionCoroutine != null)
		{
			StopCoroutine(_ikLookAtTransitionCoroutine);
		}
		_ikLookAtTransitionCoroutine = StartCoroutine(TransitionIKLookAtRoutine(start, end, duration));
	}

	private IEnumerator TransitionIKLookAtRoutine(float start, float end, float duration = 0.125f)
	{
		yield return this.PerformOverDuration(duration, onTick, smooth: true);
		_lookAtIKDisabled = end < start;
		void onTick(float t)
		{
			_ikController.solver.IKPositionWeight = Mathf.Lerp(start, end, t);
		}
	}

	private void TransitionLayerWeight(int[] layerIndexes, float start, float end, float duration = 0.125f, float delay = 0f)
	{
		if (_layerWeightTransitionCoroutine != null)
		{
			StopCoroutine(_layerWeightTransitionCoroutine);
		}
		_layerWeightTransitionCoroutine = StartCoroutine(TransitionLayerWeightRoutine(layerIndexes, start, end, duration, delay));
	}

	private void TransitionArmLayerWeight(int[] layerIndexes, float start, float end, float duration = 0.125f, float delay = 0f)
	{
		if (_armLayerWeightTransitionCoroutine != null)
		{
			StopCoroutine(_armLayerWeightTransitionCoroutine);
		}
		_armLayerWeightTransitionCoroutine = StartCoroutine(TransitionLayerWeightRoutine(layerIndexes, start, end, duration, delay));
	}

	private void TransitionHandGunLayerWeight(int[] layerIndexes, float start, float end, float duration = 0.125f, float delay = 0f)
	{
		if (_handGunLayerWeightTransitionCoroutine != null)
		{
			StopCoroutine(_handGunLayerWeightTransitionCoroutine);
		}
		_handGunLayerWeightTransitionCoroutine = StartCoroutine(TransitionLayerWeightRoutine(layerIndexes, start, end, duration, delay));
	}

	private IEnumerator TransitionLayerWeightRoutine(int[] layerIndexes, float start, float end, float duration = 0.125f, float delay = 0f)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		yield return this.PerformOverDuration(duration, onTick, smooth: true);
		void onTick(float t)
		{
			int[] array = layerIndexes;
			foreach (int layerIndex in array)
			{
				_animator.SetLayerWeight(layerIndex, Mathf.Lerp(start, end, t));
			}
		}
	}

	public void TransitionLayerWeight(int[] layerIndexes, float[] end, float duration = 0.125f, float delay = 0f)
	{
		if (layerIndexes != null && end != null && layerIndexes.Length == end.Length)
		{
			float[] arrLayerWeight = GetArrLayerWeight(layerIndexes);
			StartCoroutine(TransitionLayerWeightRoutine(layerIndexes, arrLayerWeight, end, duration, delay));
		}
	}

	private IEnumerator TransitionLayerWeightRoutine(int[] layerIndexes, float[] start, float[] end, float duration = 0.125f, float delay = 0f)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		yield return this.PerformOverDuration(duration, onTick, smooth: true);
		void onTick(float t)
		{
			for (int i = 0; i < layerIndexes.Length; i++)
			{
				float a = ((i >= 0 && i < start.Length) ? start[i] : 0f);
				float b = ((i >= 0 && i < end.Length) ? end[i] : 0f);
				_animator.SetLayerWeight(layerIndexes[i], Mathf.Lerp(a, b, t));
			}
		}
	}
}
