using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Check to see if the any objects are within hearing range of the current agent.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=12")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanHearObjectIcon.png")]
	public class CanHearObject : Conditional
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

		[Tooltip("How far away the unit can hear")]
		public SharedFloat hearingRadius = 50f;

		[Tooltip("The further away a sound source is the less likely the agent will be able to hear it. Set a threshold for the the minimum audibility level that the agent can hear")]
		public SharedFloat audibilityThreshold = 0.05f;

		[Tooltip("The hearing offset relative to the pivot position")]
		public SharedVector3 offset;

		[Tooltip("The returned object that is heard")]
		public SharedGameObject returnedObject;

		public override TaskStatus OnUpdate()
		{
			if (targetObjects.Value != null && targetObjects.Value.Count > 0)
			{
				GameObject value = null;
				for (int i = 0; i < targetObjects.Value.Count; i++)
				{
					float audibility = 0f;
					GameObject gameObject;
					if (Vector3.Distance(targetObjects.Value[i].transform.position, transform.position) < hearingRadius.Value && (gameObject = MovementUtility.WithinHearingRange(transform, offset.Value, audibilityThreshold.Value, targetObjects.Value[i], ref audibility)) != null)
					{
						value = gameObject;
					}
				}
				returnedObject.Value = value;
			}
			else if (targetObject.Value == null)
			{
				if (usePhysics2D)
				{
					returnedObject.Value = MovementUtility.WithinHearingRange2D(transform, offset.Value, audibilityThreshold.Value, hearingRadius.Value, objectLayerMask);
				}
				else
				{
					returnedObject.Value = MovementUtility.WithinHearingRange(transform, offset.Value, audibilityThreshold.Value, hearingRadius.Value, objectLayerMask);
				}
			}
			else
			{
				GameObject gameObject2 = (string.IsNullOrEmpty(targetTag.Value) ? targetObject.Value : GameObject.FindGameObjectWithTag(targetTag.Value));
				if (Vector3.Distance(gameObject2.transform.position, transform.position) < hearingRadius.Value)
				{
					returnedObject.Value = MovementUtility.WithinHearingRange(transform, offset.Value, audibilityThreshold.Value, targetObject.Value);
				}
			}
			if (returnedObject.Value != null)
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		public override void OnReset()
		{
			hearingRadius = 50f;
			audibilityThreshold = 0.05f;
		}

		public override void OnDrawGizmos()
		{
		}

		public override void OnBehaviorComplete()
		{
			MovementUtility.ClearCache();
		}
	}
}
