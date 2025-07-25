using System.Collections;
using UnityEngine;
using Zenject;

public class PathBlockedIndicatorController : MonoBehaviour
{
	[Inject]
	private SignalBus _signalbus;

	[SerializeField]
	private float _shouldShowThreshold = 0.25f;

	private float _distanceThreshold = 0.5f;

	private bool _shouldShow;

	private float _shouldShowTimer;

	private IEnumerator Start()
	{
		yield return new WaitUntil(() => Camera.main != null);
		_signalbus.Subscribe<ShotPathData>(OnShotPathDataUpdated);
		base.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		_signalbus.TryUnsubscribe<ShotPathData>(OnShotPathDataUpdated);
	}

	private void OnShotPathDataUpdated(ShotPathData data)
	{
		_shouldShow = !data.HideShotPathBlockedIcon && Vector3.Distance(data.AimPosition, data.HitPosition) > _distanceThreshold;
		if (_shouldShow)
		{
			_shouldShowTimer += Time.deltaTime;
		}
		else
		{
			_shouldShowTimer = 0f;
		}
		base.gameObject.SetActive(_shouldShowTimer >= _shouldShowThreshold);
		base.transform.position = Camera.main.WorldToScreenPoint(data.HitPosition);
	}
}
