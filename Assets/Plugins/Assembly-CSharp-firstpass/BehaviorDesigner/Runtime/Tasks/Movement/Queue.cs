using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Queue in a line using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=15")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}QueueIcon.png")]
	public class Queue : NavMeshGroupMovement
	{
		[Tooltip("Agents less than this distance apart are neighbors")]
		public SharedFloat neighborDistance = 10f;

		[Tooltip("The distance that the agents should be separated")]
		public SharedFloat separationDistance = 2f;

		[Tooltip("The distance the the agent should look ahead to see if another agent is in the way")]
		public SharedFloat maxQueueAheadDistance = 2f;

		[Tooltip("The radius that the agent should check to see if another agent is in the way")]
		public SharedFloat maxQueueRadius = 20f;

		[Tooltip("The multiplier to slow down if an agent is in front of the current agent")]
		public SharedFloat slowDownSpeed = 0.15f;

		[Tooltip("The target to seek towards")]
		public SharedGameObject target;

		public override TaskStatus OnUpdate()
		{
			for (int i = 0; i < agents.Length; i++)
			{
				if (AgentAhead(i))
				{
					SetDestination(i, transforms[i].position + transforms[i].forward * slowDownSpeed.Value + DetermineSeparation(i));
				}
				else
				{
					SetDestination(i, target.Value.transform.position);
				}
			}
			return TaskStatus.Running;
		}

		private bool AgentAhead(int index)
		{
			Vector3 vector = Velocity(index) * maxQueueAheadDistance.Value;
			for (int i = 0; i < agents.Length; i++)
			{
				if (index != i && Vector3.SqrMagnitude(vector - transforms[i].position) < maxQueueRadius.Value)
				{
					return true;
				}
			}
			return false;
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

		public override void OnReset()
		{
			base.OnReset();
			neighborDistance = 10f;
			separationDistance = 2f;
			maxQueueAheadDistance = 2f;
			maxQueueRadius = 20f;
			slowDownSpeed = 0.15f;
		}
	}
}
