using System;
using UnityEngine;

[Serializable]
public class MovementDetails
{
	public float ForwardMaxSpeed = 1f;

	public float BackwardMaxSpeed = 0.8f;

	public float SidewaysMaxSpeed = 0.8f;

	public float GroundedAcceleration = 1f;

	public float AirAcceleration = 1f;

	public float MaxFallSpeed = 40f;

	public float SlidingSpeed = 2f;

	public float HighSpeedSlipDamp = 0.6f;

	public float MaxSpeedAfterFireDelay = 1f;

	public float TurnSpeed = 5.5f;

	public float StableMovementSharpness = 15f;

	public float TurnAngleMax = 60f;

	public float ForwardBackwardSpeed { get; set; }

	public float SidewaysSpeed { get; set; }

	public Vector3 Velocity { get; set; } = Vector3.zero;

	public Vector3 LocalVelocity { get; set; } = Vector3.zero;

	public CollisionFlags CollisionFlags { get; set; }

	public Vector3 LastHitPoint { get; set; } = Vector3.zero;

	public float GreatestMaxSpeed => Mathf.Max(ForwardMaxSpeed, BackwardMaxSpeed, SidewaysMaxSpeed);

	public float MaxAcceleration(bool isGrounded)
	{
		if (!isGrounded)
		{
			return AirAcceleration;
		}
		return GroundedAcceleration;
	}

	public void SetSpeed(HeroClassProfile heroClass, WeaponProfile activeWeapon)
	{
		ForwardMaxSpeed = heroClass.Speed.Forward;
		BackwardMaxSpeed = heroClass.Speed.Backward;
		SidewaysMaxSpeed = heroClass.Speed.Sideways;
		AirAcceleration = heroClass.Acceleration.Air;
		GroundedAcceleration = heroClass.Acceleration.Ground;
		if (activeWeapon != null && activeWeapon.CharacterMultiplier.Speed > 0f)
		{
			ForwardMaxSpeed *= activeWeapon.CharacterMultiplier.Speed;
			BackwardMaxSpeed *= activeWeapon.CharacterMultiplier.Speed;
			SidewaysMaxSpeed *= activeWeapon.CharacterMultiplier.Speed;
		}
	}

	public void TryApplySpeedEquipmentModifications(LoadoutController loadoutController)
	{
		if (loadoutController.MajorEquipmentSlotProfile != null && loadoutController.MajorEquipmentSlotProfile.Type == EquipmentType.Speed)
		{
			ForwardMaxSpeed = loadoutController.MajorEquipmentSlotProfile.GetMajorModifiedValue(ForwardMaxSpeed);
			BackwardMaxSpeed = loadoutController.MajorEquipmentSlotProfile.GetMajorModifiedValue(BackwardMaxSpeed);
			SidewaysMaxSpeed = loadoutController.MajorEquipmentSlotProfile.GetMajorModifiedValue(SidewaysMaxSpeed);
		}
		if (loadoutController.MinorEquipmentSlotProfile != null && loadoutController.MinorEquipmentSlotProfile.Type == EquipmentType.Speed)
		{
			ForwardMaxSpeed = loadoutController.MinorEquipmentSlotProfile.GetMinorModifiedValue(ForwardMaxSpeed);
			BackwardMaxSpeed = loadoutController.MinorEquipmentSlotProfile.GetMinorModifiedValue(BackwardMaxSpeed);
			SidewaysMaxSpeed = loadoutController.MinorEquipmentSlotProfile.GetMinorModifiedValue(SidewaysMaxSpeed);
		}
	}
}
