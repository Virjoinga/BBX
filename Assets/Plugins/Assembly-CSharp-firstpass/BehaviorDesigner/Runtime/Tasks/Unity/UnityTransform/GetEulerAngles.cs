using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform
{
	[TaskCategory("Unity/Transform")]
	[TaskDescription("Stores the euler angles of the Transform. Returns Success.")]
	public class GetEulerAngles : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The euler angles of the Transform")]
		[RequiredField]
		public SharedVector3 storeValue;

		private Transform targetTransform;

		private GameObject prevGameObject;

		public override void OnStart()
		{
			GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
			if (defaultGameObject != prevGameObject)
			{
				targetTransform = defaultGameObject.GetComponent<Transform>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (targetTransform == null)
			{
				Debug.LogWarning("Transform is null");
				return TaskStatus.Failure;
			}
			storeValue.Value = targetTransform.eulerAngles;
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeValue = Vector3.zero;
		}
	}
}
