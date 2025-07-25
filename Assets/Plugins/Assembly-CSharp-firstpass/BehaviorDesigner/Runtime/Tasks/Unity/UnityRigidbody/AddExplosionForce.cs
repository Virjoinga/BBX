using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody
{
	[TaskCategory("Unity/Rigidbody")]
	[TaskDescription("Applies a force to the rigidbody that simulates explosion effects. Returns Success.")]
	public class AddExplosionForce : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The force of the explosion")]
		public SharedFloat explosionForce;

		[Tooltip("The position of the explosion")]
		public SharedVector3 explosionPosition;

		[Tooltip("The radius of the explosion")]
		public SharedFloat explosionRadius;

		[Tooltip("Applies the force as if it was applied from beneath the object")]
		public float upwardsModifier;

		[Tooltip("The type of force")]
		public ForceMode forceMode;

		private Rigidbody rigidbody;

		private GameObject prevGameObject;

		public override void OnStart()
		{
			GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
			if (defaultGameObject != prevGameObject)
			{
				rigidbody = defaultGameObject.GetComponent<Rigidbody>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (rigidbody == null)
			{
				Debug.LogWarning("Rigidbody is null");
				return TaskStatus.Failure;
			}
			rigidbody.AddExplosionForce(explosionForce.Value, explosionPosition.Value, explosionRadius.Value, upwardsModifier, forceMode);
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			explosionForce = 0f;
			explosionPosition = Vector3.zero;
			explosionRadius = 0f;
			upwardsModifier = 0f;
			forceMode = ForceMode.Force;
		}
	}
}
