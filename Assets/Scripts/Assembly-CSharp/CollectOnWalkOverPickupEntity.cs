using System;
using System.Collections;
using Bolt;
using UnityEngine;

public abstract class CollectOnWalkOverPickupEntity<TBasePickupState> : BasePickupEntity<TBasePickupState> where TBasePickupState : IBasePickupState, IState, IDisposable
{
	protected bool TryCollect(Collider other)
	{
		ICanPickup component = other.GetComponent<ICanPickup>();
		if (base.entity.isOwner)
		{
			return component.TryPickup(_pickupData, base.entity);
		}
		return component.CanPickup(_pickupData);
	}

	private void OnTriggerStay(Collider other)
	{
		if (base.IsActive && other.CompareTag("Player") && TryCollect(other))
		{
			if (base.entity.isOwner)
			{
				TBasePickupState val = base.state;
				val.ActiveState = 2;
				StartRefill();
			}
			else
			{
				_modelSpawnPoint.gameObject.SetActive(value: false);
				_particleEffect.gameObject.SetActive(value: false);
				LagCompensationCheck();
			}
		}
	}

	private void LagCompensationCheck()
	{
		StartCoroutine(LagCompensationCheckRoutine());
	}

	private IEnumerator LagCompensationCheckRoutine()
	{
		yield return new WaitForSeconds(0.5f);
		_modelSpawnPoint.gameObject.SetActive(base.IsActive);
		_particleEffect.gameObject.SetActive(base.IsActive);
	}
}
