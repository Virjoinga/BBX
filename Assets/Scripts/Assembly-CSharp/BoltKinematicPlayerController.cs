using System;
using KinematicCharacterController;
using UnityEngine;

[RequireComponent(typeof(BoltKinematicCharacterMotor))]
public class BoltKinematicPlayerController : MonoBehaviour, IBoltKinematicCharacterController, ICharacterController, IPlayerMotor
{
	[Serializable]
	public struct Inputs
	{
		public bool Enabled;

		public Vector3 Movement;

		public Vector2 Camera;

		public bool JumpPressed;

		public float SpeedMultiplier;
	}

	private const float MAX_LATERAL_VELOCITY_FUDGE = 0.001f;

	[SerializeField]
	protected MovementDetails _movementDetails;

	[SerializeField]
	protected CameraDetails _cameraDetails;

	[SerializeField]
	protected JumpDetails _jumpDetails;

	private BoltKinematicCharacterMotor _motor;

	private AimPointHandler _aimPointHandler;

	private PlayerMotorState _state;

	private Inputs _input;

	private Quaternion _aimPointHandlerWorldRotationCache;

	protected JumpPad _jumpPad;

	private bool _forceUngroundNextFrame;

	private LoadoutController _loadoutController;

	public PlayerMotorState State => _state;

	public float Radius => _motor.Capsule.radius;

	public float Height => _motor.Capsule.height;

	public JumpDetails JumpDetails => _jumpDetails;

	Transform IPlayerMotor.transform => base.transform;

	private void Awake()
	{
		_loadoutController = GetComponent<LoadoutController>();
		_motor = GetComponent<BoltKinematicCharacterMotor>();
		_motor.CharacterController = this;
	}

	public void InitializeCharacter(Outfit outfit)
	{
		_aimPointHandler = outfit.AimPointHandler;
		Outfit.ColliderData movementColliderData = outfit.MovementColliderData;
		_motor.SetCapsuleDimensions(movementColliderData.Radius, movementColliderData.Height, movementColliderData.Center.y);
	}

	public void ApplyForce(Vector3 force, bool breaksMelee)
	{
		if (_state.IsMeleeing)
		{
			if (!breaksMelee)
			{
				return;
			}
			_state.VelocityOverriden = false;
			_state.IsMeleeing = false;
		}
		_state.Velocity += force;
		_forceUngroundNextFrame = true;
		SetState(_state);
	}

	public void OverrideInput(float horizontal, float vertical, WeaponProfile.MeleeOptionData meleeOption)
	{
		Vector2 input = meleeOption.GetInput(horizontal, vertical);
		Vector3 direction = base.transform.InverseTransformDirection(_state.Velocity);
		bool num = Mathf.Abs(input.x) > 0.1f;
		if (Mathf.Abs(input.y) > 0.1f)
		{
			if (input.y >= -0.1f)
			{
				direction.z = _movementDetails.ForwardMaxSpeed;
			}
			else
			{
				direction.z = 0f - _movementDetails.BackwardMaxSpeed;
			}
		}
		else
		{
			direction.z = 0f;
		}
		if (num)
		{
			direction.x = _movementDetails.SidewaysMaxSpeed * Mathf.Sign(input.x);
		}
		else
		{
			direction.x = 0f;
		}
		OverrideInput(input, base.transform.TransformDirection(direction), meleeOption.MovementDelay, meleeOption.MovementDuration, meleeOption.Recovery, isMelee: true);
	}

	public void OverrideInput(Vector2 input, Vector3 initialVelocity, float delay, float duration, float recovery, bool isMelee)
	{
		_state.Velocity = initialVelocity;
		SetState(_state);
	}

	public void OverrideVelocity(WeaponProfile.MeleeOptionData meleeOption)
	{
		OverrideVelocity(meleeOption.GetVelocity(base.transform), meleeOption.MovementDelay, meleeOption.MovementDuration, meleeOption.Recovery, isMelee: true, meleeOption.UntilGrounded);
	}

