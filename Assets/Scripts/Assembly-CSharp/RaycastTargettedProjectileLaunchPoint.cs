using System;
using UnityEngine;

public class RaycastTargettedProjectileLaunchPoint : LaunchPoint
{
	[SerializeField]
	private bool _overrideProjectileForward;

	[SerializeField]
	private Vector3 _projectileInitialForward = Vector3.up;

	protected override void FireInternal(Vector3 position, ref Vector3 forward, float fireDelay, int projectileId, int serverFrame, Action<HitInfo> onHit, bool isMock = false)
	{
		Vector3 forward2 = (_overrideProjectileForward ? (_projectileInitialForward + forward * 0.001f) : forward);
		FireProjectile(base.transform.position, ref forward2, fireDelay, projectileId, serverFrame, onHit, isMock);
	}

	protected override Projectile.LaunchDetails GenerateLaunchDetails(int projectileId, int serverFrame)
	{
		Projectile.LaunchDetails result = base.GenerateLaunchDetails(projectileId, serverFrame);
		result.aimTarget = base.AimPosition;
		if (_muzzles.Length > 1)
		{
			result.aimTarget += _muzzles[_muzzleIndex].position - base.transform.position;
		}
		if (_useAimPointHandlerForAiming)
		{
			result.isAimOriginOverriden = true;
			result.originOverride = base._aimPointHandler.transform.position;
			result.forwardOverride = base._aimPointHandler.transform.forward;
		}
		return result;
	}
}
