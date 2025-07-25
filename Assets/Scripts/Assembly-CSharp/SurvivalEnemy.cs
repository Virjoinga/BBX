using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using Constants;
using UdpKit;
using UnityEngine;

public class SurvivalEnemy : EntityEventListener<ISurvivalEnemy>, IDamageable, IStatusAffectable
{
	public class SurvivalEnemyAttachToken : IProtocolToken
	{
		public string name;

		public SurvivalEnemyAttachToken()
		{
		}

		public SurvivalEnemyAttachToken(string name)
		{
			this.name = name;
		}

		public void Read(UdpPacket packet)
		{
			name = packet.ReadString();
		}

		public void Write(UdpPacket packet)
		{
			packet.WriteString(name);
		}
	}

	[Serializable]
	private struct EnemySettingsByName
	{
		public string Name;

		public float Scale;

		public Material Material;
	}

	public struct SurvivalEnemyDamageInfo
	{
		public SurvivalEnemy Enemy;

		public float Damage;

		public BoltEntity Attacker;
	}

	[SerializeField]
	private SkinnedMeshRenderer _meshRenderer;

	[SerializeField]
	private SurvivalEnemyAI _survivalEnemyAI;

	[SerializeField]
	private ParticleSystem _damageEffect;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private ObjectFader _fader;

	[SerializeField]
	private AudioSource _audioSource;

	[SerializeField]
	private AudioClip _deathClip;

	[SerializeField]
	private AudioClip _headshotClip;

	[SerializeField]
	private EnemySettingsByName[] _settingsByName;

	private Rigidbody _rigidBody;

	private List<PlayerController> _trackedPlayers = new List<PlayerController>();

	private Collider[] _colliders;

	private float _speedIncrease;

	private float _speedDecrease;

	private float _knockbackForce = 300f;

	private float _knockbackHeight = 1f;

	public SurvivalEnemyConfigData Config { get; private set; }

	public int Team => -1;

	public Match.StatusType StatusFlags { get; set; }

	public bool CanGetStatusApplied => base.state.Damageable.Health > 0f;

	public bool Stunned
	{
		get
		{
			return base.state.Stunned;
		}
		set
		{
			bool stunned = base.state.Stunned;
			base.state.Stunned = value;
			if (!stunned && base.state.Stunned)
			{
				_survivalEnemyAI.IsPaused = true;
			}
			else if (stunned && !base.state.Stunned)
			{
				_survivalEnemyAI.IsPaused = false;
			}
		}
	}

	public bool ForcedMovement { get; set; }

	public float SpeedIncrease
	{
		get
		{
			return _speedIncrease;
		}
		set
		{
			_speedIncrease = value;
			_survivalEnemyAI.MaxSpeed = Config.Speed * (1f + _speedIncrease + _speedDecrease);
		}
	}

	public float SpeedDecrease
	{
		get
		{
			return _speedDecrease;
		}
		set
		{
			_speedDecrease = value;
			_survivalEnemyAI.MaxSpeed = Config.Speed * (1f + _speedIncrease + _speedDecrease);
		}
	}

	public float DamageModifier { get; set; }

	public float DamageShield { get; set; }

	public float Size { get; set; }

	public bool PreventJump { get; set; }

	public Collider HurtCollider
	{
		get
		{
			if (_colliders == null || _colliders.Length == 0)
			{
				return null;
			}
			return _colliders[0];
		}
	}

	public StatusEffectController StatusEffectController { get; private set; }

	public Transform DamageNumberSpawn => base.transform;

	public float MeleeDamageModifier { get; set; }

	private static event Action<SurvivalEnemy, BoltEntity> _died;

	public static event Action<SurvivalEnemy, BoltEntity> Died
	{
		add
		{
			_died += value;
		}
		remove
		{
			_died -= value;
		}
	}

	private static event Action<SurvivalEnemyDamageInfo> _damaged;

	public static event Action<SurvivalEnemyDamageInfo> Damaged
	{
		add
		{
			_damaged += value;
		}
		remove
		{
			_damaged -= value;
		}
	}

	private void Awake()
	{
		_rigidBody = GetComponent<Rigidbody>();
		StatusEffectController = GetComponent<StatusEffectController>();
		_colliders = GetComponentsInChildren<Collider>();
		_fader.SetFadeLevel(0f);
		StartCoroutine(FadeInNextFrame());
	}

	private IEnumerator FadeInNextFrame()
	{
		yield return null;
		_fader.SetFadeLevel(1f);
	}

