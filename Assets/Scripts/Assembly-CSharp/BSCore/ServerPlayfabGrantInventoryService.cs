using System;
using System.Collections.Generic;

namespace BSCore
{
	public class ServerPlayfabGrantInventoryService : PlayFabService, IGrantInventoryService
	{
		public void GrantCurrency(CurrencyType currency, int amount, string serviceId, Action onSuccess, Action<FailureReasons> onFailure)
		{
		}

		public void GrantInventoryItems(List<string> itemIds, string serviceId, Action onSuccess, Action<FailureReasons> onFailure)
		{
		}
	}
}
