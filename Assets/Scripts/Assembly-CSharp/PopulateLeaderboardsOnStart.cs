using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulateLeaderboardsOnStart : MonoBehaviour
{
	[SerializeField]
	private List<LeaderboardDisplay> _leaderboardDisplays;

	private void Reset()
	{
		_leaderboardDisplays = GetComponentsInChildren<LeaderboardDisplay>().ToList();
	}

	private void Start()
	{
		foreach (LeaderboardDisplay leaderboardDisplay in _leaderboardDisplays)
		{
			leaderboardDisplay.FetchLeaderboard();
		}
	}
}
