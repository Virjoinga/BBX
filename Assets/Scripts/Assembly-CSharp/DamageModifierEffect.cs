using System.Collections.Generic;
using UnityEngine;

public class DamageModifierEffect : BaseWeaponEffect
{
	private static readonly Dictionary<BoltEntity, List<DamageModifierEffect>> _activeDamageModifierEffects = new Dictionary<BoltEntity, List<DamageModifierEffect>>();

	public readonly float Amount;

	public DamageModifierEffect(string weaponId, WeaponProfile.EffectData effect, BoltEntity victim, BoltEntity applier)
		: base(weaponId, effect, victim, applier)
	{
		Amount = effect.Modifier;
	}

	public override void StartEffect()
	{
		IStatusAffectable component = _victim.GetComponent<IStatusAffectable>();
		if (!_activeDamageModifierEffects.TryGetValue(_victim, out var value))
		{
			value = new List<DamageModifierEffect>();
			_activeDamageModifierEffects.Add(_victim, value);
		}
		value.Add(this);
		component.DamageModifier += Amount;
		Debug.Log($"[DamageModifierEffect] Starting damage modifier effect (amout: {Amount}, duration: {_remainingTime})");
	}

	public override void EndEffect()
	{
		IStatusAffectable component = _victim.GetComponent<IStatusAffectable>();
		if (_activeDamageModifierEffects.TryGetValue(_victim, out var value))
		{
			value.Remove(this);
		}
		component.DamageModifier -= Amount;
		if ((double)Mathf.Abs(component.DamageModifier) < 0.01)
		{
			component.DamageModifier = 0f;
		}
		Debug.Log($"[DamageModifierEffect] Ending damage modifier effect (amout: {Amount})");
	}
}
