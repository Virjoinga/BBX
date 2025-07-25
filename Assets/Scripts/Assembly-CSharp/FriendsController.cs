using System.Collections.Generic;
using BSCore;
using UnityEngine;
using Zenject;

public class FriendsController : MonoBehaviour
{
	[Inject]
	private ClientFriendsManager _friendsManager;

	[Inject]
	private GroupManager _groupManager;

	[SerializeField]
	private FriendsList _friendsList;

	[SerializeField]
	private FriendRequestsList _friendRequestsList;

	private void Start()
	{
		Populate(_friendsManager.Friends);
		_friendsManager.FriendsFetched += Populate;
		_friendsManager.FriendStatusUpdated += _friendsList.OnFriendStatusUpdated;
		_groupManager.GroupUpdated += OnGroupUpdated;
		_groupManager.InviteRejected += OnInviteRejected;
	}

	private void OnDestroy()
	{
		_friendsManager.FriendsFetched -= Populate;
		_friendsManager.FriendStatusUpdated -= _friendsList.OnFriendStatusUpdated;
		_groupManager.GroupUpdated -= OnGroupUpdated;
		_groupManager.InviteRejected -= OnInviteRejected;
	}

	private void Populate(List<Friend> friends)
	{
		Debug.Log($"[FriendsList] Populating Friends List with {friends.Count} friends");
		_friendsList.ClearPlates();
		Debug.Log($"[FriendRequestsList] Populating Friend Requests List with {friends.Count} friends");
		_friendRequestsList.ClearPlates();
		foreach (Friend friend in friends)
		{
			if (friend.IsConfirmed)
			{
				_friendsList.Populate(friend, RemoveFriend, InviteFriendToGroup);
			}
			else
			{
				_friendRequestsList.Populate(friend, AcceptFriend, RemoveFriend);
			}
		}
	}

	private void RemoveFriend(string friendId)
	{
		_friendsManager.RemoveFriend(friendId);
	}

	private void AcceptFriend(string friendId)
	{
		_friendsManager.ConfirmFriendRequest(friendId);
	}

	private void InviteFriendToGroup(FriendPlate friendPlate)
	{
		friendPlate.GroupButtonEnabled = false;
		_groupManager.Invite(friendPlate.FriendId, friendPlate.Displayname, delegate(InviteResponse response)
		{
			OnInviteSent(response, friendPlate);
		}, delegate
		{
		});
	}

	private void OnInviteSent(InviteResponse response, FriendPlate friendPlate)
	{
		if (!response.Success)
		{
			friendPlate.GroupButtonEnabled = true;
		}
	}

	private void OnInviteRejected(string playerId)
	{
		_friendsList.UpdateGroupAvailability(playerId);
	}

	private void OnGroupUpdated()
	{
		_friendsList.UpdateGroupAvailability();
	}
}
