using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityParticleSystem
{
	[TaskCategory("Unity/ParticleSystem")]
	[TaskDescription("Simulate the Particle System.")]
	public class Simulate : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("Time to fastfoward the Particle System to")]
		public SharedFloat time;

		private ParticleSystem particleSystem;

		private GameObject prevGameObject;

		public override void OnStart()
		{
			GameObject defaultGameObject = GetDefaultGameObject(targetGameObject.Value);
			if (defaultGameObject != prevGameObject)
			{
				particleSystem = defaultGameObject.GetComponent<ParticleSystem>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (particleSystem == null)
			{
				Debug.LogWarning("ParticleSystem is null");
				return TaskStatus.Failure;
			}
			particleSystem.Simulate(time.Value);
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			time = 0f;
		}
	}
}
