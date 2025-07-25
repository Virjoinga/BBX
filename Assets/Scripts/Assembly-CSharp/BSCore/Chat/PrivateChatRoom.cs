namespace BSCore.Chat
{
	public class PrivateChatRoom : BaseChatRoom
	{
		public override string Id => "Private";

		public PrivateChatRoom(ChatClient chatClient, GameConfigData gameConfigData)
			: base(gameConfigData.PrivateMessageColor, chatClient, gameConfigData)
		{
		}

		~PrivateChatRoom()
		{
		}

		public void SendMessage(string recipientId, string message)
		{
			_chatClient.SendPrivateMessage(recipientId, message);
		}

		public void OnMessageReceived(PrivateMessage message)
		{
			string text = string.Format("{0} {1}", message.isMe ? "To" : message.senderNickname, message.isMe ? message.senderNickname : "whispered");
			RaiseMessageReceived("<font=\"American Captain SDF\">" + text + "</font>: ", message.message);
		}
	}
}
