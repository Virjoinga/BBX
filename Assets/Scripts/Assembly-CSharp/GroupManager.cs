using System;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using NodeClient;
using UnityEngine;

public class GroupManager
{
	public readonly Dictionary<string, InvitedRelayCrumb> ReceivedInvites = new Dictionary<string, InvitedRelayCrumb>();

	private UserManager _userManager;

	private SocketClient _socketClient;

	private ChatCommandController _chatCommandController;

	private readonly Dictionary<string, string> _inviteNamesById = new Dictionary<string, string>();

	private static readonly string[] COMMAND_MODES = new string[6] { "list - Lists your current group members", "invite <player_name> - (leader only) Invite a player to join your group", "kick <player_name> - (leader only) Remove a player from your group", "promote <player_name> - (leader only) Make this player your group's leader", "leave - Leave your group", "<text> - Send a chat message to the other members of your group" };

	public Group Group { get; private set; } = Group.Empty;

	public bool IAmLeader => Group.IAmLeader;

	private event Action _groupUpdated;

	public event Action GroupUpdated
	{
		add
		{
			_groupUpdated += value;
		}
		remove
		{
			_groupUpdated -= value;
		}
	}

	private event Action<MatchmakingQueue, string> _matchmakingJoined;

	public event Action<MatchmakingQueue, string> MatchmakingJoined
	{
		add
		{
			_matchmakingJoined += value;
		}
		remove
		{
			_matchmakingJoined -= value;
		}
	}

	private event Action _matchmakingCanceled;

	public event Action MatchmakingCanceled
	{
		add
		{
			_matchmakingCanceled += value;
		}
		remove
		{
			_matchmakingCanceled -= value;
		}
	}

	private event Action<InvitedRelayCrumb> _inviteReceived;

	public event Action<InvitedRelayCrumb> InviteReceived
	{
		add
		{
			_inviteReceived += value;
		}
		remove
		{
			_inviteReceived -= value;
		}
	}

	private event Action<string> _inviteRejected;

	public event Action<string> InviteRejected
	{
		add
		{
			_inviteRejected += value;
		}
		remove
		{
			_inviteRejected -= value;
		}
	}

	public GroupManager(UserManager userManager, SocketClient socketClient, ChatCommandController chatCommandController)
	{
		_userManager = userManager;
		_socketClient = socketClient;
		_socketClient.Disconnected += OnDisconnected;
		_chatCommandController = chatCommandController;
		SetupChatCommand();
		_socketClient.OnResponse<InviteResponse>(EventNames.InviteResponse);
		_socketClient.OnResponse<RespondToInviteResponse>(EventNames.RespondToInviteResponse);
		_socketClient.OnResponse<LeaveGroupResponse>(EventNames.LeaveGroupResponse);
		_socketClient.OnResponse<RelayMatchmakingTicketIdResponse>(EventNames.RelayMatchmakingTicketIdResponse);
		_socketClient.OnResponse<PromoteResponse>(EventNames.PromoteResponse);
		_socketClient.OnResponse<RemoveResponse>(EventNames.RemoveResponse);
		_socketClient.On(EventNames.Invited, OnInvited);
		_socketClient.On(EventNames.InviteRejected, OnInviteRejected);
		_socketClient.On(EventNames.GroupUpdated, OnGroupUpdated);
		_socketClient.On(EventNames.MatchmakingJoined, OnMatchmakingJoined);
		_socketClient.On(EventNames.MatchmakingCanceled, OnMatchmakingCanceled);
	}

	public bool CanInvite(string playerId)
	{
		if (playerId != _socketClient.ServiceId && Group.Members.Count < 4)
		{
			return !Group.Members.Any((GroupMember m) => m.Id == playerId);
		}
		return false;
	}

	public void InviteByDisplayName(string displayName, Action<InviteResponse> onResponse = null, Action<string> onFailure = null)
	{
		_userManager.FetchServiceIdByDisplayName(displayName, delegate(string serviceId)
		{
			Invite(serviceId, displayName, onResponse, onFailure);
		}, delegate
		{
			string text = "Failed to invite player. Unable to find a player named " + displayName;
			onFailure?.Invoke(text);
			SendSystemMessage(text);
		});
	}

