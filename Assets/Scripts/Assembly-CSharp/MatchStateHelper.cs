using UnityEngine;
using Zenject;

public class MatchStateHelper : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	public MatchState MatchStateCached { get; private set; }

	private void Start()
	{
		_signalBus.Subscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
	}

	private void OnMatchStateUpdated(MatchStateUpdatedSignal signal)
	{
		MatchStateCached = signal.MatchState;
	}
}
