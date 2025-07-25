using System.Collections.Generic;

namespace BSCore
{
	public abstract class BaseProfileFactory
	{
		protected ConfigManager _configManager;

		public BaseProfileFactory(ConfigManager configManager)
		{
			_configManager = configManager;
		}

		public virtual Dictionary<string, BaseProfile> GenerateProfiles(List<GameItem> gameItems)
		{
			List<GameItem> list = SortGameItemsByLoadOrder(gameItems);
			Dictionary<string, BaseProfile> dictionary = new Dictionary<string, BaseProfile>();
			foreach (GameItem item in list)
			{
				BaseProfile baseProfile = GenerateProfile(item, dictionary);
				if (baseProfile != null)
				{
					dictionary.Add(baseProfile.Id, baseProfile);
				}
			}
			return dictionary;
		}

		protected abstract List<GameItem> SortGameItemsByLoadOrder(List<GameItem> gameItems);

		protected abstract BaseProfile GenerateProfile(GameItem gameItem, Dictionary<string, BaseProfile> profiles);
	}
}
