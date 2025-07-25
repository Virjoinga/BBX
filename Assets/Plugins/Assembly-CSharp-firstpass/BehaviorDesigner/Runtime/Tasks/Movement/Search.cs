using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Search for a target by combining the wander, within hearing range, and the within seeing range tasks using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=10")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SearchIcon.png")]
	public class Search : NavMeshMovement
	{
		[Tooltip("Minimum distance ahead of the current position to look ahead for a destination")]
		public SharedFloat minWanderDistance = 20f;

		[Tooltip("Maximum distance ahead of the current position to look ahead for a destination")]
		public SharedFloat maxWanderDistance = 20f;

		[Tooltip("The amount that the agent rotates direction")]
		public SharedFloat wanderRate = 1f;

		[Tooltip("The minimum length of time that the agent should pause at each destination")]
		public SharedFloat minPauseDuration = 0f;

		[Tooltip("The maximum length of time that the agent should pause at each destination (zero to disable)")]
		public SharedFloat maxPauseDuration = 0f;

		[Tooltip("The maximum number of retries per tick (set higher if using a slow tick time)")]
		public SharedInt targetRetries = 1;

		[Tooltip("The field of view angle of the agent (in degrees)")]
		public SharedFloat fieldOfViewAngle = 90f;

		[Tooltip("The distance that the agent can see")]
		public SharedFloat viewDistance = 30f;

		[Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
		public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");

		[Tooltip("Should the search end if audio was heard?")]
		public SharedBool senseAudio = true;

		[Tooltip("How far away the unit can hear")]
		public SharedFloat hearingRadius = 30f;

		[Tooltip("The raycast offset relative to the pivot position")]
		public SharedVector3 offset;

		[Tooltip("The target raycast offset relative to the pivot position")]
		public SharedVector3 targetOffset;

		[Tooltip("The LayerMask of the objects that we are searching for")]
		public LayerMask objectLayerMask;

		[Tooltip("Should the target bone be used?")]
		public SharedBool useTargetBone;

		[Tooltip("The target's bone if the target is a humanoid")]
		public HumanBodyBones targetBone;

		[Tooltip("The further away a sound source is the less likely the agent will be able to hear it. Set a threshold for the the minimum audibility level that the agent can hear")]
		public SharedFloat audibilityThreshold = 0.05f;

		[Tooltip("The object that is found")]
		public SharedGameObject returnedObject;

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
			returnedObject.Value = MovementUtility.WithinSight(transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, objectLayerMask, targetOffset.Value, ignoreLayerMask, useTargetBone.Value, targetBone);
			if (returnedObject.Value != null)
			{
				return TaskStatus.Success;
			}
			if (senseAudio.Value)
			{
				returnedObject.Value = MovementUtility.WithinHearingRange(transform, offset.Value, audibilityThreshold.Value, hearingRadius.Value, objectLayerMask);
				if (returnedObject.Value != null)
				{
					return TaskStatus.Success;
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

		public override void OnDrawGizmos()
		{
		}

		public override void OnBehaviorComplete()
		{
			MovementUtility.ClearCache();
		}

		public override void OnReset()
		{
			base.OnReset();
			minWanderDistance = 20f;
			maxWanderDistance = 20f;
			wanderRate = 2f;
			minPauseDuration = 0f;
			maxPauseDuration = 0f;
			targetRetries = 1;
			fieldOfViewAngle = 90f;
			viewDistance = 30f;
			senseAudio = true;
			hearingRadius = 30f;
			audibilityThreshold = 0.05f;
		}
	}
}
