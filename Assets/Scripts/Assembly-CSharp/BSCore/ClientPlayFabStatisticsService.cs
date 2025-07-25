using System;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;

namespace BSCore
{
	public class ClientPlayFabStatisticsService : PlayFabService, IStatisticsService
	{
		public void FetchPlayerStatistics(string serviceId, List<StatisticKey> keys, Action<Dictionary<StatisticKey, int>> onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest
			{
				StatisticNames = keys.Select((StatisticKey k) => k.ToString()).ToList()
			}, errorCallback: OnFailureCallback(delegate
			{
				FetchPlayerStatistics(serviceId, keys, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: onSuccessWrapper);
			void onSuccessWrapper(GetPlayerStatisticsResult result)
			{
				Dictionary<StatisticKey, int> dictionary = new Dictionary<StatisticKey, int>();
				foreach (StatisticValue statistic in result.Statistics)
				{
					Enum<StatisticKey>.TryParse(statistic.StatisticName, out var value);
					dictionary.Add(value, statistic.Value);
				}
				onSuccess(dictionary);
			}
		}

		public void UpdatePlayerStatistics(string serviceId, Dictionary<StatisticKey, int> updates, Action onSuccess, Action<FailureReasons> onFailure)
		{
			throw new NotImplementedException();
		}
	}
}
