using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCircleCollider2D
{
	[TaskCategory("Unity/CircleCollider2D")]
	[TaskDescription("Sets the radius of the CircleCollider2D. Returns Success.")]
	public class SetRadius : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The radius of the CircleCollider2D")]
		public SharedFloat radius;

		private CircleCollider2D circleCollider2D;

		private GameObject prevGameObject;

		public override void OnStart()
		{
			GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
			if (defaultGameObject != prevGameObject)
			{
				circleCollider2D = defaultGameObject.GetComponent<CircleCollider2D>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (circleCollider2D == null)
			{
				Debug.LogWarning("CircleCollider2D is null");
				return TaskStatus.Failure;
			}
			circleCollider2D.radius = radius.Value;
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			radius = 0f;
		}
	}
}
