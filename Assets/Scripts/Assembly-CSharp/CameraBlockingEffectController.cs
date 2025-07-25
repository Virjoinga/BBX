using System;
using System.Linq;
using Constants;
using UnityEngine;
using Zenject;

public class CameraBlockingEffectController : MonoBehaviour
{
	[Serializable]
	private class StatusVisual
	{
		public Match.StatusType statusType;

		public GameObject visual;
	}

	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private StatusVisual[] _statusVisuals;

	private void Start()
	{
		_signalBus.Subscribe<CameraBlockingEffectUpdatedSignal>(OnCameraBlockingEffectUpdated);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<CameraBlockingEffectUpdatedSignal>(OnCameraBlockingEffectUpdated);
	}

	private void OnCameraBlockingEffectUpdated(CameraBlockingEffectUpdatedSignal signal)
	{
		StatusVisual statusVisual = _statusVisuals.FirstOrDefault((StatusVisual x) => x.statusType == signal.StatusType);
		if (statusVisual == null)
		{
			Debug.LogError($"[CameraBlockingEffectController] No Status Visual setup for type {signal.StatusType}");
		}
		else
		{
			statusVisual.visual.SetActive(signal.IsActive);
		}
	}
}
