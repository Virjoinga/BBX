using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ReloadIndicatorController : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private Image _indicator;

	[SerializeField]
	private GameObject _container;

	private void Start()
	{
		_signalBus.Subscribe<ReloadStateUpdatedSignal>(OnReloadStateUpdated);
		_container.SetActive(value: false);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<ReloadStateUpdatedSignal>(OnReloadStateUpdated);
	}

	private void OnReloadStateUpdated(ReloadStateUpdatedSignal signal)
	{
		if (signal.isReloading)
		{
			float time = signal.profile.Reload.Time;
			float reloadStartTime = signal.reloadStartTime;
			float num = signal.currentTime - reloadStartTime;
			_indicator.fillAmount = num / time;
		}
		_container.SetActive(signal.isReloading);
	}
}
