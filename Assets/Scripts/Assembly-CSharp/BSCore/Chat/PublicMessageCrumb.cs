namespace BSCore.Chat
{
	public class PublicMessageCrumb
	{
		public string i;

		public string m;

		public string c;

		public bool a;

		public string senderId => i;

		public string message => m;

		public string channelId => c;

		public bool isAdmin => a;

		public PublicMessage ToMessage(string senderNickname, string myId)
		{
			return new PublicMessage(senderId, message, senderNickname, channelId, isAdmin, myId);
		}

		public override string ToString()
		{
			return $"{senderId} -> {channelId}:[{message}]";
		}
	}
}
