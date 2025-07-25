using Constants;
using UnityEngine;
using Zenject;

public class ShowWhenStatusFlagSet : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private Match.StatusType _statusFlag;

	private void Awake()
	{
		_signalBus.Subscribe<StatusFlagsupdatedSignal>(OnStatusFlagsUpdated);
		base.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<StatusFlagsupdatedSignal>(OnStatusFlagsUpdated);
	}

	private void OnStatusFlagsUpdated(StatusFlagsupdatedSignal signal)
	{
		base.gameObject.SetActive(signal.Flags.Has(_statusFlag));
	}
}
