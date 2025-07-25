using System.Collections;
using UnityEngine;

public class RepetativeShotLaunchPoint : ContinuousFireLaunchPoint
{
	protected override IEnumerator ContinuousFireRoutine()
	{
		if (_display != null)
		{
			_display.DisplayFor(base.Profile.Cooldown);
		}
		float timer = 0f;
		Debug.Log($"[ContinuousFireLaunchPoint] Starting fire sequence at {Time.realtimeSinceStartup}. Duration: {base.Profile.Cooldown}, interval: {base.Profile.DamageInterval}");
		int serverFrame = _parentWeapon.GetServerFrame();
		if (base.Profile.DamageInterval > 0f)
		{
			for (; timer < base.Profile.Cooldown; timer += base.Profile.DamageInterval)
			{
				Vector3 position = base.transform.position;
				Vector3 forward = base.transform.forward;
				Debug.Log($"[ContinuousFireLaunchPoint] Firing at {Time.realtimeSinceStartup} (timer: {timer}, interval: {base.Profile.DamageInterval}, cooldown: {base.Profile.Cooldown}");
				if (_projectilePrefab == null)
				{
					FireRayCast(position, ref forward, serverFrame, _onHit);
				}
				else
				{
					FireProjectile(position, ref forward, 0f, 0, serverFrame, _onHit);
				}
				yield return new WaitForSeconds(base.Profile.DamageInterval);
			}
		}
		else
		{
			yield return new WaitForSeconds(base.Profile.Cooldown);
		}
		Debug.Log($"[ContinuousFireLaunchPoint] Ending hit detection at {Time.realtimeSinceStartup}");
		_fireCoroutine = null;
	}
}
