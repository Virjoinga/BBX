using NodeClient;

public struct RespondToInviteRequest : ISocketClientRequest
{
	public string i;

	public bool a;

	public ulong id { get; set; }

	public RespondToInviteRequest(string playerId, bool accepted)
	{
		id = 0uL;
		i = playerId;
		a = accepted;
	}
}
