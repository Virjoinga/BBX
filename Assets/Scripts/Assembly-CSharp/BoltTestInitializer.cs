using System.Collections.Generic;
using UdpKit;
using UnityEngine;

public class BoltTestInitializer : MonoBehaviour
{
	private enum Mode
	{
		Offline = 0,
		Server = 1,
		Client = 2
	}

	[SerializeField]
	private Mode _mode;

	[SerializeField]
	private GameObject _virtualController;

	[SerializeField]
	private GameObject _serverCamera;

	[SerializeField]
	private GameObject _loadingScreen;

	[SerializeField]
	private GameObject _characterSelector;

	[SerializeField]
	private OTSCamera _otsCamera;

	[SerializeField]
	private OfflinePlayerController _offlinePlayer;

	[SerializeField]
	private HealthDisplay _localPlayerHealthDisplay;

	[SerializeField]
	private int _dummies;

	[SerializeField]
	private AudioSource _backgroundAudio;

	public static BoltTestInitializer Instance { get; private set; }

	public int Dummies => _dummies;

	public bool IsOnline => _mode != Mode.Offline;

	private void Awake()
	{
		Instance = this;
		Screen.sleepTimeout = -1;
		ConnectionManager.PlayerDisconnected += OnDisconnected;
		ConnectionManager.ConnectionFailed += OnConnectFailed;
	}

	private void Start()
	{
		switch (_mode)
		{
		case Mode.Server:
			ConnectionManager.StartServer();
			break;
		case Mode.Client:
			ConnectionManager.SessionsListUpdated -= SessionsListUpdated;
			ConnectionManager.SessionsListUpdated += SessionsListUpdated;
			ConnectionManager.StartClient();
			break;
		}
		_virtualController.SetActive(value: false);
	}

	private void OnValidate()
	{
		switch (_mode)
		{
		case Mode.Server:
			_loadingScreen.SetActive(value: false);
			_serverCamera.SetActive(value: true);
			_characterSelector.SetActive(value: false);
			_otsCamera.gameObject.SetActive(value: false);
			_offlinePlayer.gameObject.SetActive(value: false);
			_localPlayerHealthDisplay.gameObject.SetActive(value: false);
			_backgroundAudio.gameObject.SetActive(value: false);
			break;
		case Mode.Client:
			_loadingScreen.SetActive(value: true);
			_serverCamera.SetActive(value: true);
			_characterSelector.SetActive(value: true);
			_otsCamera.gameObject.SetActive(value: false);
			_offlinePlayer.gameObject.SetActive(value: false);
			_localPlayerHealthDisplay.gameObject.SetActive(value: true);
			_backgroundAudio.gameObject.SetActive(value: true);
			break;
		default:
			_loadingScreen.SetActive(value: false);
			_serverCamera.SetActive(value: false);
			_characterSelector.SetActive(value: false);
			_otsCamera.gameObject.SetActive(value: true);
			_offlinePlayer.gameObject.SetActive(value: true);
			_localPlayerHealthDisplay.gameObject.SetActive(value: false);
			_backgroundAudio.gameObject.SetActive(value: true);
			break;
		}
	}

	private void SessionsListUpdated(List<UdpSession> sessions)
	{
		Debug.Log($"[BoltTestInitializer] Session list updated. Found {sessions.Count} sessions");
		if (sessions.Count > 0)
		{
			ConnectionManager.JoinSession(sessions[0]);
		}
	}

	public void DisableLoadingCamera()
	{
		_serverCamera.SetActive(value: false);
	}

	public void DisableLoadingScreen()
	{
		_loadingScreen.SetActive(value: false);
	}

	private void OnDisconnected()
	{
		if (!(base.gameObject == null))
		{
			Reconnect();
		}
	}

	private void OnConnectFailed(string error)
	{
		Debug.LogError("[BoltTestInitializer] Error connecting: " + error);
	}

	private void Reconnect()
	{
		Invoke("Start", 0.5f);
	}
}
