using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Pursue the target specified using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=5")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PursueIcon.png")]
	public class Pursue : NavMeshMovement
	{
		[Tooltip("How far to predict the distance ahead of the target. Lower values indicate less distance should be predicated")]
		public SharedFloat targetDistPrediction = 20f;

		[Tooltip("Multiplier for predicting the look ahead distance")]
		public SharedFloat targetDistPredictionMult = 20f;

		[Tooltip("The GameObject that the agent is pursuing")]
		public SharedGameObject target;

		private Vector3 targetPosition;

		public override void OnStart()
		{
			base.OnStart();
			if (target.Value != null)
			{
				targetPosition = target.Value.transform.position;
				SetDestination(Target());
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (target.Value == null)
			{
				return TaskStatus.Failure;
			}
			if (HasArrived())
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
			return targetPosition + (targetPosition - vector) * num;
		}

		public override void OnReset()
		{
			base.OnReset();
			targetDistPrediction = 20f;
			targetDistPredictionMult = 20f;
			target = null;
		}
	}
}
