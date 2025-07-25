using System.Collections.Generic;

public class SizeChangeEffect : BaseWeaponEffect
{
	private static readonly Dictionary<BoltEntity, List<SizeChangeEffect>> _activeEffects = new Dictionary<BoltEntity, List<SizeChangeEffect>>();

	private const float NEUTRAL = 0f;

	public readonly float Amount;

	public SizeChangeEffect(string weaponId, WeaponProfile.EffectData effect, BoltEntity victim, BoltEntity applier)
		: base(weaponId, effect, victim, applier)
	{
		Amount = effect.Modifier;
	}

	public override void StartEffect()
	{
		if (!_activeEffects.TryGetValue(_victim, out var value))
		{
			value = new List<SizeChangeEffect>();
			_activeEffects.Add(_victim, value);
		}
		value.Add(this);
		_victim.GetComponent<IStatusAffectable>().Size = Amount;
	}

	public override void EndEffect()
	{
		IStatusAffectable component = _victim.GetComponent<IStatusAffectable>();
		if (_activeEffects.TryGetValue(_victim, out var value))
		{
			value.Remove(this);
			if (value.Count == 0)
			{
				component.Size = 0f;
			}
			else
			{
				component.Size = value[0].Amount;
			}
		}
	}
}
