using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Flock around the scene using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=13")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}FlockIcon.png")]
	public class Flock : NavMeshGroupMovement
	{
		[Tooltip("Agents less than this distance apart are neighbors")]
		public SharedFloat neighborDistance = 100f;

		[Tooltip("How far the agent should look ahead when determine its pathfinding destination")]
		public SharedFloat lookAheadDistance = 5f;

		[Tooltip("The greater the alignmentWeight is the more likely it is that the agents will be facing the same direction")]
		public SharedFloat alignmentWeight = 0.4f;

		[Tooltip("The greater the cohesionWeight is the more likely it is that the agents will be moving towards a common position")]
		public SharedFloat cohesionWeight = 0.5f;

		[Tooltip("The greater the separationWeight is the more likely it is that the agents will be separated")]
		public SharedFloat separationWeight = 0.6f;

		public override TaskStatus OnUpdate()
		{
			for (int i = 0; i < agents.Length; i++)
			{
				DetermineFlockParameters(i, out var alignment, out var cohesion, out var separation);
				Vector3 vector = alignment * alignmentWeight.Value + cohesion * cohesionWeight.Value + separation * separationWeight.Value;
				if (!SetDestination(i, transforms[i].position + vector * lookAheadDistance.Value))
				{
					vector *= -1f;
					SetDestination(i, transforms[i].position + vector * lookAheadDistance.Value);
				}
			}
			return TaskStatus.Running;
		}

		private void DetermineFlockParameters(int index, out Vector3 alignment, out Vector3 cohesion, out Vector3 separation)
		{
			alignment = (cohesion = (separation = Vector3.zero));
			int num = 0;
			Transform transform = transforms[index];
			for (int i = 0; i < agents.Length; i++)
			{
				if (index != i && Vector3.Magnitude(transforms[i].position - transform.position) < neighborDistance.Value)
				{
					alignment += Velocity(i);
					cohesion += transforms[i].position;
					separation += transforms[i].position - transform.position;
					num++;
				}
			}
			if (num != 0)
			{
				alignment = (alignment / num).normalized;
				cohesion = (cohesion / num - transform.position).normalized;
				separation = (separation / num * -1f).normalized;
			}
		}

		public override void OnReset()
		{
			base.OnReset();
			neighborDistance = 100f;
			lookAheadDistance = 5f;
			alignmentWeight = 0.4f;
			cohesionWeight = 0.5f;
			separationWeight = 0.6f;
		}
	}
}
