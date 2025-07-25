using UnityEngine;

namespace KinematicCharacterController
{
	[CreateAssetMenu]
	public class KCCSettings : ScriptableObject
	{
		[Tooltip("Determines if the system simulates automatically. If true, the simulation is done on FixedUpdate")]
		public bool AutoSimulation = true;

		[Tooltip("Should interpolation of characters and PhysicsMovers be handled")]
		public bool Interpolate = true;

		[Tooltip("Determines if the system calls Physics.SyncTransforms() in interpolation frames, for interpolated collider position information")]
		public bool SyncInterpolatedPhysicsTransforms;
	}
}
