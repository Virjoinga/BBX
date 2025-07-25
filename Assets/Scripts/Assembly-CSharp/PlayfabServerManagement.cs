using System;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using Microsoft.Playfab.Gaming.GSDK.CSharp;
using UnityEngine;

public static class PlayfabServerManagement
{
	private static readonly float NO_CONNECTIONS_TIMEOUT = 240f;

	private static DelayedAction.DelayedActionHandle _noConnectionsTimeoutHandle;

	private static List<string> _expectedPlayers = new List<string>();

	private static List<ConnectedPlayer> _connectedPlayers = new List<ConnectedPlayer>();

	public static string SessionId { get; private set; }

	public static string ServerId { get; private set; }

	public static bool IsInitializedAndReady => !string.IsNullOrEmpty(SessionId);

	public static int ExpectedPlayerCount => _expectedPlayers.Count;

	public static int ConnectedPlayerCount { get; private set; }

	public static void InitServer()
	{
		Application.logMessageReceived -= OnLogMessageRecieved;
		Application.logMessageReceived += OnLogMessageRecieved;
		try
		{
			GameserverSDK.Start();
		}
		catch (GSDKInitializationException ex)
		{
			Debug.Log("[PlayfabServerManagement] Cannot start GSDK. Please make sure the MockAgent is running. ");
			Debug.Log("[PlayfabServerManagement] Got Exception: " + ex.ToString());
			return;
		}
		catch (Exception ex2)
		{
			Debug.Log("[PlayfabServerManagement] Got Exception Starting GSDK: " + ex2.ToString());
			return;
		}
		GameserverSDK.RegisterHealthCallback(OnHealthUpdate);
		GameserverSDK.RegisterMaintenanceCallback(OnMaintenanceUpdate);
		GameserverSDK.RegisterShutdownCallback(OnShutdownCallback);
	}

	public static void StandByUntilActive()
	{
		try
		{
			if (GameserverSDK.ReadyForPlayers())
			{
				Debug.Log("[PlayfabServerManagement] Server is Ready for Players!");
				GetConfigData();
				_noConnectionsTimeoutHandle = DelayedAction.RunAfterSeconds(NO_CONNECTIONS_TIMEOUT, delegate
				{
					Debug.Log("[PlayfabServerManagement] No Players Connected Within Time Limit");
					ShutdownServer();
				});
			}
			else
			{
				Debug.Log("[PlayfabServerManagement] Server is getting terminated.");
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[PlayfabServerManagement] Got Exception Calling Ready for Players");
			Debug.LogError("[PlayfabServerManagement] Exception: " + ex.Message);
		}
	}

	public static void PlayerConnected(string playerId)
	{
		Debug.Log("[PlayfabServerManagement] Player Connected: " + playerId);
		_connectedPlayers.Add(new ConnectedPlayer(playerId));
		ConnectedPlayerCount++;
		DelayedAction.KillCoroutine(_noConnectionsTimeoutHandle);
		GameserverSDK.UpdateConnectedPlayers(_connectedPlayers);
	}

	public static void PlayerDisconnected(string playerId)
	{
		Debug.Log("[PlayfabServerManagement] Player Disconnected: " + playerId);
		_connectedPlayers.RemoveAll((ConnectedPlayer p) => p.PlayerId == playerId);
		GameserverSDK.UpdateConnectedPlayers(_connectedPlayers);
		if (_connectedPlayers.Count <= 0)
		{
			Debug.Log("[PlayfabServerManagement] All Players Disconnected.");
			ShutdownServer();
		}
	}

	public static void ShutdownServer()
	{
		Debug.Log("[PlayfabServerManagement] Shutting Down Server");
		Application.Quit();
	}

	public static bool IsPlayerExpected(string playerId)
	{
		return _expectedPlayers.Contains(playerId);
	}

	private static void OnLogMessageRecieved(string condition, string stackTrace, LogType type)
	{
		GameserverSDK.LogMessage($"{type} - {condition}");
		GameserverSDK.LogMessage(stackTrace);
	}

	private static bool OnHealthUpdate()
	{
		return true;
	}

	private static void OnMaintenanceUpdate(DateTimeOffset obj)
	{
		Debug.Log("[PlayfabServerManagement] OnMaintenanceUpdate called");
	}

	private static void OnShutdownCallback()
	{
		Debug.Log("[PlayfabServerManagement] OnShutdownCallback Called");
		ShutdownServer();
	}

	private static void GetConfigData()
	{
		IDictionary<string, string> configSettings = GameserverSDK.getConfigSettings();
		if (configSettings.TryGetValue("sessionId", out var value))
		{
			SessionId = value;
			Debug.Log("Got Session Id: " + SessionId);
		}
		else
		{
			Debug.LogError("[PlayfabServerManagement] Failed to get Session Id of Server");
		}
		if (configSettings.TryGetValue("instanceId", out var value2))
		{
			ServerId = value2;
			Debug.Log("Got Server Id: " + ServerId);
		}
		else
		{
			Debug.LogError("[PlayfabServerManagement] Failed to get Server Id of Server");
		}
		_expectedPlayers = GameserverSDK.GetInitialPlayers().ToList();
		Debug.Log($"[PlayfabServerManagement] Got {_expectedPlayers.Count} expected players");
		foreach (string expectedPlayer in _expectedPlayers)
		{
			Debug.Log("Expected Player: " + expectedPlayer);
		}
		Debug.Log("[PlayfabServerManagement] Log Directory: " + GameserverSDK.GetLogsDirectory());
	}
}
