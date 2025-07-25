using System;
using System.Collections.Generic;

namespace MatchMaking
{
	[Serializable]
	public class Match
	{
		public string matchId;

		public List<MatchedMember> members;
	}
}
