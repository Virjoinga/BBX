using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
	[TaskCategory("Unity/GameObject")]
	[TaskDescription("Finds a GameObject by tag. Returns Success.")]
	public class FindGameObjectsWithTag : Action
	{
		[Tooltip("The tag of the GameObject to find")]
		public SharedString tag;

		[Tooltip("The objects found by name")]
		[RequiredField]
		public SharedGameObjectList storeValue;

		public override TaskStatus OnUpdate()
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(tag.Value);
			for (int i = 0; i < array.Length; i++)
			{
				storeValue.Value.Add(array[i]);
			}
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			tag.Value = null;
			storeValue.Value = null;
		}
	}
}
