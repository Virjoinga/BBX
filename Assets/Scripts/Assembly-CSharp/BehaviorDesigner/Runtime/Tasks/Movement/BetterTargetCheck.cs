using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Check to see if there are any better targets.")]
	[TaskCategory("Movement")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
	public class BetterTargetCheck : Conditional
	{
		[Tooltip("The current target we are pursuing")]
		public SharedGameObject currentTarget;

		[Tooltip("The LayerMask of the object to check")]
		public LayerMask targetLayerMask;

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

		private Collider[] _overlapResults = new Collider[50];

		public override TaskStatus OnUpdate()
		{
			if (currentTarget.Value == null)
			{
				return TaskStatus.Failure;
			}
			float num = Vector3.Distance(transform.position, currentTarget.Value.transform.position);
			int num2 = Physics.OverlapSphereNonAlloc(transform.position, viewDistance.Value, _overlapResults, targetLayerMask);
			if (num2 > 0)
			{
				GameObject gameObject = null;
				for (int i = 0; i < num2; i++)
				{
					GameObject gameObject2 = _overlapResults[i].gameObject;
					if (gameObject2 == currentTarget.Value)
					{
						continue;
					}
					float num3 = Vector3.Distance(transform.position, gameObject2.transform.position);
					if (num3 < num)
					{
						GameObject gameObject3 = MovementUtility.WithinSight(transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, gameObject2, targetOffset.Value, ignoreLayerMask, useTargetBone.Value, targetBone);
						if (gameObject3 == gameObject2 && gameObject3 != currentTarget.Value)
						{
							gameObject = gameObject2;
							num = num3;
						}
					}
				}
				if (gameObject != null)
				{
					currentTarget.Value = gameObject;
				}
			}
			return TaskStatus.Running;
		}

		public override void OnReset()
		{
			fieldOfViewAngle = 90f;
			viewDistance = 1000f;
			offset = Vector3.zero;
			targetOffset = Vector3.zero;
			angleOffset2D = 0f;
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
