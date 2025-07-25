namespace NodeClient
{
	public class SocketErrorSignal
	{
		public SocketClient.ErrorType ErrorType;

		public SocketErrorSignal(SocketClient.ErrorType errorType)
		{
			ErrorType = errorType;
		}
	}
}
