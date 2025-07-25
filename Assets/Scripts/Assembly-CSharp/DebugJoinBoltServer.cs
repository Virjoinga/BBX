using System.Collections.Generic;
using BSCore;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DebugJoinBoltServer : MonoBehaviour
{
	[Inject]
	protected UserManager _userManager;

	[SerializeField]
	private Button _debugJoinButton;

	private bool _connectingToBolt;

	private List<UdpSession> _sessions = new List<UdpSession>();

	private void Awake()
	{
		_debugJoinButton.onClick.AddListener(delegate
		{
			ConnectToBolt();
		});
	}

	private void Update()
	{
		if (!UIExtensions.IsInputFieldFocused() && Input.GetKeyDown(KeyCode.P))
		{
			ConnectToBolt();
		}
	}

	private void ConnectToBolt()
	{
		if (!_connectingToBolt)
		{
			_connectingToBolt = true;
			ConnectionManager.SessionsListUpdated -= OnSessionsListUpdated;
			ConnectionManager.SessionsListUpdated += OnSessionsListUpdated;
			ConnectionManager.StartClient();
		}
	}

	private void OnSessionsListUpdated(List<UdpSession> sessions)
	{
		Debug.Log($"[DebugJoinBoltServer] Session List Updated {sessions.Count}");
		_sessions = sessions;
	}

	private void OnGUI()
	{
		if (!_connectingToBolt)
		{
			return;
		}
		GUIStyle gUIStyle = new GUIStyle("button");
		gUIStyle.fontSize = 34;
		float num = 825f;
		float x = ((float)Screen.width - num) / 2f;
		float num2 = 100f;
		foreach (UdpSession session in _sessions)
		{
			if (GUI.Button(new Rect(x, num2, num, 100f), session.HostName, gUIStyle))
			{
				JoinSession(session);
			}
			num2 += 125f;
		}
		if (GUI.Button(new Rect(x, num2, num, 100f), "Cancel", gUIStyle))
		{
			ConnectionManager.Shutdown();
			_connectingToBolt = false;
		}
	}

	private void JoinSession(UdpSession session)
	{
		Debug.Log("[DebugJoinBoltServer] Joining Session - " + session.HostName);
		UIPrefabManager.Instantiate(UIPrefabIds.TDMMatchLoadingScreen, delegate(GameObject go)
		{
			OnLoadingScreenCreated(go, session);
		}, interactive: false, 11);
	}

	private void OnLoadingScreenCreated(GameObject uiGameobject, UdpSession session)
	{
		UIPrefabManager.Destroy(UIPrefabIds.MainMenu);
		ServerConnectToken serverConnectToken = new ServerConnectToken();
		ServerConnectToken.Data data = new ServerConnectToken.Data
		{
			serviceToken = _userManager.CurrentUser.SessionTicket,
			Platform = PlatformCheck.GetPlatform()
		};
		serverConnectToken.SetData(data);
		ConnectionManager.JoinSession(session, serverConnectToken);
	}
}