	public void OverrideVelocity(Vector3 velocity, float delay, float duration, float recovery, bool isMelee, bool untilGrounded)
	{
		_state.IsMeleeing = isMelee;
		_state.VelocityOverriden = true;
		_state.VelocityOverrideDelayRemaining = delay;
		_state.VelocityOverridenUntilGrounded = untilGrounded;
		if (_state.VelocityOverridenUntilGrounded)
		{
			_state.VelocityOverrideDurationRemaining = float.MaxValue;
		}
		else
		{
			_state.VelocityOverrideDurationRemaining = duration;
		}
		_state.VelocityOverrideRecoveryRemaining = recovery;
		_state.Velocity = velocity;
		SetState(_state);
	}

	public void SetState(PlayerMotorState state)
	{
		_state = state;
		_aimPointHandler.transform.localEulerAngles = _state.Aim;
		_motor.ApplyState(new KinematicCharacterMotorState
		{
			Position = _state.Position,
			Rotation = _state.Rotation,
			BaseVelocity = _state.Velocity,
			MustUnground = _state.MustUnground,
			MustUngroundTime = _state.MustUngroundTime,
			LastMovementIterationFoundAnyGround = _state.LastMovementIterationFoundAnyGround,
			GroundingStatus = new CharacterTransientGroundingReport
			{
				FoundAnyGround = _state.FoundAnyGround,
				IsStableOnGround = _state.IsStableOnGround,
				SnappingPrevented = _state.SnappingPrevented,
				GroundNormal = _state.GroundNormal,
				InnerGroundNormal = _state.InnerGroundNormal,
				OuterGroundNormal = _state.OuterGroundNormal
			}
		});
	}

	public PlayerMotorState Move(Vector2 movement, Vector2 camera, bool jump, float speedMultiplier, bool inputEnabled, float deltaTime)
	{
		if (_state.InputOverriden)
		{
			movement = _state.InputOverride;
			_state.VelocityOverrideDurationRemaining -= deltaTime;
			if (_state.VelocityOverrideDurationRemaining <= 0f)
			{
				_state.InputOverriden = false;
				_state.InputOverride = Vector3.zero;
				_state.IsMeleeing = false;
			}
		}
		_input.Enabled = inputEnabled;
		movement.x = Mathf.Clamp(movement.x, -1f, 1f);
		movement.y = Mathf.Clamp(movement.y, -1f, 1f);
		if (Mathf.Abs(movement.y) <= 0.01f)
		{
			movement.y = 0f;
		}
		if (Mathf.Abs(movement.x) <= 0.01f)
		{
			movement.x = 0f;
		}
		_input.Movement = new Vector3(movement.x, 0f, movement.y);
		_input.Camera = camera;
		if (jump)
		{
			_state.TimeSinceJumpRequested = 0f;
			_state.JumpRequested = jump;
		}
		_input.SpeedMultiplier = speedMultiplier;
		_motor.PerformFullSimulation(deltaTime);
		UpdateCurrentMotorState();
		return _state;
	}

	public void SetProfileProperties(HeroClassProfile heroClass, WeaponProfile activeWeapon)
	{
		_movementDetails.SetSpeed(heroClass, activeWeapon);
		_movementDetails.TryApplySpeedEquipmentModifications(_loadoutController);
		_jumpDetails.SetJumpProperties(heroClass, activeWeapon);
	}

	public void BeforeCharacterUpdate(float deltaTime)
	{
		Vector2 zero = Vector2.zero;
		if (_input.Enabled)
		{
			zero.x = _input.Camera.x * _cameraDetails.VerticalRotationSensitivity;
			zero.y = _input.Camera.y * _cameraDetails.HorizontalRotationSensitivity;
		}
		_aimPointHandler.transform.Rotate(Vector3.up, zero.y * Time.deltaTime, Space.World);
		Vector3 localEulerAngles = _aimPointHandler.transform.localEulerAngles;
		localEulerAngles.x = localEulerAngles.x.NormalizeAngle();
		localEulerAngles.x = Mathf.Clamp(localEulerAngles.x - zero.x * deltaTime, 0f - _cameraDetails.MaxUpPitch, _cameraDetails.MaxDownPitch);
		localEulerAngles.y = localEulerAngles.y.NormalizeAngle();
		_aimPointHandler.transform.localEulerAngles = localEulerAngles;
		_aimPointHandlerWorldRotationCache = _aimPointHandler.transform.rotation;
	}

