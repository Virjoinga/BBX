using System.Collections.Generic;
using System.Linq;

public struct MatchResults
{
	public int winningTeam;

	public Dictionary<int, int> scoresPerTeam;

	public int team0Score
	{
		get
		{
			if (scoresPerTeam != null && scoresPerTeam.ContainsKey(0))
			{
				return scoresPerTeam[0];
			}
			return 0;
		}
	}

	public int team1Score
	{
		get
		{
			if (scoresPerTeam != null && scoresPerTeam.ContainsKey(1))
			{
				return scoresPerTeam[1];
			}
			return 0;
		}
	}

	public string ScoreTextByTeam(int teamId)
	{
		if (scoresPerTeam != null && scoresPerTeam.ContainsKey(teamId))
		{
			return scoresPerTeam[teamId].ToString();
		}
		return "N/A";
	}

	public string EnemyScoreText(int localPlayerTeamId)
	{
		if (scoresPerTeam == null)
		{
			return "N/A";
		}
		int key = scoresPerTeam.Where((KeyValuePair<int, int> kvp) => kvp.Key != localPlayerTeamId).FirstOrDefault().Key;
		if (key == localPlayerTeamId)
		{
			return "N/A";
		}
		return ScoreTextByTeam(key);
	}

	public bool LocalPlayerTeamWon(int localPlayerTeamId)
	{
		if (team0Score == team1Score)
		{
			return true;
		}
		return scoresPerTeam.Aggregate((KeyValuePair<int, int> l, KeyValuePair<int, int> r) => (l.Value <= r.Value) ? r : l).Key == localPlayerTeamId;
	}
}
