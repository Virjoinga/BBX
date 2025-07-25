using System;
using System.Collections.Generic;

namespace BSCore
{
	public class ServerFriendsManager
	{
		private ServerPlayFabFriendsService _service;

		public ServerFriendsManager(ServerPlayFabFriendsService friendsService)
		{
			_service = friendsService;
		}

		public void SetTags(string serviceId, string friendId, List<string> tags, Action onComplete, Action<FailureReasons> onfailed)
		{
			_service.SetTags(serviceId, friendId, tags, onComplete, onfailed);
		}

		public void SetTags(string serviceId, Friend friend, List<string> tags, Action onComplete, Action<FailureReasons> onfailed)
		{
			SetTags(serviceId, friend.ServiceId, tags, onComplete, onfailed);
		}
	}
}
