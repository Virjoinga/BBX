using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCapsuleCollider
{
	[TaskCategory("Unity/CapsuleCollider")]
	[TaskDescription("Sets the height of the CapsuleCollider. Returns Success.")]
	public class SetHeight : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The height of the CapsuleCollider")]
		public SharedFloat direction;

		private CapsuleCollider capsuleCollider;

		private GameObject prevGameObject;

		public override void OnStart()
		{
			GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
			if (defaultGameObject != prevGameObject)
			{
				capsuleCollider = defaultGameObject.GetComponent<CapsuleCollider>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (capsuleCollider == null)
			{
				Debug.LogWarning("CapsuleCollider is null");
				return TaskStatus.Failure;
			}
			capsuleCollider.height = direction.Value;
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			direction = 0f;
		}
	}
}
