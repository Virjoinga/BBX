using System.Collections.Generic;
using UnityEngine;

public class MeleeDamageModifierEffect : BaseWeaponEffect
{
	private static readonly Dictionary<BoltEntity, List<MeleeDamageModifierEffect>> _activeMeleeDamageModifierEffects = new Dictionary<BoltEntity, List<MeleeDamageModifierEffect>>();

	public readonly float Amount;

	public MeleeDamageModifierEffect(string weaponId, WeaponProfile.EffectData effect, BoltEntity victim, BoltEntity applier)
		: base(weaponId, effect, victim, applier)
	{
		Amount = effect.Modifier;
	}

	public override void StartEffect()
	{
		IStatusAffectable component = _victim.GetComponent<IStatusAffectable>();
		if (!_activeMeleeDamageModifierEffects.TryGetValue(_victim, out var value))
		{
			value = new List<MeleeDamageModifierEffect>();
			_activeMeleeDamageModifierEffects.Add(_victim, value);
		}
		value.Add(this);
		component.MeleeDamageModifier += Amount;
		Debug.Log($"[MeleeDamageModifierEffect] Starting damage modifier effect (amout: {Amount}, duration: {_remainingTime})");
	}

	public override void EndEffect()
	{
		IStatusAffectable component = _victim.GetComponent<IStatusAffectable>();
		if (_activeMeleeDamageModifierEffects.TryGetValue(_victim, out var value))
		{
			value.Remove(this);
		}
		component.MeleeDamageModifier -= Amount;
		if ((double)Mathf.Abs(component.MeleeDamageModifier) < 0.01)
		{
			component.MeleeDamageModifier = 0f;
		}
		Debug.Log($"[MeleeDamageModifierEffect] Ending damage modifier effect (amout: {Amount})");
	}
}
