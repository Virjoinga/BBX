using System.Collections.Generic;
using BSCore;
using PlayFab.ClientModels;
using UnityEngine;
using Zenject;

public class LeaderboardDisplay : MonoBehaviour
{
	[Inject]
	private LeaderboardManager _leaderboardManager;

	[Inject]
	private UserManager _userManager;

	[SerializeField]
	private StatisticKey _leaderboardStat;

	[SerializeField]
	private SmartPool _smartPool;

	[SerializeField]
	private LeaderboardPlate _localPlayerPlate;

	public void FetchLeaderboard()
	{
		_leaderboardManager.GetLeaderboard(_leaderboardStat, 0, 100, OnLeaderboardFetched, delegate(FailureReasons e)
		{
			Debug.LogError($"[LeaderboardDisplay] Failed to fetch leaderboard {_leaderboardStat}. Reason {e}");
		});
		_leaderboardManager.GetLeaderboardAroundLocalPlayer(_leaderboardStat, 1, OnLocalPlayerLeaderboardFetched, delegate(FailureReasons e)
		{
			Debug.LogError($"[LeaderboardDisplay] Failed to fetch leaderboard for local player {_leaderboardStat}. Reason {e}");
		});
	}

	private void OnLeaderboardFetched(LeaderboardData leaderboardData)
	{
		PopulateLeaderboardList(leaderboardData.Leaderboard);
	}

	private void OnLocalPlayerLeaderboardFetched(LeaderboardData leaderboardData)
	{
		if (leaderboardData.Leaderboard.Count <= 0 || leaderboardData.Leaderboard[0].StatValue <= 0)
		{
			_localPlayerPlate.PopulateNoStat(_userManager.CurrentUser.DisplayName, isLocalPlayer: true);
		}
		else
		{
			_localPlayerPlate.Populate(leaderboardData.Leaderboard[0], isLocalPlayer: true);
		}
	}

	private void PopulateLeaderboardList(List<PlayerLeaderboardEntry> leaderboardEntries)
	{
		_smartPool.DespawnAllItems();
		foreach (PlayerLeaderboardEntry leaderboardEntry in leaderboardEntries)
		{
			LeaderboardPlate component = _smartPool.SpawnItem().GetComponent<LeaderboardPlate>();
			if (component == null)
			{
				Debug.LogError("[LeaderboardDisplay] Failed to get plate from prefab");
				continue;
			}
			bool isLocalPlayer = _userManager.CurrentUser.Id == leaderboardEntry.PlayFabId;
			component.Populate(leaderboardEntry, isLocalPlayer);
		}
	}
}
