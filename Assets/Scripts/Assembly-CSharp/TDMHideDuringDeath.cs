using System.Collections;
using Constants;
using UnityEngine;
using Zenject;

public class TDMHideDuringDeath : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private GameObject _container;

	private void Start()
	{
		_signalBus.Subscribe<LocalPlayerDiedSignal>(OnLocalPlayerDied);
		_signalBus.Subscribe<LocalPlayerRespawnedSignal>(OnLocalPlayerRespawned);
	}

	private void OnDestroy()
	{
		_signalBus.TryUnsubscribe<LocalPlayerDiedSignal>(OnLocalPlayerDied);
		_signalBus.TryUnsubscribe<LocalPlayerRespawnedSignal>(OnLocalPlayerRespawned);
	}

	private void OnLocalPlayerDied()
	{
		StartCoroutine(DisableUIAfterDelay());
	}

	private void OnLocalPlayerRespawned()
	{
		_container.SetActive(value: true);
	}

	private IEnumerator DisableUIAfterDelay()
	{
		yield return new WaitForSeconds(Match.RESPAWNUI_DELAY);
		_container.SetActive(value: false);
	}
}
