namespace NodeClient
{
	public interface ISocketClientResponse
	{
		ulong Id { get; }

		bool Success { get; }

		string ErrorMsg { get; }
	}
}
