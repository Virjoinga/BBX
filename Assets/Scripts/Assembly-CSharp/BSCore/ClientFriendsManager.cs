using System;
using System.Collections.Generic;
using System.Linq;
using NodeClient;
using UnityEngine;

namespace BSCore
{
	public class ClientFriendsManager
	{
		private static readonly string[] COMMAND_MODES = new string[3] { "list - List all friends", "add <player_name> - Add <player_name> as a friend", "remove <player_name> - Remove <player_name> from your friends list" };

		private ChatCommandController _chatCommandController;

		private Dictionary<string, Friend> _friends = new Dictionary<string, Friend>();

		private SocketClient _socketClient;

		private UserManager _userManager;

		public List<Friend> Friends => new List<Friend>(_friends.Values);

		private event Action<List<Friend>> _friendsFetched;

		public event Action<List<Friend>> FriendsFetched
		{
			add
			{
				_friendsFetched += value;
			}
			remove
			{
				_friendsFetched -= value;
			}
		}

		private event Action<string, PlayerStatus> _friendStatusUpdated;

		public event Action<string, PlayerStatus> FriendStatusUpdated
		{
			add
			{
				_friendStatusUpdated += value;
			}
			remove
			{
				_friendStatusUpdated -= value;
			}
		}

		public ClientFriendsManager(UserManager userManager, ChatCommandController chatCommandController, SocketClient socketClient)
		{
			_userManager = userManager;
			_chatCommandController = chatCommandController;
			_chatCommandController.RegisterChatCommand("friend", HandleCommand, "Manage your list of friends", COMMAND_MODES);
			_socketClient = socketClient;
			_socketClient.On(EventNames.FriendListUpdated, OnFriendListUpdated);
			_socketClient.On(EventNames.FriendStatusUpdated, OnFriendStatusUpdated);
		}

		public Friend GetFriend(string friendId)
		{
			_friends.TryGetValue(friendId, out var value);
			return value;
		}

		public bool IsFriendByDisplayName(string displayName)
		{
			return _friends.Values.Any((Friend x) => x.DisplayName == displayName);
		}

		public void SendFriendRequestByName(string displayName, Action onComplete, Action<FailureReasons> onFailure)
		{
			Debug.Log("[ClientFriendsManager] Fetching serviceId for player " + displayName + " in order to send friend request");
			_userManager.FetchServiceIdByDisplayName(displayName, delegate(string serviceId)
			{
				onComplete();
				SendFriendRequest(serviceId);
			}, onFailure);
		}

		public void SendFriendRequest(string friendId)
		{
			Debug.Log("[ClientFriendsManager] Sending friend request to " + friendId);
			_socketClient.Emit(EventNames.RequestFriend, new RequestFriendRequest(friendId));
		}

		public void ConfirmFriendRequest(string friendId)
		{
			Debug.Log("[ClientFriendsManager] Sending friend request to " + friendId);
			_socketClient.Emit(EventNames.ConfirmFriend, new ConfirmFriendRequest(friendId));
		}

		public void RemoveFriendByDisplayName(string displayName, Action onComplete, Action<FailureReasons> onFailure)
		{
			Debug.Log("[ClientFriendsManager] Fetching serviceId for player " + displayName + " in order to remove friend");
			_userManager.FetchServiceIdByDisplayName(displayName, delegate(string serviceId)
			{
				onComplete();
				RemoveFriend(serviceId);
			}, onFailure);
		}

		public void RemoveFriend(string friendId)
		{
			RemoveFriendRequest removeFriendRequest = new RemoveFriendRequest(friendId);
			_socketClient.Emit(EventNames.RemoveFriend, removeFriendRequest);
		}

		private void UpdateFriendsMap(List<Friend> friends)
		{
			_friends.Clear();
			foreach (Friend friend in friends)
			{
				_friends.Add(friend.ServiceId, friend);
			}
		}

		private void OnFriendListUpdated(string payload, object[] args)
		{
			FriendListUpdatedCrumb friendListUpdatedCrumb = SocketClient.ConvertTo<FriendListUpdatedCrumb>(args[0]);
			List<Friend> list = friendListUpdatedCrumb.friends.Select((FriendCrumb f) => f.ToFriend()).ToList();
			Debug.Log($"[ClientFriendsManager] Friends list updated: {friendListUpdatedCrumb}");
			UpdateFriendsMap(list);
			this._friendsFetched?.Invoke(list);
		}

		private void OnFriendStatusUpdated(string payload, object[] args)
		{
			FriendStatusUpdatedCrumb friendStatusUpdatedCrumb = SocketClient.ConvertTo<FriendStatusUpdatedCrumb>(args[0]);
			Debug.Log($"[ClientFriendsManager] Friends status updated: {friendStatusUpdatedCrumb}");
			if (_friends.TryGetValue(friendStatusUpdatedCrumb.Id, out var value))
			{
				value.Status = friendStatusUpdatedCrumb.Status;
				_friends[friendStatusUpdatedCrumb.Id] = value;
			}
			this._friendStatusUpdated?.Invoke(friendStatusUpdatedCrumb.Id, friendStatusUpdatedCrumb.Status);
		}

		private bool HandleCommand(string[] args)
		{
			if (args.Length == 0)
			{
				return false;
			}
			switch (args[0])
			{
			case "list":
			{
				string message = "Friends:\n" + string.Join("\n", _friends.Values.Select((Friend f) => f.DisplayName));
				SendSystemMessage(message);
				break;
			}
			case "add":
				if (args.Length < 1)
				{
					return false;
				}
				SendFriendRequestByName(args[1], delegate
				{
					SendSystemMessage("Friend request sent to " + args[1]);
				}, delegate(FailureReasons error)
				{
					SendSystemMessage($"Error sending friend request: {error}");
				});
				break;
			case "remove":
				if (args.Length < 1)
				{
					return false;
				}
				RemoveFriendByDisplayName(args[1], delegate
				{
					SendSystemMessage("Friend " + args[1] + " removed");
				}, delegate(FailureReasons error)
				{
					SendSystemMessage($"Error removing {args[1]} as a friend: {error}");
				});
				break;
			default:
				return false;
			}
			return true;
		}

		private void SendSystemMessage(string message)
		{
			_chatCommandController.ChatClient.SystemChatRoom.DisplayMessage(message);
		}
	}
}
