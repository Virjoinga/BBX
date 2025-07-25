using System.Collections.Generic;
using UnityEngine;

public static class AreaEffectCalculator
{
	private static Collider[] _hitColliders = new Collider[50];

	private static Vector3[] _hitPositions = new Vector3[50];

	private static LayerMask _explodableLayers => LayerMaskConfig.AffectableLayers;

	public static List<HitInfo> GetAffectableInRange(float effectRadius, HitInfo originalHit)
	{
		return GetAffectableInRange(effectRadius, Vector3.zero, 360f, originalHit);
	}

	public static List<HitInfo> GetAffectableInRange(float effectRadius, Vector3 weaponForward, float maxAngle, HitInfo originalHit)
	{
		_hitColliders = new Collider[50];
		int num = Physics.OverlapSphereNonAlloc(originalHit.point, effectRadius, _hitColliders, _explodableLayers);
		for (int i = 0; i < num; i++)
		{
			_hitPositions[i] = _hitColliders[i].bounds.center;
		}
		return GetAffectableInRange(num, weaponForward, maxAngle, originalHit);
	}

	public static List<HitInfo> GetAffectableInRangeServer(float effectRadius, HitInfo originalHit, int serverFrame)
	{
		return GetAffectableInRangeServer(effectRadius, Vector3.zero, 360f, originalHit, serverFrame);
	}

	public static List<HitInfo> GetAffectableInRangeServer(float effectRadius, Vector3 weaponForward, float maxAngle, HitInfo originalHit, int serverFrame)
	{
		_hitColliders = new Collider[50];
		using (BoltPhysicsHits boltPhysicsHits = BoltNetwork.OverlapSphereAll(originalHit.point, effectRadius, serverFrame))
		{
			Debug.Log($"[AreaEffectCalculator] Found {boltPhysicsHits.count} BoltHitboxes within ({effectRadius})m of {originalHit.point}");
			Dictionary<BoltHitboxBody, BoltHit> dictionary = new Dictionary<BoltHitboxBody, BoltHit>();
			for (int i = 0; i < boltPhysicsHits.count; i++)
			{
				BoltPhysicsHit hit = boltPhysicsHits[i];
				if (hit.hitbox.hitboxType != BoltHitboxType.Proximity && hit.hitbox.hitboxType != BoltHitboxType.Head)
				{
					if (!dictionary.TryGetValue(hit.body, out var value))
					{
						value = new BoltHit(hit);
						dictionary.Add(hit.body, value);
					}
					else
					{
						value.Add(boltPhysicsHits[i]);
					}
				}
			}
			int num = 0;
			foreach (BoltHit value2 in dictionary.Values)
			{
				_hitColliders[num] = value2.Damageable.HurtCollider;
				BoltHitbox closestHitbox = value2.ClosestHitbox;
				Vector3 normalized = (closestHitbox.transform.TransformPoint(closestHitbox.hitboxCenter) - originalHit.point).normalized;
				_hitPositions[num] = originalHit.point + normalized * value2.Distance;
				num++;
			}
			return GetAffectableInRange(num, weaponForward, maxAngle, originalHit);
		}
	}

	private static List<HitInfo> GetAffectableInRange(int count, Vector3 weaponForward, float maxAngle, HitInfo originalHit)
	{
		List<HitInfo> list = new List<HitInfo>();
		Vector3 point = originalHit.point;
		Debug.Log($"[AreaEffectCalculator] Found {count} affectables within range");
		for (int i = 0; i < count; i++)
		{
			if (_hitColliders[i] == null)
			{
				continue;
			}
			Collider collider = _hitColliders[i];
			Vector3 vector = collider.ClosestPointOnBounds(point);
			if (collider.bounds.Contains(point))
			{
				HitInfo item = new HitInfo
				{
					hitType = originalHit.hitType,
					launchServerFrame = originalHit.launchServerFrame,
					directHit = (collider == originalHit.collider),
					origin = point,
					forward = originalHit.forward,
					point = point,
					normal = originalHit.normal,
					collider = collider,
					distance = 0f,
					chargeTime = originalHit.chargeTime,
					weaponId = originalHit.weaponId
				};
				IDamageable componentInParent = collider.GetComponentInParent<IDamageable>();
				if (componentInParent != null)
				{
					item.hitEntity = componentInParent.entity;
				}
				list.Add(item);
				_hitColliders[i] = null;
				continue;
			}
			foreach (Vector3 item3 in new List<Vector3>
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
				Vector3 vector2 = item3 - point;
				bool num = IsWithinAngle(vector2, weaponForward, maxAngle);
				bool flag = PathIsClear(item3, point);
				if (num && flag)
				{
					HitInfo item2 = new HitInfo
					{
						hitType = originalHit.hitType,
						launchServerFrame = originalHit.launchServerFrame,
						directHit = (collider == originalHit.collider),
						origin = point,
						forward = vector2,
						point = vector,
						normal = -vector2,
						collider = collider,
						distance = Vector3.Distance(point, vector),
						chargeTime = originalHit.chargeTime,
						weaponId = originalHit.weaponId
					};
					IDamageable componentInParent2 = collider.GetComponentInParent<IDamageable>();
					if (componentInParent2 != null)
					{
						item2.hitEntity = componentInParent2.entity;
					}
					list.Add(item2);
					_hitColliders[i] = null;
					break;
				}
			}
		}
		Debug.Log($"[AreaEffectCalculator] Found {list.Count} validated affectables within range");
		return list;
	}

	public static bool IsWithinAngle(Vector3 directionToPoint, Vector3 weaponForward, float maxAngle)
	{
		if (maxAngle >= 360f)
		{
			return true;
		}
		return Vector3.Angle(weaponForward, directionToPoint) <= maxAngle;
	}

	private static bool PathIsClear(Vector3 point, Vector3 origin)
	{
		return !Physics.Linecast(origin, point, LayerMaskConfig.GroundLayers);
	}
}
