using NodeClient;

public struct RelayMatchmakingTicketIdRequest : ISocketClientRequest
{
	public string t;

	public int q;

	public ulong id { get; set; }

	public RelayMatchmakingTicketIdRequest(MatchmakingQueue queue, string ticketId)
	{
		id = 0uL;
		t = ticketId;
		q = (int)queue;
	}
}
