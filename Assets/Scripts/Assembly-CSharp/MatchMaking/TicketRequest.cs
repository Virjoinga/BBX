using System;

namespace MatchMaking
{
	[Serializable]
	public struct TicketRequest
	{
		public string queueName;

		public string titleId;

		public int timeout;

		public PlayerAttribute playerAttributes;

		public TicketRequest(string queueName, string titleId, int timeout, PlayerAttribute playerAttributes)
		{
			this.queueName = queueName;
			this.titleId = titleId;
			this.timeout = timeout;
			this.playerAttributes = playerAttributes;
		}
	}
}
