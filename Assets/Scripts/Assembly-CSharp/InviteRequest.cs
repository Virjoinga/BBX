using NodeClient;

public struct InviteRequest : ISocketClientRequest
{
	public string i;

	public ulong id { get; set; }

	public InviteRequest(string playerId)
	{
		id = 0uL;
		i = playerId;
	}
}