	public void PostGroundingUpdate(float deltaTime)
	{
		_state.IsGrounded = _motor.GroundingStatus.IsStableOnGround && _motor.LastGroundingStatus.IsStableOnGround;
		if (_state.IsGrounded)
		{
			_state.TimeSinceLastGrounded = 0f;
		}
	}

	public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{
		Vector3 vector = Vector3.zero;
		if (_input.Enabled)
		{
			vector = _input.Movement;
		}
		_state.PrevFrameHadInput = vector != Vector3.zero;
		Vector3 vector2 = Vector3.ProjectOnPlane(_aimPointHandler.transform.forward, _motor.CharacterUp);
		if (_state.VelocityOverriden || _state.InputOverriden)
		{
			return;
		}
		if (vector.sqrMagnitude >= 0.01f)
		{
			if (Mathf.Abs(vector.z) >= 0.1f && Mathf.Abs(vector.x) >= 0.1f)
			{
				vector2 = Vector3.ProjectOnPlane(_aimPointHandler.transform.TransformDirection(vector * Mathf.Sign(vector.z)), _motor.CharacterUp);
				_input.Movement.x = 0f;
			}
			_state.IsRotatingToForward = !_state.VelocityOverriden;
		}
		else if (Vector3.Angle(vector2, currentRotation * Vector3.forward) >= _movementDetails.TurnAngleMax || _state.PrevFrameHadInput)
		{
			_state.IsRotatingToForward = !_state.VelocityOverriden;
		}
		if (_state.IsRotatingToForward)
		{
			Quaternion quaternion = Quaternion.LookRotation(vector2, _motor.CharacterUp);
			currentRotation = Quaternion.RotateTowards(currentRotation, quaternion, _movementDetails.TurnSpeed);
			_state.RotationSign = 0f - Mathf.Sign(Vector3.Dot(currentRotation * Vector3.right, quaternion * Vector3.forward));
			if (Quaternion.Angle(quaternion, currentRotation) <= 5f)
			{
				_state.IsRotatingToForward = false;
			}
		}
	}

