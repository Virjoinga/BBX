using NodeClient;

public struct PromoteRequest : ISocketClientRequest
{
	public string i;

	public ulong id { get; set; }

	public PromoteRequest(string playerId)
	{
		id = 0uL;
		i = playerId;
	}
}
