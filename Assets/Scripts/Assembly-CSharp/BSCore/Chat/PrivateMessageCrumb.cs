namespace BSCore.Chat
{
	public class PrivateMessageCrumb
	{
		public string i;

		public string m;

		public string sn;

		public string senderId => i;

		public string message => m;

		public string senderNickname => sn;

		public PrivateMessage ToMessage(string myId)
		{
			return new PrivateMessage(message, senderId, senderNickname, myId);
		}

		public override string ToString()
		{
			return $"{senderNickname}({senderId}) -> me:[{message}]";
		}
	}
}
