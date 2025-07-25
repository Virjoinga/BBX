using System;
using System.Collections;
using System.Collections.Generic;
using Constants;
using UnityEngine;

public class HealthChangeOverTimeEffect : BaseWeaponEffect
{
	protected static readonly Dictionary<BoltEntity, Dictionary<Match.StatusType, List<HealthChangeOverTimeEffect>>> _activeHCOTEffectsByVictim = new Dictionary<BoltEntity, Dictionary<Match.StatusType, List<HealthChangeOverTimeEffect>>>();

	private readonly float _amountPerTick;

	private readonly IDamageable _damageable;

	private readonly WeaponProfile.EffectData _effectData;

	public HealthChangeOverTimeEffect(string weaponId, WeaponProfile.EffectData effectData, BoltEntity victim, BoltEntity applier)
		: base(weaponId, effectData, victim, applier)
	{
		float num = effectData.Modifier;
		if (applier.TryFindState<IPlayerState>(out var state))
		{
			num *= 1f + state.DamageModifier;
		}
		_amountPerTick = num * 0.25f;
		_damageable = _victim.GetComponent<IDamageable>();
		_effectData = effectData;
	}

	public override IEnumerator ApplyForDuration()
	{
		Debug.Log("[HealthChangeOverTimeEffect] Effect for weapon " + WeaponId + " started");
		StartEffect();
		while (_remainingTime >= 0f)
		{
			ApplyTick();
			yield return new WaitForSeconds(0.25f);
			_remainingTime -= 0.25f;
		}
		EndEffect();
	}

	public override IEnumerator ApplyWhile(Func<bool> conditional)
	{
		StartEffect();
		while (conditional())
		{
			ApplyTick();
			yield return new WaitForSeconds(0.25f);
		}
		EndEffect();
	}

	public override void StartEffect()
	{
		if (!_activeHCOTEffectsByVictim.TryGetValue(_victim, out var value))
		{
			value = new Dictionary<Match.StatusType, List<HealthChangeOverTimeEffect>>();
			_activeHCOTEffectsByVictim.Add(_victim, value);
		}
		if (!value.TryGetValue(base.Status, out var value2))
		{
			value2 = new List<HealthChangeOverTimeEffect>();
			value.Add(base.Status, value2);
		}
		value2.Add(this);
		value2.Sort(SortByLargest);
	}

	public override void EndEffect()
	{
		if (_activeHCOTEffectsByVictim.TryGetValue(_victim, out var value))
		{
			Debug.Log("[HealthChangeOverTimeEffect] Effect for weapon " + WeaponId + " ended");
			value[base.Status].Remove(this);
			value[base.Status].Sort(SortByLargest);
		}
	}

	private int SortByLargest(HealthChangeOverTimeEffect x, HealthChangeOverTimeEffect y)
	{
		return y._amountPerTick.CompareTo(x._amountPerTick);
	}

	private void ApplyTick()
	{
		Debug.Log("[HealthChangeOverTimeEffect] Effect for weapon " + WeaponId + " applied by " + Applier.name);
		HitInfo hitInfo = new HitInfo
		{
			weaponId = WeaponId,
			effects = new WeaponProfile.EffectData[1] { _effectData }
		};
		_damageable.TakeDamage(hitInfo, _amountPerTick, Applier);
	}
}
