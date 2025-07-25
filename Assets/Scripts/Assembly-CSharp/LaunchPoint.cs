using System;
using System.Collections;
using UnityEngine;

public class LaunchPoint : MonoBehaviour
{
	[SerializeField]
	protected Projectile _projectilePrefab;

	[SerializeField]
	protected Transform[] _projectileFakes;

	[SerializeField]
	protected GameObject _launchEffect;

	[SerializeField]
	protected GameObject _impactEffect;

	[SerializeField]
	protected Transform[] _muzzles;

	[SerializeField]
	private LayerMaskConfig.Group _aimsAtLayer;

	[SerializeField]
	protected bool _useAimPointHandlerForAiming;

	protected BaseWeapon _parentWeapon;

	private BoltKinematicPlayerController _playerMotor;

	protected int _muzzleIndex;

	public Projectile ProjectilePrefab => _projectilePrefab;

	public virtual float Radius
	{
		get
		{
			if (!(_projectilePrefab != null))
			{
				return 0.1f;
			}
			return _projectilePrefab.Radius;
		}
	}

	public Vector3 AimPosition { get; protected set; }

	public Vector3 AimOrigin
	{
		get
		{
			if (!_useAimPointHandlerForAiming)
			{
				return base.transform.position;
			}
			return _aimPointHandler.transform.position;
		}
	}

	public virtual float SpreadPercentToMax => 0f;

	public WeaponProfile Profile => _parentWeapon.Profile;

	protected LayerMask _hitableLayers => LayerMaskConfig.HitableLayers;

	protected AimPointHandler _aimPointHandler { get; private set; }

	private event Action<Vector3, int> _fired;

	public event Action<Vector3, int> Fired
	{
		add
		{
			_fired += value;
		}
		remove
		{
			_fired -= value;
		}
	}

	protected virtual void Awake()
	{
		_parentWeapon = GetComponentInParent<BaseWeapon>();
		_aimPointHandler = GetComponentInParent<Outfit>().AimPointHandler;
	}

	protected virtual void Start()
	{
		_playerMotor = GetComponentInParent<BoltKinematicPlayerController>();
	}

	protected virtual void Update()
	{
		Vector3 aimOrigin;
		if (_useAimPointHandlerForAiming)
		{
			aimOrigin = _aimPointHandler.AimOrigin;
			AimPosition = _aimPointHandler.AimPosition;
			return;
		}
		aimOrigin = base.transform.position;
		if (!Physics.SphereCast(aimOrigin, Radius, base.transform.forward, out var hitInfo, 150f, LayerMaskConfig.GetLayerMask(_aimsAtLayer)))
		{
			hitInfo.point = aimOrigin + base.transform.forward * 150f;
		}
		AimPosition = hitInfo.point;
	}

	protected virtual void OnEnable()
	{
		if (_projectileFakes != null)
		{
			Transform[] projectileFakes = _projectileFakes;
			for (int i = 0; i < projectileFakes.Length; i++)
			{
				projectileFakes[i].localScale = Vector3.one;
			}
		}
	}

	protected virtual void OnDisable()
	{
	}

	public virtual void UpdateProperties(WeaponProfile profile)
	{
	}

	public void FireVolley(int volleySize, float volleyDelay, Vector3 position, Vector3 forward, float fireDelay, int projectileId, int serverFrame, Action<HitInfo> onHit, bool isMock = false)
	{
		StartCoroutine(FireVolleyRoutine(volleySize, volleyDelay, position, forward, fireDelay, projectileId, serverFrame, onHit, isMock));
	}

	protected virtual IEnumerator FireVolleyRoutine(int volleySize, float volleyDelay, Vector3 position, Vector3 forward, float fireDelay, int projectileId, int serverFrame, Action<HitInfo> onHit, bool isMock)
	{
		for (int i = 0; i < volleySize; i++)
		{
			Fire(position, ref forward, fireDelay, projectileId, serverFrame, onHit, isMock);
			yield return new WaitForSeconds(volleyDelay);
			projectileId++;
		}
	}

