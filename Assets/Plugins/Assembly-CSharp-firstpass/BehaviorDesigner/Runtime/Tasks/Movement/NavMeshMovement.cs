using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	public abstract class NavMeshMovement : Movement
	{
		[Tooltip("The speed of the agent")]
		public SharedFloat speed = 10f;

		[Tooltip("The angular speed of the agent")]
		public SharedFloat angularSpeed = 120f;

		[Tooltip("The agent has arrived when the destination is less than the specified amount")]
		public SharedFloat arriveDistance = 0.2f;

		[Tooltip("Should the NavMeshAgent be stopped when the task ends?")]
		public SharedBool stopOnTaskEnd = true;

		[Tooltip("Should the NavMeshAgent rotation be updated when the task ends?")]
		public SharedBool updateRotation = true;

		protected NavMeshAgent navMeshAgent;

		public override void OnAwake()
		{
			navMeshAgent = GetComponent<NavMeshAgent>();
		}

		public override void OnStart()
		{
			navMeshAgent.speed = speed.Value;
			navMeshAgent.angularSpeed = angularSpeed.Value;
			navMeshAgent.isStopped = false;
			if (!updateRotation.Value)
			{
				UpdateRotation(update: true);
			}
		}

		protected override bool SetDestination(Vector3 destination)
		{
			navMeshAgent.isStopped = false;
			return navMeshAgent.SetDestination(destination);
		}

		protected override void UpdateRotation(bool update)
		{
			navMeshAgent.updateRotation = update;
		}

		protected override bool HasPath()
		{
			if (navMeshAgent.hasPath)
			{
				return navMeshAgent.remainingDistance > arriveDistance.Value;
			}
			return false;
		}

		protected override Vector3 Velocity()
		{
			return navMeshAgent.velocity;
		}

		protected bool SamplePosition(Vector3 position)
		{
			NavMeshHit hit;
			return NavMesh.SamplePosition(position, out hit, 1.5f, -1);
		}

		protected override bool HasArrived()
		{
			float num = ((!navMeshAgent.pathPending) ? navMeshAgent.remainingDistance : float.PositiveInfinity);
			return num <= arriveDistance.Value;
		}

		protected override void Stop()
		{
			UpdateRotation(updateRotation.Value);
			if (navMeshAgent.hasPath)
			{
				navMeshAgent.isStopped = true;
			}
		}

		public override void OnEnd()
		{
			if (stopOnTaskEnd.Value)
			{
				Stop();
			}
			else
			{
				UpdateRotation(updateRotation.Value);
			}
		}

		public override void OnBehaviorComplete()
		{
			Stop();
		}

		public override void OnReset()
		{
			speed = 10f;
			angularSpeed = 120f;
			arriveDistance = 1f;
			stopOnTaskEnd = true;
		}
	}
}
