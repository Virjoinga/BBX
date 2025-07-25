using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController.Examples
{
	public class ExampleCharacterController : MonoBehaviour, ICharacterController
	{
		public KinematicCharacterMotor Motor;

		[Header("Stable Movement")]
		public float MaxStableMoveSpeed = 10f;

		public float StableMovementSharpness = 15f;

		public float OrientationSharpness = 10f;

		public OrientationMethod OrientationMethod;

		[Header("Air Movement")]
		public float MaxAirMoveSpeed = 15f;

		public float AirAccelerationSpeed = 15f;

		public float Drag = 0.1f;

		[Header("Jumping")]
		public bool AllowJumpingWhenSliding;

		public float JumpUpSpeed = 10f;

		public float JumpScalableForwardSpeed = 10f;

		public float JumpPreGroundingGraceTime;

		public float JumpPostGroundingGraceTime;

		[Header("Misc")]
		public List<Collider> IgnoredColliders = new List<Collider>();

		public BonusOrientationMethod BonusOrientationMethod;

		public float BonusOrientationSharpness = 10f;

		public Vector3 Gravity = new Vector3(0f, -30f, 0f);

		public Transform MeshRoot;

		public Transform CameraFollowPoint;

		private Collider[] _probedColliders = new Collider[8];

		private RaycastHit[] _probedHits = new RaycastHit[8];

		private Vector3 _moveInputVector;

		private Vector3 _lookInputVector;

		private bool _jumpRequested;

		private bool _jumpConsumed;

		private bool _jumpedThisFrame;

		private float _timeSinceJumpRequested = float.PositiveInfinity;

		private float _timeSinceLastAbleToJump;

		private Vector3 _internalVelocityAdd = Vector3.zero;

		private bool _shouldBeCrouching;

		private bool _isCrouching;

		private Vector3 lastInnerNormal = Vector3.zero;

		private Vector3 lastOuterNormal = Vector3.zero;

		private Quaternion _tmpTransientRot;

		public CharacterState CurrentCharacterState { get; private set; }

		private void Start()
		{
			TransitionToState(CharacterState.Default);
			Motor.CharacterController = this;
		}

		public void TransitionToState(CharacterState newState)
		{
			CharacterState currentCharacterState = CurrentCharacterState;
			OnStateExit(currentCharacterState, newState);
			CurrentCharacterState = newState;
			OnStateEnter(newState, currentCharacterState);
		}

		public void OnStateEnter(CharacterState state, CharacterState fromState)
		{
		}

		public void OnStateExit(CharacterState state, CharacterState toState)
		{
		}

		public void SetInputs(ref PlayerCharacterInputs inputs)
		{
			Vector3 vector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);
			Vector3 normalized = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
			if (normalized.sqrMagnitude == 0f)
			{
				normalized = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
			}
			Quaternion quaternion = Quaternion.LookRotation(normalized, Motor.CharacterUp);
			if (CurrentCharacterState != CharacterState.Default)
			{
				return;
			}
			_moveInputVector = quaternion * vector;
			switch (OrientationMethod)
			{
			case OrientationMethod.TowardsCamera:
				_lookInputVector = normalized;
				break;
			case OrientationMethod.TowardsMovement:
				_lookInputVector = _moveInputVector.normalized;
				break;
			}
			if (inputs.JumpDown)
			{
				_timeSinceJumpRequested = 0f;
				_jumpRequested = true;
			}
			if (inputs.CrouchDown)
			{
				_shouldBeCrouching = true;
				if (!_isCrouching)
				{
					_isCrouching = true;
					Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
					MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
				}
			}
			else if (inputs.CrouchUp)
			{
				_shouldBeCrouching = false;
			}
		}

		public void SetInputs(ref AICharacterInputs inputs)
		{
			_moveInputVector = inputs.MoveVector;
			_lookInputVector = inputs.LookVector;
		}

		public void BeforeCharacterUpdate(float deltaTime)
		{
		}

		public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
		{
			if (CurrentCharacterState != CharacterState.Default)
			{
				return;
			}
			if (_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
			{
				Vector3 normalized = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1f - Mathf.Exp((0f - OrientationSharpness) * deltaTime)).normalized;
				currentRotation = Quaternion.LookRotation(normalized, Motor.CharacterUp);
			}
			Vector3 vector = currentRotation * Vector3.up;
			if (BonusOrientationMethod == BonusOrientationMethod.TowardsGravity)
			{
				Vector3 toDirection = Vector3.Slerp(vector, -Gravity.normalized, 1f - Mathf.Exp((0f - BonusOrientationSharpness) * deltaTime));
				currentRotation = Quaternion.FromToRotation(vector, toDirection) * currentRotation;
			}
			else if (BonusOrientationMethod == BonusOrientationMethod.TowardsGroundSlopeAndGravity)
			{
				if (Motor.GroundingStatus.IsStableOnGround)
				{
					Vector3 vector2 = Motor.TransientPosition + vector * Motor.Capsule.radius;
					Vector3 toDirection2 = Vector3.Slerp(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal, 1f - Mathf.Exp((0f - BonusOrientationSharpness) * deltaTime));
					currentRotation = Quaternion.FromToRotation(vector, toDirection2) * currentRotation;
					Motor.SetTransientPosition(vector2 + currentRotation * Vector3.down * Motor.Capsule.radius);
				}
				else
				{
					Vector3 toDirection3 = Vector3.Slerp(vector, -Gravity.normalized, 1f - Mathf.Exp((0f - BonusOrientationSharpness) * deltaTime));
					currentRotation = Quaternion.FromToRotation(vector, toDirection3) * currentRotation;
				}
			}
			else
			{
				Vector3 toDirection4 = Vector3.Slerp(vector, Vector3.up, 1f - Mathf.Exp((0f - BonusOrientationSharpness) * deltaTime));
				currentRotation = Quaternion.FromToRotation(vector, toDirection4) * currentRotation;
			}
		}

		public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
		{
			if (CurrentCharacterState != CharacterState.Default)
			{
				return;
			}
			if (Motor.GroundingStatus.IsStableOnGround)
			{
				float magnitude = currentVelocity.magnitude;
				Vector3 vector = Motor.GroundingStatus.GroundNormal;
				if (magnitude > 0f && Motor.GroundingStatus.SnappingPrevented)
				{
					Vector3 rhs = Motor.TransientPosition - Motor.GroundingStatus.GroundPoint;
					vector = ((!(Vector3.Dot(currentVelocity, rhs) >= 0f)) ? Motor.GroundingStatus.InnerGroundNormal : Motor.GroundingStatus.OuterGroundNormal);
				}
				currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, vector) * magnitude;
				Vector3 rhs2 = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
				Vector3 b = Vector3.Cross(vector, rhs2).normalized * _moveInputVector.magnitude * MaxStableMoveSpeed;
				currentVelocity = Vector3.Lerp(currentVelocity, b, 1f - Mathf.Exp((0f - StableMovementSharpness) * deltaTime));
			}
			else
			{
				if (_moveInputVector.sqrMagnitude > 0f)
				{
					Vector3 vector2 = _moveInputVector * AirAccelerationSpeed * deltaTime;
					Vector3 vector3 = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);
					if (vector3.magnitude < MaxAirMoveSpeed)
					{
						vector2 = Vector3.ClampMagnitude(vector3 + vector2, MaxAirMoveSpeed) - vector3;
					}
					else if (Vector3.Dot(vector3, vector2) > 0f)
					{
						vector2 = Vector3.ProjectOnPlane(vector2, vector3.normalized);
					}
					if (Motor.GroundingStatus.FoundAnyGround && Vector3.Dot(currentVelocity + vector2, vector2) > 0f)
					{
						Vector3 normalized = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
						vector2 = Vector3.ProjectOnPlane(vector2, normalized);
					}
					currentVelocity += vector2;
				}
				currentVelocity += Gravity * deltaTime;
				currentVelocity *= 1f / (1f + Drag * deltaTime);
			}
			_jumpedThisFrame = false;
			_timeSinceJumpRequested += deltaTime;
			if (_jumpRequested && !_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
			{
				Vector3 vector4 = Motor.CharacterUp;
				if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
				{
					vector4 = Motor.GroundingStatus.GroundNormal;
				}
				Motor.ForceUnground();
				currentVelocity += vector4 * JumpUpSpeed - Vector3.Project(currentVelocity, Motor.CharacterUp);
				currentVelocity += _moveInputVector * JumpScalableForwardSpeed;
				_jumpRequested = false;
				_jumpConsumed = true;
				_jumpedThisFrame = true;
			}
			if (_internalVelocityAdd.sqrMagnitude > 0f)
			{
				currentVelocity += _internalVelocityAdd;
				_internalVelocityAdd = Vector3.zero;
			}
		}

		public void AfterCharacterUpdate(float deltaTime)
		{
			if (CurrentCharacterState != CharacterState.Default)
			{
				return;
			}
			if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
			{
				_jumpRequested = false;
			}
			if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
			{
				if (!_jumpedThisFrame)
				{
					_jumpConsumed = false;
				}
				_timeSinceLastAbleToJump = 0f;
			}
			else
			{
				_timeSinceLastAbleToJump += deltaTime;
			}
			if (_isCrouching && !_shouldBeCrouching)
			{
				Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
				if (Motor.CharacterOverlap(Motor.TransientPosition, Motor.TransientRotation, _probedColliders, Motor.CollidableLayers, QueryTriggerInteraction.Ignore) > 0)
				{
					Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
					return;
				}
				MeshRoot.localScale = new Vector3(1f, 1f, 1f);
				_isCrouching = false;
			}
		}

		public void PostGroundingUpdate(float deltaTime)
		{
			if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
			{
				OnLanded();
			}
			else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
			{
				OnLeaveStableGround();
			}
		}

		public bool IsColliderValidForCollisions(Collider coll)
		{
			if (IgnoredColliders.Count == 0)
			{
				return true;
			}
			if (IgnoredColliders.Contains(coll))
			{
				return false;
			}
			return true;
		}

		public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
		}

		public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
			Rigidbody attachedRigidbody = hitCollider.attachedRigidbody;
			if ((bool)attachedRigidbody)
			{
				_ = Vector3.Project(attachedRigidbody.velocity, hitNormal) - Vector3.Project(Motor.Velocity, hitNormal);
			}
		}

		public void AddVelocity(Vector3 velocity)
		{
			if (CurrentCharacterState == CharacterState.Default)
			{
				_internalVelocityAdd += velocity;
			}
		}

		public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
		{
		}

		protected void OnLanded()
		{
		}

		protected void OnLeaveStableGround()
		{
		}

		public void OnDiscreteCollisionDetected(Collider hitCollider)
		{
		}
	}
}
