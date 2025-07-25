using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody2D
{
	[TaskCategory("Unity/Rigidbody2D")]
	[TaskDescription("Moves the Rigidbody2D to the specified position. Returns Success.")]
	public class MovePosition : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The new position of the Rigidbody")]
		public SharedVector2 position;

		private Rigidbody2D rigidbody2D;

		private GameObject prevGameObject;

		public override void OnStart()
		{
			GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
			if (defaultGameObject != prevGameObject)
			{
				rigidbody2D = defaultGameObject.GetComponent<Rigidbody2D>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (rigidbody2D == null)
			{
				Debug.LogWarning("Rigidbody is null");
				return TaskStatus.Failure;
			}
			rigidbody2D.MovePosition(position.Value);
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			position = Vector2.zero;
		}
	}
}
