using System;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;

namespace BSCore
{
	public class PlayFabFriendsService : PlayFabService
	{
		public void Fetch(Action<List<Friend>> onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
			{
				IncludeSteamFriends = false,
				IncludeFacebookFriends = false
			}, errorCallback: OnFailureCallback(delegate
			{
				Fetch(onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: delegate(GetFriendsListResult response)
			{
				onSuccess(ConvertToFriends(response.Friends));
			});
		}

		private List<Friend> ConvertToFriends(List<FriendInfo> friendInfos)
		{
			return friendInfos.Select(ConvertToFriend).ToList();
		}

		private Friend ConvertToFriend(FriendInfo friendInfo)
		{
			return new Friend(friendInfo.FriendPlayFabId, friendInfo.TitleDisplayName, friendInfo.Tags);
		}
	}
}
