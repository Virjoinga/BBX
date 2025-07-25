using System;
using System.Collections.Generic;

namespace BSCore
{
	public class ServerPlayFabGameItemService : BasePlayFabGameItemService
	{
		public ServerPlayFabGameItemService(BaseProfileFactory profileFactory, GameConfigData configData)
			: base(profileFactory, configData)
		{
		}

		public override void Fetch(string catalogName, Action<Dictionary<string, BaseProfile>> onSuccess, Action<FailureReasons> onFailure)
		{
		}

		private Dictionary<CurrencyType, int> GenerateCostDictionary(Dictionary<string, uint> prices)
		{
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			foreach (KeyValuePair<string, uint> price in prices)
			{
				dictionary.Add(Enum<CurrencyType>.Parse(price.Key), (int)price.Value);
			}
			return dictionary;
		}
	}
}