	public override void Attached()
	{
		base.state.SetTransforms(base.state.Transform, base.transform);
		base.state.SetAnimator(_animator);
		SurvivalEnemyAttachToken survivalEnemyAttachToken = base.entity.attachToken as SurvivalEnemyAttachToken;
		SetByName(survivalEnemyAttachToken.name);
		if (!base.entity.isOwner)
		{
			_survivalEnemyAI.Disable();
			ISurvivalEnemy survivalEnemy = base.state;
			survivalEnemy.OnDeathHeadshot = (Action)Delegate.Combine(survivalEnemy.OnDeathHeadshot, new Action(OnHeadshot));
			ISurvivalEnemy survivalEnemy2 = base.state;
			survivalEnemy2.OnDeathNormal = (Action)Delegate.Combine(survivalEnemy2.OnDeathNormal, new Action(OnNormalDeath));
			ISurvivalEnemy survivalEnemy3 = base.state;
			survivalEnemy3.OnDamaged = (Action)Delegate.Combine(survivalEnemy3.OnDamaged, new Action(OnDamaged));
			ISurvivalEnemy survivalEnemy4 = base.state;
			survivalEnemy4.OnFlung = (Action)Delegate.Combine(survivalEnemy4.OnFlung, new Action(OnFlung));
			ISurvivalEnemy survivalEnemy5 = base.state;
			survivalEnemy5.OnGetUp = (Action)Delegate.Combine(survivalEnemy5.OnGetUp, new Action(OnGetUp));
			PlayHugMeAudio();
		}
	}

	public bool IsAttacking(BoltEntity playerEntity)
	{
		return _trackedPlayers.Any((PlayerController p) => p.entity == playerEntity);
	}

	private void SetByName(string enemyName)
	{
		EnemySettingsByName enemySettingsByName = _settingsByName.Where((EnemySettingsByName mbn) => mbn.Name == enemyName).FirstOrDefault();
		if (enemySettingsByName.Material != null)
		{
			_meshRenderer.material = enemySettingsByName.Material;
			base.transform.localScale = Vector3.one * enemySettingsByName.Scale;
		}
	}

	private void LateUpdate()
	{
		if (!base.entity.isOwner)
		{
			return;
		}
		bool isAttacking = base.state.IsAttacking;
		base.state.IsAttacking = _animator.GetBool("IsAttacking");
		if (isAttacking || !base.state.IsAttacking)
		{
			return;
		}
		foreach (PlayerController trackedPlayer in _trackedPlayers)
		{
			PlayerController player = trackedPlayer;
			player.StatusEffectController.ApplyEffectWhile("survivalEnemy", GenerateHugEffectData(), base.entity, conditional);
			bool conditional()
			{
				if (this != null && base.entity != null && base.entity.isAttached && base.state.IsAttacking)
				{
					return _trackedPlayers.Contains(player);
				}
				return false;
			}
		}
	}

	private void FixedUpdate()
	{
		if (base.entity.isOwner)
		{
			base.state.Speed = _survivalEnemyAI.CurrentSpeed;
		}
	}

	public void SetConfig(SurvivalEnemyConfigData config)
	{
		Config = config;
		_survivalEnemyAI.Acceleration = Config.Acceleration;
		_survivalEnemyAI.AngularSpeed = Config.RotationSpeed;
		_survivalEnemyAI.MaxSpeed = Config.Speed;
	}

	public void Knockback(Vector3 direction)
	{
		StopAllCoroutines();
		StartCoroutine(KnockbackRoutine(direction));
	}

	private IEnumerator KnockbackRoutine(Vector3 direction)
	{
		_survivalEnemyAI.IsPaused = true;
		base.state.IsAttacking = false;
		base.state.Flung();
		_rigidBody.isKinematic = false;
		_rigidBody.useGravity = true;
		direction.y = _knockbackHeight;
		_rigidBody.AddForce(direction * _knockbackForce);
		yield return new WaitForSeconds(0.5f);
		yield return new WaitUntil(HitGround);
		float seconds = UnityEngine.Random.Range(1f, 2.5f);
		yield return new WaitForSeconds(seconds);
		base.state.GetUp();
		yield return new WaitForSeconds(1.1f);
		_rigidBody.useGravity = false;
		_rigidBody.velocity = Vector3.zero;
		_rigidBody.isKinematic = true;
		_survivalEnemyAI.IsPaused = false;
	}

	private bool HitGround()
	{
		Ray ray = new Ray(base.transform.position, Vector3.down);
		float num = 0.1f;
		DebugExtension.DebugArrow(ray.origin, ray.direction * num, Color.blue);
		if (Physics.Raycast(ray, num, LayerMaskConfig.GroundLayers))
		{
			return true;
		}
		return false;
	}

	private void OnFlung()
	{
		StartCoroutine(ClientFlingRoutine());
	}

	private IEnumerator ClientFlingRoutine()
	{
		_animator.SetTrigger("Knockback");
		yield return new WaitForSeconds(0.5f);
		yield return new WaitUntil(HitGround);
		_animator.SetTrigger("HitGround");
	}

	private void OnGetUp()
	{
		StartCoroutine(ClientGetUpRoutine());
	}

