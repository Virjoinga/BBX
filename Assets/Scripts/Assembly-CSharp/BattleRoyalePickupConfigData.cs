using System;
using System.Collections.Generic;
using BSCore;
using UnityEngine;

[Serializable]
public class BattleRoyalePickupConfigData
{
	public WeaponConfigData Weapons;

	public WeaponConfigData MeleeWeapons;

	public AmmoClipPickupConfigData AmmoClip;

	public List<RaritySpawnRate> RaritySpawnRates;

	private ProfileManager _profileManager;

	private Dictionary<Rarity, List<string>> _itemIdsByRarity;

	private BRPickupItemRandomizer _brPickupItemRandomizer;

	public void Init(ProfileManager profileManager)
	{
		_profileManager = profileManager;
		float num = 0f;
		foreach (RaritySpawnRate raritySpawnRate in RaritySpawnRates)
		{
			raritySpawnRate.PopulateRarity();
			num += raritySpawnRate.SpawnChance;
		}
		if (num != 100f)
		{
			Debug.LogError("Spawn Chances not setup properly. Spawn Percents should sum to 100%");
		}
		_brPickupItemRandomizer = new BRPickupItemRandomizer(_profileManager, RaritySpawnRates);
	}

	public List<string> GetItemPickups()
	{
		List<string> pickupItems = _brPickupItemRandomizer.GetPickupItems(Weapons);
		pickupItems.AddRange(_brPickupItemRandomizer.GetPickupItems(MeleeWeapons));
		return pickupItems;
	}
}
