using System;
using BSCore;
using PlayFab;
using PlayFab.ClientModels;

public class ClientPlayFabLeaderboardService : PlayFabService, ILeaderboardService
{
	public void GetLeaderboard(StatisticKey statistic, int startPosition, int maxResults, Action<LeaderboardData> onSuccess, Action<FailureReasons> onFailure)
	{
		PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
		{
			StatisticName = statistic.ToString(),
			StartPosition = startPosition,
			MaxResultsCount = maxResults
		}, errorCallback: OnFailureCallback(delegate
		{
			GetLeaderboard(statistic, startPosition, maxResults, onSuccess, onFailure);
		}, delegate(FailureReasons reason)
		{
			onFailure(reason);
		}), resultCallback: onSuccessWrapper);
		void onSuccessWrapper(GetLeaderboardResult result)
		{
			LeaderboardData obj = new LeaderboardData(result.Leaderboard, result.NextReset);
			onSuccess(obj);
		}
	}

	public void GetLeaderboardAroundPlayer(StatisticKey statistic, string serviceId, int maxResults, Action<LeaderboardData> onSuccess, Action<FailureReasons> onFailure)
	{
		PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest
		{
			StatisticName = statistic.ToString(),
			PlayFabId = serviceId,
			MaxResultsCount = maxResults
		}, errorCallback: OnFailureCallback(delegate
		{
			GetLeaderboardAroundPlayer(statistic, serviceId, maxResults, onSuccess, onFailure);
		}, delegate(FailureReasons reason)
		{
			onFailure(reason);
		}), resultCallback: onSuccessWrapper);
		void onSuccessWrapper(GetLeaderboardAroundPlayerResult result)
		{
			LeaderboardData obj = new LeaderboardData(result.Leaderboard, result.NextReset);
			onSuccess(obj);
		}
	}
}
