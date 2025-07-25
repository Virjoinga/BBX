using System;

namespace MatchMaking
{
	[Serializable]
	public struct PlayerAttribute
	{
		public string playerId;

		public string entityId;

		public int skill;

		public int playerCount;

		public string buildVersion;

		public string displayName;

		public string outfit;

		public Latency[] latency;

		public PlayerAttribute(string playerId, string entityId, int skill, int playerCount, string buildVersion, string displayName, string outfit, Latency[] latency)
		{
			this.playerId = playerId;
			this.entityId = entityId;
			this.skill = skill;
			this.playerCount = playerCount;
			this.buildVersion = buildVersion;
			this.displayName = displayName;
			this.outfit = outfit;
			this.latency = latency;
		}
	}
}
