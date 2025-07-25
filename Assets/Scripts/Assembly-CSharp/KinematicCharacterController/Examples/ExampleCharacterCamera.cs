using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController.Examples
{
	public class ExampleCharacterCamera : MonoBehaviour
	{
		[Header("Framing")]
		public Camera Camera;

		public Vector2 FollowPointFraming = new Vector2(0f, 0f);

		public float FollowingSharpness = 10000f;

		[Header("Distance")]
		public float DefaultDistance = 6f;

		public float MinDistance;

		public float MaxDistance = 10f;

		public float DistanceMovementSpeed = 5f;

		public float DistanceMovementSharpness = 10f;

		[Header("Rotation")]
		public bool InvertX;

		public bool InvertY;

		[Range(-90f, 90f)]
		public float DefaultVerticalAngle = 20f;

		[Range(-90f, 90f)]
		public float MinVerticalAngle = -90f;

		[Range(-90f, 90f)]
		public float MaxVerticalAngle = 90f;

		public float RotationSpeed = 1f;

		public float RotationSharpness = 10000f;

		[Header("Obstruction")]
		public float ObstructionCheckRadius = 0.2f;

		public LayerMask ObstructionLayers = -1;

		public float ObstructionSharpness = 10000f;

		public List<Collider> IgnoredColliders = new List<Collider>();

		private bool _distanceIsObstructed;

		private float _currentDistance;

		private float _targetVerticalAngle;

		private RaycastHit _obstructionHit;

		private int _obstructionCount;

		private RaycastHit[] _obstructions = new RaycastHit[32];

		private float _obstructionTime;

		private Vector3 _currentFollowPosition;

		private const int MaxObstructions = 32;

		public Transform Transform { get; private set; }

		public Transform FollowTransform { get; private set; }

		public Vector3 PlanarDirection { get; set; }

		public float TargetDistance { get; set; }

		private void OnValidate()
		{
			DefaultDistance = Mathf.Clamp(DefaultDistance, MinDistance, MaxDistance);
			DefaultVerticalAngle = Mathf.Clamp(DefaultVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
		}

		private void Awake()
		{
			Transform = base.transform;
			_currentDistance = DefaultDistance;
			TargetDistance = _currentDistance;
			_targetVerticalAngle = 0f;
			PlanarDirection = Vector3.forward;
		}

		public void SetFollowTransform(Transform t)
		{
			FollowTransform = t;
			PlanarDirection = FollowTransform.forward;
			_currentFollowPosition = FollowTransform.position;
		}

		public void UpdateWithInput(float deltaTime, float zoomInput, Vector3 rotationInput)
		{
			if (!FollowTransform)
			{
				return;
			}
			if (InvertX)
			{
				rotationInput.x *= -1f;
			}
			if (InvertY)
			{
				rotationInput.y *= -1f;
			}
			Quaternion quaternion = Quaternion.Euler(FollowTransform.up * (rotationInput.x * RotationSpeed));
			PlanarDirection = quaternion * PlanarDirection;
			PlanarDirection = Vector3.Cross(FollowTransform.up, Vector3.Cross(PlanarDirection, FollowTransform.up));
			_targetVerticalAngle -= rotationInput.y * RotationSpeed;
			_targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
			if (_distanceIsObstructed && Mathf.Abs(zoomInput) > 0f)
			{
				TargetDistance = _currentDistance;
			}
			TargetDistance += zoomInput * DistanceMovementSpeed;
			TargetDistance = Mathf.Clamp(TargetDistance, MinDistance, MaxDistance);
			_currentFollowPosition = Vector3.Lerp(_currentFollowPosition, FollowTransform.position, 1f - Mathf.Exp((0f - FollowingSharpness) * deltaTime));
			Quaternion quaternion2 = Quaternion.LookRotation(PlanarDirection, FollowTransform.up);
			Quaternion quaternion3 = Quaternion.Euler(_targetVerticalAngle, 0f, 0f);
			Quaternion quaternion4 = Quaternion.Slerp(Transform.rotation, quaternion2 * quaternion3, 1f - Mathf.Exp((0f - RotationSharpness) * deltaTime));
			Transform.rotation = quaternion4;
			RaycastHit raycastHit = new RaycastHit
			{
				distance = float.PositiveInfinity
			};
			_obstructionCount = Physics.SphereCastNonAlloc(_currentFollowPosition, ObstructionCheckRadius, -Transform.forward, _obstructions, TargetDistance, ObstructionLayers, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < _obstructionCount; i++)
			{
				bool flag = false;
				for (int j = 0; j < IgnoredColliders.Count; j++)
				{
					if (IgnoredColliders[j] == _obstructions[i].collider)
					{
						flag = true;
						break;
					}
				}
				for (int k = 0; k < IgnoredColliders.Count; k++)
				{
					if (IgnoredColliders[k] == _obstructions[i].collider)
					{
						flag = true;
						break;
					}
				}
				if (!flag && _obstructions[i].distance < raycastHit.distance && _obstructions[i].distance > 0f)
				{
					raycastHit = _obstructions[i];
				}
			}
			if (raycastHit.distance < float.PositiveInfinity)
			{
				_distanceIsObstructed = true;
				_currentDistance = Mathf.Lerp(_currentDistance, raycastHit.distance, 1f - Mathf.Exp((0f - ObstructionSharpness) * deltaTime));
			}
			else
			{
				_distanceIsObstructed = false;
				_currentDistance = Mathf.Lerp(_currentDistance, TargetDistance, 1f - Mathf.Exp((0f - DistanceMovementSharpness) * deltaTime));
			}
			Vector3 position = _currentFollowPosition - quaternion4 * Vector3.forward * _currentDistance;
			position += Transform.right * FollowPointFraming.x;
			position += Transform.up * FollowPointFraming.y;
			Transform.position = position;
		}
	}
}
