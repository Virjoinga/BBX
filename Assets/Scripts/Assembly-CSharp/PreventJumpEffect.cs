using System.Collections.Generic;
using UnityEngine;

public class PreventJumpEffect : BaseWeaponEffect
{
	private static readonly Dictionary<BoltEntity, List<PreventJumpEffect>> _activeEffects = new Dictionary<BoltEntity, List<PreventJumpEffect>>();

	private readonly IStatusAffectable _statusAffectable;

	public PreventJumpEffect(string weaponId, WeaponProfile.EffectData effect, BoltEntity victim, BoltEntity applier)
		: base(weaponId, effect, victim, applier)
	{
		_statusAffectable = _victim.GetComponent<IStatusAffectable>();
	}

	public override void StartEffect()
	{
		if (!_activeEffects.TryGetValue(_victim, out var value))
		{
			value = new List<PreventJumpEffect>();
			_activeEffects.Add(_victim, value);
		}
		value.Add(this);
		_statusAffectable.PreventJump = true;
		Debug.Log($"[SpeedChangeEffect] Starting prevent jump effect (duration: {_remainingTime})");
	}

	public override void EndEffect()
	{
		if (_activeEffects.TryGetValue(_victim, out var value))
		{
			value.Remove(this);
			if (value.Count == 0)
			{
				_activeEffects.Remove(_victim);
				_statusAffectable.PreventJump = false;
			}
		}
		Debug.Log($"[SpeedChangeEffect] Ending prevent jump effect (duration: {_remainingTime})");
	}
}
