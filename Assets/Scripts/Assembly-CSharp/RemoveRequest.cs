using NodeClient;

public struct RemoveRequest : ISocketClientRequest
{
	public string i;

	public ulong id { get; set; }

	public RemoveRequest(string playerId)
	{
		id = 0uL;
		i = playerId;
	}
}
