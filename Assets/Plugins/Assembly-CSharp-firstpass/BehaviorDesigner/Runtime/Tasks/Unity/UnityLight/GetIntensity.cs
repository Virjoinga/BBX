using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityLight
{
	[TaskCategory("Unity/Light")]
	[TaskDescription("Stores the intensity of the light.")]
	public class GetIntensity : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[RequiredField]
		[Tooltip("The intensity to store")]
		public SharedFloat storeValue;

		private Light light;

		private GameObject prevGameObject;

		public override void OnStart()
		{
			GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
			if (defaultGameObject != prevGameObject)
			{
				light = defaultGameObject.GetComponent<Light>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (light == null)
			{
				Debug.LogWarning("Light is null");
				return TaskStatus.Failure;
			}
			storeValue = light.intensity;
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeValue = 0f;
		}
	}
}
