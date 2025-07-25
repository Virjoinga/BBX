using UnityEngine;

public class SimplePlayerMotor : BasePlayerMotor
{
	[SerializeField]
	protected JumpDetails _jumpDetails;

	private AimPointHandler _aimPointHandler;

	protected JumpPad _jumpPad;

	private RaycastHit _groundHit;

	private bool _groundIsTooSteep;

	public override void InitializeCharacter(Outfit outfit)
	{
		_aimPointHandler = outfit.AimPointHandler;
		_characterController.center = outfit.MovementColliderData.Center;
		_characterController.height = outfit.MovementColliderData.Height;
		_characterController.radius = outfit.MovementColliderData.Radius;
	}

	public override void SetProfileProperties(HeroClassProfile heroClass, WeaponProfile weapon)
	{
		_movementDetails.SetSpeed(heroClass, weapon);
	}

	public override void SetState(PlayerMotorState newState)
	{
		_state.Position = newState.Position;
		_state.Rotation = newState.Rotation;
		_state.Velocity = newState.Velocity;
		_state.Aim = newState.Aim;
		_state.IsGrounded = newState.IsGrounded;
		base.transform.position = _state.Position;
		base.transform.rotation = _state.Rotation;
		_velocity = newState.Velocity;
		_aimPointHandler.transform.localEulerAngles = _state.Aim;
		_state.IsRotatingToForward = newState.IsRotatingToForward;
		_state.RotationSign = newState.RotationSign;
		_state.IsJumping = newState.IsJumping;
		_state.JumpedThisFrame = newState.JumpedThisFrame;
		_state.JumpRequested = newState.JumpRequested;
		_state.TimeSinceJumpRequested = newState.TimeSinceJumpRequested;
		_state.TimeSinceLastAbleToJump = newState.TimeSinceLastAbleToJump;
		_state.JumpConsumed = newState.JumpConsumed;
		_state.IsOnJumpPad = newState.IsOnJumpPad;
		_state.HasConsumedJumpPadJump = newState.HasConsumedJumpPadJump;
	}

