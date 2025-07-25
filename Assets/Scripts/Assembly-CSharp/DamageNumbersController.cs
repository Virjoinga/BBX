using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DamageNumbersController : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private SmartPool _damageNumbersPool;

	[SerializeField]
	private float _lifetime = 2f;

	private List<DamageNumber> _activeDamageNumbers = new List<DamageNumber>();

	private void Start()
	{
		_signalBus.Subscribe<DamagableDamagedSignal>(OnDamagableDamaged);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<DamagableDamagedSignal>(OnDamagableDamaged);
	}

	private void OnDamagableDamaged(DamagableDamagedSignal signal)
	{
		if (!(signal.Victim == null) && (!PlayerController.HasLocalPlayer || !(signal.Attacker != PlayerController.LocalPlayer.entity)))
		{
			IDamageable componentInChildren = signal.Victim.GetComponentInChildren<IDamageable>();
			if (componentInChildren == null || componentInChildren.DamageNumberSpawn == null)
			{
				Debug.LogError("[DamageNumbersController] Unable to get damageable from Victim");
			}
			else
			{
				ShowDamage(componentInChildren.DamageNumberSpawn, signal.Damage);
			}
		}
	}

	public void ShowDamage(Transform followTransform, float damage)
	{
		DamageNumber component = _damageNumbersPool.SpawnItem().GetComponent<DamageNumber>();
		_activeDamageNumbers.Add(component);
		component.SetText(damage, followTransform);
		StartCoroutine(DespawnLifetime(component));
	}

	private void Update()
	{
		Sort();
	}

	private void Sort()
	{
		_activeDamageNumbers.Sort((DamageNumber x, DamageNumber y) => x.Depth.CompareTo(y.Depth));
		for (int num = 0; num < _activeDamageNumbers.Count; num++)
		{
			_activeDamageNumbers[num].transform.SetSiblingIndex(num);
		}
	}

	private IEnumerator DespawnLifetime(DamageNumber damageNumber)
	{
		yield return new WaitForSeconds(_lifetime);
		SmartPool.Despawn(damageNumber.gameObject);
		_activeDamageNumbers.Remove(damageNumber);
	}
}
