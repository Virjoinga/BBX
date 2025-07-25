using UnityEngine;

namespace BSCore
{
	public class InventoryItem
	{
		public struct Data
		{
			public string Id;

			public string InstanceId;

			public BaseProfile Profile;

			public uint Count;
		}

		public string Id { get; private set; }

		public string InstanceId { get; private set; }

		public BaseProfile Profile { get; private set; }

		public ItemType Type => Profile.ItemType;

		public string Name => Profile.Name;

		public string Description => Profile.Description;

		public Sprite Icon => Profile.Icon;

		public uint Count { get; set; }

		public InventoryItem(Data data)
		{
			Id = data.Id;
			InstanceId = data.InstanceId;
			Profile = data.Profile;
			Count = data.Count;
		}

		public void ConsumeItem()
		{
			if (Count != 0)
			{
				Count--;
			}
		}
	}
}
