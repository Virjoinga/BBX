using System.Collections;
using UnityEngine;

public class DropGroundEffectOnDamageInterval : BaseContinuousFireDisplay
{
	[SerializeField]
	private GameObject _effect;

	[SerializeField]
	private float _maxDistance = 1f;

	private LaunchPoint _launchPoint;

	private Coroutine _dropRoutine;

	private Ray _ray = new Ray(Vector3.zero, Vector3.down);

	private void Awake()
	{
		_launchPoint = GetComponent<LaunchPoint>();
	}

	public override void Toggle(bool isOn)
	{
		if (isOn)
		{
			_dropRoutine = StartCoroutine(DropRoutine());
		}
		else if (_dropRoutine != null)
		{
			StopCoroutine(_dropRoutine);
			_dropRoutine = null;
		}
	}

	private IEnumerator DropRoutine()
	{
		float interval = _launchPoint.Profile.DamageInterval;
		while (true)
		{
			_ray.origin = base.transform.position;
			if (!Physics.Raycast(_ray, out var hitInfo, _maxDistance, LayerMaskConfig.GroundLayers))
			{
				hitInfo.point = _ray.GetPoint(_maxDistance);
				hitInfo.normal = Vector3.up;
			}
			SmartPool.Spawn(_effect, hitInfo.point, Quaternion.LookRotation(base.transform.forward, hitInfo.normal));
			yield return new WaitForSeconds(interval);
		}
	}
}
