using System;
using System.Collections.Generic;
using UnityEngine;

public class SpreadLaunchPoint : LaunchPoint
{
	private Collider[] _hitColliders = new Collider[15];

	private LayerMask _spreadableLayers => LayerMaskConfig.AffectableLayers;

	protected override void FireRayCast(Vector3 position, ref Vector3 forward, int serverFrame, Action<HitInfo> onHit)
	{
		int num = Physics.OverlapSphereNonAlloc(position, base.Profile.Range, _hitColliders, _spreadableLayers);
		for (int i = 0; i < num; i++)
		{
			if (_hitColliders[i] == null)
			{
				continue;
			}
			Collider collider = _hitColliders[i];
			if (collider.bounds.Contains(position))
			{
				HitInfo obj = new HitInfo
				{
					launchServerFrame = serverFrame,
					origin = base.transform.position,
					forward = base.transform.forward,
					collider = collider,
					point = position,
					normal = position,
					distance = 0f,
					weaponProfile = base.Profile,
					weaponId = base.Profile.Id
				};
				onHit(obj);
				_hitColliders[i] = null;
				continue;
			}
			foreach (Vector3 item in new List<Vector3>
			{
				collider.bounds.center,
				new Vector3(collider.bounds.center.x, collider.bounds.max.y, collider.bounds.center.z),
				new Vector3(collider.bounds.center.x, collider.bounds.min.y, collider.bounds.center.z),
				new Vector3(collider.bounds.max.x, collider.bounds.center.y, collider.bounds.center.z),
				new Vector3(collider.bounds.min.x, collider.bounds.center.y, collider.bounds.center.z),
				new Vector3(collider.bounds.center.x, collider.bounds.center.y, collider.bounds.max.z),
				new Vector3(collider.bounds.center.x, collider.bounds.center.y, collider.bounds.min.z)
			})
			{
				Debug.DrawLine(position, item, Color.green, 5f);
				Vector3 to = item - position;
				if (Vector3.Angle(forward, to) <= base.Profile.Spread.Amount && Physics.Linecast(position, item, out var hitInfo, base._hitableLayers))
				{
					HitInfo obj2 = new HitInfo
					{
						launchServerFrame = serverFrame,
						origin = base.transform.position,
						forward = base.transform.forward,
						collider = collider,
						point = hitInfo.point,
						normal = hitInfo.normal,
						distance = hitInfo.distance,
						weaponProfile = base.Profile,
						weaponId = base.Profile.Id
					};
					onHit(obj2);
					TrySpawnImpactEffect(hitInfo);
					_hitColliders[i] = null;
					break;
				}
			}
		}
		TrySpawnLaunchEffect(position, forward, Vector3.one);
	}
}