	public void Invite(string playerId, string displayName, Action<InviteResponse> onResponse = null, Action<string> onFailure = null)
	{
		if (!_socketClient.IsConnected)
		{
			string text = "Not connected to the server.";
			onFailure?.Invoke(text);
			SendSystemMessage(text);
			return;
		}
		if (Group.IsValid && Group.Members.Count >= 4)
		{
			string text2 = "Your group is full.";
			onFailure?.Invoke(text2);
			SendSystemMessage(text2);
			return;
		}
		if (_inviteNamesById.ContainsKey(playerId))
		{
			string text3 = "You have already invited " + displayName + " to your group.";
			onFailure?.Invoke(text3);
			SendSystemMessage(text3);
			return;
		}
		Debug.Log("[GroupManager] Inviting " + displayName + "(" + playerId + ") to join your group");
		_inviteNamesById.Add(playerId, displayName);
		Action<InviteResponse> onResponse2 = delegate(InviteResponse response)
		{
			OnInviteResponse(response);
			onResponse?.Invoke(response);
		};
		Action<string> onFailure2 = delegate(string error)
		{
			SendSystemMessage("Cannot invite " + displayName + ": " + error);
			onFailure?.Invoke(error);
		};
		_socketClient.Request(EventNames.Invite, new InviteRequest(playerId), onResponse2, onFailure2);
	}

	public void RespondToInvite(InvitedRelayCrumb crumb, bool accepted, Action<RespondToInviteResponse> onResponse = null, Action<string> onFailure = null)
	{
		if (!_socketClient.IsConnected)
		{
			string text = "Not connected to the server";
			onFailure?.Invoke(text);
			SendSystemMessage(text);
			return;
		}
		if (Group.IsValid)
		{
			accepted = false;
		}
		Action<RespondToInviteResponse> onResponse2 = delegate(RespondToInviteResponse response)
		{
			OnRespondToInviteResponse(response);
			onResponse?.Invoke(response);
		};
		Action<string> onFailure2 = delegate(string error)
		{
			SendSystemMessage(error);
			onFailure?.Invoke(error);
		};
		_socketClient.Request(EventNames.RespondToInvite, new RespondToInviteRequest(crumb.InviterId, accepted), onResponse2, onFailure2);
		if (accepted)
		{
			ReceivedInvites.Clear();
		}
		else
		{
			ReceivedInvites.Remove(crumb.InviterId);
		}
	}

	public void LeaveGroup(Action<LeaveGroupResponse> onResponse = null, Action<string> onFailure = null)
	{
		if (Group.IsValid)
		{
			Debug.Log("[GroupManager] Leaving group");
			Action<LeaveGroupResponse> onResponse2 = delegate(LeaveGroupResponse response)
			{
				OnLeaveGroupResponse(response);
				onResponse?.Invoke(response);
			};
			Action<string> onFailure2 = delegate(string error)
			{
				SendSystemMessage(error);
				onFailure?.Invoke(error);
			};
			_socketClient.Request(EventNames.LeaveGroup, default(LeaveGroupRequest), onResponse2, onFailure2);
		}
		else
		{
			string text = "You are not in a group.";
			onFailure?.Invoke(text);
			SendSystemMessage(text);
		}
	}

	public void RemoveByDisplayName(string displayName, Action<RemoveResponse> onResponse = null, Action<string> onFailure = null)
	{
		if (Group.IsValid)
		{
			GroupMember groupMember = Group.Members.FirstOrDefault((GroupMember m) => m.DisplayName.ToLower() == displayName.ToLower());
			if (!string.IsNullOrEmpty(groupMember.Id))
			{
				Remove(groupMember.Id, onResponse, onFailure);
				return;
			}
			string text = displayName + " is not in your group";
			onFailure?.Invoke(text);
			SendSystemMessage(text);
		}
		else
		{
			string text2 = "You are not in a group.";
			onFailure?.Invoke(text2);
			SendSystemMessage(text2);
		}
	}

