using UnityEngine;

public interface IPlayerMotor
{
	Transform transform { get; }

	PlayerMotorState State { get; }

	float Radius { get; }

	float Height { get; }

	JumpDetails JumpDetails { get; }

	void InitializeCharacter(Outfit outfit);

	void SetState(PlayerMotorState state);

	PlayerMotorState Move(Vector2 movement, Vector2 camera, bool jump, float speedMultiplier, bool inputEnabled, float deltaTime);

	void SetProfileProperties(HeroClassProfile heroClass, WeaponProfile activeWeapon);

	void ApplyForce(Vector3 force, bool breaksMelee);

	void OverrideVelocity(Vector3 velocity, float delay, float duration, float recovery, bool isMelee, bool untilGrounded);

	void OverrideVelocity(WeaponProfile.MeleeOptionData meleeOption);

	void OverrideInput(float horizontal, float vertical, WeaponProfile.MeleeOptionData meleeOption);
}
