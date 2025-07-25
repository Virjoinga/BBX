using NodeClient;

namespace BSCore.Chat
{
	public class SystemMessage : Message
	{
		public SystemMessage(string message)
			: base("system", message, "system", "system", isAdmin: true)
		{
		}
	}
}
