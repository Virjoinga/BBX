using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityLight
{
	[TaskCategory("Unity/Light")]
	[TaskDescription("Sets the shadow type of the light.")]
	public class SetShadows : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The shadow type to set")]
		public LightShadows shadows;

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
			light.shadows = shadows;
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
		}
	}
}
