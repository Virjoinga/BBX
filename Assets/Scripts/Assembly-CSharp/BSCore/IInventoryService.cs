using System;
using System.Collections.Generic;

namespace BSCore
{
	public interface IInventoryService
	{
		void Fetch(string serviceId, Action<Dictionary<string, InventoryItem>, IDictionary<CurrencyType, int>> onSuccess, Action<FailureReasons> onFailure);

		void ConsumeItem(string instanceId, int usesToConsume, Action onSuccess, Action<FailureReasons> onFailure);

		void GrantItem(BaseProfile profile, Action onSuccess, Action<FailureReasons> onFailure);

		void GrantCurrency(string playfabId, CurrencyType currencyType, int amount, Action onSuccess, Action<FailureReasons> onFailure);
	}
}
