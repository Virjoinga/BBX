using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Check to see if had target and lost sight of it.")]
	[TaskCategory("Movement")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
	public class LostSight : Conditional
	{
		[Tooltip("The object that we are searching for")]
		public SharedGameObject targetObject;

		[Tooltip("The time to be out of site before the AI gives up the chase")]
		public SharedFloat timeToLostTarget;

		[Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
		public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");

		[Tooltip("The field of view angle of the agent (in degrees)")]
		public SharedFloat fieldOfViewAngle = 90f;

		[Tooltip("The distance that the agent can see")]
		public SharedFloat viewDistance = 1000f;

		[Tooltip("The raycast offset relative to the pivot position")]
		public SharedVector3 offset;

		[Tooltip("The target raycast offset relative to the pivot position")]
		public SharedVector3 targetOffset;

		[Tooltip("The offset to apply to 2D angles")]
		public SharedFloat angleOffset2D;

		[Tooltip("Should the target bone be used?")]
		public SharedBool useTargetBone;

		[Tooltip("The target's bone if the target is a humanoid")]
		public HumanBodyBones targetBone;

		[Tooltip("The last seen point of the player")]
		public SharedVector3 lastSeenPosition;

		private float _timer;

		public override void OnStart()
		{
			_timer = 0f;
		}

		public override TaskStatus OnUpdate()
		{
			if (targetObject.Value == null)
			{
				return TaskStatus.Failure;
			}
			if (MovementUtility.WithinSight(transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, targetObject.Value, targetOffset.Value, ignoreLayerMask, useTargetBone.Value, targetBone) != targetObject.Value)
			{
				_timer += Time.deltaTime;
				if (_timer >= timeToLostTarget.Value)
				{
					_timer = 0f;
					targetObject.Value = null;
					return TaskStatus.Success;
				}
				return TaskStatus.Failure;
			}
			_timer = 0f;
			return TaskStatus.Failure;
		}

		public override void OnReset()
		{
			fieldOfViewAngle = 90f;
			viewDistance = 1000f;
			offset = Vector3.zero;
			targetOffset = Vector3.zero;
			angleOffset2D = 0f;
			timeToLostTarget = 3f;
			_timer = 0f;
		}

		public override void OnDrawGizmos()
		{
			MovementUtility.DrawLineOfSight(base.Owner.transform, offset.Value, fieldOfViewAngle.Value, angleOffset2D.Value, viewDistance.Value, usePhysics2D: false);
		}

		public override void OnBehaviorComplete()
		{
			MovementUtility.ClearCache();
		}
	}
}
