using System.Collections.Generic;

namespace BSCore
{
	public struct StoreData
	{
		public readonly string ID;

		public readonly IList<StoreItem> Items;

		public readonly string Name;

		public readonly string Description;

		public StoreData(string id, List<StoreItem> storeItems, string name, string description)
		{
			ID = id;
			Items = new List<StoreItem>(storeItems);
			Name = name;
			Description = description;
		}
	}
}
