using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Check to see if the any objects are within sight of the agent.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=11")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
	public class CanSeeObject : Conditional
	{
		[Tooltip("Should the 2D version be used?")]
		public bool usePhysics2D;

		[Tooltip("The object that we are searching for")]
		public SharedGameObject targetObject;

		[Tooltip("The objects that we are searching for")]
		public SharedGameObjectList targetObjects;

		[Tooltip("The tag of the object that we are searching for")]
		public SharedString targetTag;

		[Tooltip("The LayerMask of the objects that we are searching for")]
		public LayerMask objectLayerMask;

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

		[Tooltip("The object that is within sight")]
		public SharedGameObject returnedObject;

		public override TaskStatus OnUpdate()
		{
			if (usePhysics2D)
			{
				if (targetObjects.Value != null && targetObjects.Value.Count > 0)
				{
					GameObject value = null;
					float num = float.PositiveInfinity;
					for (int i = 0; i < targetObjects.Value.Count; i++)
					{
						GameObject gameObject;
						if ((gameObject = MovementUtility.WithinSight(transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, targetObjects.Value[i], targetOffset.Value, usePhysics2D: true, angleOffset2D.Value, out var angle, ignoreLayerMask, useTargetBone.Value, targetBone)) != null && angle < num)
						{
							num = angle;
							value = gameObject;
						}
					}
					returnedObject.Value = value;
				}
				else if (targetObject.Value == null)
				{
					returnedObject.Value = MovementUtility.WithinSight2D(transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, objectLayerMask, targetOffset.Value, angleOffset2D.Value, ignoreLayerMask);
				}
				else if (!string.IsNullOrEmpty(targetTag.Value))
				{
					returnedObject.Value = MovementUtility.WithinSight2D(transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, GameObject.FindGameObjectWithTag(targetTag.Value), targetOffset.Value, angleOffset2D.Value, ignoreLayerMask, useTargetBone.Value, targetBone);
				}
				else
				{
					returnedObject.Value = MovementUtility.WithinSight2D(transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, targetObject.Value, targetOffset.Value, angleOffset2D.Value, ignoreLayerMask, useTargetBone.Value, targetBone);
				}
			}
			else if (targetObjects.Value != null && targetObjects.Value.Count > 0)
			{
				GameObject value2 = null;
				float num2 = float.PositiveInfinity;
				for (int j = 0; j < targetObjects.Value.Count; j++)
				{
					GameObject gameObject2;
					if ((gameObject2 = MovementUtility.WithinSight(transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, targetObjects.Value[j], targetOffset.Value, usePhysics2D: false, angleOffset2D.Value, out var angle2, ignoreLayerMask, useTargetBone.Value, targetBone)) != null && angle2 < num2)
					{
						num2 = angle2;
						value2 = gameObject2;
					}
				}
				returnedObject.Value = value2;
			}
			else if (targetObject.Value == null)
			{
				returnedObject.Value = MovementUtility.WithinSight(transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, objectLayerMask, targetOffset.Value, ignoreLayerMask, useTargetBone.Value, targetBone);
			}
			else if (!string.IsNullOrEmpty(targetTag.Value))
			{
				returnedObject.Value = MovementUtility.WithinSight(transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, GameObject.FindGameObjectWithTag(targetTag.Value), targetOffset.Value, ignoreLayerMask, useTargetBone.Value, targetBone);
			}
			else
			{
				returnedObject.Value = MovementUtility.WithinSight(transform, offset.Value, fieldOfViewAngle.Value, viewDistance.Value, targetObject.Value, targetOffset.Value, ignoreLayerMask, useTargetBone.Value, targetBone);
			}
			if (returnedObject.Value != null)
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		public override void OnReset()
		{
			fieldOfViewAngle = 90f;
			viewDistance = 1000f;
			offset = Vector3.zero;
			targetOffset = Vector3.zero;
			angleOffset2D = 0f;
			targetTag = "";
		}

		public override void OnDrawGizmos()
		{
			MovementUtility.DrawLineOfSight(base.Owner.transform, offset.Value, fieldOfViewAngle.Value, angleOffset2D.Value, viewDistance.Value, usePhysics2D);
		}

		public override void OnBehaviorComplete()
		{
			MovementUtility.ClearCache();
		}
	}
}
