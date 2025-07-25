using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	public abstract class NavMeshGroupMovement : GroupMovement
	{
		[Tooltip("All of the agents")]
		public SharedGameObject[] agents;

		[Tooltip("The speed of the agents")]
		public SharedFloat speed = 10f;

		[Tooltip("The angular speed of the agents")]
		public SharedFloat angularSpeed = 120f;

		private NavMeshAgent[] navMeshAgents;

		protected Transform[] transforms;

		public override void OnStart()
		{
			navMeshAgents = new NavMeshAgent[agents.Length];
			transforms = new Transform[agents.Length];
			for (int i = 0; i < agents.Length; i++)
			{
				transforms[i] = agents[i].Value.transform;
				navMeshAgents[i] = agents[i].Value.GetComponent<NavMeshAgent>();
				navMeshAgents[i].speed = speed.Value;
				navMeshAgents[i].angularSpeed = angularSpeed.Value;
				navMeshAgents[i].isStopped = false;
			}
		}

		protected override bool SetDestination(int index, Vector3 target)
		{
			if (navMeshAgents[index].destination == target)
			{
				return true;
			}
			return navMeshAgents[index].SetDestination(target);
		}

		protected override Vector3 Velocity(int index)
		{
			return navMeshAgents[index].velocity;
		}

		public override void OnEnd()
		{
			for (int i = 0; i < navMeshAgents.Length; i++)
			{
				if (navMeshAgents[i] != null)
				{
					navMeshAgents[i].isStopped = true;
				}
			}
		}

		public override void OnReset()
		{
			agents = null;
		}
	}
}
