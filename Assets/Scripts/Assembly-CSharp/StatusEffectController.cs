using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Constants;
using UnityEngine;

public class StatusEffectController : MonoBehaviour
{
	[Serializable]
	private struct StatusVisual
	{
		public Match.StatusType statusType;

		public GameObject visual;
	}

	[SerializeField]
	private StatusVisual[] _statusVisuals;

	private readonly List<BaseWeaponEffect> _activeEffects = new List<BaseWeaponEffect>();

	private MatchStateHelper _matchStateHelper;

	private IStatusAffectable _statusAffectable;

	private LoadoutController _loadoutController;

	private BaseWeaponEffect _powerupEffect;

	private Coroutine _powerupEffectRoutine;

	private void Awake()
	{
		_matchStateHelper = GetComponent<MatchStateHelper>();
		_statusAffectable = GetComponent<IStatusAffectable>();
		_loadoutController = GetComponent<LoadoutController>();
		StatusVisual[] statusVisuals = _statusVisuals;
		for (int i = 0; i < statusVisuals.Length; i++)
		{
			statusVisuals[i].visual.SetActive(value: false);
		}
	}

	public void Setup(Outfit outfit)
	{
		StatusVisual[] statusVisuals = _statusVisuals;
		for (int i = 0; i < statusVisuals.Length; i++)
		{
			BaseVisualEffect[] components = statusVisuals[i].visual.GetComponents<BaseVisualEffect>();
			for (int j = 0; j < components.Length; j++)
			{
				components[j].Setup(outfit);
			}
		}
	}

	public bool HasStatus(Match.StatusType status)
	{
		return _statusAffectable.StatusFlags.Has(status);
	}

	public void OnStatusFlagsUpdated(Match.StatusType statusFlags)
	{
		StatusVisual[] statusVisuals = _statusVisuals;
		for (int i = 0; i < statusVisuals.Length; i++)
		{
			StatusVisual statusVisual = statusVisuals[i];
			bool flag = statusFlags.Has(statusVisual.statusType);
			bool num = !statusVisual.visual.activeInHierarchy && flag;
			statusVisual.visual.SetActive(flag);
			ParticleSystem component = statusVisual.visual.GetComponent<ParticleSystem>();
			if (num && component != null)
			{
				component.Play();
			}
		}
	}

	public void KillAllEffects()
	{
		StopAllCoroutines();
		foreach (BaseWeaponEffect activeEffect in _activeEffects)
		{
			activeEffect.EndEffect();
		}
		_activeEffects.Clear();
		_statusAffectable.StatusFlags = Match.StatusType.None;
	}

	public void TryKillExistingPowerupEffect()
	{
		if (_powerupEffect != null)
		{
			Debug.Log($"[StatusEffectController] Killing Powerup Effect {_powerupEffect.Effect}");
			StopCoroutine(_powerupEffectRoutine);
			_powerupEffectRoutine = null;
			_powerupEffect.EndEffect();
			RemoveActiveEffect(_powerupEffect);
			_powerupEffect = null;
		}
	}

	public void TryCancelActiveEffectsFromWeapon(string weaponId, BoltEntity applier)
	{
		Debug.Log("[StatusEffectController] Cancelling " + applier.name + "'s effects from weapon " + weaponId);
		List<BaseWeaponEffect> list = new List<BaseWeaponEffect>();
		foreach (BaseWeaponEffect activeEffect in _activeEffects)
		{
			if (activeEffect.WeaponId == weaponId && activeEffect.Applier == applier)
			{
				list.Add(activeEffect);
			}
		}
		Debug.Log($"[StatusEffectController] Found {list.Count} effects to kill");
		foreach (BaseWeaponEffect item in list)
		{
			Debug.Log($"[StatusEffectController] Killing effect {item.Effect}");
			item.EndEffect();
			RemoveActiveEffect(item);
		}
	}

	public bool TryApplyEffect(string weaponId, WeaponProfile.EffectData[] effectDatas, BoltEntity applier, bool isPowerup = false, Action onComplete = null)
	{
		if (effectDatas != null && effectDatas.Length != 0)
		{
			foreach (WeaponProfile.EffectData effectData in effectDatas)
			{
				TryApplyEffect(weaponId, effectData, applier, isPowerup, onComplete);
			}
			return true;
		}
		return false;
	}

	public bool TryApplyEffect(string weaponId, WeaponProfile.EffectData effectData, BoltEntity applier, bool isPowerup = false, Action onComplete = null)
	{
		if (_matchStateHelper.MatchStateCached != MatchState.Active)
		{
			return false;
		}
		if (_statusAffectable.CanGetStatusApplied)
		{
			BaseWeaponEffect baseWeaponEffect = GenerateWeaponEffect(weaponId, effectData, applier);
			if (baseWeaponEffect.ReducedByTenacity)
			{
				baseWeaponEffect.TryModifyDurationForEquipment(_loadoutController);
			}
			Coroutine powerupEffectRoutine = StartCoroutine(ApplyEffect(effectData.Delay, baseWeaponEffect, onComplete, isPowerup));
			if (isPowerup)
			{
				_powerupEffectRoutine = powerupEffectRoutine;
			}
			return true;
		}
		return false;
	}

