using System;
using System.Collections.Generic;
using Zenject;

namespace BSCore
{
	public class StatisticsManager
	{
		private IStatisticsService _service;

		[Inject]
		public StatisticsManager(IStatisticsService service)
		{
			_service = service;
		}

		public void FetchPlayerStatistic(PlayerProfile player, StatisticKey key, Action<StatisticKey, int> onSuccess, Action<FailureReasons> onFailure)
		{
			_service.FetchPlayerStatistics(player.Id, new List<StatisticKey> { key }, onSuccessWrapper, onFailure);
			void onSuccessWrapper(Dictionary<StatisticKey, int> statistics)
			{
				statistics.TryGetValue(key, out var value);
				onSuccess(key, value);
			}
		}

		public void FetchPlayerStatistics(PlayerProfile player, Action onSuccess, Action<FailureReasons> onFailure)
		{
			FetchPlayerStatistics(player, null, onSuccess, onFailure);
		}

		public void FetchPlayerStatistics(PlayerProfile player, List<StatisticKey> keys, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_service.FetchPlayerStatistics(player.Id, keys, onSuccessWrapper, onFailure);
			void onSuccessWrapper(Dictionary<StatisticKey, int> statistics)
			{
				player.OnStatisticsFetched(statistics);
				onSuccess();
			}
		}

		public void UpdatePlayerStatistic(PlayerProfile player, StatisticKey key, int value, Action onSuccess = null, Action<FailureReasons> onFailure = null)
		{
			UpdatePlayerStatistics(player, new Dictionary<StatisticKey, int> { { key, value } }, onSuccess, onFailure);
		}

		public void UpdatePlayerStatistics(PlayerProfile player, Dictionary<StatisticKey, int> updates, Action onSuccess = null, Action<FailureReasons> onFailure = null)
		{
			_service.UpdatePlayerStatistics(player.Id, updates, onSuccessWrapper, onFailure);
			void onSuccessWrapper()
			{
				player.OnStatisticsFetched(updates);
				onSuccess?.Invoke();
			}
		}
	}
}
