using System;
using System.Collections;
using System.Collections.Generic;
using BSCore;
using BSCore.Constants.Config;
using MatchMaking;
using NodeClient;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using UnityEngine;
using Zenject;

public class SkyVuMatchmaker
{
	private const float DATA_REFRESH_TIMEOUT = 60f;

	[Inject]
	private SocketClient _socketClient;

	[Inject]
	private MatchMakerApi _matchMakerApi;

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

	private event Action<Match> _matchFound;

	public event Action<Match> MatchFound
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
	public SkyVuMatchmaker(UserManager userManager, ConfigManager configManager, ProfileManager profileManager, GameConfigData gameConfigData)
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
		Debug.Log($"[Matchmaker] Starting Matchmaking for Queue {matchmakingQueue}");
		TryUpdatePlayfabData();
		_activeQueue = matchmakingQueue;
		TicketRequest request = new TicketRequest
		{
			queueName = _activeQueue.ToString(),
			timeout = _timeout,
			titleId = _gameConfigData.GetTitleId(),
			playerAttributes = GetPlayerAttribute()
		};
		_matchMakerApi.CreateMatchMakingTicket(request, delegate(TicketCreatedResponse response)
		{
			OnCreatedMatchmakingTicket(new CreateMatchmakingTicketResult
			{
				TicketId = response.ticketId
			});
			onJoinedQueue(matchmakingQueue);
		}, delegate(ErrorResponse error)
		{
			OnCreateMatchmakingTicketFailed(error);
		});
	}

	public void JoinMatchmakingTicket(MatchmakingQueue matchmakingQueue, string ticketId, Action<MatchmakingQueue> onJoinedQueue)
	{
		Debug.Log("[Matchmaker] Starting Matchmaking with ticket " + ticketId);
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
		_matchMakerApi.CancelTicket(ActiveTicketId, delegate
		{
			Debug.Log("[Matchmaker] Successfully Cancelled Matchmaking Ticket");
			onCancelled?.Invoke();
			_socketClient.UpdateStatus(PlayerStatus.InLobby);
			IsMatchmaking = false;
		}, delegate(ErrorResponse error)
		{
			Debug.LogError($"[PlayfabMatchmaker] Failed to cancel matchmaking ticket: {error}");
		});
	}

	private PlayerAttribute GetPlayerAttribute()
	{
		PlayerProfile currentUser = _userManager.CurrentUser;
		return new PlayerAttribute
		{
			playerId = currentUser.Id,
			entityId = currentUser.Entity.Id,
			playerCount = 1,
			skill = 1,
			buildVersion = Application.version,
			displayName = currentUser.DisplayName,
			outfit = currentUser.LoadoutManager.GetEquippedLoadout().Outfit,
			latency = new Latency[1]
			{
				new Latency
				{
					region = "EastUs",
					latency = 30
				}
			}
		};
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
		Debug.Log("[Matchmaker] Created Matchmaking Ticket: " + result.TicketId);
		ActiveTicketId = result.TicketId;
		_pollingRoutineHandle = DelayedAction.RunCoroutine(CheckTicketStatusRoutine(result.TicketId));
	}

	private void OnJoinedMatchmakingTicket(JoinMatchmakingTicketResult result, string ticketId)
	{
		_socketClient.UpdateStatus(PlayerStatus.InMatchmaking);
		IsMatchmaking = true;
		Debug.Log("[Matchmaker] Joined Matchmaking Ticket: " + ticketId);
		ActiveTicketId = ticketId;
		_pollingRoutineHandle = DelayedAction.RunCoroutine(CheckTicketStatusRoutine(ActiveTicketId));
	}

	private void OnCreateMatchmakingTicketFailed(ErrorResponse error)
	{
		Debug.LogError("[Matchmaker] Failed to created Matchmaking Ticket: " + error.message);
	}

	private IEnumerator CheckTicketStatusRoutine(string ticketId)
	{
		bool cancelPolling = false;
		bool pollingCompleted = false;
		while (!cancelPolling)
		{
			pollingCompleted = false;
			_matchMakerApi.GetTicket(ticketId, delegate(TicketResponse response)
			{
				Debug.Log("[Matchmaker] Got Ticket: " + ticketId + " / Status: " + response.status);
				if (response.status == "Matched")
				{
					GetMatch(response.matchId);
					cancelPolling = true;
				}
				pollingCompleted = true;
			}, delegate(ErrorResponse error)
			{
				pollingCompleted = true;
				Debug.LogError("[Matchmaker] Failed to get ticket: " + error);
			});
			if (cancelPolling)
			{
				break;
			}
			yield return new WaitUntil(() => pollingCompleted);
		}
	}

	private void GetMatch(string matchId)
	{
		Debug.Log("[Matchmaker] Fetching Match with Id: " + matchId);
		_matchMakerApi.GetMatch(matchId, delegate(Match response)
		{
			Debug.Log("[PlayfabMatchmaker] Got Match");
			this._matchFound?.Invoke(response);
		}, delegate(ErrorResponse error)
		{
			Debug.LogError("[PlayfabMatchmaker] Failed to get match: " + error);
			RaiseMatchMakingFailed("Unable to Join Match\nPlease Try Again");
		});
	}
}
