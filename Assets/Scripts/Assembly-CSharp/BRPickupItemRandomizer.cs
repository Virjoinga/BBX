using System.Collections.Generic;
using BSCore;
using UnityEngine;

public class BRPickupItemRandomizer
{
	private ProfileManager _profileManager;

	private List<RaritySpawnRate> _raritySpawnRates;

	private List<string> _itemOptions;

	private int _itemCount;

	private Dictionary<Rarity, List<string>> _itemIdsByRarity;

	public BRPickupItemRandomizer(ProfileManager profileManager, List<RaritySpawnRate> raritySpawnRates)
	{
		_profileManager = profileManager;
		_raritySpawnRates = raritySpawnRates;
	}

	public List<string> GetPickupItems(WeaponConfigData weaponsConfig)
	{
		_itemOptions = weaponsConfig.ItemIds;
		_itemCount = weaponsConfig.Count;
		_itemIdsByRarity = new Dictionary<Rarity, List<string>>();
		return GetRandomPickupItems();
	}

	private List<string> GetRandomPickupItems()
	{
		List<string> list = new List<string>();
		PopulateItemsByRarity();
		foreach (RaritySpawnRate raritySpawnRate in _raritySpawnRates)
		{
			int count = Mathf.RoundToInt(raritySpawnRate.SpawnChance / 100f * (float)_itemCount);
			bool flag = false;
			Rarity rarity = raritySpawnRate.Rarity;
			while (!flag)
			{
				if (HaveItemsInSpawnPoolForRarity(rarity))
				{
					flag = true;
					list.AddRange(GetItemsForRarity(rarity, count));
				}
				else if (rarity == Rarity.Common)
				{
					flag = true;
				}
				else
				{
					rarity = GetNextRarityDown(rarity);
					PopulateItemsByRarity();
				}
			}
		}
		return list;
	}

	private List<string> GetItemsForRarity(Rarity rarity, int count)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < count; i++)
		{
			if (!HaveItemsInSpawnPoolForRarity(rarity))
			{
				PopulateItemsByRarity();
			}
			string item = _itemIdsByRarity[rarity].Random();
			_itemIdsByRarity[rarity].Remove(item);
			list.Add(item);
		}
		return list;
	}

	private Rarity GetNextRarityDown(Rarity currentRarity)
	{
		switch (currentRarity)
		{
		case Rarity.Legendary:
			return Rarity.Epic;
		case Rarity.Epic:
			return Rarity.Rare;
		case Rarity.Rare:
			return Rarity.Uncommon;
		case Rarity.Uncommon:
			return Rarity.Common;
		default:
			return Rarity.Common;
		}
	}

	private bool HaveItemsInSpawnPoolForRarity(Rarity rarity)
	{
		if (_itemIdsByRarity.ContainsKey(rarity))
		{
			return _itemIdsByRarity[rarity].Count > 0;
		}
		return false;
	}

	private void PopulateItemsByRarity()
	{
		_itemIdsByRarity = new Dictionary<Rarity, List<string>>();
		foreach (string itemOption in _itemOptions)
		{
			BaseProfile byId = _profileManager.GetById(itemOption);
			if (byId != null)
			{
				if (!_itemIdsByRarity.ContainsKey(byId.Rarity))
				{
					_itemIdsByRarity.Add(byId.Rarity, new List<string>());
				}
				_itemIdsByRarity[byId.Rarity].Add(itemOption);
			}
		}
	}
}
