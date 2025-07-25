using BSCore;
using Zenject;

public class RequestRespawnButton : UIBaseButtonClickHandler
{
	[Inject]
	private SignalBus _signalBus;

	protected override void OnClick()
	{
		_signalBus.Fire(default(TryRequestRespawn));
	}
}