	public void ApplyEffectWhile(string weaponId, WeaponProfile.EffectData[] effectDatas, BoltEntity applier, Func<bool> conditional, Action onComplete = null)
	{
		foreach (WeaponProfile.EffectData effectData in effectDatas)
		{
			ApplyEffectWhile(weaponId, effectData, applier, conditional, onComplete);
		}
	}

	public void ApplyEffectWhile(string weaponId, WeaponProfile.EffectData effectData, BoltEntity applier, Func<bool> conditional, Action onComplete = null)
	{
		if (_matchStateHelper.MatchStateCached == MatchState.Active && _statusAffectable.CanGetStatusApplied)
		{
			StartCoroutine(ApplyEffectWhile(GenerateWeaponEffect(weaponId, effectData, applier), conditional, onComplete));
		}
	}

	private BaseWeaponEffect GenerateWeaponEffect(string weaponId, WeaponProfile.EffectData effectData, BoltEntity applier)
	{
		switch (effectData.EffectType)
		{
		case Match.EffectType.HealthChangeOverTime:
			return new HealthChangeOverTimeEffect(weaponId, effectData, _statusAffectable.entity, applier);
		case Match.EffectType.SpeedChange:
			return new SpeedChangeEffect(weaponId, effectData, _statusAffectable.entity, applier);
		case Match.EffectType.ForcedMovement:
			return new ForcedMovementEffect(weaponId, effectData, _statusAffectable.entity, applier);
		case Match.EffectType.PreventJump:
			return new PreventJumpEffect(weaponId, effectData, _statusAffectable.entity, applier);
		case Match.EffectType.SizeChange:
			return new SizeChangeEffect(weaponId, effectData, _statusAffectable.entity, applier);
		case Match.EffectType.Stun:
			return new StunEffect(weaponId, effectData, _statusAffectable.entity, applier);
		case Match.EffectType.Blind:
			return new PurelyVisualEffect(weaponId, effectData, _statusAffectable.entity, applier);
		case Match.EffectType.DamageMultiplier:
			return new DamageModifierEffect(weaponId, effectData, _statusAffectable.entity, applier);
		case Match.EffectType.MeleeDamageMultiplier:
			return new MeleeDamageModifierEffect(weaponId, effectData, _statusAffectable.entity, applier);
		case Match.EffectType.DamageShield:
			return new DamageShieldEffect(weaponId, effectData, _statusAffectable.entity, applier);
		case Match.EffectType.Hugged:
			if (_statusAffectable.entity.GetState<IPlayerState>() == null)
			{
				throw new ArgumentException("[StatusEffectController] Trying to hug a non-player. This is not allowed.");
			}
			return new HuggedEffect(weaponId, effectData, _statusAffectable.entity, applier);
		case Match.EffectType.ForcedMelee:
			return new ForcedMeleeEffect(weaponId, effectData, _statusAffectable.entity, applier);
		default:
			throw new ArgumentException($"[ConditionEffectController] Unknown EffectType {effectData.EffectType} trying to generate effect");
		}
	}

	private IEnumerator ApplyEffect(float delay, BaseWeaponEffect effect, Action onComplete, bool isPowerup = false)
	{
		if (isPowerup)
		{
			_powerupEffect = effect;
		}
		if (delay >= 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		ApplyActiveEffect(effect);
		yield return effect.ApplyForDuration();
		RemoveActiveEffect(effect);
		if (isPowerup)
		{
			_powerupEffect = null;
		}
		onComplete?.Invoke();
	}

	private IEnumerator ApplyEffectWhile(BaseWeaponEffect effect, Func<bool> conditional, Action onComplete)
	{
		ApplyActiveEffect(effect);
		yield return effect.ApplyWhile(conditional);
		RemoveActiveEffect(effect);
		onComplete?.Invoke();
	}

	private void ApplyActiveEffect(BaseWeaponEffect effect)
	{
		_activeEffects.Add(effect);
		_statusAffectable.StatusFlags = _statusAffectable.StatusFlags.Add(effect.Status);
	}

	private void RemoveActiveEffect(BaseWeaponEffect effect)
	{
		_activeEffects.Remove(effect);
		if (!_activeEffects.Any((BaseWeaponEffect e) => e.Status == effect.Status))
		{
			_statusAffectable.StatusFlags = _statusAffectable.StatusFlags.Remove(effect.Status);
		}
	}
}