	private IEnumerator ClientGetUpRoutine()
	{
		int value = UnityEngine.Random.Range(1, 4);
		_animator.SetInteger("GetUpIndex", value);
		yield return new WaitForSeconds(0.5f);
		_animator.SetInteger("GetUpIndex", 0);
	}

	private WeaponProfile.EffectData GenerateHugEffectData()
	{
		return new WeaponProfile.EffectData(new WeaponProfile.WeaponProfileData.EffectProfileData
		{
			effectType = "Hugged",
			statusType = "Hugged",
			modifier = Config.Damage
		});
	}

	private void OnDamaged()
	{
		if (_damageEffect != null)
		{
			SmartPool.Spawn(_damageEffect.gameObject, base.transform.position + Vector3.up * 0.45f, Quaternion.identity);
		}
	}

	public void TakeDamage(HitInfo hitInfo, float damage, BoltEntity attacker)
	{
		TakeDamage(hitInfo, damage, attacker, hitInfo.effects);
	}

	public void TakeDamage(HitInfo hitInfo, float damage, BoltEntity attacker, WeaponProfile.EffectData[] effects)
	{
		if (base.state.Damageable.Health <= 0f)
		{
			return;
		}
		Debug.Log($"[SurvivalEnemy] {base.name} has taken {damage} damage from {attacker.name}");
		if (damage > base.state.Damageable.Health)
		{
			damage = base.state.Damageable.Health;
		}
		SurvivalEnemy._damaged?.Invoke(new SurvivalEnemyDamageInfo
		{
			Enemy = this,
			Damage = damage,
			Attacker = attacker
		});
		base.state.Damageable.Health = Mathf.Clamp(base.state.Damageable.Health - damage, 0f, base.state.Damageable.MaxHealth);
		base.state.Damaged();
		if (!base.state.IsAttacking)
		{
			_survivalEnemyAI.SetTarget(attacker.gameObject);
		}
		if (base.state.Damageable.Health == 0f)
		{
			if (UnityEngine.Random.Range(0f, 100f) > 80f)
			{
				base.state.DeathHeadshot();
			}
			else
			{
				base.state.DeathNormal();
			}
			SurvivalEnemy._died?.Invoke(this, attacker);
			Collider[] colliders = _colliders;
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].enabled = false;
			}
			_trackedPlayers.Clear();
			base.state.IsAttacking = false;
			_survivalEnemyAI.IsPaused = true;
			Invoke("DestroySelf", 1.5f);
		}
		else if (effects != null && effects.Length != 0)
		{
			foreach (WeaponProfile.EffectData effectData in effects)
			{
				StatusEffectController.TryApplyEffect(hitInfo.weaponId, effectData, attacker);
			}
		}
	}

	private IEnumerator FadeAfterDeath()
	{
		yield return new WaitForSeconds(1f);
	}

	public void DestroySelf()
	{
		BoltNetwork.Destroy(base.gameObject);
	}

	private void OnHeadshot()
	{
		StartCoroutine(OnDeath());
		_audioSource.Stop();
		_audioSource.clip = _headshotClip;
		_audioSource.Play();
	}

	private void OnNormalDeath()
	{
		StartCoroutine(OnDeath());
		_audioSource.Stop();
		_audioSource.clip = _deathClip;
		_audioSource.Play();
	}

	private IEnumerator OnDeath()
	{
		Collider[] colliders = _colliders;
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = false;
		}
		_trackedPlayers.Clear();
		yield return new WaitForSeconds(1f);
		for (float t = 0f; t <= 1f; t += Time.deltaTime * 2f)
		{
			_fader.SetFadeLevel(Mathf.SmoothStep(1f, 0f, t));
			yield return null;
		}
	}

	private void PlayHugMeAudio()
	{
		_audioSource.Play();
		Invoke("PlayHugMeAudio", UnityEngine.Random.Range(_audioSource.clip.length, _audioSource.clip.length + 10f));
	}

	private void OnTriggerEnter(Collider other)
	{
		if (base.entity.isOwner)
		{
			TryAddTrackedPlayer(other);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (base.entity.isOwner)
		{
			TryAddTrackedPlayer(other);
		}
	}

	private void TryAddTrackedPlayer(Collider collider)
	{
		PlayerController componentInParent = collider.gameObject.GetComponentInParent<PlayerController>();
		if (componentInParent != null && !_trackedPlayers.Contains(componentInParent))
		{
			_trackedPlayers.Add(componentInParent);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (base.entity.isOwner)
		{
			PlayerController componentInParent = other.gameObject.GetComponentInParent<PlayerController>();
			if (componentInParent != null)
			{
				_trackedPlayers.Remove(componentInParent);
			}
		}
	}

	public bool TryPickup(PickupData pickup)
	{
		return false;
	}

	public bool MockPickup(PickupData pickup)
	{
		return false;
	}
}
