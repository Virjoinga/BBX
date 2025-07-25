using System.Collections.Generic;

public class HuggedEffect : HealthChangeOverTimeEffect
{
	private static readonly Dictionary<BoltEntity, List<HuggedEffect>> _activeHuggedEffects = new Dictionary<BoltEntity, List<HuggedEffect>>();

	private SpeedChangeEffect _immobilizeEffect;

	private ForcedMeleeEffect _forcedMeleeEffect;

	public HuggedEffect(string weaponId, WeaponProfile.EffectData effect, BoltEntity victim, BoltEntity applier)
		: base(weaponId, effect, victim, applier)
	{
		_immobilizeEffect = new SpeedChangeEffect(weaponId, new WeaponProfile.EffectData(new WeaponProfile.WeaponProfileData.EffectProfileData
		{
			modifier = -100f
		}), victim, applier);
		_forcedMeleeEffect = new ForcedMeleeEffect(weaponId, new WeaponProfile.EffectData(new WeaponProfile.WeaponProfileData.EffectProfileData()), victim, applier);
	}

	public override void StartEffect()
	{
		base.StartEffect();
		if (!_activeHuggedEffects.TryGetValue(_victim, out var value))
		{
			value = new List<HuggedEffect>();
			_activeHuggedEffects.Add(_victim, value);
		}
		value.Add(this);
		_immobilizeEffect.StartEffect();
		_forcedMeleeEffect.StartEffect();
	}

	public override void EndEffect()
	{
		base.EndEffect();
		_immobilizeEffect.EndEffect();
		_forcedMeleeEffect.EndEffect();
		if (_activeHuggedEffects.TryGetValue(_victim, out var value))
		{
			value.Remove(this);
			if (value.Count == 0)
			{
				_activeHuggedEffects.Remove(_victim);
			}
		}
	}
}
