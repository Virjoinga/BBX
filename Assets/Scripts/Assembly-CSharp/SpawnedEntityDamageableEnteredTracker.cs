using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedEntityDamageableEnteredTracker<TState> : SpawnedEntity<TState> where TState : ISpawnedEntity
{
	[SerializeField]
	private Collider _collider;

	protected Dictionary<IDamageable, bool> _trackedDamageablesAffected = new Dictionary<IDamageable, bool>();

	protected WeaponProfile.EffectData[] _effectDatas;

	protected bool _shouldDealDamage;

	protected string _weaponId;

	protected virtual void Reset()
	{
		_collider = GetComponent<Collider>();
	}

	protected override void Start()
	{
		base.Start();
		if (base.entity.isOwner)
		{
			_weaponId = base._weaponProfile.Id;
			_effectDatas = base._weaponProfile.Effects;
			StartCoroutine(LifetimeRoutine(base._weaponProfile.SpawnedEntity.Duration));
		}
	}

	private IEnumerator LifetimeRoutine(float lifetime)
	{
		_shouldDealDamage = true;
		yield return new WaitForSeconds(lifetime);
		_shouldDealDamage = false;
		_collider.enabled = false;
		BoltNetwork.Destroy(base.gameObject);
	}

	private void OnTriggerStay(Collider other)
	{
		if (base.entity.isAttached && base.entity.isOwner)
		{
			TryAddTrackedDamageable(other);
		}
	}

	private void TryAddTrackedDamageable(Collider collider)
	{
		IDamageable componentInParent = collider.gameObject.GetComponentInParent<IDamageable>();
		if (componentInParent != null && !_trackedDamageablesAffected.ContainsKey(componentInParent) && (!componentInParent.entity.TryFindState<IPlayerState>(out var playerState) || (!playerState.IsShielded && (!base._ownerIsPlayer || playerState.Team != base._ownerPlayerState.Team || !(componentInParent.entity != base._ownerEntity)))))
		{
			_trackedDamageablesAffected.Add(componentInParent, value: false);
		}
	}
}
