using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator
{
	[TaskCategory("Unity/Animator")]
	[TaskDescription("Gets the avatar delta position for the last evaluated frame. Returns Success.")]
	public class GetDeltaPosition : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The avatar delta position")]
		[RequiredField]
		public SharedVector3 storeValue;

		private Animator animator;

		private GameObject prevGameObject;

		public override void OnStart()
		{
			GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
			if (defaultGameObject != prevGameObject)
			{
				animator = defaultGameObject.GetComponent<Animator>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (animator == null)
			{
				Debug.LogWarning("Animator is null");
				return TaskStatus.Failure;
			}
			storeValue.Value = animator.deltaPosition;
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeValue = Vector3.zero;
		}
	}
}
