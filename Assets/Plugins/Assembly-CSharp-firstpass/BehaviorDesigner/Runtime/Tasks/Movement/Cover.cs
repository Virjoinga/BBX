using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Find a place to hide and move to it using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=8")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CoverIcon.png")]
	public class Cover : NavMeshMovement
	{
		[Tooltip("The distance to search for cover")]
		public SharedFloat maxCoverDistance = 1000f;

		[Tooltip("The layermask of the available cover positions")]
		public LayerMask availableLayerCovers;

		[Tooltip("The maximum number of raycasts that should be fired before the agent gives up looking for an agent to find cover behind")]
		public SharedInt maxRaycasts = 100;

		[Tooltip("How large the step should be between raycasts")]
		public SharedFloat rayStep = 1f;

		[Tooltip("Once a cover point has been found, multiply this offset by the normal to prevent the agent from hugging the wall")]
		public SharedFloat coverOffset = 2f;

		[Tooltip("Should the agent look at the cover point after it has arrived?")]
		public SharedBool lookAtCoverPoint = false;

		[Tooltip("The agent is done rotating to the cover point when the square magnitude is less than this value")]
		public SharedFloat rotationEpsilon = 0.5f;

		[Tooltip("Max rotation delta if lookAtCoverPoint")]
		public SharedFloat maxLookAtRotationDelta;

		private Vector3 coverPoint;

		private Vector3 coverTarget;

		private bool foundCover;

		public override void OnStart()
		{
			int i = 0;
			Vector3 direction = transform.forward;
			float num = 0f;
			coverTarget = transform.position;
			for (foundCover = false; i < maxRaycasts.Value; i++)
			{
				if (Physics.Raycast(new Ray(transform.position, direction), out var hitInfo, maxCoverDistance.Value, availableLayerCovers.value) && hitInfo.collider.Raycast(new Ray(hitInfo.point - hitInfo.normal * maxCoverDistance.Value, hitInfo.normal), out hitInfo, float.PositiveInfinity))
				{
					coverPoint = hitInfo.point;
					coverTarget = hitInfo.point + hitInfo.normal * coverOffset.Value;
					foundCover = true;
					break;
				}
				num += rayStep.Value;
				direction = Quaternion.Euler(0f, transform.eulerAngles.y + num, 0f) * Vector3.forward;
			}
			if (foundCover)
			{
				SetDestination(coverTarget);
			}
			base.OnStart();
		}

		public override TaskStatus OnUpdate()
		{
			if (!foundCover)
			{
				return TaskStatus.Failure;
			}
			if (HasArrived())
			{
				Quaternion quaternion = Quaternion.LookRotation(coverPoint - transform.position);
				if (!lookAtCoverPoint.Value || Quaternion.Angle(transform.rotation, quaternion) < rotationEpsilon.Value)
				{
					return TaskStatus.Success;
				}
				transform.rotation = Quaternion.RotateTowards(transform.rotation, quaternion, maxLookAtRotationDelta.Value);
			}
			return TaskStatus.Running;
		}

		public override void OnReset()
		{
			base.OnStart();
			maxCoverDistance = 1000f;
			maxRaycasts = 100;
			rayStep = 1f;
			coverOffset = 2f;
			lookAtCoverPoint = false;
			rotationEpsilon = 0.5f;
		}
	}
}
