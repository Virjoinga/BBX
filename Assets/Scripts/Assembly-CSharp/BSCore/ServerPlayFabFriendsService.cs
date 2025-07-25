using System;
using System.Collections.Generic;

namespace BSCore
{
	public class ServerPlayFabFriendsService : PlayFabService
	{
		public void SetTags(string playFabId, string friendId, List<string> tags, Action onSuccess, Action<FailureReasons> onFailure)
		{
		}
	}
}
