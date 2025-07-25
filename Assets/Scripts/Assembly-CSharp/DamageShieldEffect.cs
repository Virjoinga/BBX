using System.Collections.Generic;
using UnityEngine;

public class DamageShieldEffect : BaseWeaponEffect
{
	private static readonly Dictionary<BoltEntity, DamageShieldEffect> _activeEffects = new Dictionary<BoltEntity, DamageShieldEffect>();

	public readonly float Amount;

	private readonly IStatusAffectable _victimStatusAffectable;

	public DamageShieldEffect(string weaponId, WeaponProfile.EffectData effect, BoltEntity victim, BoltEntity applier)
		: base(weaponId, effect, victim, applier)
	{
		Amount = effect.Modifier;
		_victimStatusAffectable = _victim.GetComponent<IStatusAffectable>();
	}

	public override void StartEffect()
	{
		if (_activeEffects.ContainsKey(_victim))
		{
			if (_victimStatusAffectable.DamageShield < Amount)
			{
				DamageShieldEffect damageShieldEffect = _activeEffects[_victim];
				_activeEffects[_victim] = this;
				_victimStatusAffectable.DamageShield = Amount;
				damageShieldEffect._remainingTime = 0f;
			}
		}
		else
		{
			_activeEffects.Add(_victim, this);
			_victimStatusAffectable.DamageShield = Amount;
		}
		Debug.Log($"[DamageModifierEffect] Starting damage shield effect (amout: {Amount}, duration: {_remainingTime})");
	}

	public override void EndEffect()
	{
		if (_activeEffects.TryGetValue(_victim, out var value) && value == this)
		{
			_activeEffects.Remove(_victim);
			_victimStatusAffectable.DamageShield = 0f;
		}
		Debug.Log($"[DamageModifierEffect] Ending damage shield effect (amout: {Amount})");
	}

	protected override void OnEffectTick()
	{
		if (_victimStatusAffectable.DamageShield <= 0f)
		{
			_remainingTime = 0f;
		}
	}
}
