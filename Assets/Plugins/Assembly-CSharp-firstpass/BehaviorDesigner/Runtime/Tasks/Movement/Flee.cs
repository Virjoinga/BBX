using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Flee from the target specified using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=4")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}FleeIcon.png")]
	public class Flee : NavMeshMovement
	{
		[Tooltip("The agent has fleed when the magnitude is greater than this value")]
		public SharedFloat fleedDistance = 20f;

		[Tooltip("The distance to look ahead when fleeing")]
		public SharedFloat lookAheadDistance = 5f;

		[Tooltip("The GameObject that the agent is fleeing from")]
		public SharedGameObject target;

		private bool hasMoved;

		public override void OnStart()
		{
			base.OnStart();
			hasMoved = false;
			SetDestination(Target());
		}

		public override TaskStatus OnUpdate()
		{
			if (Vector3.Magnitude(transform.position - target.Value.transform.position) > fleedDistance.Value)
			{
				return TaskStatus.Success;
			}
			if (HasArrived())
			{
				if (!hasMoved)
				{
					return TaskStatus.Failure;
				}
				if (!SetDestination(Target()))
				{
					return TaskStatus.Failure;
				}
				hasMoved = false;
			}
			else
			{
				float sqrMagnitude = Velocity().sqrMagnitude;
				if (hasMoved && sqrMagnitude <= 0f)
				{
					return TaskStatus.Failure;
				}
				hasMoved = sqrMagnitude > 0f;
			}
			return TaskStatus.Running;
		}

		private Vector3 Target()
		{
			return transform.position + (transform.position - target.Value.transform.position).normalized * lookAheadDistance.Value;
		}

		protected override bool SetDestination(Vector3 destination)
		{
			if (!SamplePosition(destination))
			{
				return false;
			}
			return base.SetDestination(destination);
		}

		public override void OnReset()
		{
			base.OnReset();
			fleedDistance = 20f;
			lookAheadDistance = 5f;
			target = null;
		}
	}
}
