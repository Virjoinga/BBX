using System;
using System.Collections.Generic;

namespace BSCore
{
	public class ServerPlayFabStatisticsService : PlayFabService, IStatisticsService
	{
		public void UpdatePlayerStatistics(string serviceId, Dictionary<StatisticKey, int> updates, Action onSuccess, Action<FailureReasons> onFailure)
		{
		}

		public void FetchPlayerStatistics(string serviceId, List<StatisticKey> keys, Action<Dictionary<StatisticKey, int>> onSuccess, Action<FailureReasons> onFailure)
		{
		}
	}
}
