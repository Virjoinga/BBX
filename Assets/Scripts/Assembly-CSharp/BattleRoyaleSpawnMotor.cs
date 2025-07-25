using UnityEngine;

public class BattleRoyaleSpawnMotor : BasePlayerMotor
{
	private RaycastHit _groundHit;

	private bool _groundIsTooSteep;

	public bool InputEnabledOverride { get; set; } = true;

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
	}

	public override PlayerMotorState Move(Vector2 movement, Vector2 camera, bool jump, float speedMultiplier, bool inputEnabled, float deltaTime)
	{
		if (_characterController == null)
		{
			return _state;
		}
		Vector3 zero = Vector3.zero;
		Vector2 vector = Vector3.zero;
		if (inputEnabled && InputEnabledOverride)
		{
			zero.x = movement.x;
			zero.z = Mathf.Max(0f, movement.y);
			vector.x = camera.x * _cameraDetails.HorizontalRotationSensitivity;
			vector.y = camera.y * _cameraDetails.VerticalRotationSensitivity;
		}
		base.transform.Rotate(Vector3.up, vector.x * deltaTime, Space.World);
		_state.Aim.x = Mathf.Clamp(_state.Aim.x - vector.y * deltaTime, 0f - _cameraDetails.MaxUpPitch, _cameraDetails.MaxDownPitch);
		float num = 1f - Mathf.InverseLerp(0f, _cameraDetails.MaxDownPitch, Mathf.Abs(_state.Aim.x));
		zero.z *= num * _movementDetails.ForwardMaxSpeed;
		zero.x *= _movementDetails.SidewaysMaxSpeed;
		float airAcceleration = _movementDetails.AirAcceleration;
		Vector3 vector2 = base.transform.TransformDirection(zero) - _velocity;
		vector2.y = 0f;
		if (vector2.sqrMagnitude > airAcceleration * airAcceleration)
		{
			vector2 = vector2.normalized * airAcceleration;
		}
		_velocity += vector2;
		_velocity += Physics.gravity * deltaTime;
		float num2 = _movementDetails.MaxFallSpeed;
		if (zero.z > 0f)
		{
			num2 += 10f * (1f - num);
		}
		if (_groundIsTooSteep)
		{
			Vector3 vector3 = new Vector3((1f - _groundHit.normal.y) * _groundHit.normal.x, 0f, (1f - _groundHit.normal.y) * _groundHit.normal.z);
			_velocity += vector3 * _movementDetails.SlidingSpeed;
		}
		_velocity.y = Mathf.Max(_velocity.y, 0f - num2);
		_state.IsJumping = false;
		base.CollisionFlags = _characterController.Move(_velocity * deltaTime);
		if ((base.CollisionFlags & CollisionFlags.Above & CollisionFlags.Above) != CollisionFlags.None)
		{
			_velocity.y = 0f;
		}
		_groundHit = ProbeGround();
		_groundIsTooSteep = GroundIsTooSteep(_groundHit);
		_state.IsGrounded = base.CollisionFlags.HasCollidedBelow() && !_groundIsTooSteep;
		if (_state.IsGrounded)
		{
			_velocity.y = Physics.gravity.y;
		}
		_state.Position = base.transform.position;
		_state.Rotation = base.transform.rotation;
		_state.Velocity = _velocity;
		Vector3 vector4 = base.transform.InverseTransformDirection(_velocity);
		_state.Speed = vector4.z / ((vector4.z > 0f) ? _movementDetails.ForwardMaxSpeed : _movementDetails.BackwardMaxSpeed);
		_state.StrafeSpeed = vector4.x / _movementDetails.SidewaysMaxSpeed;
		_state.TurnSpeed = _movementDetails.TurnSpeed;
		return _state;
	}
}
