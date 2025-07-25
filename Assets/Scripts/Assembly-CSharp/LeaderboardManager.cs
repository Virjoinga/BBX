using System;
using BSCore;
using Zenject;

public class LeaderboardManager
{
	private ILeaderboardService _service;

	private UserManager _userManager;

	[Inject]
	public LeaderboardManager(ILeaderboardService service, UserManager userManager)
	{
		_service = service;
		_userManager = userManager;
	}

	public void GetLeaderboard(StatisticKey statistic, int startPosition, int maxResults, Action<LeaderboardData> onSuccess, Action<FailureReasons> onFailure)
	{
		_service.GetLeaderboard(statistic, startPosition, maxResults, onSuccess, onFailure);
	}

	public void GetLeaderboardAroundLocalPlayer(StatisticKey statistic, int maxResults, Action<LeaderboardData> onSuccess, Action<FailureReasons> onFailure)
	{
		string id = _userManager.CurrentUser.Id;
		_service.GetLeaderboardAroundPlayer(statistic, id, maxResults, onSuccess, onFailure);
	}

	public void GetLeaderboardAroundPlayer(StatisticKey statistic, string serviceId, int maxResults, Action<LeaderboardData> onSuccess, Action<FailureReasons> onFailure)
	{
		_service.GetLeaderboardAroundPlayer(statistic, serviceId, maxResults, onSuccess, onFailure);
	}
}
