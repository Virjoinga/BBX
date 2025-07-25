using System.Collections.Generic;
using UnityEngine;

public class ForcedMovementEffect : BaseWeaponEffect
{
	private static readonly Dictionary<BoltEntity, List<ForcedMovementEffect>> _activeEffects = new Dictionary<BoltEntity, List<ForcedMovementEffect>>();

	private readonly IStatusAffectable _statusAffectable;

	public ForcedMovementEffect(string weaponId, WeaponProfile.EffectData effect, BoltEntity victim, BoltEntity applier)
		: base(weaponId, effect, victim, applier)
	{
		_statusAffectable = _victim.GetComponent<IStatusAffectable>();
	}

	public override void StartEffect()
	{
		if (!_activeEffects.TryGetValue(_victim, out var value))
		{
			value = new List<ForcedMovementEffect>();
			_activeEffects.Add(_victim, value);
		}
		value.Add(this);
		_statusAffectable.ForcedMovement = true;
		Debug.Log($"[SpeedChangeEffect] Starting forced movement effect (duration: {_remainingTime})");
	}

	public override void EndEffect()
	{
		if (_activeEffects.TryGetValue(_victim, out var value))
		{
			value.Remove(this);
			if (value.Count == 0)
			{
				_activeEffects.Remove(_victim);
				_statusAffectable.ForcedMovement = false;
			}
		}
		Debug.Log($"[SpeedChangeEffect] Ending forced movement effect (duration: {_remainingTime})");
	}
}