	public void Remove(string playerId, Action<RemoveResponse> onResponse = null, Action<string> onFailure = null)
	{
		if (Group.IsValid)
		{
			if (Group.Members.Any((GroupMember m) => m.Id == playerId))
			{
				Debug.Log("[GroupManager] Removing " + playerId + " from the group");
				Action<RemoveResponse> onResponse2 = delegate(RemoveResponse response)
				{
					OnRemoveResponse(response);
					onResponse?.Invoke(response);
				};
				Action<string> onFailure2 = delegate(string error)
				{
					SendSystemMessage(error);
					onFailure?.Invoke(error);
				};
				_socketClient.Request(EventNames.Remove, new RemoveRequest(playerId), onResponse2, onFailure2);
			}
			else
			{
				string text = "Cannot find player to remove.";
				onFailure?.Invoke(text);
				SendSystemMessage(text);
			}
		}
		else
		{
			string text2 = "You are not in a group.";
			onFailure?.Invoke(text2);
			SendSystemMessage(text2);
		}
	}

	public void RelayMatchmakingTicketId(MatchmakingQueue queue, string ticketId, Action<RelayMatchmakingTicketIdResponse> onResponse = null, Action<string> onFailure = null)
	{
		if (Group.IsValid)
		{
			Action<RelayMatchmakingTicketIdResponse> onResponse2 = delegate(RelayMatchmakingTicketIdResponse response)
			{
				OnRelayMatchmakingTicketIdResponse(response);
				onResponse?.Invoke(response);
			};
			Action<string> onFailure2 = delegate(string error)
			{
				SendSystemMessage(error);
				onFailure?.Invoke(error);
			};
			_socketClient.Request(EventNames.RelayMatchmakingTicketId, new RelayMatchmakingTicketIdRequest(queue, ticketId), onResponse2, onFailure2);
		}
	}

	public void RelayMatchmakingCanceled(Action<RelayMatchmakingCanceledResponse> onResponse = null, Action<string> onFailure = null)
	{
		if (Group.IsValid)
		{
			Action<RelayMatchmakingCanceledResponse> onResponse2 = delegate(RelayMatchmakingCanceledResponse response)
			{
				OnMatchmakingCanceledResponse(response);
				onResponse?.Invoke(response);
			};
			Action<string> onFailure2 = delegate(string error)
			{
				SendSystemMessage(error);
				onFailure?.Invoke(error);
			};
			_socketClient.Request(EventNames.RelayMatchmakingCanceled, default(RelayMatchmakingCanceledRequest), onResponse2, onFailure2);
		}
	}

	public void PromoteByDisplayName(string displayName, Action<PromoteResponse> onResponse = null, Action<string> onFailure = null)
	{
		if (Group.IsValid)
		{
			GroupMember groupMember = Group.Members.FirstOrDefault((GroupMember m) => m.DisplayName.ToLower() == displayName.ToLower());
			if (!string.IsNullOrEmpty(groupMember.Id))
			{
				Promote(groupMember.Id, onResponse, onFailure);
				return;
			}
			string text = displayName + " is not in your group.";
			onFailure?.Invoke(text);
			SendSystemMessage(text);
		}
		else
		{
			string text2 = "You are not in a group.";
			onFailure?.Invoke(text2);
			SendSystemMessage(text2);
		}
	}

	public void Promote(string playerId, Action<PromoteResponse> onResponse = null, Action<string> onFailure = null)
	{
		if (Group.IsValid)
		{
			if (Group.Members.Any((GroupMember m) => m.Id == playerId))
			{
				Debug.Log("[GroupManager] Promoting " + playerId + " to leader of the group");
				Action<PromoteResponse> onResponse2 = delegate(PromoteResponse response)
				{
					OnPromoteResponse(response);
					onResponse?.Invoke(response);
				};
				Action<string> onFailure2 = delegate(string error)
				{
					SendSystemMessage(error);
					onFailure?.Invoke(error);
				};
				_socketClient.Request(EventNames.Promote, new PromoteRequest(playerId), onResponse2, onFailure2);
			}
			else
			{
				string text = "Cannot find player to promote.";
				onFailure?.Invoke(text);
				SendSystemMessage(text);
			}
		}
		else
		{
			string text2 = "You are not in a group.";
			onFailure?.Invoke(text2);
			SendSystemMessage(text2);
		}
	}

