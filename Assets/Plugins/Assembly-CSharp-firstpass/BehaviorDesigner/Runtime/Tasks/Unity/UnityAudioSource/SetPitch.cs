using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAudioSource
{
	[TaskCategory("Unity/AudioSource")]
	[TaskDescription("Sets the pitch value of the AudioSource. Returns Success.")]
	public class SetPitch : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The pitch value of the AudioSource")]
		public SharedFloat pitch;

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
			audioSource.pitch = pitch.Value;
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			pitch = 1f;
		}
	}
}
