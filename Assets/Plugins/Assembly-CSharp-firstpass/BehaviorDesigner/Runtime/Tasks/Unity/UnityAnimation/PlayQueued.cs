using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimation
{
	[TaskCategory("Unity/Animation")]
	[TaskDescription("Plays an animation after previous animations has finished playing. Returns Success.")]
	public class PlayQueued : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The name of the animation")]
		public SharedString animationName;

		[Tooltip("Specifies when the animation should start playing")]
		public QueueMode queue;

		[Tooltip("The play mode of the animation")]
		public PlayMode playMode;

		private Animation animation;

		private GameObject prevGameObject;

		public override void OnStart()
		{
			GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
			if (defaultGameObject != prevGameObject)
			{
				animation = defaultGameObject.GetComponent<Animation>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (animation == null)
			{
				Debug.LogWarning("Animation is null");
				return TaskStatus.Failure;
			}
			animation.PlayQueued(animationName.Value, queue, playMode);
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			animationName.Value = "";
			queue = QueueMode.CompleteOthers;
			playMode = PlayMode.StopSameLayer;
		}
	}
}
