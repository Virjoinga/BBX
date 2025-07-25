using System;
using System.Collections.Generic;

namespace BSCore
{
	public class ServerPlayFabInventoryService : BasePlayFabInventoryService
	{
		public ServerPlayFabInventoryService(ProfileManager profileManager)
			: base(profileManager)
		{
		}

		public override void Fetch(string serviceId, Action<Dictionary<string, InventoryItem>, IDictionary<CurrencyType, int>> onSuccess, Action<FailureReasons> onFailure)
		{
		}

		public override void ConsumeItem(string instanceId, int usesToConsume, Action onSuccess, Action<FailureReasons> onFailure)
		{
		}

		public override void GrantCurrency(string playfabId, CurrencyType currencyType, int amount, Action onSuccess, Action<FailureReasons> onFailure)
		{
		}
	}
}
