using System;
using System.Collections;
using Constants;
using UnityEngine;

public abstract class BaseWeaponEffect
{
	public readonly string WeaponId;

	public readonly BoltEntity Applier;

	protected readonly BoltEntity _victim;

	protected readonly bool _inverseForAlly;

	protected float _remainingTime;

	public Match.EffectType Effect { get; private set; }

	public Match.StatusType Status { get; private set; }

	public bool ReducedByTenacity { get; private set; }

	public BaseWeaponEffect(string weaponId, WeaponProfile.EffectData effect, BoltEntity victim, BoltEntity applier)
	{
		WeaponId = weaponId;
		_remainingTime = effect.Duration;
		_victim = victim;
		Applier = applier;
		Effect = effect.EffectType;
		Status = effect.StatusType;
		_inverseForAlly = effect.InverseForAlly;
		ReducedByTenacity = effect.ReducedByTenacity;
	}

	public void TryModifyDurationForEquipment(LoadoutController loadoutController)
	{
		if (loadoutController.MajorEquipmentSlotProfile != null && loadoutController.MajorEquipmentSlotProfile.Type == EquipmentType.Tenacity)
		{
			_remainingTime = loadoutController.MajorEquipmentSlotProfile.GetMajorModifiedValue(_remainingTime);
		}
		if (loadoutController.MinorEquipmentSlotProfile != null && loadoutController.MinorEquipmentSlotProfile.Type == EquipmentType.Tenacity)
		{
			_remainingTime = loadoutController.MinorEquipmentSlotProfile.GetMinorModifiedValue(_remainingTime);
		}
	}

	public virtual IEnumerator ApplyForDuration()
	{
		StartEffect();
		while (_remainingTime >= 0f)
		{
			OnEffectTick();
			_remainingTime -= Time.deltaTime;
			yield return null;
		}
		EndEffect();
	}

	public virtual IEnumerator ApplyWhile(Func<bool> conditional)
	{
		StartEffect();
		yield return null;
		while (conditional())
		{
			OnEffectTick();
			yield return null;
		}
		EndEffect();
	}

	public virtual void StartEffect()
	{
	}

	public virtual void EndEffect()
	{
	}

	protected virtual void OnEffectTick()
	{
	}
}
