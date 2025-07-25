using System;
using System.Collections.Generic;

namespace BSCore
{
	public class GrantInventoryManager
	{
		private IGrantInventoryService _grantInventoryService;

		public GrantInventoryManager(IGrantInventoryService grantInventoryService)
		{
			_grantInventoryService = grantInventoryService;
		}

		public void GrantCurrency(CurrencyType currencyType, int amount, string serviceId, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_grantInventoryService.GrantCurrency(currencyType, amount, serviceId, onSuccess, onFailure);
		}

		public void GrantInventoryItem(string itemId, string serviceId, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_grantInventoryService.GrantInventoryItems(new List<string> { itemId }, serviceId, onSuccess, onFailure);
		}

		public void GrantInventoryItems(List<string> itemIds, string serviceId, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_grantInventoryService.GrantInventoryItems(itemIds, serviceId, onSuccess, onFailure);
		}
	}
}
