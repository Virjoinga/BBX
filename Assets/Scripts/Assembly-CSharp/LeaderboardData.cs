using System;
using System.Collections.Generic;
using PlayFab.ClientModels;

public class LeaderboardData
{
	public List<PlayerLeaderboardEntry> Leaderboard { get; private set; }

	public DateTime? NextReset { get; private set; }

	public LeaderboardData(List<PlayerLeaderboardEntry> leaderboard, DateTime? nextReset)
	{
		Leaderboard = leaderboard;
		NextReset = nextReset;
	}
}