	public void Fire(Vector3 position, ref Vector3 forward, float fireDelay, int projectileId, int serverFrame, Action<HitInfo> onHit, bool isMock = false)
	{
		if (!isMock)
		{
			AdjustLaunchPositionAndForwardIfInWall(ref position, ref forward);
		}
		FireInternal(position, ref forward, fireDelay, projectileId, serverFrame, onHit, isMock);
	}

	protected virtual void FireInternal(Vector3 position, ref Vector3 forward, float fireDelay, int projectileId, int serverFrame, Action<HitInfo> onHit, bool isMock = false)
	{
		if (_projectilePrefab == null)
		{
			FireRayCast(position, ref forward, serverFrame, onHit);
		}
		else
		{
			FireProjectile(position, ref forward, fireDelay, projectileId, serverFrame, onHit, isMock);
		}
	}

	protected virtual void FireProjectile(Vector3 position, ref Vector3 forward, float fireDelay, int projectileId, int serverFrame, Action<HitInfo> onHit, bool isMock = false)
	{
		if (_muzzles.Length > 1)
		{
			position += _muzzles[_muzzleIndex].position - base.transform.position;
		}
		Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up);
		SmartPool.Spawn(GetProjectilePrefab(), position, rotation).Launch(GenerateLaunchDetails(projectileId, serverFrame), onHit, isMock);
		this._fired?.Invoke(position, _muzzleIndex);
		if (_projectileFakes != null && _projectileFakes.Length != 0)
		{
			Transform[] projectileFakes = _projectileFakes;
			for (int i = 0; i < projectileFakes.Length; i++)
			{
				projectileFakes[i].gameObject.SetActive(value: false);
			}
			StartCoroutine(SpawnInNewFake(fireDelay));
		}
		if (_muzzles.Length > 1)
		{
			_muzzleIndex++;
			if (_muzzleIndex >= _muzzles.Length)
			{
				_muzzleIndex = 0;
			}
		}
	}

	protected virtual Projectile GetProjectilePrefab()
	{
		return _projectilePrefab;
	}

	protected IEnumerator SpawnInNewFake(float fireDelay)
	{
		float fFireDelay = fireDelay * Time.fixedDeltaTime;
		yield return new WaitForSeconds(fFireDelay * 0.5f);
		Transform fake = _projectileFakes[_muzzleIndex];
		fake.gameObject.SetActive(value: true);
		Vector3 start = Vector3.zero;
		Vector3 end = Vector3.one;
		float step = 1f / (fFireDelay * 0.4f);
		for (float t = 0f; t <= 1f; t += Time.deltaTime * step)
		{
			fake.localScale = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t));
			yield return null;
		}
	}

	protected virtual void FireRayCast(Vector3 position, ref Vector3 forward, int serverFrame, Action<HitInfo> onHit)
	{
		if (_muzzles.Length > 1)
		{
			position += _muzzles[_muzzleIndex].position - base.transform.position;
		}
		if (Physics.Raycast(position, forward, out var hitInfo, 250f, _hitableLayers))
		{
			HitInfo obj = GenerateHitInfo(hitInfo, serverFrame);
			onHit(obj);
			TrySpawnImpactEffect(hitInfo);
		}
		else
		{
			hitInfo.point = position + forward * 50f;
		}
		TrySpawnLaunchEffect(position, forward, hitInfo.point);
		this._fired?.Invoke(position, _muzzleIndex);
		if (_muzzles.Length > 1)
		{
			_muzzleIndex++;
			if (_muzzleIndex >= _muzzles.Length)
			{
				_muzzleIndex = 0;
			}
		}
	}

	protected virtual HitInfo GenerateHitInfo(RaycastHit hit, int serverFrame)
	{
		return new HitInfo
		{
			origin = AimOrigin,
			forward = base.transform.forward,
			launchServerFrame = serverFrame,
			point = hit.point,
			normal = hit.normal,
			collider = hit.collider,
			weaponProfile = Profile,
			weaponId = Profile.Id
		};
	}

	protected virtual void TrySpawnLaunchEffect(Vector3 position, Vector3 forward, Vector3 hitPoint)
	{
		if (_launchEffect != null && !BoltNetwork.IsServer)
		{
			GameObject effect = SmartPool.Spawn(_launchEffect, position, Quaternion.LookRotation(forward, Vector3.up));
			TryDisplayLaunchEffect(effect, forward);
		}
	}

	protected void TryDisplayLaunchEffect(GameObject effect, Vector3 forward)
	{
		IRaycastEffect component = effect.GetComponent<IRaycastEffect>();
		if (component != null)
		{
			float forwardVelocity = 0f;
			if (_playerMotor != null)
			{
				forwardVelocity = _playerMotor.transform.InverseTransformDirection(_playerMotor.State.Velocity).z;
			}
			component.Display(forward, forwardVelocity);
		}
	}

	protected virtual GameObject TrySpawnImpactEffect(RaycastHit hit)
	{
		Debug.Log("[LaunchPoint] Trying to spawn impact effect");
		if (_impactEffect != null && !BoltNetwork.IsServer)
		{
			Debug.Log($"[LaunchPoint] Spawning impact effect at {hit.point}");
			DebugExtension.DebugPoint(hit.point, Color.red, 0.25f, 5f);
			GameObject gameObject = SmartPool.Spawn(_impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
			if (Profile.Explosion.Explodes && !Profile.Charge.CanCharge)
			{
				gameObject.transform.localScale = Vector3.one * Profile.CalculateRangeAtTime(0f);
				Debug.Log($"[LaunchPoint] Spawning impact effect with scale: {gameObject.transform.localScale.x}");
			}
			return gameObject;
		}
		return null;
	}

	protected virtual Projectile.LaunchDetails GenerateLaunchDetails(int projectileId, int serverFrame)
	{
		Projectile.LaunchDetails result = new Projectile.LaunchDetails
		{
			projectileId = projectileId,
			launchServerFrame = serverFrame,
			speed = Profile.Projectile.Speed,
			playerController = GetComponentInParent<IPlayerController>(),
			weaponProfile = Profile,
			launchPointTransform = base.transform
		};
		if (Profile.Explosion.Explodes)
		{
			result.effectRadius = Profile.Explosion.Range;
		}
		result.range = Profile.Range;
		return result;
	}

	protected void AdjustLaunchPositionAndForwardIfInWall(ref Vector3 adjustedOrigin, ref Vector3 adjustedForward)
	{
		Vector3 position = _aimPointHandler.transform.position;
		Vector3 normalized = (base.transform.position - position).normalized;
		if (PathIsBlockedByGround(position, base.transform.position, out var hit))
		{
			adjustedOrigin = position + normalized * (hit.distance - Radius - 0.01f);
			adjustedForward = normalized;
		}
	}

	protected bool IsInWall()
	{
		return PathIsBlockedByGround(_aimPointHandler.transform.position, base.transform.position);
	}

	protected bool IsInWall(out RaycastHit hit)
	{
		return PathIsBlockedByGround(_aimPointHandler.transform.position, base.transform.position, out hit);
	}

	protected bool PathIsBlockedByGround(Vector3 origin, Vector3 point)
	{
		Vector3 normalized = (point - origin).normalized;
		float distance = Vector3.Distance(origin, point);
		Ray ray = new Ray(origin, normalized);
		return CastAgainstGround(ray, distance);
	}

	protected bool PathIsBlockedByGround(Vector3 origin, Vector3 point, out RaycastHit hit)
	{
		Vector3 normalized = (point - origin).normalized;
		float distance = Vector3.Distance(origin, point);
		Ray ray = new Ray(origin, normalized);
		return CastAgainstGround(ray, distance, out hit);
	}

	protected virtual bool CastAgainstGround(Ray ray, float distance, out RaycastHit hit)
	{
		DebugExtension.DebugCapsule(ray.origin, ray.origin + ray.direction * distance, Color.red, Radius, 2f);
		return Physics.SphereCast(ray, Radius, out hit, distance, LayerMaskConfig.GroundLayers);
	}

	protected virtual bool CastAgainstGround(Ray ray, float distance)
	{
		DebugExtension.DebugCapsule(ray.origin, ray.origin + ray.direction * distance, Color.red, Radius, 2f);
		return Physics.SphereCast(ray, Radius, distance, LayerMaskConfig.GroundLayers);
	}
}
