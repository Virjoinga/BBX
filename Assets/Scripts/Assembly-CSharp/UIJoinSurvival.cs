using System.Collections;
using System.Collections.Generic;
using BSCore;
using BSCore.Constants.CloudCode;
using TMPro;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using udpkit.platform.photon.photon;

public class UIJoinSurvival : MonoBehaviour
{
	private const string TIME_FORMAT = "{0:00}:{1:00}";

	[Inject]
	private UserManager _userManager;

	[Inject]
	private CloudCodeManager _cloudCodeManager;

	[SerializeField]
	private Button _survivalButton;

	[SerializeField]
	private TextMeshProUGUI _searchingText;

	private float _searchTimer;

	private BackoffTimeout _timeout = new BackoffTimeout();

	private bool _joiningMatch;

	private void Awake()
	{
		_survivalButton.onClick.AddListener(JoinSurvival);
	}

	private void JoinSurvival()
	{
		_survivalButton.interactable = false;
		StartCoroutine(SearchTimerRoutine());
		TryRequestServer();
		ConnectionManager.SessionsListUpdated -= SessionsListUpdated;
		ConnectionManager.SessionsListUpdated += SessionsListUpdated;
		ConnectionManager.StartClient();
	}

	private IEnumerator SearchTimerRoutine()
	{
		_searchTimer = 0f;
		while (true)
		{
			_searchTimer += Time.deltaTime;
			int num = (int)_searchTimer / 60;
			int num2 = (int)_searchTimer % 60;
			string text = $"{num:00}:{num2:00}";
			_searchingText.text = "Joining... " + text;
			yield return null;
		}
	}

	private void TryRequestServer()
	{
		if (!_joiningMatch)
		{
			_cloudCodeManager.Run(FunctionName.RequestSurvivalServer, null, ServerRequestCompleted, ServerRequestFailed);
		}
	}

	private void ServerRequestCompleted(object _)
	{
		Debug.Log("[UIJoinSurvival] Request Server CC Completed");
	}

	private void ServerRequestFailed(FailureReasons error)
	{
		Debug.Log($"[UIJoinSurvival] Request Server CC Failed {error}");
		if (!_joiningMatch && !_timeout.RunAfterBackoff(this, TryRequestServer))
		{
			Debug.Log($"[UIJoinSurvival] Final Request Server CC Error: {error}");
			_survivalButton.interactable = true;
			UIGenericPopupManager.ShowConfirmPopup("Unable to Find and Join Server\nPlease Try Again", null);
		}
	}

	private void SessionsListUpdated(List<UdpSession> sessions)
	{
		Debug.Log($"[UIJoinSurvival] Session list updated. Found {sessions.Count} sessions");
		List<UdpSession> list = new List<UdpSession>();
		foreach (UdpSession session in sessions)
		{
			if (session.Source == UdpSessionSource.Photon && (session as PhotonSession).Properties.ContainsKey("s"))
			{
				list.Add(session);
			}
		}
		if (list.Count > 0)
		{
			Debug.Log($"[UIJoinSurvival] Found existing Survival Servers {list.Count}");
			JoinMatch(list[0]);
		}
	}

	private void JoinMatch(UdpSession sessionToJoin)
	{
		_joiningMatch = true;
		ConnectionManager.SessionsListUpdated -= SessionsListUpdated;
		UIPrefabManager.Instantiate(UIPrefabIds.LoadingOverlay, delegate(GameObject go)
		{
			OnLoadingOverlayCreated(go, sessionToJoin);
		}, interactive: false, 11);
	}

	private void OnLoadingOverlayCreated(GameObject uiGameobject, UdpSession sessionToJoin)
	{
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
