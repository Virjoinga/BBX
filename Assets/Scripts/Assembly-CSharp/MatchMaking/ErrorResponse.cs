using System;

namespace MatchMaking
{
	[Serializable]
	public struct ErrorResponse
	{
		public int code;

		public string message;
	}
}