	public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
	{
		Vector3 vector = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
		if (_state.VelocityOverriden)
		{
			if (_state.VelocityOverrideDelayRemaining > 0f)
			{
				currentVelocity = Vector3.zero;
				_state.VelocityOverrideDelayRemaining -= deltaTime;
			}
			else if (_state.VelocityOverrideDurationRemaining > 0f)
			{
				currentVelocity = _state.Velocity;
				if (_state.VelocityOverridenUntilGrounded && _motor.GroundingStatus.IsStableOnGround)
				{
					_state.VelocityOverrideDurationRemaining = 0f;
					_state.VelocityOverridenUntilGrounded = false;
				}
				else
				{
					_state.VelocityOverrideDurationRemaining -= deltaTime;
				}
			}
			else if (_state.VelocityOverrideRecoveryRemaining > 0f)
			{
				currentVelocity -= vector;
				if (!_motor.GroundingStatus.IsStableOnGround)
				{
					currentVelocity += Physics.gravity * deltaTime;
				}
				_state.VelocityOverrideRecoveryRemaining -= deltaTime;
			}
			else
			{
				_state.VelocityOverriden = false;
				_state.IsMeleeing = false;
			}
			return;
		}
		Vector3 vector2 = Vector3.zero;
		if (_input.Enabled)
		{
			vector2 = _input.Movement;
		}
		if (vector2.z >= 0f)
		{
			vector2.z *= _movementDetails.ForwardMaxSpeed;
		}
		else
		{
			vector2.z *= _movementDetails.BackwardMaxSpeed;
		}
		vector2.z *= _input.SpeedMultiplier;
		vector2.x *= _movementDetails.SidewaysMaxSpeed * _input.SpeedMultiplier;
		Vector3 direction = base.transform.rotation * vector2;
		Vector3 surfaceNormal = _motor.GroundingStatus.GroundNormal;
		float num = 0f;
		float num2 = _input.SpeedMultiplier + 0.001f;
		float num3 = _movementDetails.GreatestMaxSpeed * _movementDetails.GreatestMaxSpeed * num2 * num2;
		float sqrMagnitude = (_motor.GetDirectionTangentToSurface(new Vector3(currentVelocity.x, 0f, currentVelocity.z), surfaceNormal) * currentVelocity.magnitude).sqrMagnitude;
		if (_forceUngroundNextFrame)
		{
			_motor.ForceUnground();
			_forceUngroundNextFrame = false;
		}
		Vector3 vector4;
		if (_motor.GroundingStatus.IsStableOnGround)
		{
			num = ((!(sqrMagnitude > num3)) ? _movementDetails.GroundedAcceleration : _movementDetails.HighSpeedSlipDamp);
			if (surfaceNormal.sqrMagnitude <= 0f || surfaceNormal.y <= 0f)
			{
				surfaceNormal = _motor.CharacterUp;
			}
			currentVelocity = _motor.GetDirectionTangentToSurface(currentVelocity, surfaceNormal) * currentVelocity.magnitude;
			Vector3 vector3 = _motor.GetDirectionTangentToSurface(direction, surfaceNormal) * direction.magnitude;
			vector4 = vector3 - currentVelocity;
		}
		else
		{
			if (vector2.sqrMagnitude > 0.1f && sqrMagnitude < num3 * 2f)
			{
				num = _movementDetails.AirAcceleration;
			}
			Vector3 vector3 = base.transform.rotation * vector2;
			vector3.y = currentVelocity.y;
			vector4 = vector3 - currentVelocity;
		}
		if (num > 0f)
		{
			if (vector4.sqrMagnitude > num * num)
			{
				vector4 = vector4.normalized * num;
			}
			currentVelocity += vector4;
		}
		_state.JumpedThisFrame = false;
		_state.DoubleJumpedThisFrame = false;
		_state.TimeSinceJumpRequested += deltaTime;
		if (_state.JumpRequested)
		{
			bool flag = _motor.GroundingStatus.IsStableOnGround || _state.TimeSinceLastAbleToJump <= _jumpDetails.PostGroundingGraceTime;
			bool flag2 = _jumpDetails.CanDoubleJump && !_motor.GroundingStatus.IsStableOnGround && _state.TimeSinceLastAbleToJump >= _jumpDetails.PostGroundingDoubleJumpDelay && _state.DoubleJumpCooldown <= 0f;
			if ((!_state.JumpConsumed && flag) || (!_state.DoubleJumpConsumed && flag2))
			{
				_motor.ForceUnground();
				float num4 = (flag2 ? _jumpDetails.DoubleJumpHeight : _jumpDetails.JumpHeight);
				currentVelocity += _motor.CharacterUp * num4 - Vector3.Project(currentVelocity, _motor.CharacterUp);
				_state.JumpConsumed = true;
				_state.JumpedThisFrame = true;
				_state.JumpRequested = false;
				if (flag2)
				{
					_state.DoubleJumpConsumed = true;
					_state.DoubleJumpedThisFrame = true;
				}
			}
		}
		else if (_state.IsOnJumpPad && _input.Enabled && !_state.HasConsumedJumpPadJump)
		{
			_motor.ForceUnground();
			currentVelocity = _jumpPad.JumpDirection;
			_state.JumpConsumed = true;
			_state.JumpedThisFrame = true;
			_state.IsJumping = true;
			_state.HasConsumedJumpPadJump = true;
		}
		if (!_motor.GroundingStatus.IsStableOnGround)
		{
			currentVelocity += Physics.gravity * deltaTime;
		}
	}

