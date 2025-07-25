using UnityEngine;

public static class ColliderExtensions
{
	public static Vector3 ClosestPointInside(this Collider collider, Vector3 point)
	{
		Vector3 vector = collider.ClosestPoint(point);
		if (vector == point)
		{
			vector = collider.ClosestPoint(GetPositionOutsideCollider(collider, point));
		}
		return vector;
	}

	public static Vector3 ClosestPointOnBoundsInside(this Collider collider, Vector3 point)
	{
		Vector3 vector = collider.ClosestPointOnBounds(point);
		if (vector == point)
		{
			vector = collider.ClosestPointOnBounds(GetPositionOutsideCollider(collider, point));
		}
		return vector;
	}

	private static Vector3 GetPositionOutsideCollider(Collider collider, Vector3 point)
	{
		Vector3 center = collider.bounds.center;
		Vector3 normalized = Vector3.ProjectOnPlane(point - center, Vector3.up).normalized;
		return point + normalized * (collider.bounds.extents.magnitude - Vector3.Distance(point, center));
	}
}