	private void OnInviteResponse(InviteResponse response)
	{
		if (_inviteNamesById.TryGetValue(response.RecipientId, out var value))
		{
			Debug.Log("[GroupManager] OnInviteResponse: " + SocketClient.ToJson(response));
			if (response.Success)
			{
				SendSystemMessage(value + " has been invited to your group");
			}
			else
			{
				SendSystemMessage("Error inviting " + value + ". " + ErrorToHumanReadable(response.Error));
			}
			_inviteNamesById.Remove(response.RecipientId);
		}
	}

	private void OnRespondToInviteResponse(RespondToInviteResponse response)
	{
		if (!response.Success)
		{
			SendSystemMessage(response.ErrorMsg);
			Debug.LogError("[GroupManager] Error responding to invite: " + response.ErrorMsg);
		}
	}

	private void OnLeaveGroupResponse(LeaveGroupResponse response)
	{
		if (!response.Success)
		{
			string text = "";
			SendSystemMessage(text);
			Debug.LogError("[GroupManager] Error leaving group: " + text);
		}
	}

	private void OnRelayMatchmakingTicketIdResponse(RelayMatchmakingTicketIdResponse response)
	{
		if (!response.Success)
		{
			string text = "";
			SendSystemMessage(text);
			Debug.LogError("[GroupManager] Error relaying matchmaking ticketId: " + text);
		}
	}

	private void OnMatchmakingCanceledResponse(RelayMatchmakingCanceledResponse response)
	{
		if (!response.Success)
		{
			string text = "";
			SendSystemMessage(text);
			Debug.LogError("[GroupManager] Error relaying matchmakking canceled: " + text);
		}
	}

	private void OnPromoteResponse(PromoteResponse response)
	{
		if (!response.Success)
		{
			string text = "";
			SendSystemMessage(text);
			Debug.LogError("[GroupManager] Error promoting another player: " + text);
		}
	}

	private void OnRemoveResponse(RemoveResponse response)
	{
		if (!response.Success)
		{
			string text = "";
			SendSystemMessage(text);
			Debug.LogError("[GroupManager] Error removing another player from your group: " + text);
		}
	}

	private void OnInvited(string payload, object[] args)
	{
		InvitedRelayCrumb invitedRelayCrumb = SocketClient.ConvertTo<InvitedRelayCrumb>(args[0]);
		ReceivedInvites.Add(invitedRelayCrumb.InviterId, invitedRelayCrumb);
		this._inviteReceived?.Invoke(invitedRelayCrumb);
		SendSystemMessage("You have been invited to join a group by " + invitedRelayCrumb.InviterName);
	}

	private void OnInviteRejected(string payload, object[] args)
	{
		InviteRejectedCrumb inviteRejectedCrumb = SocketClient.ConvertTo<InviteRejectedCrumb>(args[0]);
		if (_inviteNamesById.TryGetValue(inviteRejectedCrumb.RecipientId, out var value))
		{
			SendSystemMessage(value + " has declined your group invite.");
			_inviteNamesById.Remove(inviteRejectedCrumb.RecipientId);
			this._inviteRejected?.Invoke(inviteRejectedCrumb.RecipientId);
		}
	}

	private void OnGroupUpdated(string payload, object[] args)
	{
		Group newGroup = SocketClient.ConvertTo<GroupCrumb>(args[0]).ToGroup(_socketClient.ServiceId);
		Group obj = Group;
		Group = newGroup;
		if (!newGroup.IsValid && !obj.IsValid)
		{
			return;
		}
		if (newGroup.IsValid && obj.IsValid)
		{
			IEnumerable<string> enumerable = obj.Members?.Select((GroupMember m) => m.DisplayName) ?? new string[0];
			IEnumerable<string> enumerable2 = newGroup.Members?.Select((GroupMember m) => m.DisplayName) ?? new string[0];
			foreach (string item in enumerable)
			{
				if (!enumerable2.Contains(item))
				{
					SendSystemMessage(item + " has left the group.");
				}
			}
			foreach (string item2 in enumerable2)
			{
				if (!enumerable.Contains(item2))
				{
					SendSystemMessage(item2 + " has joined the group.");
				}
			}
			if (obj.LeaderId != newGroup.LeaderId)
			{
				GroupMember groupMember = newGroup.Members.FirstOrDefault((GroupMember m) => m.Id == newGroup.LeaderId);
				if (!string.IsNullOrEmpty(groupMember.Id))
				{
					SendSystemMessage(groupMember.DisplayName + " has been promoted to group leader.");
				}
			}
		}
		else if (newGroup.IsValid && !obj.IsValid)
		{
			SendSystemMessage("You have joined a group");
		}
		else if (!newGroup.IsValid && obj.IsValid)
		{
			SendSystemMessage("You have left your group");
		}
		this._groupUpdated?.Invoke();
		foreach (GroupMember member in Group.Members)
		{
			if (_inviteNamesById.TryGetValue(member.Id, out var _))
			{
				_inviteNamesById.Remove(member.Id);
			}
		}
	}

