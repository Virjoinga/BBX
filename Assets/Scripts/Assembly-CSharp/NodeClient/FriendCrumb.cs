using System.Collections.Generic;
using BSCore;

namespace NodeClient
{
	public struct FriendCrumb
	{
		public string i;

		public string n;

		public int s;

		public bool r;

		public bool p;

		public bool c;

		public List<string> t;

		public string Id => i;

		public PlayerStatus Status => (PlayerStatus)s;

		public bool IsRequest => r;

		public bool IsPending => p;

		public bool IsConfirmed => c;

		public List<string> Tags => t;

		public bool IsSteam => Tags.Contains("steam");

		public Friend ToFriend()
		{
			Friend result = new Friend(i, n, Tags);
			result.Status = Status;
			return result;
		}

		public override string ToString()
		{
			return $"FriendCrumb(Id: {i}, Name: {n}, Status: {Status}, IsRequest: {r}, IsPending: {p}, IsConfirmed: {c})";
		}
	}
}
