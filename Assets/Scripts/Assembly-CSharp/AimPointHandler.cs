using BSCore;
using UnityEngine;

public class AimPointHandler : MonoBehaviour
{
	[SerializeField]
	private Transform _aimPoint;

	[SerializeField]
	private Transform _weaponPositioner;

	[SerializeField]
	private float _liftedWeaponOffset;

	[SerializeField]
	private float _weaponLiftSpeed = 1f;

	private float _minAimDistance = 2f;

	private float _maxAimDistance = 150f;

	private readonly float _aimSpeed = 4f;

	private readonly float _aimRadius = 0.1f;

	private float _desiredDistance = 50f;

	private Collider _ownerCollider;

	private Transform _rootTransform;

	[SerializeField]
	private Vector3 _offset = Vector3.zero;

	[SerializeField]
	private Vector3 _weaponPositionerInitialLocalPosition;

	private float _weaponPositionerInitialHeight;

	private DataStoreFloat _cameraOffsetX;

	private DataStoreFloat _cameraOffsetZ;

	private float _clippedOffsetChangeSpeed = 7f;

	private bool _armLifted;

	public bool IsLocalPlayer { get; set; }

	public Vector3 Offset => _offset;

	public float CameraDistanceOffset { get; private set; }

	public Transform AimPoint => _aimPoint;

	public Vector3 AimPosition { get; private set; }

	public Vector3 AimOrigin => base.transform.position + _offset;

	public void SetCameraOffsetDataStore(DataStoreFloat cameraOffsetX, DataStoreFloat cameraOffsetZ)
	{
		_cameraOffsetX = cameraOffsetX;
		_cameraOffsetX.Changed += OnCameraOffsetXChanged;
		OnCameraOffsetXChanged(_cameraOffsetX.Value);
		_cameraOffsetZ = cameraOffsetZ;
		_cameraOffsetZ.Changed += OnCameraOffsetZChanged;
		OnCameraOffsetZChanged(_cameraOffsetZ.Value);
	}

	private void Awake()
	{
		IPlayerController componentInParent = GetComponentInParent<IPlayerController>();
		if (componentInParent != null)
		{
			_ownerCollider = componentInParent.HurtCollider;
		}
		_rootTransform = GetComponentInParent<Outfit>().transform;
		_weaponPositionerInitialLocalPosition = _weaponPositioner.localPosition;
		_weaponPositionerInitialHeight = _weaponPositionerInitialLocalPosition.y;
	}

	private void Start()
	{
		_aimPoint.position = base.transform.position + base.transform.forward * 50f;
	}

	private void OnDestroy()
	{
		if (_cameraOffsetX != null)
		{
			_cameraOffsetX.Changed -= OnCameraOffsetXChanged;
		}
		if (_cameraOffsetZ != null)
		{
			_cameraOffsetZ.Changed -= OnCameraOffsetZChanged;
		}
	}

