using Bolt;
using UnityEngine;

[BoltGlobalBehaviour]
public class BoltRuntimeConfigs : GlobalEventListener
{
	private void Awake()
	{
		UnitySettings.IsBuildMono = true;
		UnitySettings.CurrentPlatform = RuntimePlatform.WindowsPlayer;
	}
}
