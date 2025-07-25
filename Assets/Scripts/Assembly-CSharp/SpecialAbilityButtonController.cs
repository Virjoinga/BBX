using Rewired.ComponentControls;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SpecialAbilityButtonController : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private Button _button;

	[SerializeField]
	private TouchButton _touchbutton;

	[SerializeField]
	private Image _cooldownDisplay;

	private void Awake()
	{
		_signalBus.Subscribe<SpecialAbilityStateUpdatedSignal>(OnStateUpdated);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<SpecialAbilityStateUpdatedSignal>(OnStateUpdated);
	}

	private void OnStateUpdated(SpecialAbilityStateUpdatedSignal signal)
	{
		if (_button != null)
		{
			_button.interactable = signal.CanUse;
		}
		if (_touchbutton != null)
		{
			_touchbutton.interactable = signal.CanUse;
		}
		_cooldownDisplay.fillAmount = signal.ChargePercent;
	}
}
