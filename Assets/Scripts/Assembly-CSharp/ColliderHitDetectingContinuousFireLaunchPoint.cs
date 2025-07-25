using UnityEngine;

public class ColliderHitDetectingContinuousFireLaunchPoint : ContinuousFireLaunchPoint
{
	[SerializeField]
	private int _sweepCount = 5;

	private void OnTriggerStay(Collider collider)
	{
		if (_detecting && !(collider == null) && !(collider == _wielder) && !_collidersHit.Contains(collider))
		{
			_collidersHit.Add(collider);
			Vector3 vector = collider.ClosestPoint(base.transform.position);
			Vector3 normalized = (vector - base.transform.position).normalized;
			HitInfo obj = new HitInfo
			{
				origin = base.transform.position,
				forward = normalized,
				launchServerFrame = _serverFrame,
				collider = collider,
				point = vector,
				normal = -normalized,
				weaponProfile = base.Profile,
				weaponId = base.Profile.Id,
				hitType = HitType.Continuous
			};
			_onHit(obj);
		}
	}

	protected bool SweepForHits(Collider target, out Vector3 lastValidHitPosition)
	{
		Vector3 center = target.bounds.center;
		Vector3 forward = base.transform.forward;
		Vector3 normalized = (center - base.transform.position).normalized;
		lastValidHitPosition = Vector3.zero;
		Ray ray = new Ray(base.transform.position, base.transform.forward);
		float range = base.Profile.Range;
		int num = 0;
		RaycastHit[] array = new RaycastHit[50];
		float num2 = 1f / (float)_sweepCount;
		for (float num3 = 0f; num3 <= 1f; num3 += num2)
		{
			Vector3 normalized2 = Vector3.Lerp(forward, normalized, num3).normalized;
			ray.direction = normalized2;
			Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 2f);
			int num4 = Physics.RaycastNonAlloc(ray, array, range, LayerMaskConfig.AffectableLayers);
			if (num4 > 0)
			{
				float distance = array[0].distance;
				RaycastHit raycastHit = array[0];
				for (int i = 0; i < num4; i++)
				{
					DebugExtension.DebugPoint(array[i].point, Color.red, 0.5f, 2f);
					if (array[i].distance < distance)
					{
						distance = array[i].distance;
					}
				}
				if (!LayerMaskConfig.GroundLayers.ContainsLayer(raycastHit.collider.gameObject.layer))
				{
					num++;
					lastValidHitPosition = raycastHit.point;
				}
			}
			if (num >= _minHitCountForValidSweep)
			{
				return true;
			}
		}
		return false;
	}

	protected override bool CastAgainstGround(Ray ray, float distance)
	{
		Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 2f);
		return Physics.Raycast(ray, distance, LayerMaskConfig.GroundLayers);
	}

	protected override bool CastAgainstGround(Ray ray, float distance, out RaycastHit hit)
	{
		Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 2f);
		return Physics.Raycast(ray, out hit, distance, LayerMaskConfig.GroundLayers);
	}
}
