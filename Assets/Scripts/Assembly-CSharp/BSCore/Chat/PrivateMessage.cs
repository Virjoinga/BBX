using NodeClient;

namespace BSCore.Chat
{
	public class PrivateMessage : Message
	{
		public bool isMe { get; private set; }

		public PrivateMessage(string message, string senderNickname, string channelId, string myId)
			: base(myId, message, senderNickname, channelId, isAdmin: false)
		{
			isMe = base.senderId == myId;
		}
	}
}
