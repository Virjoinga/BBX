using BoltInternal;
using UdpKit;
using UnityEngine;

public class BoltDebugStart : GlobalEventListenerBase
{
	private UdpEndPoint _serverEndPoint;

	private UdpEndPoint _clientEndPoint;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		Application.targetFrameRate = 60;
	}

	private void Start()
	{
		BoltRuntimeSettings instance = BoltRuntimeSettings.instance;
		_serverEndPoint = new UdpEndPoint(UdpIPv4Address.Localhost, (ushort)instance.debugStartPort);
		_clientEndPoint = new UdpEndPoint(UdpIPv4Address.Localhost, 0);
		BoltConfig configCopy = instance.GetConfigCopy();
		configCopy.connectionTimeout = 60000000;
		configCopy.connectionRequestTimeout = 500;
		configCopy.connectionRequestAttempts = 1000;
		if (!string.IsNullOrEmpty(instance.debugStartMapName))
		{
			if (BoltDebugStartSettings.DebugStartIsServer)
			{
				BoltLauncher.StartServer(_serverEndPoint, configCopy);
			}
			else if (BoltDebugStartSettings.DebugStartIsClient)
			{
				BoltLauncher.StartClient(_clientEndPoint, configCopy);
			}
			BoltDebugStartSettings.PositionWindow();
		}
	}

	public override void BoltStartFailed()
	{
	}

	public override void BoltStartDone()
	{
		if (BoltNetwork.IsServer)
		{
			BoltNetwork.LoadScene(BoltRuntimeSettings.instance.debugStartMapName);
		}
		else
		{
			BoltNetwork.Connect((ushort)BoltRuntimeSettings.instance.debugStartPort);
		}
	}

	public override void SceneLoadLocalDone(string map)
	{
		Object.Destroy(base.gameObject);
	}
}
