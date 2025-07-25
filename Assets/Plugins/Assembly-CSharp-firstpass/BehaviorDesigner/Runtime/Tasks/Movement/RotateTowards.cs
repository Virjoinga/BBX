using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Rotates towards the specified rotation. The rotation can either be specified by a transform or rotation. If the transform is used then the rotation will not be used.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=2")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}RotateTowardsIcon.png")]
	public class RotateTowards : Action
	{
		[Tooltip("Should the 2D version be used?")]
		public bool usePhysics2D;

		[Tooltip("The agent is done rotating when the angle is less than this value")]
		public SharedFloat rotationEpsilon = 0.5f;

		[Tooltip("The maximum number of angles the agent can rotate in a single tick")]
		public SharedFloat maxLookAtRotationDelta = 1f;

		[Tooltip("Should the rotation only affect the Y axis?")]
		public SharedBool onlyY;

		[Tooltip("The GameObject that the agent is rotating towards")]
		public SharedGameObject target;

		[Tooltip("If target is null then use the target rotation")]
		public SharedVector3 targetRotation;

		public override TaskStatus OnUpdate()
		{
			Quaternion quaternion = Target();
			if (Quaternion.Angle(transform.rotation, quaternion) < rotationEpsilon.Value)
			{
				return TaskStatus.Success;
			}
			transform.rotation = Quaternion.RotateTowards(transform.rotation, quaternion, maxLookAtRotationDelta.Value);
			return TaskStatus.Running;
		}

		private Quaternion Target()
		{
			if (target == null || target.Value == null)
			{
				return Quaternion.Euler(targetRotation.Value);
			}
			Vector3 forward = target.Value.transform.position - transform.position;
			if (onlyY.Value)
			{
				forward.y = 0f;
			}
			if (usePhysics2D)
			{
				return Quaternion.AngleAxis(Mathf.Atan2(forward.y, forward.x) * 57.29578f, Vector3.forward);
			}
			return Quaternion.LookRotation(forward);
		}

		public override void OnReset()
		{
			usePhysics2D = false;
			rotationEpsilon = 0.5f;
			maxLookAtRotationDelta = 1f;
			onlyY = false;
			target = null;
			targetRotation = Vector3.zero;
		}
	}
}
