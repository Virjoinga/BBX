using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Check to see if the any object specified by the object list or tag is within the distance specified of the current agent.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=18")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WithinDistanceIcon.png")]
	public class WithinDistance : Conditional
	{
		[Tooltip("Should the 2D version be used?")]
		public bool usePhysics2D;

		[Tooltip("The object that we are searching for")]
		public SharedGameObject targetObject;

		[Tooltip("The tag of the object that we are searching for")]
		public SharedString targetTag;

		[Tooltip("The LayerMask of the objects that we are searching for")]
		public LayerMask objectLayerMask;

		[Tooltip("The distance that the object needs to be within")]
		public SharedFloat magnitude = 5f;

		[Tooltip("If true, the object must be within line of sight to be within distance. For example, if this option is enabled then an object behind a wall will not be within distance even though it may be physically close to the other object")]
		public SharedBool lineOfSight;

		[Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
		public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");

		[Tooltip("The raycast offset relative to the pivot position")]
		public SharedVector3 offset;

		[Tooltip("The target raycast offset relative to the pivot position")]
		public SharedVector3 targetOffset;

		[Tooltip("The object variable that will be set when a object is found what the object is")]
		public SharedGameObject returnedObject;

		private List<GameObject> objects;

		private float sqrMagnitude;

		public override void OnStart()
		{
			sqrMagnitude = magnitude.Value * magnitude.Value;
			if (objects != null)
			{
				objects.Clear();
			}
			else
			{
				objects = new List<GameObject>();
			}
			if (targetObject.Value == null)
			{
				if (!string.IsNullOrEmpty(targetTag.Value))
				{
					GameObject[] array = GameObject.FindGameObjectsWithTag(targetTag.Value);
					for (int i = 0; i < array.Length; i++)
					{
						objects.Add(array[i]);
					}
				}
				else
				{
					Collider[] array2 = Physics.OverlapSphere(transform.position, magnitude.Value, objectLayerMask.value);
					for (int j = 0; j < array2.Length; j++)
					{
						objects.Add(array2[j].gameObject);
					}
				}
			}
			else
			{
				objects.Add(targetObject.Value);
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (transform == null || objects == null)
			{
				return TaskStatus.Failure;
			}
			for (int i = 0; i < objects.Count; i++)
			{
				if (!(objects[i] == null) && Vector3.SqrMagnitude(objects[i].transform.position - (transform.position + offset.Value)) < sqrMagnitude)
				{
					if (!lineOfSight.Value)
					{
						returnedObject.Value = objects[i];
						return TaskStatus.Success;
					}
					if ((bool)MovementUtility.LineOfSight(transform, offset.Value, objects[i], targetOffset.Value, usePhysics2D, ignoreLayerMask.value))
					{
						returnedObject.Value = objects[i];
						return TaskStatus.Success;
					}
				}
			}
			return TaskStatus.Failure;
		}

		public override void OnReset()
		{
			usePhysics2D = false;
			targetObject = null;
			targetTag = string.Empty;
			objectLayerMask = 0;
			magnitude = 5f;
			lineOfSight = true;
			ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
			offset = Vector3.zero;
			targetOffset = Vector3.zero;
		}

		public override void OnDrawGizmos()
		{
		}
	}
}
