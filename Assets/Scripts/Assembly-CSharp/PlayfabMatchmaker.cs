using System;
using System.Collections;
using System.Collections.Generic;
using BSCore;
using BSCore.Constants.Config;
using NodeClient;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using UnityEngine;
using Zenject;

public class PlayfabMatchmaker : PlayFabService
{
	private const float TICKET_POLLING_RATE = 10f;

	private const float DATA_REFRESH_TIMEOUT = 60f;

	[Inject]
	private SocketClient _socketClient;

	private UserManager _userManager;

	private ConfigManager _configManager;

	private ProfileManager _profileManager;

	private GameConfigData _gameConfigData;

	private MatchmakingQueue _activeQueue;

	private DelayedAction.DelayedActionHandle _pollingRoutineHandle;

	private float _lastDataRefreshTime;

	private int _timeout = 120;

	public bool IsMatchmaking { get; private set; }

	public string ActiveTicketId { get; private set; }

	private event Action<GetMatchResult> _matchFound;

	public event Action<GetMatchResult> MatchFound
	{
		add
		{
			_matchFound += value;
		}
		remove
		{
			_matchFound -= value;
		}
	}

	private event Action<string> _matchmakingFailed;

	public event Action<string> MatchmakingFailed
	{
		add
		{
			_matchmakingFailed += value;
		}
		remove
		{
			_matchmakingFailed -= value;
		}
	}

	private void RaiseMatchMakingFailed(string error)
	{
		IsMatchmaking = false;
		this._matchmakingFailed?.Invoke(error);
		_socketClient.UpdateStatus(PlayerStatus.InLobby);
	}

	[Inject]
	public PlayfabMatchmaker(UserManager userManager, ConfigManager configManager, ProfileManager profileManager, GameConfigData gameConfigData)
	{
		_userManager = userManager;
		_profileManager = profileManager;
		_gameConfigData = gameConfigData;
		_configManager = configManager;
		_configManager.Fetched += OnConfigManagerFetched;
		if (_configManager.HasFetched)
		{
			OnConfigManagerFetched();
		}
	}

	private void OnConfigManagerFetched()
	{
		_timeout = _configManager.Get(DataKeys.matchmakerTimeout, 120);
	}

	public void StartMatchmaking(MatchmakingQueue matchmakingQueue, List<PlayFab.MultiplayerModels.EntityKey> memberEntityIds, Action<MatchmakingQueue> onJoinedQueue)
	{
		Debug.Log($"[PlayfabMatchmaker] Starting Matchmaking for Queue {matchmakingQueue}");
		TryUpdatePlayfabData();
		_activeQueue = matchmakingQueue;
		CreateMatchmakingTicketRequest createMatchmakingTicketRequest = new CreateMatchmakingTicketRequest
		{
			Creator = GetCurrentUserMatchmakingPlayer(),
			GiveUpAfterSeconds = _timeout,
			QueueName = _activeQueue.ToString()
		};
		if (memberEntityIds != null && memberEntityIds.Count > 0)
		{
			createMatchmakingTicketRequest.MembersToMatchWith = memberEntityIds;
		}
		PlayFabMultiplayerAPI.CreateMatchmakingTicket(createMatchmakingTicketRequest, delegate(CreateMatchmakingTicketResult result)
		{
			OnCreatedMatchmakingTicket(result);
			onJoinedQueue(matchmakingQueue);
		}, OnCreateMatchmakingTicketFailed);
	}

	public void JoinMatchmakingTicket(MatchmakingQueue matchmakingQueue, string ticketId, Action<MatchmakingQueue> onJoinedQueue)
	{
		Debug.Log("[PlayfabMatchmaker] Starting Matchmaking with ticket " + ticketId);
		TryUpdatePlayfabData();
		_activeQueue = matchmakingQueue;
		PlayFabMultiplayerAPI.JoinMatchmakingTicket(new JoinMatchmakingTicketRequest
		{
			Member = GetCurrentUserMatchmakingPlayer(),
			QueueName = _activeQueue.ToString(),
			TicketId = ticketId
		}, delegate(JoinMatchmakingTicketResult result)
		{
			OnJoinedMatchmakingTicket(result, ticketId);
			onJoinedQueue(matchmakingQueue);
		}, OnCreateMatchmakingTicketFailed);
	}

