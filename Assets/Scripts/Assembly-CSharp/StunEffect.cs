using System.Collections.Generic;

public class StunEffect : BaseWeaponEffect
{
	private static readonly Dictionary<BoltEntity, List<StunEffect>> _activeStunEffects = new Dictionary<BoltEntity, List<StunEffect>>();

	public StunEffect(string weaponId, WeaponProfile.EffectData effect, BoltEntity victim, BoltEntity applier)
		: base(weaponId, effect, victim, applier)
	{
	}

	public override void StartEffect()
	{
		if (!_activeStunEffects.TryGetValue(_victim, out var value))
		{
			value = new List<StunEffect>();
			_activeStunEffects.Add(_victim, value);
		}
		value.Add(this);
		_victim.GetComponent<IStatusAffectable>().Stunned = true;
	}

	public override void EndEffect()
	{
		IStatusAffectable component = _victim.GetComponent<IStatusAffectable>();
		if (_activeStunEffects.TryGetValue(_victim, out var value))
		{
			value.Remove(this);
			if (value.Count == 0)
			{
				_activeStunEffects.Remove(_victim);
				component.Stunned = false;
			}
		}
	}
}
