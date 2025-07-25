using System.Collections;
using Constants;
using UnityEngine;
using Zenject;

public class RespawnCamera : MonoBehaviourSingleton<RespawnCamera>
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private Camera _camera;

	[SerializeField]
	private MeshRenderer _mapBounds;

	private bool _matchCompleted;

	public Camera RespawnCam => _camera;

	private void Start()
	{
		SetCameraOrthSize();
		_camera.gameObject.SetActive(value: false);
		_signalBus.Subscribe<LocalPlayerDiedSignal>(OnLocalPlayerDied);
		_signalBus.Subscribe<LocalPlayerRespawnedSignal>(OnLocalPlayerRespawned);
		_signalBus.Subscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
	}

	protected override void OnDestroy()
	{
		_signalBus.TryUnsubscribe<LocalPlayerDiedSignal>(OnLocalPlayerDied);
		_signalBus.TryUnsubscribe<LocalPlayerRespawnedSignal>(OnLocalPlayerRespawned);
		_signalBus.TryUnsubscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
		base.OnDestroy();
	}

	private void SetCameraOrthSize()
	{
		float num = (float)_camera.pixelWidth / (float)_camera.pixelHeight;
		float num2 = _mapBounds.bounds.size.x / _mapBounds.bounds.size.y;
		if (num >= num2)
		{
			_camera.orthographicSize = _mapBounds.bounds.size.y / 2f;
			return;
		}
		float num3 = num2 / num;
		_camera.orthographicSize = _mapBounds.bounds.size.y / 2f * num3;
	}

	private void OnLocalPlayerDied()
	{
		if (!_matchCompleted)
		{
			StartCoroutine(EnableAfterDelay());
		}
	}

	private void OnLocalPlayerRespawned()
	{
		if (!_matchCompleted)
		{
			_camera.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator EnableAfterDelay()
	{
		yield return new WaitForSeconds(Match.RESPAWNUI_DELAY);
		if (!_matchCompleted)
		{
			_camera.gameObject.SetActive(value: true);
		}
	}

	private void OnMatchStateUpdated(MatchStateUpdatedSignal signal)
	{
		if (signal.MatchState == MatchState.Complete)
		{
			_matchCompleted = true;
			_camera.gameObject.SetActive(value: false);
		}
	}
}
