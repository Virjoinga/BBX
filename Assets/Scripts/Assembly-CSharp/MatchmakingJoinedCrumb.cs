public struct MatchmakingJoinedCrumb
{
	public string t;

	public int q;

	public string TicketId => t;

	public MatchmakingQueue Queue => (MatchmakingQueue)q;
}
