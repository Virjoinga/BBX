using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAudioSource
{
	[TaskCategory("Unity/AudioSource")]
	[TaskDescription("Returns Success if the AudioClip is playing, otherwise Failure.")]
	public class IsPlaying : Conditional
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		private AudioSource audioSource;

		private GameObject prevGameObject;

		public override void OnStart()
		{
			GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
			if (defaultGameObject != prevGameObject)
			{
				audioSource = defaultGameObject.GetComponent<AudioSource>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (audioSource == null)
			{
				Debug.LogWarning("AudioSource is null");
				return TaskStatus.Failure;
			}
			if (!audioSource.isPlaying)
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
		}
	}
}
