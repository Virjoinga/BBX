using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityLight
{
	[TaskCategory("Unity/Light")]
	[TaskDescription("Sets the shadow bias of the light.")]
	public class SetShadowBias : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The shadow bias to set")]
		public SharedFloat shadowBias;

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
			light.shadowBias = shadowBias.Value;
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			shadowBias = 0f;
		}
	}
}
