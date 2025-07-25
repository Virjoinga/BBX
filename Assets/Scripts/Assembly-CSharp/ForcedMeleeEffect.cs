using System.Collections.Generic;

public class ForcedMeleeEffect : BaseWeaponEffect
{
	private static readonly Dictionary<BoltEntity, List<ForcedMeleeEffect>> _activeForcedMeleeEffects = new Dictionary<BoltEntity, List<ForcedMeleeEffect>>();

	private static readonly Dictionary<BoltEntity, int> _prevActiveWeapons = new Dictionary<BoltEntity, int>();

	public ForcedMeleeEffect(string weaponId, WeaponProfile.EffectData effect, BoltEntity victim, BoltEntity applier)
		: base(weaponId, effect, victim, applier)
	{
	}

	public override void StartEffect()
	{
		base.StartEffect();
		if (!_activeForcedMeleeEffects.TryGetValue(_victim, out var value))
		{
			value = new List<ForcedMeleeEffect>();
			_activeForcedMeleeEffects.Add(_victim, value);
		}
		value.Add(this);
		IPlayerState state = _victim.GetState<IPlayerState>();
		if (state.Loadouts[0].ActiveWeapon != 0)
		{
			if (!_prevActiveWeapons.ContainsKey(_victim))
			{
				_prevActiveWeapons.Add(_victim, state.Loadouts[0].ActiveWeapon);
			}
			state.Loadouts[0].ActiveWeapon = 0;
		}
		if (!state.WeaponsDisabled)
		{
			state.WeaponsDisabled = true;
		}
	}

	public override void EndEffect()
	{
		base.EndEffect();
		IPlayerState state = _victim.GetState<IPlayerState>();
		if (!_activeForcedMeleeEffects.TryGetValue(_victim, out var value))
		{
			return;
		}
		value.Remove(this);
		if (value.Count == 0)
		{
			_activeForcedMeleeEffects.Remove(_victim);
			state.WeaponsDisabled = false;
			if (_prevActiveWeapons.ContainsKey(_victim))
			{
				state.Loadouts[0].ActiveWeapon = _prevActiveWeapons[_victim];
				_prevActiveWeapons.Remove(_victim);
			}
		}
	}
}
