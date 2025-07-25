namespace NodeClient
{
	public class Message
	{
		public string senderId { get; protected set; }

		public string message { get; protected set; }

		public string senderNickname { get; protected set; }

		public string channelId { get; protected set; }

		public bool isAdmin { get; protected set; }

		public Message(string channelId, string message, string senderId, string senderNickname, bool isAdmin)
		{
			this.channelId = channelId;
			this.message = message;
			this.senderId = senderId;
			this.senderNickname = senderNickname;
			this.isAdmin = isAdmin;
		}
	}
}
