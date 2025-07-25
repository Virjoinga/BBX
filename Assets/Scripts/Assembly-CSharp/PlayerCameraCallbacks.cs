using Bolt;
using UnityEngine;
using Zenject;

[BoltGlobalBehaviour(BoltNetworkModes.Client, new string[] { "OfflineMovementTest" })]
public class PlayerCameraCallbacks : GlobalEventListener
{
	[Inject]
	private ZenjectInstantiater _zenjectInstantiater;

	[SerializeField]
	private GameObject _otsCamera;

	public override void SceneLoadLocalDone(string map)
	{
		_zenjectInstantiater.Instantiate(_otsCamera);
	}
}
