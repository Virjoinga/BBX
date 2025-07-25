using System;
using UnityEngine;

[Serializable]
public class JumpDetails
{
	public bool CanJump = true;

	public float JumpHeight = 1f;

	public float JumpCooldown = 0.5f;

	public float PostGroundingGraceTime = 0.1f;

	public bool CanDoubleJump;

	public float PostGroundingDoubleJumpDelay = 0.15f;

	public float DoubleJumpCooldown = 5f;

	public float DoubleJumpHeight = 7f;

	public bool IsJumping { get; set; }

	public bool JumpHeld { get; set; }

	public float LastJumpStartTime { get; set; }

	public float LastJumpHoldTime { get; set; } = -100f;

	public Vector3 JumpDirection { get; set; } = Vector3.up;

	public void SetJumpProperties(HeroClassProfile heroClass, WeaponProfile weapon)
	{
		JumpHeight = heroClass.Jump.Height;
		CanDoubleJump = true;
		DoubleJumpCooldown = heroClass.Jump.DoubleJumpCooldown;
		DoubleJumpHeight = heroClass.Jump.DoubleJumpHeight;
	}
}
