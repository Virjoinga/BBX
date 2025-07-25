using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Returns success when an object exits the 2D trigger. This task will only receive the physics callback if it is being reevaluated (with a conditional abort or under a parallel task).")]
	[TaskCategory("Physics")]
	public class HasExitedTrigger2D : Conditional
	{
		[Tooltip("The tag of the GameObject to check for a trigger against")]
		public SharedString tag = "";

		[Tooltip("The object that exited the trigger")]
		public SharedGameObject otherGameObject;

		private bool exitedTrigger;

		public override TaskStatus OnUpdate()
		{
			if (!exitedTrigger)
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		}

		public override void OnEnd()
		{
			exitedTrigger = false;
		}

		public override void OnTriggerExit2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(tag.Value) || tag.Value.Equals(other.gameObject.tag))
			{
				otherGameObject.Value = other.gameObject;
				exitedTrigger = true;
			}
		}

		public override void OnReset()
		{
			tag = "";
			otherGameObject = null;
		}
	}
}
