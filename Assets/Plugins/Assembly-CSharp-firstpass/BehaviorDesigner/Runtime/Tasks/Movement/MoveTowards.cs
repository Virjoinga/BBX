using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Move towards the specified position. The position can either be specified by a transform or position. If the transform is used then the position will not be used.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=1")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}MoveTowardsIcon.png")]
	public class MoveTowards : Action
	{
		[Tooltip("The speed of the agent")]
		public SharedFloat speed;

		[Tooltip("The agent has arrived when the magnitude is less than this value")]
		public SharedFloat arriveDistance = 0.1f;

		[Tooltip("Should the agent be looking at the target position?")]
		public SharedBool lookAtTarget = true;

		[Tooltip("Max rotation delta if lookAtTarget is enabled")]
		public SharedFloat maxLookAtRotationDelta;

		[Tooltip("The GameObject that the agent is moving towards")]
		public SharedGameObject target;

		[Tooltip("If target is null then use the target position")]
		public SharedVector3 targetPosition;

		public override TaskStatus OnUpdate()
		{
			Vector3 vector = Target();
			if (Vector3.Magnitude(transform.position - vector) < arriveDistance.Value)
			{
				return TaskStatus.Success;
			}
			transform.position = Vector3.MoveTowards(transform.position, vector, speed.Value * Time.deltaTime);
			if (lookAtTarget.Value && (vector - transform.position).sqrMagnitude > 0.01f)
			{
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vector - transform.position), maxLookAtRotationDelta.Value);
			}
			return TaskStatus.Running;
		}

		private Vector3 Target()
		{
			if (target == null || target.Value == null)
			{
				return targetPosition.Value;
			}
			return target.Value.transform.position;
		}

		public override void OnReset()
		{
			arriveDistance = 0.1f;
			lookAtTarget = true;
		}
	}
}
