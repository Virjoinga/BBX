using UnityEngine;

public class SizeController : MonoBehaviour
{
	[SerializeField]
	private float _scaleChangeRate = 0.5f;

	[SerializeField]
	private Transform[] _objectsToScale;

	private IStatusAffectable _statusAffectable;

	private LoadoutController _loadoutController;

	private BoltKinematicCharacterMotor _motor;

	private float _currentScale;

	private float _scale => _statusAffectable.Size;

	private Outfit _outfit => _loadoutController.Outfit;

	private void Awake()
	{
		_statusAffectable = GetComponent<IStatusAffectable>();
		_loadoutController = GetComponent<LoadoutController>();
		_motor = GetComponent<BoltKinematicCharacterMotor>();
	}

	private void FixedUpdate()
	{
		if (_outfit == null || Mathf.Approximately(_currentScale, _scale))
		{
			return;
		}
		_currentScale = Mathf.MoveTowards(_currentScale, _scale, _scaleChangeRate * Time.fixedDeltaTime);
		float num = 1f * (1f + _currentScale);
		Outfit outfit = _outfit;
		Outfit.ColliderData movementColliderData = outfit.MovementColliderData;
		_motor.SetCapsuleDimensions(movementColliderData.Radius * num, movementColliderData.Height * num, movementColliderData.Center.y * num);
		Vector3 localScale = Vector3.one * num;
		outfit.ModelRoot.transform.localScale = localScale;
		Transform[] objectsToScale = _objectsToScale;
		for (int i = 0; i < objectsToScale.Length; i++)
		{
			objectsToScale[i].localScale = localScale;
		}
		Outfit.ColliderData hurtBoxColliderData = outfit.HurtBoxColliderData;
		Outfit.ColliderData headHurtBoxColliderData = outfit.HeadHurtBoxColliderData;
		float num2 = hurtBoxColliderData.Radius * num;
		float num3 = num2 * 2f;
		float y = hurtBoxColliderData.Height * num;
		Vector3 hitboxBoxSize = new Vector3(num3, y, num3);
		Vector3 hitboxCenter = hurtBoxColliderData.Center * num;
		outfit.ProximityHitbox.hitboxBoxSize = hitboxBoxSize;
		outfit.ProximityHitbox.hitboxCenter = hitboxCenter;
		BoltHitbox[] hitboxes = outfit.Hitboxes;
		foreach (BoltHitbox boltHitbox in hitboxes)
		{
			if (boltHitbox.hitboxType == BoltHitboxType.Head)
			{
				boltHitbox.hitboxSphereRadius = headHurtBoxColliderData.Radius * num;
				continue;
			}
			if (boltHitbox.hitboxShape == BoltHitboxShape.Box)
			{
				boltHitbox.hitboxBoxSize = hitboxBoxSize;
			}
			else
			{
				boltHitbox.hitboxSphereRadius = num2;
			}
			boltHitbox.hitboxCenter = hitboxCenter;
		}
	}
}
