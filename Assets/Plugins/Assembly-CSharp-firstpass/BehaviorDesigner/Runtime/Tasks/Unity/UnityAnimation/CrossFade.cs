using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimation
{
	[TaskCategory("Unity/Animation")]
	[TaskDescription("Fades the animation over a period of time and fades other animations out. Returns Success.")]
	public class CrossFade : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The name of the animation")]
		public SharedString animationName;

		[Tooltip("The speed of the animation. Use a negative value to play the animation backwards")]
		public SharedFloat animationSpeed = 1f;

		[Tooltip("The amount of time it takes to blend")]
		public SharedFloat fadeLength = 0.3f;

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
			animation[animationName.Value].speed = animationSpeed.Value;
			if (animation[animationName.Value].speed < 0f)
			{
				animation[animationName.Value].time = animation[animationName.Value].length;
			}
			animation.CrossFade(animationName.Value, fadeLength.Value, playMode);
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			animationName.Value = "";
			animationSpeed = 1f;
			fadeLength = 0.3f;
			playMode = PlayMode.StopSameLayer;
		}
	}
}
