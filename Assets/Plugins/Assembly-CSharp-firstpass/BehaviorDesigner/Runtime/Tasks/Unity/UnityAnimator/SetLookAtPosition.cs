using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator
{
	[TaskCategory("Unity/Animator")]
	[TaskDescription("Sets the look at position. Returns Success.")]
	public class SetLookAtPosition : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The position to lookAt")]
		public SharedVector3 position;

		private Animator animator;

		private GameObject prevGameObject;

		private bool positionSet;

		public override void OnStart()
		{
			GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
			if (defaultGameObject != prevGameObject)
			{
				animator = defaultGameObject.GetComponent<Animator>();
				prevGameObject = defaultGameObject;
			}
			positionSet = false;
		}

		public override TaskStatus OnUpdate()
		{
			if (animator == null)
			{
				Debug.LogWarning("Animator is null");
				return TaskStatus.Failure;
			}
			if (!positionSet)
			{
				return TaskStatus.Running;
			}
			return TaskStatus.Success;
		}

		public override void OnAnimatorIK()
		{
			if (!(animator == null))
			{
				animator.SetLookAtPosition(position.Value);
				positionSet = true;
			}
		}

		public override void OnReset()
		{
			targetGameObject = null;
			position = Vector3.zero;
		}
	}
}
