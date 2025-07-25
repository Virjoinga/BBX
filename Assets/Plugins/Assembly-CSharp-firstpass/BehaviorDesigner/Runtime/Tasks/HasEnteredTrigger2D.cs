using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Returns success when an object enters the 2D trigger. This task will only receive the physics callback if it is being reevaluated (with a conditional abort or under a parallel task).")]
	[TaskCategory("Physics")]
	public class HasEnteredTrigger2D : Conditional
	{
		[Tooltip("The tag of the GameObject to check for a trigger against")]
		public SharedString tag = "";

		[Tooltip("The object that entered the trigger")]
		public SharedGameObject otherGameObject;

		private bool enteredTrigger;

		public override TaskStatus OnUpdate()
		{
			if (!enteredTrigger)
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		}

		public override void OnEnd()
		{
			enteredTrigger = false;
		}

		public override void OnTriggerEnter2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(tag.Value) || tag.Value.Equals(other.gameObject.tag))
			{
				otherGameObject.Value = other.gameObject;
				enteredTrigger = true;
			}
		}

		public override void OnReset()
		{
			tag = "";
			otherGameObject = null;
		}
	}
}
