using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
	[TaskCategory("Unity/GameObject")]
	[TaskDescription("Destorys the specified GameObject immediately. Returns Success.")]
	public class DestroyImmediate : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		public override TaskStatus OnUpdate()
		{
			Object.DestroyImmediate(GetDefaultGameObject(targetGameObject.Value));
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
		}
	}
}
