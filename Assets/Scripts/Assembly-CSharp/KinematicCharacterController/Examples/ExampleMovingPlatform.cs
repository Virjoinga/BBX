using UnityEngine;

namespace KinematicCharacterController.Examples
{
	public class ExampleMovingPlatform : MonoBehaviour, IMoverController
	{
		public PhysicsMover Mover;

		public Vector3 TranslationAxis = Vector3.right;

		public float TranslationPeriod = 10f;

		public float TranslationSpeed = 1f;

		public Vector3 RotationAxis = Vector3.up;

		public float RotSpeed = 10f;

		public Vector3 OscillationAxis = Vector3.zero;

		public float OscillationPeriod = 10f;

		public float OscillationSpeed = 10f;

		private Vector3 _originalPosition;

		private Quaternion _originalRotation;

		private void Start()
		{
			_originalPosition = Mover.Rigidbody.position;
			_originalRotation = Mover.Rigidbody.rotation;
			Mover.MoverController = this;
		}

		public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
		{
			goalPosition = _originalPosition + TranslationAxis.normalized * Mathf.Sin(Time.time * TranslationSpeed) * TranslationPeriod;
			Quaternion quaternion = Quaternion.Euler(OscillationAxis.normalized * (Mathf.Sin(Time.time * OscillationSpeed) * OscillationPeriod)) * _originalRotation;
			goalRotation = Quaternion.Euler(RotationAxis * RotSpeed * Time.time) * quaternion;
		}
	}
}
