using System;
using System.Collections;
using UnityEngine;

public class SpawnedEntityStickyExplosion<TState> : SpawnedEntity<TState> where TState : IStickySpawnedEntity
{
	[SerializeField]
	private GameObject _explosionEffect;

	[SerializeField]
	private Renderer _renderer;

	[SerializeField]
	private AudioSource _audio;

	[SerializeField]
	private GameObject[] _disableOnExplode;

	protected Vector3 _localPositionOffset;

	protected Quaternion _localRotationOffset;

	private Action<HitInfo> _onHit;

	private float _timer;

	private bool _parentSet;

	protected override void Start()
	{
		base.Start();
		_audio.Stop();
	}

	public override void Attached()
	{
		if (!base.entity.isOwner)
		{
			base.state.AddCallback("IsExploded", IsExplodedUpdated);
			base.state.AddCallback("FollowTarget", SetFollowTarget);
			base.state.AddCallback("PositionOffset", SetLocalPositionOffset);
			base.state.AddCallback("RotationOffset", SetLocalRotationOffset);
		}
		else
		{
			StartCoroutine(WaitForWeaponProfile());
		}
	}

	private IEnumerator WaitForWeaponProfile()
	{
		yield return new WaitUntil(() => base._weaponProfile != null);
		TState val = base.state;
		val.Range = base._weaponProfile.Explosion.Range;
	}

	public void StickToPlayer(BoltEntity player)
	{
		base.transform.SetParent(player.transform, worldPositionStays: true);
		TState val = base.state;
		val.FollowTarget = player;
		val = base.state;
		val.PositionOffset = base.transform.localPosition;
		val = base.state;
		val.RotationOffset = base.transform.localRotation;
	}

	public void ListenForExplosion(Action<HitInfo> onHit)
	{
		_onHit = onHit;
	}

	private void SetFollowTarget()
	{
		if (base.state.FollowTarget != null)
		{
			base.transform.SetParent(base.state.FollowTarget.transform, worldPositionStays: true);
			base.transform.localPosition = _localPositionOffset;
			base.transform.localRotation = _localRotationOffset;
			_parentSet = true;
		}
	}

	private void SetLocalPositionOffset()
	{
		_localPositionOffset = base.state.PositionOffset;
		if (_parentSet)
		{
			base.transform.localPosition = _localPositionOffset;
		}
	}

	private void SetLocalRotationOffset()
	{
		_localRotationOffset = base.state.RotationOffset;
		if (_parentSet)
		{
			base.transform.localRotation = _localRotationOffset;
		}
	}

	private void Update()
	{
		if (base.entity.isOwner)
		{
			_timer += Time.deltaTime;
			if (_timer >= base._weaponProfile.SpawnedEntity.Duration)
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

	private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
	{
		Vector3 vector = point - pivot;
		vector = rotation * vector;
		point = vector + pivot;
		return point;
	}

	protected void Explode(HitInfo hitInfo)
	{
		if (base.state.IsExploded)
		{
			return;
		}
		TState val = base.state;
		val.IsExploded = true;
		foreach (HitInfo item in AreaEffectCalculator.GetAffectableInRange(base._weaponProfile.Explosion.Range, hitInfo))
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
		yield return new WaitForSeconds(1f);
		BoltNetwork.Destroy(base.gameObject);
	}

	private void IsExplodedUpdated()
	{
		if (!base.state.IsExploded)
		{
			return;
		}
		_renderer.enabled = false;
		if (_disableOnExplode != null && _disableOnExplode.Length != 0)
		{
			GameObject[] disableOnExplode = _disableOnExplode;
			for (int i = 0; i < disableOnExplode.Length; i++)
			{
				disableOnExplode[i].SetActive(value: false);
			}
		}
		StartCoroutine(ExplodeAfterRangeSet());
	}

	private IEnumerator ExplodeAfterRangeSet()
	{
		yield return new WaitUntil(() => base.state.Range > 0f);
		SmartPool.Spawn(_explosionEffect, base.transform.position, base.transform.rotation).transform.localScale = Vector3.one * base.state.Range;
	}
}
