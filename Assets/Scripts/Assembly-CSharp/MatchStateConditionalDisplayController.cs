using UnityEngine;
using Zenject;

public class MatchStateConditionalDisplayController : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private MatchState _dispalyIf = MatchState.Active;

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
		base.gameObject.SetActive(signal.MatchState == _dispalyIf);
	}
}
