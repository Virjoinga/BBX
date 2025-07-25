using UnityEngine;
using Zenject;

public class TrippingVisualEffect : BaseVisualEffect
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private GameObject _trippingMarker;

	private PlayerController _playerController;

	private void Awake()
	{
		_playerController = GetComponentInParent<PlayerController>();
	}

	protected override void Show()
	{
		if (_playerController != null && _playerController.IsLocal)
		{
			_trippingMarker.SetActive(value: false);
			_signalBus.Fire(new TrippingEffectUpdatedSignal
			{
				IsActive = true
			});
		}
	}

	protected override void Hide()
	{
		if (_playerController != null && _playerController.IsLocal)
		{
			_signalBus.Fire(new TrippingEffectUpdatedSignal
			{
				IsActive = false
			});
		}
	}
}
