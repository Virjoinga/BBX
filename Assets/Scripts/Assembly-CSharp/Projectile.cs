using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public struct LaunchDetails
	{
		public int projectileId;

		public int launchServerFrame;

		public float scale;

		public float speed;

		public float range;

		public float radius;

		public float effectRadius;

		public float chargeTime;

		public IPlayerController playerController;

		public WeaponProfile weaponProfile;

		public Transform launchPointTransform;

		public Vector3 aimTarget;

		public bool isAimOriginOverriden;

		public Vector3 originOverride;

		public Vector3 forwardOverride;
	}

	public enum MovementType
	{
		Linear = 0,
		Arcing = 1,
		Homing = 2
	}

	[SerializeField]
	private GameObject _launchEffectPrefab;

	[SerializeField]
	protected GameObject _impactEffectPrefab;

	[SerializeField]
	protected bool _isHoming;

	[SerializeField]
	protected float _radius = 0.1f;

	[SerializeField]
	private AudioSource _projectileAudio;

	[SerializeField]
	private ParticleSystem _particleSystem;

	[SerializeField]
	private Material _allyMaterial;

	[SerializeField]
	private float _maxLifetime = 10f;

	[SerializeField]
	private bool _collidesWithTeammates = true;

	[SerializeField]
	private bool _collidesWithEnemies = true;

	[SerializeField]
	private bool _updateForward = true;

	protected bool _hasProcessedImpact;

	protected float _lifetime;

	protected float _speed;

	protected Vector3 _lastPosition;

	private Vector3 _lateralVelocityOnLaunch;

	private float _verticalSpeedOnLaunch;

	protected Action<HitInfo> _onHit;

	protected LaunchDetails _details;

	protected Vector3 _originOnLaunch;

	protected Vector3 _forwardOnLaunch;

	private Renderer[] _renderers;

	private Material _standardMaterial;

	private int _frameOnLaunch;

	private int _lagFrameCount;

	private bool _compensateForLag;

	public float Radius => _radius;

	protected virtual void Awake()
	{
		_renderers = GetComponentsInChildren<Renderer>();
		if (_renderers != null && _renderers.Length != 0)
		{
			_standardMaterial = _renderers[0].material;
		}
	}

	protected virtual void Start()
	{
	}

	protected virtual void OnEnable()
	{
		if (_projectileAudio != null)
		{
			_projectileAudio.Play();
		}
		if (_particleSystem != null)
		{
			_particleSystem.Play();
		}
	}

	protected virtual void OnDisable()
	{
		_onHit = null;
		if (_projectileAudio != null)
		{
			_projectileAudio.Stop();
		}
		if (_particleSystem != null)
		{
			_particleSystem.Stop();
		}
	}

	public virtual void Launch(LaunchDetails details, Action<HitInfo> onHit, bool isMock = false)
	{
		_lagFrameCount = 0;
		_compensateForLag = isMock;
		_details = details;
		_onHit = onHit;
		_hasProcessedImpact = false;
		_lifetime = 0f;
		_originOnLaunch = (_details.isAimOriginOverriden ? _details.originOverride : base.transform.position);
		_forwardOnLaunch = (_details.isAimOriginOverriden ? _details.forwardOverride : base.transform.forward);
		if (_details.weaponProfile.Projectile.MovementType == MovementType.Arcing)
		{
			_lateralVelocityOnLaunch = Vector3.ProjectOnPlane(_forwardOnLaunch * _details.speed, Vector3.up);
			_verticalSpeedOnLaunch = (_forwardOnLaunch * _details.speed).y;
		}
		if (_details.scale > 0f)
		{
			base.transform.localScale = Vector3.one * _details.scale;
		}
		if (_launchEffectPrefab != null && !BoltNetwork.IsServer)
		{
			SmartPool.Spawn(_launchEffectPrefab, base.transform.position, base.transform.rotation);
		}
		_speed = _details.speed;
		if (_details.radius > 0f)
		{
			_radius = _details.radius;
		}
		_lastPosition = base.transform.position;
		if (_allyMaterial != null && !BoltNetwork.IsServer)
		{
			if (details.playerController.state.Team > -1 && details.playerController.state.Team == PlayerController.LocalPlayer.state.Team)
			{
				SetMaterial(_allyMaterial);
			}
			else
			{
				SetMaterial(_standardMaterial);
			}
		}
	}

	private void SetMaterial(Material material)
	{
		if (_renderers != null && _renderers.Length != 0)
		{
			Renderer[] renderers = _renderers;
			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].material = material;
			}
		}
	}

	private void OnDrawGizmos()
	{
		DebugExtension.DebugWireSphere(base.transform.position, Color.green, _radius);
	}

	protected virtual void FixedUpdate()
	{
		if (_hasProcessedImpact)
		{
			return;
		}
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = _speed * fixedDeltaTime;
		if (_compensateForLag)
		{
			if (_lagFrameCount < _details.launchServerFrame)
			{
				num *= 2f;
				_lagFrameCount++;
			}
			else
			{
				_compensateForLag = false;
			}
		}
		_lifetime += fixedDeltaTime;
		if (_lifetime >= _maxLifetime)
		{
			RaycastHit hit = new RaycastHit
			{
				point = base.transform.position,
				normal = -base.transform.forward
			};
			Impact(hit);
			return;
		}
		Move(num);
		Ray ray = new Ray(_lastPosition, base.transform.position - _lastPosition);
		if (CheckForImpact(ray, out var hit2))
		{
			Impact(hit2);
		}
		if (_updateForward)
		{
			base.transform.forward = ray.direction;
		}
		_lastPosition = base.transform.position;
	}

	protected virtual void Move(float speed)
	{
		switch (_details.weaponProfile.Projectile.MovementType)
		{
		case MovementType.Arcing:
			MoveArcing(speed);
			break;
		case MovementType.Homing:
			MoveHoming(speed);
			break;
		default:
			MoveLinear(speed);
			break;
		}
	}

	private void MoveLinear(float speed)
	{
		base.transform.position = Vector3.MoveTowards(base.transform.position, base.transform.position + base.transform.forward, speed);
	}

	private void MoveArcing(float speed)
	{
		Vector3 position = Vector3.MoveTowards(base.transform.position, base.transform.position + _lateralVelocityOnLaunch, speed);
		position.y = _originOnLaunch.y + _verticalSpeedOnLaunch * _lifetime + 0.5f * Physics.gravity.y * _details.weaponProfile.Projectile.ArcMultiplier * _lifetime * _lifetime;
		base.transform.position = position;
	}

	private void MoveHoming(float speed)
	{
		if (_details.playerController != null && _details.playerController.AimPoint != null)
		{
			Quaternion to = Quaternion.LookRotation((_details.playerController.AimPoint.position - base.transform.position).normalized, Vector3.up);
			float turnSpeed = _details.weaponProfile.Projectile.TurnSpeed;
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, turnSpeed * Time.deltaTime);
		}
		MoveLinear(speed);
	}

	protected virtual bool CheckForImpact(Ray ray, out RaycastHit hit)
	{
		Debug.DrawLine(base.transform.position, _lastPosition, Color.green);
		if (Physics.SphereCast(ray, _radius, out hit, Vector3.Distance(base.transform.position, _lastPosition), LayerMaskConfig.HitableLayers) && CanHit(hit))
		{
			hit.point = ray.origin + ray.direction * hit.distance;
			return true;
		}
		return false;
	}

	protected bool CanHit(RaycastHit hit)
	{
		PlayerController componentInParent = hit.collider.GetComponentInParent<PlayerController>();
		if (componentInParent != null)
		{
			if (!_collidesWithTeammates && componentInParent.Team == _details.playerController.state.Team)
			{
				return false;
			}
			if (!_collidesWithEnemies && componentInParent.Team != _details.playerController.state.Team)
			{
				return false;
			}
		}
		return hit.collider != _details.playerController.HurtCollider;
	}

	protected virtual void Impact(Vector3 position, Vector3 normal)
	{
		if (!_hasProcessedImpact)
		{
			TrySpawnImpactEffect(position, normal);
			_hasProcessedImpact = true;
		}
	}

	protected virtual void Impact(RaycastHit hit)
	{
		if (!_hasProcessedImpact)
		{
			Impact(hit.point, hit.normal);
			_onHit?.Invoke(GenerateHitInfo(hit));
			_onHit = null;
			StopAllCoroutines();
			SmartPool.Despawn(base.gameObject);
		}
	}

	protected virtual GameObject TrySpawnImpactEffect(Vector3 position, Vector3 normal)
	{
		if (_impactEffectPrefab != null && !BoltNetwork.IsServer)
		{
			GameObject gameObject = SmartPool.Spawn(_impactEffectPrefab, position, Quaternion.FromToRotation(Vector3.up, normal));
			if (_details.weaponProfile != null && _details.weaponProfile.Explosion.Explodes)
			{
				gameObject.transform.localScale = Vector3.one * _details.weaponProfile.Explosion.Range;
			}
			return gameObject;
		}
		return null;
	}

	protected virtual HitInfo GenerateHitInfo(RaycastHit hit)
	{
		return new HitInfo
		{
			hitType = HitType.Projectile,
			projectileId = _details.projectileId,
			launchServerFrame = _details.launchServerFrame,
			origin = _originOnLaunch,
			forward = _forwardOnLaunch,
			forwardOnHit = base.transform.forward,
			point = hit.point,
			normal = hit.normal,
			projectileRotation = base.transform.rotation,
			collider = hit.collider,
			chargeTime = _details.chargeTime,
			weaponProfile = _details.weaponProfile,
			weaponId = _details.weaponProfile.Id
		};
	}
}
