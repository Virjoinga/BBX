using UnityEngine;

public class PlayerColliderInitializer : MonoBehaviour
{
	private CapsuleCollider _hurtCollider;

	private BoltHitboxBody _boltHitBoxBody;

	private void Awake()
	{
		_boltHitBoxBody = GetComponent<BoltHitboxBody>();
		_hurtCollider = GetComponent<IPlayerController>().HurtCollider;
	}

	public void InitializeCharacter(Outfit outfit)
	{
		_hurtCollider.center = outfit.HurtBoxColliderData.Center;
		_hurtCollider.height = outfit.HurtBoxColliderData.Height;
		_hurtCollider.radius = outfit.HurtBoxColliderData.Radius;
		_boltHitBoxBody.proximity = outfit.ProximityHitbox;
		_boltHitBoxBody.hitboxes = outfit.Hitboxes;
	}

	public void SetCollidersActiveState(bool isEnabled)
	{
		_hurtCollider.enabled = isEnabled;
		_boltHitBoxBody.enabled = isEnabled;
	}
}
