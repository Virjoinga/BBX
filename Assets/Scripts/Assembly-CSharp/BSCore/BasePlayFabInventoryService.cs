using System;
using System.Collections.Generic;

namespace BSCore
{
	public abstract class BasePlayFabInventoryService : PlayFabService, IInventoryService
	{
		protected ProfileManager _profileManager;

		public BasePlayFabInventoryService(ProfileManager profileManager)
		{
			_profileManager = profileManager;
		}

		public abstract void Fetch(string serviceId, Action<Dictionary<string, InventoryItem>, IDictionary<CurrencyType, int>> onSuccess, Action<FailureReasons> onFailure);

		public abstract void ConsumeItem(string instanceId, int usesToConsume, Action onSuccess, Action<FailureReasons> onFailure);

		public virtual void GrantItem(BaseProfile profile, Action onSuccess, Action<FailureReasons> onFailure)
		{
			throw new NotImplementedException();
		}

		protected virtual Dictionary<CurrencyType, int> ParseCurrency(Dictionary<string, int> currencyDict)
		{
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			foreach (KeyValuePair<string, int> item in currencyDict)
			{
				dictionary.Add(Enum<CurrencyType>.Parse(item.Key), item.Value);
			}
			return dictionary;
		}

		public virtual void GrantCurrency(string playfabId, CurrencyType currencyType, int amount, Action onSuccess, Action<FailureReasons> onFailure)
		{
			throw new NotImplementedException();
		}
	}
}
