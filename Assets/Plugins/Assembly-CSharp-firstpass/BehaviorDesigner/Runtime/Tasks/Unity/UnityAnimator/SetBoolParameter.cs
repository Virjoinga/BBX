using System.Collections;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator
{
	[TaskCategory("Unity/Animator")]
	[TaskDescription("Sets the bool parameter on an animator. Returns Success.")]
	public class SetBoolParameter : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The name of the parameter")]
		public SharedString paramaterName;

		[Tooltip("The value of the bool parameter")]
		public SharedBool boolValue;

		[Tooltip("Should the value be reverted back to its original value after it has been set?")]
		public bool setOnce;

		private int hashID;

		private Animator animator;

		private GameObject prevGameObject;

		public override void OnStart()
		{
			GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
			if (defaultGameObject != prevGameObject)
			{
				animator = defaultGameObject.GetComponent<Animator>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (animator == null)
			{
				Debug.LogWarning("Animator is null");
				return TaskStatus.Failure;
			}
			hashID = Animator.StringToHash(paramaterName.Value);
			bool origVale = animator.GetBool(hashID);
			animator.SetBool(hashID, boolValue.Value);
			if (setOnce)
			{
				StartCoroutine(ResetValue(origVale));
			}
			return TaskStatus.Success;
		}

		public IEnumerator ResetValue(bool origVale)
		{
			yield return null;
			animator.SetBool(hashID, origVale);
		}

		public override void OnReset()
		{
			targetGameObject = null;
			paramaterName = "";
			boolValue = false;
		}
	}
}
