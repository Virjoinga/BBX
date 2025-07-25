using UnityEngine;
using Zenject;

public class DestroyWhenBRMatchEnds : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	private void Awake()
	{
		_signalBus.Subscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
	}

	private void OnMatchStateUpdated(MatchStateUpdatedSignal signal)
	{
		if (signal.MatchState == MatchState.Complete && base.gameObject != null)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
