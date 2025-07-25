using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Seek the target specified using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=3")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
	public class Seek : NavMeshMovement
	{
		[Tooltip("The GameObject that the agent is seeking")]
		public SharedGameObject target;

		[Tooltip("If target is null then use the target position")]
		public SharedVector3 targetPosition;

		public override void OnStart()
		{
			base.OnStart();
			SetDestination(Target());
		}

		public override TaskStatus OnUpdate()
		{
			if (HasArrived())
			{
				return TaskStatus.Success;
			}
			SetDestination(Target());
			return TaskStatus.Running;
		}

		private Vector3 Target()
		{
			if (target.Value != null)
			{
				return target.Value.transform.position;
			}
			return targetPosition.Value;
		}

		public override void OnReset()
		{
			base.OnReset();
			target = null;
			targetPosition = Vector3.zero;
		}
	}
}
