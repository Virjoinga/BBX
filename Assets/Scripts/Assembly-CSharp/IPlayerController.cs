using UnityEngine;

public interface IPlayerController
{
	BoltEntity entity { get; }

	IPlayerState state { get; }

	Transform transform { get; }

	GameObject gameObject { get; }

	CapsuleCollider HurtCollider { get; }

	Transform AimPoint { get; }

	PlayerAnimationController AnimationController { get; }

	IPlayerMotor Motor { get; }

	AimPointHandler AimPointHandler { get; }

	bool LocalInputBlocked { get; set; }

	bool IsLocal { get; }

	bool IsLocalPlayerTeammate { get; }

	void InitializeCharacter(Outfit outfit);

	void SetPosition(Vector3 position, Quaternion rotation, bool resetVelocity = false);
}
