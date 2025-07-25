using System;
using UnityEngine;

public class ExplosiveRaycastLaunchPoint : LaunchPoint
{
	[SerializeField]
	private GameObject _explosiveImpactPrefab;

	protected override void FireRayCast(Vector3 position, ref Vector3 forward, int serverFrame, Action<HitInfo> onHit)
	{
		if (Physics.Raycast(position, forward, out var hitInfo, 250f, base._hitableLayers))
		{
			return;
		}
		hitInfo.point = base.transform.position + forward * 250f;
		hitInfo.normal = -forward;
		HitInfo originalHit = new HitInfo
		{
			launchServerFrame = serverFrame,
			point = hitInfo.point,
			collider = hitInfo.collider,
			weaponProfile = base.Profile,
			weaponId = base.Profile.Id
		};
		foreach (HitInfo item in AreaEffectCalculator.GetAffectableInRange(base.Profile.Explosion.Range, originalHit))
		{
			OnHit(item);
			onHit?.Invoke(item);
		}
	}

	private void OnHit(HitInfo hitInfo)
	{
		if (_explosiveImpactPrefab != null && !BoltNetwork.IsServer)
		{
			SmartPool.Spawn(_explosiveImpactPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
		}
	}
}
