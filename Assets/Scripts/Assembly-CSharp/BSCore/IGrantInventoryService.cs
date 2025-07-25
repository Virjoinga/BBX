using System;
using System.Collections.Generic;

namespace BSCore
{
	public interface IGrantInventoryService
	{
		void GrantCurrency(CurrencyType currency, int amount, string serviceId, Action onSuccess, Action<FailureReasons> onFailure);

		void GrantInventoryItems(List<string> itemIds, string serviceId, Action onSuccess, Action<FailureReasons> onFailure);
	}
}