	public override PlayerMotorState Move(Vector2 movementInput, Vector2 cameraInput, bool jump, float speedMultiplier, bool inputEnabled, float deltaTime)
	{
		if (_characterController == null)
		{
			return _state;
		}
		Vector3 zero = Vector3.zero;
		Vector2 vector = Vector3.zero;
		if (inputEnabled)
		{
			zero.x = Mathf.Clamp(movementInput.x, -1f, 1f);
			zero.z = Mathf.Clamp(movementInput.y, -1f, 1f);
			vector.x = cameraInput.x * _cameraDetails.VerticalRotationSensitivity;
			vector.y = cameraInput.y * _cameraDetails.HorizontalRotationSensitivity;
			if (zero.z > 0f)
			{
				zero.z *= _movementDetails.ForwardMaxSpeed;
			}
			else
			{
				zero.z *= _movementDetails.BackwardMaxSpeed;
			}
			zero.z *= speedMultiplier;
			zero.x *= _movementDetails.SidewaysMaxSpeed * speedMultiplier;
		}
		else
		{
			_state.IsOnJumpPad = false;
			_state.JumpConsumed = false;
			_state.JumpedThisFrame = false;
			_state.IsJumping = false;
			_state.HasConsumedJumpPadJump = false;
		}
		if (Mathf.Abs(zero.z) <= 0.0001f)
		{
			zero.z = 0f;
		}
		if (Mathf.Abs(zero.x) <= 0.0001f)
		{
			zero.x = 0f;
		}
		_state.PrevFrameHadInput = zero != Vector3.zero;
		_aimPointHandler.transform.Rotate(Vector3.up, vector.y * Time.deltaTime, Space.World);
		Vector3 normalized = Vector3.ProjectOnPlane(_aimPointHandler.transform.forward, Vector3.up).normalized;
		if (zero != Vector3.zero)
		{
			if (zero.z != 0f && zero.x != 0f)
			{
				normalized = Vector3.ProjectOnPlane(_aimPointHandler.transform.TransformDirection(zero * Mathf.Sign(zero.z)), Vector3.up).normalized;
				zero.x = 0f;
			}
		}
		else if (Vector3.Angle(normalized, base.transform.forward) >= _movementDetails.TurnAngleMax || _state.PrevFrameHadInput)
		{
			_state.IsRotatingToForward = true;
		}
		if (_state.IsRotatingToForward || zero != Vector3.zero)
		{
			Quaternion rotation = _aimPointHandler.transform.rotation;
			Quaternion to = Quaternion.LookRotation(normalized, Vector3.up);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, _movementDetails.TurnSpeed);
			_aimPointHandler.transform.rotation = rotation;
			_state.RotationSign = 0f - Mathf.Sign(Vector3.Dot(base.transform.right, normalized));
			if (Vector3.Angle(normalized, base.transform.forward) <= 5f)
			{
				_state.IsRotatingToForward = false;
			}
		}
		Vector3 vector2 = base.transform.TransformDirection(zero);
		float num = 0f;
		bool flag = new Vector3(_velocity.x, 0f, _velocity.z).sqrMagnitude > _movementDetails.GreatestMaxSpeed * _movementDetails.GreatestMaxSpeed;
		if (_state.IsGrounded)
		{
			num = ((!flag) ? _movementDetails.GroundedAcceleration : _movementDetails.HighSpeedSlipDamp);
		}
		else if (zero.sqrMagnitude > 0.1f && !flag)
		{
			num = _movementDetails.AirAcceleration;
		}
		if (num > 0f)
		{
			Vector3 vector3 = vector2 - _velocity;
			vector3.y = 0f;
			if (vector3.sqrMagnitude > num * num)
			{
				vector3 = vector3.normalized * num;
			}
			_velocity += vector3;
		}
		if (_groundIsTooSteep)
		{
			Vector3 vector4 = new Vector3((1f - _groundHit.normal.y) * _groundHit.normal.x, 0f, (1f - _groundHit.normal.y) * _groundHit.normal.z);
			_velocity += vector4 * _movementDetails.SlidingSpeed;
		}
		_velocity += Physics.gravity * deltaTime;
		_velocity.y = Mathf.Max(_velocity.y, 0f - _movementDetails.MaxFallSpeed);
		_state.IsJumping = false;
		_state.JumpedThisFrame = false;
		_state.TimeSinceJumpRequested += deltaTime;
		if (jump && inputEnabled && !_groundIsTooSteep && speedMultiplier > 0f)
		{
			if (!_state.JumpConsumed && (_state.IsGrounded || _state.TimeSinceLastAbleToJump <= _jumpDetails.PostGroundingGraceTime))
			{
				Vector3 up = base.transform.up;
				_velocity += up * _jumpDetails.JumpHeight - Vector3.Project(_velocity, base.transform.up);
				_state.JumpRequested = false;
				_state.JumpConsumed = true;
				_state.JumpedThisFrame = true;
				_state.IsJumping = true;
			}
		}
		else if (_state.IsOnJumpPad && inputEnabled && !_state.HasConsumedJumpPadJump)
		{
			_velocity = _jumpPad.JumpDirection;
			_state.JumpConsumed = true;
			_state.JumpedThisFrame = true;
			_state.IsJumping = true;
			_state.HasConsumedJumpPadJump = true;
		}
		base.CollisionFlags = _characterController.Move(_velocity * deltaTime);
		if (base.CollisionFlags.HasCollidedAbove())
		{
			_velocity.y = 0f;
		}
		_groundHit = ProbeGround();
		_groundIsTooSteep = GroundIsTooSteep(_groundHit);
		_state.IsGrounded = base.CollisionFlags.HasCollidedBelow() && !_groundIsTooSteep;
		if (_state.JumpRequested && _state.TimeSinceJumpRequested > _jumpDetails.PostGroundingGraceTime)
		{
			_state.JumpRequested = false;
		}
		if (_state.IsGrounded)
		{
			if (!_state.JumpedThisFrame)
			{
				_state.JumpConsumed = false;
			}
			_state.TimeSinceLastAbleToJump = 0f;
			_velocity.y = Physics.gravity.y * 0.8f;
		}
		else
		{
			_state.TimeSinceLastAbleToJump += deltaTime;
		}
		_state.Position = base.transform.position;
		_state.Rotation = base.transform.rotation;
		_state.Velocity = _velocity;
		Vector3 vector5 = base.transform.InverseTransformDirection(_velocity);
		_state.Speed = vector5.z / ((vector5.z > 0f) ? _movementDetails.ForwardMaxSpeed : _movementDetails.BackwardMaxSpeed);
		_state.StrafeSpeed = vector5.x / _movementDetails.SidewaysMaxSpeed;
		_state.TurnSpeed = (_state.IsRotatingToForward ? _state.RotationSign : 0f);
		Vector3 localEulerAngles = _aimPointHandler.transform.localEulerAngles;
		localEulerAngles.x = Mathf.Clamp(_state.Aim.x - vector.x * deltaTime, 0f - _cameraDetails.MaxUpPitch, _cameraDetails.MaxDownPitch);
		localEulerAngles.y = localEulerAngles.y.NormalizeAngle();
		ref PlayerMotorState state = ref _state;
		Vector3 aim = (_aimPointHandler.transform.localEulerAngles = localEulerAngles);
		state.Aim = aim;
		return _state;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "JumpPad")
		{
			_state.IsOnJumpPad = true;
			_jumpPad = other.GetComponent<JumpPad>();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "JumpPad")
		{
			_state.IsOnJumpPad = false;
			_jumpPad = null;
			_state.HasConsumedJumpPadJump = false;
		}
	}
}
