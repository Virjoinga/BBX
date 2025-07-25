using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Follows the specified target using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=23")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}FollowIcon.png")]
	public class Follow : NavMeshMovement
	{
		[Tooltip("The GameObject that the agent is following")]
		public SharedGameObject target;

		[Tooltip("Start moving towards the target if the target is further than the specified distance")]
		public SharedFloat moveDistance = 2f;

		private Vector3 lastTargetPosition;

		private bool hasMoved;

		public override void OnStart()
		{
			base.OnStart();
			lastTargetPosition = target.Value.transform.position + Vector3.one * (moveDistance.Value + 1f);
			hasMoved = false;
		}

		public override TaskStatus OnUpdate()
		{
			if (target.Value == null)
			{
				return TaskStatus.Failure;
			}
			Vector3 position = target.Value.transform.position;
			if ((position - lastTargetPosition).magnitude >= moveDistance.Value)
			{
				SetDestination(position);
				lastTargetPosition = position;
				hasMoved = true;
			}
			else if (hasMoved && (position - transform.position).magnitude < moveDistance.Value)
			{
				Stop();
				hasMoved = false;
				lastTargetPosition = position;
			}
			return TaskStatus.Running;
		}

		public override void OnReset()
		{
			base.OnReset();
			target = null;
			moveDistance = 2f;
		}
	}
}
