using NodeClient;

public struct RelayMatchmakingCanceledRequest : ISocketClientRequest
{
	public ulong id { get; set; }
}