	private void OnMatchmakingJoined(string payload, object[] args)
	{
		MatchmakingJoinedCrumb matchmakingJoinedCrumb = SocketClient.ConvertTo<MatchmakingJoinedCrumb>(args[0]);
		Debug.Log($"[GroupManager] Leader joined matchmaking queue \"{matchmakingJoinedCrumb.Queue}\" with ticket: {matchmakingJoinedCrumb.TicketId}");
		this._matchmakingJoined?.Invoke(matchmakingJoinedCrumb.Queue, matchmakingJoinedCrumb.TicketId);
		SendSystemMessage($"Your group has joined the {matchmakingJoinedCrumb.Queue} matchmaking queue");
	}

	private void OnMatchmakingCanceled(string payload, object[] args)
	{
		this._matchmakingCanceled?.Invoke();
		SendSystemMessage("Your group has been removed from matchmaking");
	}

	private void SetupChatCommand()
	{
		_chatCommandController.RegisterChatCommand("group", HandleChatCommand, "Manage your group", COMMAND_MODES);
	}

	private bool HandleChatCommand(string[] args)
	{
		if (args.Length == 0)
		{
			_chatCommandController.HandleChatCommand("/help group");
		}
		else if (args.Length == 1)
		{
			string text = args[0];
			if (!(text == "list"))
			{
				if (text == "leave")
				{
					LeaveGroup(OnLeaveGroupResponse);
				}
				else
				{
					SendGroupChatMessage(string.Join(" ", args));
				}
			}
			else
			{
				ListGroupMembers();
			}
		}
		else if (args.Length == 2)
		{
			switch (args[0])
			{
			case "invite":
				InviteByDisplayName(args[1], OnInviteResponse);
				break;
			case "kick":
				RemoveByDisplayName(args[1], OnRemoveResponse);
				break;
			case "promote":
				PromoteByDisplayName(args[1], OnPromoteResponse);
				break;
			default:
				SendGroupChatMessage(string.Join(" ", args));
				break;
			}
		}
		else
		{
			SendGroupChatMessage(string.Join(" ", args));
		}
		return true;
	}

	private void ListGroupMembers()
	{
		if (Group.IsValid)
		{
			SendSystemMessage("Group members: " + string.Join(", ", Group.Members.Select((GroupMember m) => m.DisplayName)));
		}
		else
		{
			SendSystemMessage("You are not in a group");
		}
	}

	private void SendGroupChatMessage(string message)
	{
		if (Group.IsValid)
		{
			_chatCommandController.ChatClient.SendPublicMessage(Group.Id, message);
		}
	}

	private void SendSystemMessage(string message)
	{
		_chatCommandController.ChatClient.SystemChatRoom.DisplayMessage(message);
	}

	private string ErrorToHumanReadable(GroupErrorCode error)
	{
		switch (error)
		{
		case GroupErrorCode.FullGroup:
			return "Your group is full";
		case GroupErrorCode.UnknownPlayer:
			return "Unknown player";
		case GroupErrorCode.NotInGroup:
			return "Not in your group";
		case GroupErrorCode.NotLeader:
			return "You are not the group leader";
		case GroupErrorCode.AlreadyInGroup:
			return "Already in your group";
		default:
			return "Unknown error";
		}
	}

	private void OnDisconnected()
	{
		if (Group.IsValid)
		{
			Group = Group.Empty;
			this._groupUpdated?.Invoke();
		}
		_inviteNamesById.Clear();
		SendSystemMessage("You have left the group.");
	}
}
