using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSCore
{
	public class BaseProfile
	{
		private static List<string> _noItemNames;

		protected BaseProfileData _profileData;

		public bool IsNoItem => ItemIdIsNoItem(Id);

		public string Id { get; protected set; }

		public ItemType ItemType { get; protected set; }

		public IList<string> Tags { get; protected set; }

		public string CatalogVersion { get; protected set; }

		public string Description { get; protected set; }

		public string Name { get; protected set; }

		public Sprite Icon { get; protected set; }

		public Sprite ItemTypeIcon { get; protected set; }

		public int ReleaseVersion { get; protected set; }

		public IDictionary<CurrencyType, int> Cost { get; protected set; }

		public bool IsConsumable { get; protected set; }

		public int UsageCount { get; protected set; }

		public int UsageLifespan { get; protected set; }

		public CurrencyType FirstCostType => Cost.Keys.First();

		public int FirstCostAmount => Cost[FirstCostType];

		public Rarity Rarity { get; protected set; }

		public Color RarityColor { get; protected set; }

		public static bool ItemIdIsNoItem(string itemId)
		{
			if (_noItemNames == null)
			{
				_noItemNames = (from itn in Enum.GetNames(typeof(ItemType))
					select "No" + itn).ToList();
			}
			return _noItemNames.Contains(itemId);
		}

		public BaseProfile(GameItem gameItem)
		{
			CatalogVersion = gameItem.CatalogVersion;
			Id = gameItem.Id;
			ItemType = gameItem.ItemType;
			Tags = gameItem.Tags;
			Description = gameItem.Description;
			Name = gameItem.Name;
			Icon = Resources.Load<Sprite>(gameItem.Id + "Icon");
			Cost = gameItem.Cost;
			UsageCount = gameItem.UsageCount;
			UsageLifespan = gameItem.UsageLifespan;
			IsConsumable = UsageCount > 0 || UsageLifespan > 0;
			DeserializeData(gameItem.CustomData);
			ParseCustomData();
		}

		protected virtual void DeserializeData(string json)
		{
			_profileData = BaseProfileData.FromJson<BaseProfileData>(json);
		}

		protected virtual void ParseCustomData()
		{
			ReleaseVersion = _profileData.ReleaseVersion;
			Rarity = Rarity.Common;
			if (Enum<Rarity>.TryParse(_profileData.Rarity, out var value))
			{
				Rarity = value;
			}
		}

		public bool HasCurrencyTypeCost(CurrencyType currencyType)
		{
			if (Cost.ContainsKey(currencyType))
			{
				return Cost[currencyType] > 0;
			}
			return false;
		}

		public override string ToString()
		{
			return "[BaseProfile] Id: " + Id + " Type " + ItemType;
		}
	}
}