	private void TryUpdatePlayfabData()
	{
		if (Time.realtimeSinceStartup > _lastDataRefreshTime + 60f)
		{
			Debug.Log("Updating Playfab Data");
			_lastDataRefreshTime = Time.realtimeSinceStartup;
			_configManager.Fetch(delegate
			{
				Debug.Log("Playfab Config Data Updated");
			}, PlayfabDataUpdateFailed);
			_profileManager.Fetch(_gameConfigData.DefaultCatalog, delegate
			{
				Debug.Log("Playfab Profiles Updated");
			}, PlayfabDataUpdateFailed);
		}
	}

	private void PlayfabDataUpdateFailed(FailureReasons failureReason)
	{
		Debug.Log($"Failed to update playfab data: {failureReason}");
		_lastDataRefreshTime -= 60f;
	}

	public void CancelMatchmaking(Action onCancelled)
	{
		DelayedAction.KillCoroutine(_pollingRoutineHandle);
		PlayFabMultiplayerAPI.CancelMatchmakingTicket(new CancelMatchmakingTicketRequest
		{
			TicketId = ActiveTicketId,
			QueueName = _activeQueue.ToString()
		}, errorCallback: OnFailureCallback(delegate
		{
			CancelMatchmaking(onCancelled);
		}, delegate(FailureReasons reason)
		{
			Debug.LogError($"[PlayfabMatchmaker] Failed to cancel matchmaking ticket: {reason}");
			onCancelled?.Invoke();
			_socketClient.UpdateStatus(PlayerStatus.InLobby);
			IsMatchmaking = false;
		}), resultCallback: delegate
		{
			Debug.Log("[PlayfabMatchmaker] Successfully Cancelled Matchmaking Ticket");
			onCancelled?.Invoke();
			_socketClient.UpdateStatus(PlayerStatus.InLobby);
			IsMatchmaking = false;
		});
	}

	private MatchmakingPlayer GetCurrentUserMatchmakingPlayer()
	{
		PlayFab.ClientModels.EntityKey entity = _userManager.CurrentUser.Entity;
		return new MatchmakingPlayer
		{
			Entity = new PlayFab.MultiplayerModels.EntityKey
			{
				Id = entity.Id,
				Type = entity.Type
			},
			Attributes = GetPlayerAttributes()
		};
	}

	private MatchmakingPlayerAttributes GetPlayerAttributes()
	{
		string displayName = _userManager.CurrentUser.DisplayName;
		string outfit = _userManager.CurrentUser.LoadoutManager.GetEquippedLoadout().Outfit;
		MatchmakingPlayerProperties dataObject = new MatchmakingPlayerProperties(displayName, outfit);
		return new MatchmakingPlayerAttributes
		{
			DataObject = dataObject
		};
	}

	private void OnCreatedMatchmakingTicket(CreateMatchmakingTicketResult result)
	{
		_socketClient.UpdateStatus(PlayerStatus.InMatchmaking);
		IsMatchmaking = true;
		Debug.Log("[PlayfabMatchmaker] Created Matchmaking Ticket: " + result.TicketId);
		ActiveTicketId = result.TicketId;
		_pollingRoutineHandle = DelayedAction.RunCoroutine(CheckTicketStatusRoutine(result.TicketId));
	}

	private void OnJoinedMatchmakingTicket(JoinMatchmakingTicketResult result, string ticketId)
	{
		_socketClient.UpdateStatus(PlayerStatus.InMatchmaking);
		IsMatchmaking = true;
		Debug.Log("[PlayfabMatchmaker] Joined Matchmaking Ticket: " + ticketId);
		ActiveTicketId = ticketId;
		_pollingRoutineHandle = DelayedAction.RunCoroutine(CheckTicketStatusRoutine(ActiveTicketId));
	}

