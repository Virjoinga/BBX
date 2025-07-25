using NodeClient;

public struct LeaveGroupResponse : ISocketClientResponse
{
	public ulong id;

	public bool s;

	public int e;

	public ulong Id => id;

	public bool Success => s;

	public GroupErrorCode Error => (GroupErrorCode)e;

	public string ErrorMsg => Error.ToString();
}
