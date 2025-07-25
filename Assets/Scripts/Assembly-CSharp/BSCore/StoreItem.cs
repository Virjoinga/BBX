using System.Collections.Generic;

namespace BSCore
{
	public struct StoreItem
	{
		public readonly BaseProfile Profile;

		public readonly IDictionary<CurrencyType, int> OverrideCosts;

		public StoreItem(BaseProfile profile, Dictionary<string, uint> overridePrices)
		{
			Profile = profile;
			OverrideCosts = new Dictionary<CurrencyType, int>();
			OverrideCosts = GenerateCostDictionary(overridePrices);
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

		public bool HasCurrencyTypeCost(CurrencyType currencyType)
		{
			if (OverrideCosts.ContainsKey(currencyType))
			{
				return OverrideCosts[currencyType] > 0;
			}
			return false;
		}
	}
}