	private void OnCreateMatchmakingTicketFailed(PlayFabError error)
	{
		Debug.LogError("[PlayfabMatchmaker] Failed to created Matchmaking Ticket: " + error.ToString());
		if (error.Error == PlayFabErrorCode.MatchmakingTicketMembershipLimitExceeded)
		{
			PlayFab.ClientModels.EntityKey entity = _userManager.CurrentUser.Entity;
			PlayFabMultiplayerAPI.CancelAllMatchmakingTicketsForPlayer(new CancelAllMatchmakingTicketsForPlayerRequest
			{
				Entity = new PlayFab.MultiplayerModels.EntityKey
				{
					Id = entity.Id,
					Type = entity.Type
				},
				QueueName = _activeQueue.ToString()
			}, delegate
			{
				Debug.Log("Cancelling all Players MM Tickets");
			}, delegate(PlayFabError e)
			{
				Debug.Log("Failed to cancel all MM tickets - " + e.ToString());
			});
		}
		RaiseMatchMakingFailed("Unable to Join Matchmaking Queue\nPlease Try Again");
	}

	private IEnumerator CheckTicketStatusRoutine(string ticketId)
	{
		GetMatchmakingTicketRequest request = new GetMatchmakingTicketRequest
		{
			TicketId = ticketId,
			QueueName = _activeQueue.ToString()
		};
		bool cancelPolling = false;
		while (!cancelPolling)
		{
			PlayFabMultiplayerAPI.GetMatchmakingTicket(request, delegate(GetMatchmakingTicketResult result)
			{
				Debug.Log("[PlayfabMatchmaker] Got Ticket: " + result.TicketId + " / Status: " + result.Status);
				if (result.Status == "Matched")
				{
					GetMatch(result.MatchId);
					cancelPolling = true;
				}
				else if (result.Status == "Canceled")
				{
					Debug.Log($"[PlayfabMatchmaker] Cancelled Matchmaking Ticket Polling - {result.CancellationReason}");
					cancelPolling = true;
					if (result.CancellationReason == CancellationReason.Internal)
					{
						RaiseMatchMakingFailed("Matchmaking Failed\nPlease Try Again");
					}
					else
					{
						RaiseMatchMakingFailed("Unable to find a Match\nPlease Try Again");
					}
				}
			}, delegate(PlayFabError error)
			{
				Debug.LogError("[PlayfabMatchmaker] Failed to get ticket: " + error.ToString());
			});
			if (cancelPolling)
			{
				break;
			}
			yield return new WaitForSeconds(10f);
		}
	}

	private void GetMatch(string matchId)
	{
		Debug.Log("[PlayfabMatchmaker] Fetching Match with Id: " + matchId);
		PlayFabMultiplayerAPI.GetMatch(new GetMatchRequest
		{
			ReturnMemberAttributes = true,
			EscapeObject = true,
			MatchId = matchId,
			QueueName = _activeQueue.ToString()
		}, errorCallback: OnFailureCallback(delegate
		{
			GetMatch(matchId);
		}, delegate(FailureReasons reason)
		{
			Debug.LogError("[PlayfabMatchmaker] Failed to get match: " + reason);
			RaiseMatchMakingFailed("Unable to Join Match\nPlease Try Again");
		}), resultCallback: delegate(GetMatchResult result)
		{
			Debug.Log("[PlayfabMatchmaker] Got Match: " + result.MatchId);
			if (result.ServerDetails != null)
			{
				Debug.Log("[PlayfabMatchmaker] Server IP: " + result.ServerDetails.IPV4Address);
			}
			else
			{
				Debug.Log("[PlayfabMatchmaker] Server Details NULL");
			}
			this._matchFound?.Invoke(result);
		});
	}
}
