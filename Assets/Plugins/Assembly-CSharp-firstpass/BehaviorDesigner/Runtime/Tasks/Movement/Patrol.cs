using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Patrol around the specified waypoints using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=7")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PatrolIcon.png")]
	public class Patrol : NavMeshMovement
	{
		[Tooltip("Should the agent patrol the waypoints randomly?")]
		public SharedBool randomPatrol = false;

		[Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
		public SharedFloat waypointPauseDuration = 0f;

		[Tooltip("The waypoints to move to")]
		public SharedGameObjectList waypoints;

		private int waypointIndex;

		private float waypointReachedTime;

		public override void OnStart()
		{
			base.OnStart();
			float num = float.PositiveInfinity;
			for (int i = 0; i < waypoints.Value.Count; i++)
			{
				float num2;
				if ((num2 = Vector3.Magnitude(transform.position - waypoints.Value[i].transform.position)) < num)
				{
					num = num2;
					waypointIndex = i;
				}
			}
			waypointReachedTime = -1f;
			SetDestination(Target());
		}

		public override TaskStatus OnUpdate()
		{
			if (waypoints.Value.Count == 0)
			{
				return TaskStatus.Failure;
			}
			if (HasArrived())
			{
				if (waypointReachedTime == -1f)
				{
					waypointReachedTime = Time.time;
				}
				if (waypointReachedTime + waypointPauseDuration.Value <= Time.time)
				{
					if (randomPatrol.Value)
					{
						if (waypoints.Value.Count == 1)
						{
							waypointIndex = 0;
						}
						else
						{
							int num;
							for (num = waypointIndex; num == waypointIndex; num = Random.Range(0, waypoints.Value.Count))
							{
							}
							waypointIndex = num;
						}
					}
					else
					{
						waypointIndex = (waypointIndex + 1) % waypoints.Value.Count;
					}
					SetDestination(Target());
					waypointReachedTime = -1f;
				}
			}
			return TaskStatus.Running;
		}

		private Vector3 Target()
		{
			if (waypointIndex >= waypoints.Value.Count)
			{
				return transform.position;
			}
			return waypoints.Value[waypointIndex].transform.position;
		}

		public override void OnReset()
		{
			base.OnReset();
			randomPatrol = false;
			waypointPauseDuration = 0f;
			waypoints = null;
		}

		public override void OnDrawGizmos()
		{
		}
	}
}
