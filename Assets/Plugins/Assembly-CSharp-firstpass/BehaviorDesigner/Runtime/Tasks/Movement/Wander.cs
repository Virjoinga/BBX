using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Wander using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=9")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WanderIcon.png")]
	public class Wander : NavMeshMovement
	{
		[Tooltip("Minimum distance ahead of the current position to look ahead for a destination")]
		public SharedFloat minWanderDistance = 20f;

		[Tooltip("Maximum distance ahead of the current position to look ahead for a destination")]
		public SharedFloat maxWanderDistance = 20f;

		[Tooltip("The amount that the agent rotates direction")]
		public SharedFloat wanderRate = 2f;

		[Tooltip("The minimum length of time that the agent should pause at each destination")]
		public SharedFloat minPauseDuration = 0f;

		[Tooltip("The maximum length of time that the agent should pause at each destination (zero to disable)")]
		public SharedFloat maxPauseDuration = 0f;

		[Tooltip("The maximum number of retries per tick (set higher if using a slow tick time)")]
		public SharedInt targetRetries = 1;

		private float pauseTime;

		private float destinationReachTime;

		public override TaskStatus OnUpdate()
		{
			if (HasArrived())
			{
				if (maxPauseDuration.Value > 0f)
				{
					if (destinationReachTime == -1f)
					{
						destinationReachTime = Time.time;
						pauseTime = Random.Range(minPauseDuration.Value, maxPauseDuration.Value);
					}
					if (destinationReachTime + pauseTime <= Time.time && TrySetTarget())
					{
						destinationReachTime = -1f;
					}
				}
				else
				{
					TrySetTarget();
				}
			}
			return TaskStatus.Running;
		}

		private bool TrySetTarget()
		{
			Vector3 forward = transform.forward;
			bool flag = false;
			int num = targetRetries.Value;
			Vector3 vector = transform.position;
			while (!flag && num > 0)
			{
				forward += Random.insideUnitSphere * wanderRate.Value;
				vector = transform.position + forward.normalized * Random.Range(minWanderDistance.Value, maxWanderDistance.Value);
				flag = SamplePosition(vector);
				num--;
			}
			if (flag)
			{
				SetDestination(vector);
			}
			return flag;
		}

		public override void OnReset()
		{
			minWanderDistance = 20f;
			maxWanderDistance = 20f;
			wanderRate = 2f;
			minPauseDuration = 0f;
			maxPauseDuration = 0f;
			targetRetries = 1;
		}
	}
}
