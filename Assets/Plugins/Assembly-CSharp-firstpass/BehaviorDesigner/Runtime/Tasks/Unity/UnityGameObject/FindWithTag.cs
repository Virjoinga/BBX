using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
	[TaskCategory("Unity/GameObject")]
	[TaskDescription("Finds a GameObject by tag. Returns Success.")]
	public class FindWithTag : Action
	{
		[Tooltip("The tag of the GameObject to find")]
		public SharedString tag;

		[Tooltip("Should a random GameObject be found?")]
		public SharedBool random;

		[Tooltip("The object found by name")]
		[RequiredField]
		public SharedGameObject storeValue;

		public override TaskStatus OnUpdate()
		{
			if (random.Value)
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag(tag.Value);
				storeValue.Value = array[Random.Range(0, array.Length - 1)];
			}
			else
			{
				storeValue.Value = GameObject.FindWithTag(tag.Value);
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
