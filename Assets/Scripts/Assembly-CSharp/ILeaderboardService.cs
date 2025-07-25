using System;

public interface ILeaderboardService
{
	void GetLeaderboard(StatisticKey statistic, int startPosition, int maxResults, Action<LeaderboardData> onSuccess, Action<FailureReasons> onFailure);

	void GetLeaderboardAroundPlayer(StatisticKey statistic, string serviceId, int maxResults, Action<LeaderboardData> onSuccess, Action<FailureReasons> onFailure);
}
