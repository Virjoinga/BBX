using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using MatchMaking;
using PlayFab.MultiplayerModels;
using TMPro;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIJoinMatchmaking : MonoBehaviour
{
	private const string TIME_FORMAT = "{0:00}:{1:00}";

	private const int SECONDS_IN_MINUTE = 60;

	private const float JOIN_MATCH_TIMEOUT = 60f;

	[Inject]
	protected UserManager _userManager;

	[Inject]
	private SkyVuMatchmaker _skyvuMatchmaker;

	[Inject]
	private GroupManager _groupManager;

	[SerializeField]
	private GameObject _playButtonRoot;

	[SerializeField]
	private Button _TDMButton;

	[SerializeField]
	private Button _cancelButton;

	[SerializeField]
	private TextMeshProUGUI _searchingText;

	private float _searchTimer;

	private string _matchIdToJoin;

	private List<MatchedMember> _matchMembers;

	[Inject]
	private void Construct()
	{
		_skyvuMatchmaker.MatchFound += OnMatchFound;
		_skyvuMatchmaker.MatchmakingFailed += OnMatchmakingFailed;
		_TDMButton.onClick.AddListener(delegate
		{
			StartMatchmaking(MatchmakingQueue.teamDeathMatch);
		});
		_cancelButton.onClick.AddListener(CancelMatchmaking);
		_groupManager.GroupUpdated += OnGroupUpdated;
		_groupManager.MatchmakingJoined += JoinMatchmakingTicket;
		_groupManager.MatchmakingCanceled += CancelMatchmaking;
	}

	private void Start()
	{
		SetPlayButtonsInteractive(interactable: true);
	}

	private void OnDestroy()
	{
		_skyvuMatchmaker.MatchFound -= OnMatchFound;
		_skyvuMatchmaker.MatchmakingFailed -= OnMatchmakingFailed;
		_groupManager.GroupUpdated -= OnGroupUpdated;
		_groupManager.MatchmakingJoined -= JoinMatchmakingTicket;
		_groupManager.MatchmakingCanceled -= CancelMatchmaking;
	}

	private void SetPlayButtonsInteractive(bool interactable)
	{
		interactable &= _groupManager.IAmLeader;
		_TDMButton.interactable = interactable;
	}

	private void StartMatchmaking(MatchmakingQueue matchmakingQueue)
	{
		if (!_groupManager.IAmLeader)
		{
			return;
		}
		SetPlayButtonsInteractive(interactable: false);
		StartCoroutine(SearchTimerRoutine("Joining Queue"));
		List<EntityKey> memberEntityIds = null;
		if (_groupManager.Group.IsValid && _groupManager.Group.Members.Count > 0)
		{
			memberEntityIds = (from m in _groupManager.Group.Members
				where m.Id != _groupManager.Group.LeaderId
				select new EntityKey
				{
					Id = m.TitleId,
					Type = "title_player_account"
				}).ToList();
		}
		_skyvuMatchmaker.StartMatchmaking(matchmakingQueue, memberEntityIds, OnJoinedQueue);
	}

	private void JoinMatchmakingTicket(MatchmakingQueue matchmakingQueue, string ticketId)
	{
		if (!_groupManager.IAmLeader)
		{
			SetPlayButtonsInteractive(interactable: false);
			StartCoroutine(SearchTimerRoutine("Joining Queue"));
			_skyvuMatchmaker.JoinMatchmakingTicket(matchmakingQueue, ticketId, OnJoinedQueue);
		}
	}

	private void OnJoinedQueue(MatchmakingQueue matchmakingQueue)
	{
		_playButtonRoot.SetActive(value: false);
		_cancelButton.gameObject.SetActive(value: true);
		SetPlayButtonsInteractive(interactable: true);
		StopAllCoroutines();
		StartCoroutine(SearchTimerRoutine("Searching for Match"));
		if (_groupManager.IAmLeader)
		{
			_groupManager.RelayMatchmakingTicketId(matchmakingQueue, _skyvuMatchmaker.ActiveTicketId);
		}
	}

	private void CancelMatchmaking()
	{
		_cancelButton.interactable = false;
		StopAllCoroutines();
		_searchingText.text = "Cancelling";
		_skyvuMatchmaker.CancelMatchmaking(delegate
		{
			_cancelButton.gameObject.SetActive(value: false);
			_playButtonRoot.SetActive(value: true);
			SetPlayButtonsInteractive(interactable: true);
			_cancelButton.interactable = true;
			_searchingText.text = string.Empty;
			if (_groupManager.IAmLeader)
			{
				_groupManager.RelayMatchmakingCanceled();
			}
		});
	}

	private IEnumerator SearchTimerRoutine(string messageText)
	{
		_searchTimer = 0f;
		while (true)
		{
			_searchTimer += Time.deltaTime;
			int num = (int)_searchTimer / 60;
			int num2 = (int)_searchTimer % 60;
			string text = $"{num:00}:{num2:00}";
			_searchingText.text = messageText + "... " + text;
			yield return null;
		}
	}

	private void OnMatchFound(Match matchResults)
	{
		Debug.Log("[UIJoinMatchmaking] Got MatchId to Join, Starting Bolt Connection - " + matchResults.matchId);
		StopAllCoroutines();
		StartCoroutine(SearchTimerRoutine("Joining Match"));
		StartCoroutine(JoinMatchTimeout());
		_cancelButton.interactable = false;
		_searchingText.text = "Joining";
		_matchIdToJoin = matchResults.matchId;
		_matchMembers = matchResults.members;
		ConnectionManager.SessionsListUpdated -= SessionsListUpdated;
		ConnectionManager.SessionsListUpdated += SessionsListUpdated;
		ConnectionManager.StartClient();
	}

	private void OnMatchmakingFailed(string message)
	{
		UIGenericPopupManager.ShowConfirmPopup(message, null);
		_cancelButton.gameObject.SetActive(value: false);
		_playButtonRoot.SetActive(value: true);
		_cancelButton.interactable = true;
		SetPlayButtonsInteractive(interactable: true);
		StopAllCoroutines();
		_searchingText.text = string.Empty;
	}

	private void SessionsListUpdated(List<UdpSession> sessions)
	{
		Debug.Log($"[UIJoinMatchmaking] Session list updated. Found {sessions.Count} sessions");
		if (sessions.Count > 0)
		{
			UdpSession udpSession = sessions.Where((UdpSession x) => x.HostName.StartsWith(_matchIdToJoin)).FirstOrDefault();
			if (udpSession != null)
			{
				Debug.Log("Found matching bolt server! Joining!");
				StopAllCoroutines();
				JoinMatch(udpSession);
			}
			else
			{
				Debug.Log("Session List updated but did not include matchId from Matchmaker - " + _matchIdToJoin);
			}
		}
	}

	private IEnumerator JoinMatchTimeout()
	{
		yield return new WaitForSeconds(60f);
		Debug.Log("Failed to find and join bolt match in time");
		ConnectionManager.SessionsListUpdated -= SessionsListUpdated;
		ConnectionManager.Shutdown();
		OnMatchmakingFailed("Unable to Find and Join Server\nPlease Try Again");
	}

	private void JoinMatch(UdpSession sessionToJoin)
	{
		ConnectionManager.SessionsListUpdated -= SessionsListUpdated;
		UIPrefabManager.Instantiate(UIPrefabIds.TDMMatchLoadingScreen, delegate(GameObject go)
		{
			OnLoadingScreenCreated(go, sessionToJoin);
		}, interactive: false, 11);
	}

	private void OnLoadingScreenCreated(GameObject uiGameobject, UdpSession sessionToJoin)
	{
		if (!(uiGameobject == null))
		{
			uiGameobject.GetComponent<TDMMatchLoadingScreen>().Populate(_matchMembers);
			UIPrefabManager.Destroy(UIPrefabIds.MainMenu);
			ServerConnectToken serverConnectToken = new ServerConnectToken();
			ServerConnectToken.Data data = new ServerConnectToken.Data
			{
				serviceToken = _userManager.CurrentUser.SessionTicket,
				Platform = PlatformCheck.GetPlatform()
			};
			serverConnectToken.SetData(data);
			ConnectionManager.JoinSession(sessionToJoin, serverConnectToken);
		}
	}

	private void OnGroupUpdated()
	{
		Debug.Log($"[UIJoinMatchmaking] Group updated. IAmLeader? {_groupManager.IAmLeader}");
		if (_skyvuMatchmaker.IsMatchmaking)
		{
			Debug.Log("[UIJoinMatchmaking] Group updated. Cancelling matchmaking.");
			CancelMatchmaking();
		}
		else
		{
			Debug.Log("[UIJoinMatchmaking] Group updated. Setting button interactable states");
			SetPlayButtonsInteractive(_groupManager.IAmLeader);
		}
	}
}