	private void FixedUpdate()
	{
		if (!MonoBehaviourSingleton<OTSCamera>.IsInstantiated)
		{
			return;
		}
		if (IsLocalPlayer)
		{
			CheckForAimCollision();
		}
		Vector3 vector = base.transform.TransformPoint(_offset);
		Vector3 forward = base.transform.forward;
		RaycastHit[] array = Physics.SphereCastAll(vector, _aimRadius, forward, _maxAimDistance, LayerMaskConfig.HitableLayers);
		RaycastHit raycastHit = default(RaycastHit);
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit2 = array2[i];
			if (!(raycastHit2.collider == _ownerCollider) && (raycastHit.collider == null || raycastHit2.distance < raycastHit.distance))
			{
				raycastHit = raycastHit2;
			}
		}
		if (raycastHit.collider != null)
		{
			_desiredDistance = Mathf.Clamp(raycastHit.distance, _minAimDistance, _maxAimDistance);
			if (_desiredDistance <= _minAimDistance)
			{
				AimPosition = vector + forward * _desiredDistance;
			}
			else
			{
				AimPosition = raycastHit.point;
			}
			Debug.DrawLine(vector, AimPosition, Color.white);
			DebugExtension.DebugPoint(AimPosition, Color.white);
		}
		else
		{
			_desiredDistance = _maxAimDistance;
			AimPosition = vector + forward * _desiredDistance;
			Debug.DrawLine(vector, AimPosition, Color.green);
		}
	}

	private void Update()
	{
		_aimPoint.position = base.transform.TransformPoint(_offset) + base.transform.forward * Mathf.MoveTowards(Vector3.Distance(base.transform.TransformPoint(_offset), _aimPoint.position), _desiredDistance, _aimSpeed);
		DebugExtension.DebugPoint(_aimPoint.position, Color.cyan);
		ClampWeaponPositionerLeftAim();
		HandleArmHeight();
	}

	public void LiftWeapon()
	{
		_armLifted = true;
	}

	public void LowerWeapon()
	{
		_armLifted = false;
	}

	private void CheckForAimCollision()
	{
		Vector3 position = Vector3.right * _cameraOffsetX.Value;
		Vector3 normalized = (base.transform.TransformPoint(position) - base.transform.position).normalized;
		float newValue;
		if (Physics.SphereCast(new Ray(base.transform.position, normalized), 0.11f, out var hitInfo, Mathf.Abs(_cameraOffsetX.Value), LayerMaskConfig.GroundLayers))
		{
			float target = hitInfo.distance * Mathf.Sign(_cameraOffsetX.Value);
			newValue = Mathf.MoveTowards(_offset.x, target, _clippedOffsetChangeSpeed * Time.deltaTime);
		}
		else
		{
			newValue = Mathf.MoveTowards(_offset.x, _cameraOffsetX.Value, _clippedOffsetChangeSpeed * Time.deltaTime);
		}
		OnCameraOffsetXChanged(newValue);
	}

	private void ClampWeaponPositionerLeftAim()
	{
		Vector3 vector = base.transform.localEulerAngles.WrapEulerAngles();
		Vector3 localPosition = Quaternion.AngleAxis(vector.y, _rootTransform.up) * _weaponPositionerInitialLocalPosition;
		if (vector.y > 0f)
		{
			localPosition.x = _weaponPositionerInitialLocalPosition.x;
		}
		_weaponPositioner.localPosition = localPosition;
		_weaponPositioner.LookAt(_aimPoint, _rootTransform.up);
	}

	private void HandleArmHeight()
	{
		float num = _weaponPositionerInitialHeight;
		if (_armLifted)
		{
			num += _liftedWeaponOffset;
		}
		float newYOffset = Mathf.MoveTowards(_weaponPositionerInitialLocalPosition.y, num, _weaponLiftSpeed * Time.fixedDeltaTime);
		OnArmHeightChanged(newYOffset);
	}

	public void OnArmHeightChanged(float newYOffset)
	{
		Vector3 localPosition = _weaponPositioner.localPosition;
		localPosition.y = newYOffset;
		_weaponPositioner.localPosition = localPosition;
		_weaponPositionerInitialLocalPosition.y = newYOffset;
	}

	public void OnCameraOffsetXChanged(float newValue)
	{
		_offset.x = newValue;
		if (MonoBehaviourSingleton<OTSCamera>.IsInstantiated)
		{
			MonoBehaviourSingleton<OTSCamera>.Instance.SetOffset(this);
		}
	}

	public void OnCameraOffsetZChanged(float newValue)
	{
		float cameraDistanceOffset = Mathf.Abs(newValue) * -1f;
		CameraDistanceOffset = cameraDistanceOffset;
		if (MonoBehaviourSingleton<OTSCamera>.IsInstantiated)
		{
			MonoBehaviourSingleton<OTSCamera>.Instance.SetOffset(this);
		}
	}
}
