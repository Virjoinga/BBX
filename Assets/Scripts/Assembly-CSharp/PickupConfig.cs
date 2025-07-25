using System.Collections.Generic;
using System.Linq;
using BSCore;
using BSCore.Constants.Config;
using UnityEngine;

public class PickupConfig
{
	public bool IsInstantiated;

	private ConfigManager _configManager;

	private int _totalChance;

	private readonly Dictionary<PickupType, int> _pickupTypeChances = new Dictionary<PickupType, int>();

	public EffectApplyingPickupData Speed { get; private set; }

	public EffectApplyingPickupData Damage { get; private set; }

	public EffectApplyingPickupData Shield { get; private set; }

	public HealthPickupsBySizeData Health { get; private set; }

	public PickupConfig(ConfigManager configManager)
	{
		_configManager = configManager;
		_configManager.Fetched += OnConfigFetched;
		if (configManager.HasFetched)
		{
			OnConfigFetched();
		}
	}

	~PickupConfig()
	{
		if (_configManager != null)
		{
			_configManager.Fetched -= OnConfigFetched;
		}
	}

	public HealthPickupData GetHealthPackConfigForType(HealthPackType healthPackType)
	{
		switch (healthPackType)
		{
		case HealthPackType.Large:
			return Health.Large;
		case HealthPackType.Small:
			return Health.Small;
		default:
			return Health.Small;
		}
	}

	public PickupType GetRandomType()
	{
		int num = Random.Range(1, _totalChance + 1);
		int num2 = 0;
		foreach (KeyValuePair<PickupType, int> pickupTypeChance in _pickupTypeChances)
		{
			if (num <= num2 + pickupTypeChance.Value)
			{
				return pickupTypeChance.Key;
			}
			num2 += pickupTypeChance.Value;
		}
		return PickupType.HealthSmall;
	}

	public PickupData GetByType(PickupType type)
	{
		switch (type)
		{
		case PickupType.SpeedBoost:
			return Speed;
		case PickupType.DamageBoost:
			return Damage;
		case PickupType.DamageShield:
			return Shield;
		case PickupType.HealthLarge:
			return Health.Large;
		default:
			return Health.Small;
		}
	}

	private void OnConfigFetched()
	{
		PickupsConfigData pickupsConfigData = _configManager.Get<PickupsConfigData>(DataKeys.pickups);
		Speed = new EffectApplyingPickupData(pickupsConfigData.speed);
		Damage = new EffectApplyingPickupData(pickupsConfigData.damage);
		Shield = new EffectApplyingPickupData(pickupsConfigData.shield);
		Health = new HealthPickupsBySizeData(pickupsConfigData.health);
		_pickupTypeChances.Clear();
		_pickupTypeChances.Add(PickupType.SpeedBoost, Speed.Chance);
		_pickupTypeChances.Add(PickupType.DamageBoost, Damage.Chance);
		_pickupTypeChances.Add(PickupType.DamageShield, Shield.Chance);
		_totalChance = _pickupTypeChances.Values.Sum();
		IsInstantiated = true;
	}
}
