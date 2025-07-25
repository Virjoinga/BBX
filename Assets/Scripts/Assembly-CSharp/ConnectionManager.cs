using System;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using Bolt;
using Bolt.photon;
using UdpKit;
using UnityEngine;

[BoltGlobalBehaviour]
public class ConnectionManager : GlobalEventListener
{
	private static readonly string _version = Application.version;

	private static ConnectionManager _instance;

	private bool _allowQuit;

	private Dictionary<Guid, UdpSession> _internalSessionList = new Dictionary<Guid, UdpSession>();

	public static bool IsConnected => BoltNetwork.IsConnected;

	private static event Action<List<UdpSession>> _sessionsListUpdated;

	public static event Action<List<UdpSession>> SessionsListUpdated
	{
		add
		{
			_sessionsListUpdated += value;
		}
		remove
		{
			_sessionsListUpdated -= value;
		}
	}

	private static event Action _disconnectedFromBolt;

	public static event Action DisconnectedFromBolt
	{
		add
		{
			_disconnectedFromBolt += value;
		}
		remove
		{
			_disconnectedFromBolt -= value;
		}
	}

	private static event Action _playerDisconnected;

	public static event Action PlayerDisconnected
	{
		add
		{
			_playerDisconnected += value;
		}
		remove
		{
			_playerDisconnected -= value;
		}
	}

	private static event Action<string> _connectionFailed;

	public static event Action<string> ConnectionFailed
	{
		add
		{
			_connectionFailed += value;
		}
		remove
		{
			_connectionFailed -= value;
		}
	}

	private static event Action _initialConnect;

	public static event Action InitialConnect
	{
		add
		{
			_initialConnect += value;
		}
		remove
		{
			_initialConnect -= value;
		}
	}

	public static void StartServer()
	{
		Debug.Log("Starting Bolt Server");
		BoltLauncher.StartServer(55555);
	}

	public static void ForEachSessionListed(Action<UdpSession> callback)
	{
		if (_instance == null)
		{
			return;
		}
		foreach (KeyValuePair<Guid, UdpSession> internalSession in _instance._internalSessionList)
		{
			callback(internalSession.Value);
		}
	}

	public static void StartClient()
	{
		BoltLauncher.StartClient();
	}

	public static void Shutdown()
	{
		BoltLauncher.Shutdown();
	}

	public static void JoinSession(UdpSession session)
	{
		BoltNetwork.Connect(session);
	}

	public static void JoinSession(UdpSession session, ServerConnectToken token)
	{
		BoltNetwork.Connect(session, token);
	}

	private void Awake()
	{
		Debug.Log("[ConnectionManager] Awake");
		_instance = this;
		BoltLauncher.SetUdpPlatform(new PhotonPlatform());
		Application.wantsToQuit += CleanupBeforeQuit;
	}

	private bool CleanupBeforeQuit()
	{
		Debug.Log("[ConnectionManager] Checking if I should quit");
		if (!_allowQuit)
		{
			if (BoltNetwork.IsConnected)
			{
				Debug.Log("[ConnectionManager] Can't quit yet because Bolt is still connected");
				BoltLauncher.Shutdown();
			}
			else
			{
				Debug.Log("[ConnectionManager] Quit allowed");
				_allowQuit = true;
			}
		}
		return _allowQuit;
	}

	private void OnDestroy()
	{
		_instance = null;
		Debug.Log("[ConnectionManager] OnDestroy");
	}

	public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
	{
		if (BoltNetwork.IsServer)
		{
			return;
		}
		Debug.Log($"[ConnectionManager] Session list updated: {sessionList.Count} total sessions");
		_internalSessionList = new Dictionary<Guid, UdpSession>();
		foreach (KeyValuePair<Guid, UdpSession> session in sessionList)
		{
			Debug.Log($"Got SESSION: {session.Value.HostName} / Source: {session.Value.Source}");
			if (session.Value.Source == UdpSessionSource.Photon && session.Value.HostName.EndsWith("-" + _version))
			{
				_internalSessionList.Add(session.Value.Id, session.Value);
			}
		}
		ConnectionManager._sessionsListUpdated?.Invoke(_internalSessionList.Values.ToList());
	}

	public override void BoltStartBegin()
	{
		BoltNetwork.RegisterTokenClass<PlayerController.AttachToken>();
		BoltNetwork.RegisterTokenClass<SurvivalEnemy.SurvivalEnemyAttachToken>();
		BoltNetwork.RegisterTokenClass<RoomProtocolToken>();
		BoltNetwork.RegisterTokenClass<ServerAcceptToken>();
		BoltNetwork.RegisterTokenClass<ServerConnectToken>();
		BoltNetwork.RegisterTokenClass<DisconnectReasonToken>();
		BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
		_allowQuit = false;
	}

	public override void BoltStartDone()
	{
		if (BoltNetwork.IsServer)
		{
			PhotonRoomProperties photonRoomProperties = new PhotonRoomProperties();
			photonRoomProperties.IsOpen = true;
			photonRoomProperties.IsVisible = true;
			if (!PlayfabServerManagement.IsInitializedAndReady)
			{
				ServerBootloader serverBootloader = UnityEngine.Object.FindObjectOfType<ServerBootloader>();
				if (serverBootloader != null && serverBootloader.GameMode == GameModeType.Survival)
				{
					photonRoomProperties.AddRoomProperty("s", 1);
				}
			}
			else if (CommandLineArgsManager.GetArg("-gameMode", GameModeType.BattleRoyale) == GameModeType.Survival)
			{
				photonRoomProperties.AddRoomProperty("s", 1);
			}
			string serverName = GetServerName();
			BoltNetwork.SetServerInfo(serverName, photonRoomProperties);
			Debug.Log("[ConnectionManager] Bolt Server Started. Server Name " + serverName);
			if (!PlayfabServerManagement.IsInitializedAndReady)
			{
				UnityEngine.Object.Instantiate(Resources.Load<ServerNameDisplayDebug>("ServerNameDisplayDebug")).SetServerName(serverName);
			}
		}
		ConnectionManager._initialConnect?.Invoke();
		Debug.Log($"[ConnectionManager] Bolt started. Sessions: {BoltNetwork.SessionList.Count}");
	}

	public override void BoltShutdownBegin(AddCallback registerDoneCallback)
	{
		Debug.Log("[ConnectionManager] Bolt shutdown starting...");
		registerDoneCallback(delegate
		{
			Debug.Log("[ConnectionManager] Bolt shutdown complete");
			ConnectionManager._disconnectedFromBolt?.Invoke();
			DelayedAction.RunAfterSeconds(5f, delegate
			{
				_allowQuit = true;
			});
		});
	}

	private string GetServerName()
	{
		return (PlayfabServerManagement.IsInitializedAndReady ? PlayfabServerManagement.SessionId : Guid.NewGuid().ToString()) + "-" + _version;
	}

	public override void Disconnected(BoltConnection connection)
	{
		Debug.Log("Bolt Diconnection");
		ConnectionManager._playerDisconnected?.Invoke();
	}

	public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
	{
		Debug.Log("Bolt Connection Failed");
		ConnectionManager._connectionFailed?.Invoke("Connect failed");
	}

	public override void SceneLoadLocalBegin(string scene)
	{
	}
}
