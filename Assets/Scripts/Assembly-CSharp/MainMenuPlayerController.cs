using UnityEngine;

public class MainMenuPlayerController : MonoBehaviour, IPlayerController
{
	public BoltEntity entity => null;

	public IPlayerState state => null;

	public CapsuleCollider HurtCollider => null;

	public Transform AimPoint => null;

	public AimPointHandler AimPointHandler => null;

	public PlayerAnimationController AnimationController { get; private set; }

	public IPlayerMotor Motor => null;

	public bool LocalInputBlocked { get; set; }

	public bool IsLocal => true;

	public bool IsLocalPlayerTeammate => true;

	Transform IPlayerController.transform => base.transform;

	GameObject IPlayerController.gameObject => base.gameObject;

	private void Awake()
	{
		AnimationController = GetComponent<PlayerAnimationController>();
	}

	public void InitializeCharacter(Outfit outfit)
	{
	}

	public void SetPosition(Vector3 position, Quaternion rotation, bool resetVelocity = false)
	{
	}
}
