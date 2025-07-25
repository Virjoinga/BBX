using NodeClient;

public struct RespondToInviteResponse : ISocketClientResponse
{
	public ulong id;

	public string i;

	public bool s;

	public int e;

	public ulong Id => id;

	public string RecipientId => i;

	public bool Success => s;

	public GroupErrorCode Error => (GroupErrorCode)e;

	public string ErrorMsg => Error.ToString();
}
