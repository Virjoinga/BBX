using UnityEngine;
using UnityEngine.Playables;

namespace KinematicCharacterController.Examples
{
	public class PlayableMover : MonoBehaviour, IMoverController
	{
		public PhysicsMover Mover;

		public float Speed = 1f;

		public PlayableDirector Director;

		private Transform _transform;

		private void Start()
		{
			_transform = base.transform;
			Director.timeUpdateMode = DirectorUpdateMode.Manual;
			Mover.MoverController = this;
		}

		public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
		{
			Vector3 position = _transform.position;
			Quaternion rotation = _transform.rotation;
			EvaluateAtTime(Time.time * Speed);
			goalPosition = _transform.position;
			goalRotation = _transform.rotation;
			_transform.position = position;
			_transform.rotation = rotation;
		}

		public void EvaluateAtTime(double time)
		{
			Director.time = time % Director.duration;
			Director.Evaluate();
		}
	}
}
