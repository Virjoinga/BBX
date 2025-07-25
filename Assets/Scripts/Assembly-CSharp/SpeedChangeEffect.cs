using System.Collections.Generic;
using UnityEngine;

public class SpeedChangeEffect : BaseWeaponEffect
{
	private static readonly Dictionary<BoltEntity, List<SpeedChangeEffect>> _activeSpeedIncreaseEffects = new Dictionary<BoltEntity, List<SpeedChangeEffect>>();

	private static readonly Dictionary<BoltEntity, List<SpeedChangeEffect>> _activeSpeedDecreaseEffects = new Dictionary<BoltEntity, List<SpeedChangeEffect>>();

	private readonly float _neutral;

	public readonly float Amount;

	public bool IsIncrease => Amount > _neutral;

	public SpeedChangeEffect(string weaponId, WeaponProfile.EffectData effect, BoltEntity victim, BoltEntity applier)
		: base(weaponId, effect, victim, applier)
	{
		Amount = effect.Modifier;
	}

	public override void StartEffect()
	{
		IStatusAffectable component = _victim.GetComponent<IStatusAffectable>();
		if (IsIncrease)
		{
			if (!_activeSpeedIncreaseEffects.TryGetValue(_victim, out var value))
			{
				value = new List<SpeedChangeEffect>();
				_activeSpeedIncreaseEffects.Add(_victim, value);
			}
			value.Add(this);
			value.Sort(SortIncrease);
			float num = Amount / 100f;
			if (num > _neutral && num > component.SpeedIncrease)
			{
				component.SpeedIncrease = num;
			}
		}
		else
		{
			if (!_activeSpeedDecreaseEffects.TryGetValue(_victim, out var value2))
			{
				value2 = new List<SpeedChangeEffect>();
				_activeSpeedDecreaseEffects.Add(_victim, value2);
			}
			value2.Add(this);
			value2.Sort(SortDecrease);
			float num2 = Amount / 100f;
			if (num2 < _neutral && num2 < component.SpeedDecrease)
			{
				component.SpeedDecrease = num2;
			}
		}
		Debug.Log($"[SpeedChangeEffect] Starting speed change effect (amout: {Amount}, duration: {_remainingTime})");
	}

	public override void EndEffect()
	{
		IStatusAffectable component = _victim.GetComponent<IStatusAffectable>();
		List<SpeedChangeEffect> value2;
		if (IsIncrease)
		{
			if (_activeSpeedIncreaseEffects.TryGetValue(_victim, out var value))
			{
				value.Remove(this);
				if (value.Count == 0)
				{
					_activeSpeedIncreaseEffects.Remove(_victim);
					component.SpeedIncrease = 0f;
				}
				else
				{
					component.SpeedIncrease = value[0].Amount / 100f;
				}
			}
		}
		else if (_activeSpeedDecreaseEffects.TryGetValue(_victim, out value2))
		{
			value2.Remove(this);
			if (value2.Count == 0)
			{
				_activeSpeedDecreaseEffects.Remove(_victim);
				component.SpeedDecrease = 0f;
			}
			else
			{
				component.SpeedDecrease = value2[0].Amount / 100f;
			}
		}
		Debug.Log($"[SpeedChangeEffect] Ending speed change effect (amout: {Amount})");
	}

	private int SortIncrease(SpeedChangeEffect x, SpeedChangeEffect y)
	{
		return y.Amount.CompareTo(x.Amount);
	}

	private int SortDecrease(SpeedChangeEffect x, SpeedChangeEffect y)
	{
		return x.Amount.CompareTo(y.Amount);
	}
}
