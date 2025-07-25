using System;

namespace MatchMaking
{
	[Serializable]
	public struct Latency
	{
		public string region;

		public int latency;

		public Latency(string region, int latency)
		{
			this.region = region;
			this.latency = latency;
		}
	}
}
