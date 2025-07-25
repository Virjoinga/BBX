using UnityEngine;

public abstract class BasePlayerMotor : MonoBehaviour, IPlayerMotor
{
	[SerializeField]
	protected CharacterController _characterController;

	[SerializeField]
	protected MovementDetails _movementDetails;

	[SerializeField]
	protected CameraDetails _cameraDetails;

	[SerializeField]
	protected PlayerMotorState _state;

	[SerializeField]
	protected Vector3 _velocity;

	[SerializeField]
	private float _probeRadiusMultiplier = 0.9f;

	public CollisionFlags CollisionFlags { get; protected set; }

	public PlayerMotorState State => _state;

	public float Radius => _characterController.radius;

	public float Height => _characterController.height;

	public JumpDetails JumpDetails { get; private set; }

	Transform IPlayerMotor.transform => base.transform;

	public void ApplyForce(Vector3 force, bool breaksMelee)
	{
	}

	public void OverrideInput(float horizontal, float vertical, WeaponProfile.MeleeOptionData meleeOption)
	{
	}

	public void OverrideVelocity(WeaponProfile.MeleeOptionData meleeOption)
	{
	}

	public void OverrideVelocity(Vector3 velocity, float delay, float duration, float recovery, bool isMelee, bool untilGrounded)
	{
	}

	public virtual void InitializeCharacter(Outfit outfit)
	{
	}

	public virtual void SetProfileProperties(HeroClassProfile heroClass, WeaponProfile weapon)
	{
	}

	public abstract void SetState(PlayerMotorState newState);

	public abstract PlayerMotorState Move(Vector2 movement, Vector2 camera, bool jump, float speedMultiplier, bool inputEnabled, float deltaTime);

	protected RaycastHit ProbeGround()
	{
		float num = _characterController.radius * _probeRadiusMultiplier;
		float num2 = _characterController.height * 0.25f - (_characterController.radius - num);
		Ray ray = new Ray(base.transform.position + Vector3.up * num2, Vector3.down);
		DebugExtension.DebugWireSphere(ray.origin, Color.cyan, num);
		DebugExtension.DebugWireSphere(ray.origin + ray.direction * _characterController.skinWidth * 2f, Color.blue, num);
		if (Physics.SphereCast(ray, num, out var hitInfo, _characterController.skinWidth * 2f, LayerMaskConfig.GroundLayers))
		{
			DebugExtension.DebugArrow(hitInfo.point, hitInfo.normal * 2f, Color.blue);
		}
		return hitInfo;
	}

	protected bool GroundIsTooSteep(RaycastHit hit)
	{
		return Vector3.Angle(Vector3.up, hit.normal) > _characterController.slopeLimit;
	}
}
