public struct LeaderboardUpdatedSignal
{
	public int index;

	public SurvivalPlayer player;

	public LeaderboardUpdatedSignal(SurvivalPlayer player, int index)
	{
		this.index = index;
		this.player = player;
	}
}
