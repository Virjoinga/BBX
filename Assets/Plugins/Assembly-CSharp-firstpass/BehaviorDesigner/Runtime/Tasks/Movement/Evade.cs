using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Evade the target specified using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=6")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}EvadeIcon.png")]
	public class Evade : NavMeshMovement
	{
		[Tooltip("The agent has evaded when the magnitude is greater than this value")]
		public SharedFloat evadeDistance = 10f;

		[Tooltip("The distance to look ahead when evading")]
		public SharedFloat lookAheadDistance = 5f;

		[Tooltip("How far to predict the distance ahead of the target. Lower values indicate less distance should be predicated")]
		public SharedFloat targetDistPrediction = 20f;

		[Tooltip("Multiplier for predicting the look ahead distance")]
		public SharedFloat targetDistPredictionMult = 20f;

		[Tooltip("The GameObject that the agent is evading")]
		public SharedGameObject target;

		private Vector3 targetPosition;

		public override void OnStart()
		{
			base.OnStart();
			targetPosition = target.Value.transform.position;
			SetDestination(Target());
		}

		public override TaskStatus OnUpdate()
		{
			if (Vector3.Magnitude(transform.position - target.Value.transform.position) > evadeDistance.Value)
			{
				return TaskStatus.Success;
			}
			SetDestination(Target());
			return TaskStatus.Running;
		}

		private Vector3 Target()
		{
			float magnitude = (target.Value.transform.position - transform.position).magnitude;
			float magnitude2 = Velocity().magnitude;
			float num = 0f;
			num = ((!(magnitude2 <= magnitude / targetDistPrediction.Value)) ? (magnitude / magnitude2 * targetDistPredictionMult.Value) : targetDistPrediction.Value);
			Vector3 vector = targetPosition;
			targetPosition = target.Value.transform.position;
			Vector3 vector2 = targetPosition + (targetPosition - vector) * num;
			return transform.position + (transform.position - vector2).normalized * lookAheadDistance.Value;
		}

		public override void OnReset()
		{
			base.OnReset();
			evadeDistance = 10f;
			lookAheadDistance = 5f;
			targetDistPrediction = 20f;
			targetDistPredictionMult = 20f;
			target = null;
		}
	}
}