	public void OnDiscreteCollisionDetected(Collider hitCollider)
	{
	}

	public void AfterCharacterUpdate(float deltaTime)
	{
		if (_state.JumpRequested && _state.TimeSinceJumpRequested > _jumpDetails.PostGroundingGraceTime)
		{
			_state.JumpRequested = false;
		}
		if (_motor.GroundingStatus.IsStableOnGround)
		{
			if (!_state.JumpedThisFrame)
			{
				_state.JumpConsumed = false;
			}
			_state.TimeSinceLastAbleToJump = 0f;
		}
		else
		{
			_state.TimeSinceLastAbleToJump += deltaTime;
		}
		if (_state.DoubleJumpedThisFrame)
		{
			_state.DoubleJumpCooldown = _jumpDetails.DoubleJumpCooldown;
			_state.DoubleJumpConsumed = false;
		}
		_state.DoubleJumpCooldown = Mathf.Max(0f, _state.DoubleJumpCooldown - deltaTime);
	}

	public void AfterPositionRotationUpdate(float deltaTime)
	{
		_aimPointHandler.transform.rotation = _aimPointHandlerWorldRotationCache;
	}

	public bool IsColliderValidForCollisions(Collider coll)
	{
		return true;
	}

	public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
	}

	public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
	}

	public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
	}

	private void UpdateCurrentMotorState()
	{
		KinematicCharacterMotorState state = _motor.GetState();
		_state.Position = state.Position;
		_state.Rotation = state.Rotation;
		if (!_state.VelocityOverriden)
		{
			_state.Velocity = state.BaseVelocity;
		}
		_state.MustUnground = state.MustUnground;
		_state.LastMovementIterationFoundAnyGround = state.LastMovementIterationFoundAnyGround;
		CharacterTransientGroundingReport groundingStatus = state.GroundingStatus;
		_state.FoundAnyGround = groundingStatus.FoundAnyGround;
		if (!_state.IsStableOnGround && !groundingStatus.IsStableOnGround)
		{
			_state.TimeSinceLastGrounded += Time.fixedDeltaTime;
		}
		_state.IsStableOnGround = groundingStatus.IsStableOnGround;
		_state.SnappingPrevented = groundingStatus.SnappingPrevented;
		_state.GroundNormal = groundingStatus.GroundNormal;
		_state.InnerGroundNormal = groundingStatus.InnerGroundNormal;
		_state.OuterGroundNormal = groundingStatus.OuterGroundNormal;
		Vector3 vector = base.transform.InverseTransformDirection(_state.Velocity);
		_state.Speed = vector.z / ((vector.z > 0f) ? _movementDetails.ForwardMaxSpeed : _movementDetails.BackwardMaxSpeed);
		_state.StrafeSpeed = vector.x / _movementDetails.SidewaysMaxSpeed;
		_state.TurnSpeed = ((_state.IsRotatingToForward && _input.Movement == Vector3.zero) ? _state.RotationSign : 0f);
		_state.Aim = _aimPointHandler.transform.localEulerAngles;
		if (!Physics.SphereCast(new Ray(base.transform.position, -base.transform.up), _motor.Capsule.radius, out var hitInfo, 200f, LayerMaskConfig.GroundLayers))
		{
			hitInfo.distance = 200f;
		}
		_state.HeightAboveGround = hitInfo.distance;
		_state.VelocityOverrideDurationRemaining = Mathf.Max(0f, _state.VelocityOverrideDurationRemaining);
		_state.VelocityOverrideDelayRemaining = Mathf.Max(0f, _state.VelocityOverrideDelayRemaining);
		_state.VelocityOverrideRecoveryRemaining = Mathf.Max(0f, _state.VelocityOverrideRecoveryRemaining);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("JumpPad"))
		{
			_state.IsOnJumpPad = true;
			_jumpPad = other.GetComponent<JumpPad>();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("JumpPad"))
		{
			_state.IsOnJumpPad = false;
			_jumpPad = null;
			_state.HasConsumedJumpPadJump = false;
		}
	}
}
