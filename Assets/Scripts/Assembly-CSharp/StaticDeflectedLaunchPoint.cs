using System;
using System.Collections;
using UnityEngine;

public class StaticDeflectedLaunchPoint : DeflectedLaunchPoint
{
	[SerializeField]
	private float[] _deflections;

	private int _index;

	protected override void Awake()
	{
		base.Awake();
		if (_deflections.Length != _muzzles.Length)
		{
			Debug.LogError($"[StaticDeflectedLaunchPoint] Deflection count mismatch: {_deflections.Length} deflections, {_muzzles.Length} muzzles");
		}
	}

	protected override IEnumerator FireVolleyRoutine(int volleySize, float volleyDelay, Vector3 position, Vector3 forward, float fireDelay, int projectileId, int serverFrame, Action<HitInfo> onHit, bool isMock)
	{
		_index = 0;
		return base.FireVolleyRoutine(volleySize, volleyDelay, position, forward, fireDelay, projectileId, serverFrame, onHit, isMock);
	}

	protected override void DeflectForward(ref Vector3 forward, int shotCount)
	{
		if (_index >= _deflections.Length)
		{
			_index = 0;
			Debug.LogError($"[StaticDeflectedLaunchPoint] Deflection count mismatch: {_deflections.Length} deflections, but volley size is {base.Profile.VolleySize}");
		}
		Vector3 vector = Quaternion.AngleAxis(_deflections[_index++], Vector3.up) * forward;
		forward.x = vector.x;
		forward.y = vector.y;
		forward.z = vector.z;
	}

	private void OnDrawGizmos()
	{
		if (_deflections.Length != 0 && _muzzles.Length != 0)
		{
			for (int i = 0; i < _deflections.Length; i++)
			{
				Vector3 vector = Quaternion.AngleAxis(_deflections[i], Vector3.up) * _muzzles[i].forward;
				Debug.DrawRay(_muzzles[i].position, vector * 20f, Color.blue);
			}
		}
	}
}
