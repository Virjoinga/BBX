using System;

namespace NodeClient
{
	[Serializable]
	public class LeftChannel
	{
		public string c;

		public string channelId => c;
	}
}
