using System;

namespace NodeClient
{
	[Serializable]
	public class UpdatePlayerInChannel
	{
		public string c;

		public PlayerCrumb p;

		public string channelId => c;

		public PlayerCrumb player => p;
	}
}
