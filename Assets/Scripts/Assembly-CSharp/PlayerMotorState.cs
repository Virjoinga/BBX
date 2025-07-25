using System;
using UnityEngine;

[Serializable]
public struct PlayerMotorState
{
	public Vector3 Position;

	public Quaternion Rotation;

	public Vector3 Velocity;

	public bool MustUnground;

	public float MustUngroundTime;

	public bool LastMovementIterationFoundAnyGround;

	public bool FoundAnyGround;

	public bool IsStableOnGround;

	public bool SnappingPrevented;

	public Vector3 GroundNormal;

	public Vector3 InnerGroundNormal;

	public Vector3 OuterGroundNormal;

	public Vector3 Aim;

	public float Speed;

	public float StrafeSpeed;

	public float TurnSpeed;

	public bool IsGrounded;

	public float TimeSinceLastGrounded;

	public bool IsJumping;

	public bool JumpedThisFrame;

	public bool JumpRequested;

	public float TimeSinceJumpRequested;

	public float TimeSinceLastAbleToJump;

	public bool JumpConsumed;

	public bool IsOnJumpPad;

	public bool HasConsumedJumpPadJump;

	public bool IsFalling;

	public bool IsRotatingToForward;

	public float RotationSign;

	public bool PrevFrameHadInput;

	public bool IsMeleeing;

	public bool InputOverriden;

	public Vector2 InputOverride;

	public bool VelocityOverriden;

	public bool VelocityOverridenUntilGrounded;

	public float VelocityOverrideDelayRemaining;

	public float VelocityOverrideDurationRemaining;

	public float VelocityOverrideRecoveryRemaining;

	public float HeightAboveGround;

	public float DoubleJumpCooldown;

	public bool DoubleJumpConsumed;

	public bool DoubleJumpedThisFrame;

	public bool IsGroundedOrWithinGrace
	{
		get
		{
			if (!IsStableOnGround)
			{
				return TimeSinceLastGrounded <= 0.1f;
			}
			return true;
		}
	}
}
