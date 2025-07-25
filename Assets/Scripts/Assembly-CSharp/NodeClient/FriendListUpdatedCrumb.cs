using System.Collections.Generic;
using System.Linq;

namespace NodeClient
{
	public struct FriendListUpdatedCrumb
	{
		public List<FriendCrumb> friends;

		public override string ToString()
		{
			string text = string.Join("\n", friends.Select((FriendCrumb f) => f.ToString()));
			return "FriendListUpdatedCrumb([" + text + "])";
		}
	}
}
