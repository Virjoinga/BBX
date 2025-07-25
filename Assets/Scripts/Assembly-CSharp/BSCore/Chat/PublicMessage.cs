using NodeClient;

namespace BSCore.Chat
{
	public class PublicMessage : Message
	{
		public bool isMe { get; private set; }

		public PublicMessage(string senderId, string message, string senderNickname, string channelId, bool isAdmin, string myId)
			: base(channelId, message, senderId, senderNickname, isAdmin)
		{
			isMe = senderId == myId;
		}
	}
}
