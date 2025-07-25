using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousFireLaunchPoint : LaunchPoint
{
	protected readonly List<Collider> _collidersHit = new List<Collider>();

	protected Collider _wielder;

	protected bool _detecting;

	protected Action<HitInfo> _onHit;

	protected Coroutine _fireCoroutine;

	protected int _serverFrame;

	protected int _minHitCountForValidSweep = 1;

	protected IContinuousFireDisplay _display;

	public override float Radius => 0f;

	protected override void Awake()
	{
		base.Awake();
		_wielder = GetComponentInParent<IPlayerController>().HurtCollider;
		_display = GetComponent<IContinuousFireDisplay>();
	}

	public void CancelFire()
	{
		if (_fireCoroutine != null)
		{
			StopCoroutine(_fireCoroutine);
			if (_display != null)
			{
				_display.CancelDisplayFor();
			}
			_detecting = false;
			_fireCoroutine = null;
		}
	}

	protected override void FireInternal(Vector3 position, ref Vector3 forward, float fireDelay, int projectileId, int serverFrame, Action<HitInfo> onHit, bool isMock = false)
	{
		_onHit = onHit;
		_serverFrame = serverFrame;
		if (_fireCoroutine != null)
		{
			StopCoroutine(_fireCoroutine);
		}
		_fireCoroutine = StartCoroutine(ContinuousFireRoutine());
	}

	protected virtual void ClearHitDetectionCache()
	{
		_collidersHit.Clear();
	}

	protected virtual IEnumerator ContinuousFireRoutine()
	{
		if (_display != null)
		{
			_display.DisplayFor(base.Profile.Cooldown);
		}
		ClearHitDetectionCache();
		_detecting = true;
		float timer = 0f;
		Debug.Log($"[ContinuousFireLaunchPoint] Starting hit detection at {Time.realtimeSinceStartup}. Duration: {base.Profile.Cooldown}, interval: {base.Profile.DamageInterval}");
		if (base.Profile.DamageInterval > 0f)
		{
			while (timer < base.Profile.Cooldown)
			{
				yield return new WaitForSeconds(base.Profile.DamageInterval);
				timer += base.Profile.DamageInterval;
				Debug.Log($"[ContinuousFireLaunchPoint] Resetting hit detection at {Time.realtimeSinceStartup} (timer: {timer}, interval: {base.Profile.DamageInterval}, cooldown: {base.Profile.Cooldown}");
				ClearHitDetectionCache();
			}
		}
		else
		{
			yield return new WaitForSeconds(base.Profile.Cooldown);
		}
		Debug.Log($"[ContinuousFireLaunchPoint] Ending hit detection at {Time.realtimeSinceStartup}");
		_detecting = false;
		yield return new WaitForSeconds(0.05f);
		_fireCoroutine = null;
	}
}
