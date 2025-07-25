using System.Collections.Generic;
using BSCore;
using UnityEngine;
using Zenject;

public class FetchInventoryOnAwake : MonoBehaviour
{
	[Inject]
	private InventoryManager _inventoryManager;

	private void Awake()
	{
		_inventoryManager.Fetch(OnSuccess, OnFailure);
	}

	private void OnSuccess(List<InventoryItem> inventory, IDictionary<CurrencyType, int> currencies)
	{
		Object.Destroy(this);
	}

	private void OnFailure(FailureReasons reason)
	{
		Debug.LogError($"[FetchInventoryOnAwake] Failed to fetch inventory: {reason}");
		Object.Destroy(this);
	}
}
