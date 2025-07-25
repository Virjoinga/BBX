using System;
using System.Collections;
using UnityEngine;

public class BouncingProjectile : SpawnedEntity<ICannonBull>
{
	private const int HURTBOX_LAYER = 15;

	[SerializeField]
	private Rigidbody _rigidbody;

	[SerializeField]
	private GameObject _explosionEffect;

	[SerializeField]
	private Collider _collider;

	[SerializeField]
	private Renderer _renderer;

	[SerializeField]
	private GameObject _effects;

	[SerializeField]
	private float _force = 1000f;

	[SerializeField]
	private AudioSource _audio;

	private float _effectRadius;

	private Action<HitInfo> _onHit;

	private float _timer;

	private float _slowingDownTimer;

	protected override void Start()
	{
		base.Start();
		_audio.Stop();
	}

	public override void Attached()
	{
		base.state.SetTransforms(base.state.Transform, base.transform);
		if (!base.entity.isOwner)
		{
			_collider.enabled = false;
			base.state.AddCallback("IsExploded", IsExplodedUpdated);
		}
		else
		{
			StartCoroutine(WaitForWeaponProfile());
		}
	}

	private IEnumerator WaitForWeaponProfile()
	{
		yield return new WaitUntil(() => base._weaponProfile != null);
		base.state.Range = base._weaponProfile.Explosion.Range;
	}

	public void Launch(Vector3 direction, Action<HitInfo> onHit)
	{
		_effectRadius = base._weaponProfile.Explosion.Range;
		_onHit = onHit;
		base.transform.rotation = Quaternion.LookRotation(direction);
		_rigidbody.AddForce(direction * _force);
	}

	private void Update()
	{
		if (base.entity.isOwner)
		{
			if (_rigidbody.velocity.magnitude > 4f)
			{
				_slowingDownTimer = 0f;
				base.transform.LookAt(base.transform.position + _rigidbody.velocity);
			}
			else
			{
				_slowingDownTimer += Time.deltaTime;
			}
			_timer += Time.deltaTime;
			if (_timer >= base._weaponProfile.SpawnedEntity.Duration || _rigidbody.IsSleeping() || _slowingDownTimer > 0.5f)
			{
				HitInfo hitInfo = new HitInfo
				{
					point = base.transform.position,
					collider = null,
					weaponProfile = base._weaponProfile,
					weaponId = base._weaponProfile.Id
				};
				Explode(hitInfo);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (base.entity.isOwner && collision.collider.gameObject.layer == 15)
		{
			HitInfo hitInfo = new HitInfo
			{
				directHit = true,
				point = base.transform.position,
				normal = collision.contacts[0].normal,
				collider = collision.collider,
				weaponProfile = base._weaponProfile,
				weaponId = base._weaponProfile.Id
			};
			IDamageable componentInParent = collision.collider.gameObject.GetComponentInParent<IDamageable>();
			if (componentInParent != null)
			{
				hitInfo.hitEntity = componentInParent.entity;
			}
			Explode(hitInfo);
		}
	}

	private void Explode(HitInfo hitInfo)
	{
		if (base.state.IsExploded)
		{
			return;
		}
		base.state.IsExploded = true;
		foreach (HitInfo item in AreaEffectCalculator.GetAffectableInRange(_effectRadius, hitInfo))
		{
			OnHit(item);
		}
		StartCoroutine(DelayedDestroy());
	}

	private void OnHit(HitInfo hitInfo)
	{
		_audio.Play();
		_onHit?.Invoke(hitInfo);
	}

	private IEnumerator DelayedDestroy()
	{
		_collider.enabled = false;
		yield return new WaitForSeconds(1f);
		BoltNetwork.Destroy(base.gameObject);
	}

	private void IsExplodedUpdated()
	{
		if (base.state.IsExploded)
		{
			_renderer.enabled = false;
			_effects.SetActive(value: false);
			StartCoroutine(ExplodeAfterRangeSet());
		}
	}

	private IEnumerator ExplodeAfterRangeSet()
	{
		yield return new WaitUntil(() => base.state.Range > 0f);
		SmartPool.Spawn(_explosionEffect, base.transform.position, base.transform.rotation).transform.localScale = Vector3.one * base.state.Range;
	}
}
