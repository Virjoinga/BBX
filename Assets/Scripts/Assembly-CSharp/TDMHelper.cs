using System.Collections.Generic;
using System.Linq;

public static class TDMHelper
{
	public static TeamId GetWinningTeam(List<TDMPlayerState> players)
	{
		List<TDMPlayerState> source = players.Where((TDMPlayerState p) => !string.IsNullOrEmpty(p.EntityId)).ToList();
		List<TDMPlayerState> source2 = source.Where((TDMPlayerState p) => p.Team == 1).ToList();
		List<TDMPlayerState> source3 = source.Where((TDMPlayerState p) => p.Team == 2).ToList();
		int num = source2.Sum((TDMPlayerState p) => p.Kills);
		int num2 = source3.Sum((TDMPlayerState p) => p.Kills);
		if (num == num2)
		{
			return TeamId.Neutral;
		}
		if (num2 <= num)
		{
			return TeamId.Team1;
		}
		return TeamId.Team2;
	}
}
