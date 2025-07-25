using System.Collections.Generic;

namespace BSCore
{
	public class GameItem
	{
		public struct Data
		{
			public string Id;

			public bool IsDefault;

			public ItemType ItemType;

			public List<string> Tags;

			public string CatalogVersion;

			public string Description;

			public string Name;

			public string Icon;

			public string ReleaseVersion;

			public IDictionary<CurrencyType, int> Cost;

			public Dictionary<CurrencyType, uint> CurrencyBundle;

			public string CustomData;

			public List<string> BundledItems;

			public int UsageCount;

			public int UsageLifespan;
		}

		public string Id { get; protected set; }

		public bool IsDefault { get; protected set; }

		public ItemType ItemType { get; protected set; }

		public List<string> Tags { get; protected set; }

		public string CatalogVersion { get; protected set; }

		public string Description { get; protected set; }

		public string Name { get; protected set; }

		public string Icon { get; protected set; }

		public string ReleaseVersion { get; protected set; }

		public IDictionary<CurrencyType, int> Cost { get; private set; }

		public string CustomData { get; private set; }

		public List<string> BundledItems { get; private set; }

		public Dictionary<CurrencyType, uint> CurrencyBundle { get; private set; }

		public int UsageCount { get; private set; }

		public int UsageLifespan { get; private set; }

		public bool IsBundle
		{
			get
			{
				if (BundledItems != null)
				{
					return BundledItems.Count > 0;
				}
				return false;
			}
		}

		public GameItem(string itemId, ItemType type, string name, string description, string releaseVersion)
		{
			CatalogVersion = "";
			Id = itemId;
			Tags = new List<string> { "NoItem" };
			IsDefault = false;
			ItemType = type;
			Name = name;
			Description = description;
			Icon = "";
			ReleaseVersion = releaseVersion;
			CustomData = "";
			GenerateCostDictionary(new Dictionary<string, uint>());
			CurrencyBundle = new Dictionary<CurrencyType, uint>();
			CustomData = "{\"ReleaseVersion\": 0}";
		}

		public GameItem(Data data)
			: this(data.Id, data.ItemType, data.Name, data.Description, data.ReleaseVersion)
		{
			Tags = data.Tags;
			IsDefault = data.IsDefault;
			CatalogVersion = data.CatalogVersion;
			Icon = data.Icon;
			CustomData = data.CustomData;
			Cost = data.Cost;
			UsageCount = data.UsageCount;
			UsageLifespan = data.UsageLifespan;
			BundledItems = data.BundledItems;
			CurrencyBundle = data.CurrencyBundle;
		}

		private void GenerateCostDictionary(Dictionary<string, uint> prices)
		{
			Cost = new Dictionary<CurrencyType, int>();
			foreach (KeyValuePair<string, uint> price in prices)
			{
				Cost.Add(Enum<CurrencyType>.Parse(price.Key), (int)price.Value);
			}
		}
	}
}
