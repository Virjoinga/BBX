using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Follow the leader using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=14")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}LeaderFollowIcon.png")]
	public class LeaderFollow : NavMeshGroupMovement
	{
		[Tooltip("Agents less than this distance apart are neighbors")]
		public SharedFloat neighborDistance = 10f;

		[Tooltip("How far behind the leader the agents should follow the leader")]
		public SharedFloat leaderBehindDistance = 2f;

		[Tooltip("The distance that the agents should be separated")]
		public SharedFloat separationDistance = 2f;

		[Tooltip("The agent is getting too close to the front of the leader if they are within the aheadDistance")]
		public SharedFloat aheadDistance = 2f;

		[Tooltip("The leader to follow")]
		public SharedGameObject leader;

		private Transform leaderTransform;

		private NavMeshAgent leaderAgent;

		public override void OnStart()
		{
			leaderTransform = leader.Value.transform;
			leaderAgent = leader.Value.GetComponent<NavMeshAgent>();
			base.OnStart();
		}

		public override TaskStatus OnUpdate()
		{
			Vector3 vector = LeaderBehindPosition();
			for (int i = 0; i < agents.Length; i++)
			{
				if (LeaderLookingAtAgent(i) && Vector3.Magnitude(leaderTransform.position - transforms[i].position) < aheadDistance.Value)
				{
					SetDestination(i, transforms[i].position + (transforms[i].position - leaderTransform.position).normalized * aheadDistance.Value);
				}
				else
				{
					SetDestination(i, vector + DetermineSeparation(i));
				}
			}
			return TaskStatus.Running;
		}

		private Vector3 LeaderBehindPosition()
		{
			return leaderTransform.position + (-leaderAgent.velocity).normalized * leaderBehindDistance.Value;
		}

		private Vector3 DetermineSeparation(int agentIndex)
		{
			Vector3 zero = Vector3.zero;
			int num = 0;
			Transform transform = transforms[agentIndex];
			for (int i = 0; i < agents.Length; i++)
			{
				if (agentIndex != i && Vector3.SqrMagnitude(transforms[i].position - transform.position) < neighborDistance.Value)
				{
					zero += transforms[i].position - transform.position;
					num++;
				}
			}
			if (num == 0)
			{
				return Vector3.zero;
			}
			return (zero / num * -1f).normalized * separationDistance.Value;
		}

		public bool LeaderLookingAtAgent(int agentIndex)
		{
			return Vector3.Dot(leaderTransform.forward, transforms[agentIndex].forward) < -0.5f;
		}

		public override void OnReset()
		{
			base.OnReset();
			neighborDistance = 10f;
			leaderBehindDistance = 2f;
			separationDistance = 2f;
			aheadDistance = 2f;
			leader = null;
		}
	}
}
