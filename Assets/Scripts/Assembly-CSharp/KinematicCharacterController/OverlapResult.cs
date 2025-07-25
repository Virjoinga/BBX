using UnityEngine;

namespace KinematicCharacterController
{
	public struct OverlapResult
	{
		public Vector3 Normal;

		public Collider Collider;

		public OverlapResult(Vector3 normal, Collider collider)
		{
			Normal = normal;
			Collider = collider;
		}
	}
}
