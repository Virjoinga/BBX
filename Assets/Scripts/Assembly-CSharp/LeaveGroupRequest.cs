using NodeClient;

public struct LeaveGroupRequest : ISocketClientRequest
{
	public ulong id { get; set; }
}
