using UnityEngine;

public class SpherecastHitDetectingContinuousFireLaunchPoint : ContinuousFireLaunchPoint
{
	private void FixedUpdate()
	{
		if (base.Profile != null && _detecting)
		{
			float num = base.Profile.Range;
			if (num == 0f)
			{
				num = 200f;
			}
			Ray ray = new Ray(base.transform.position, base.transform.forward);
			DebugExtension.DebugArrow(ray.origin, ray.direction * 2f, Color.red);
			if (Physics.SphereCast(ray, Radius, out var hitInfo, num, LayerMaskConfig.AffectableLayers) && !(hitInfo.collider == null) && !(hitInfo.collider == _wielder) && !_collidersHit.Contains(hitInfo.collider))
			{
				_collidersHit.Add(hitInfo.collider);
				hitInfo.point = ray.origin + ray.direction * hitInfo.distance;
				HitInfo obj = new HitInfo
				{
					origin = base.transform.position,
					forward = base.transform.forward,
					launchServerFrame = _serverFrame,
					collider = hitInfo.collider,
					point = hitInfo.point,
					normal = hitInfo.normal,
					weaponProfile = base.Profile,
					weaponId = base.Profile.Id,
					hitType = HitType.Continuous
				};
				_onHit(obj);
			}
		}
	}
}
