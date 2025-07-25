using Constants;
using UnityEngine;
using Zenject;

public class CameraBlockingEffect : BaseVisualEffect
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private Match.StatusType _statusType;

	private PlayerController _playerController;

	private void Awake()
	{
		_playerController = GetComponentInParent<PlayerController>();
	}

	protected override void Show()
	{
		if (_playerController != null && _playerController.IsLocal)
		{
			_signalBus.Fire(new CameraBlockingEffectUpdatedSignal
			{
				StatusType = _statusType,
				IsActive = true
			});
		}
	}

	protected override void Hide()
	{
		if (_playerController != null && _playerController.IsLocal)
		{
			_signalBus.Fire(new CameraBlockingEffectUpdatedSignal
			{
				StatusType = _statusType,
				IsActive = false
			});
		}
	}
}
