using System.Collections.Generic;
using BSCore;
using UnityEngine;
using Zenject;

public class ItemShop : MonoBehaviour
{
	[Inject]
	private StoreManager _storeManager;

	[Inject]
	private InventoryManager _inventoryManager;

	[SerializeField]
	private string _storeId;

	[SerializeField]
	private GameObject _loadingOverlay;

	[SerializeField]
	private PurchaseItemPopup _purchaseItemPopup;

	[SerializeField]
	private List<ItemShopItem> _shopItems;

	private void Start()
	{
		_loadingOverlay.SetActive(value: true);
		_inventoryManager.CurrencyFetched += UpdateStoreDisplay;
		_inventoryManager.InventoryFetched += UpdateStoreDisplay;
		_storeManager.Fetch(_storeId, PopulateStore, FailedToFetchStore);
	}

	private void OnDestroy()
	{
		_inventoryManager.CurrencyFetched -= UpdateStoreDisplay;
		_inventoryManager.InventoryFetched -= UpdateStoreDisplay;
	}

	private void FailedToFetchStore(FailureReasons failureReason)
	{
		Debug.LogError($"[ItemShop] Failed to fetch store {failureReason}");
		_loadingOverlay.SetActive(value: false);
	}

	private void PopulateStore(StoreData storeData)
	{
		foreach (ItemShopItem shopItem in _shopItems)
		{
			shopItem.Clear();
			shopItem.OnClicked -= OnShopItemClick;
			shopItem.gameObject.SetActive(value: false);
		}
		int num = 0;
		foreach (StoreItem item in storeData.Items)
		{
			ItemShopItem itemShopItem = _shopItems[num];
			if (itemShopItem == null)
			{
				Debug.LogError($"[ItemShop] Unable to get shop item for profile at Index {num}");
				return;
			}
			bool ownsItem = _inventoryManager.OwnsItem(item.Profile);
			itemShopItem.Populate(item, ownsItem);
			itemShopItem.OnClicked += OnShopItemClick;
			itemShopItem.gameObject.SetActive(value: true);
			num++;
		}
		_loadingOverlay.SetActive(value: false);
	}

	private void UpdateStoreDisplay()
	{
		foreach (ItemShopItem shopItem in _shopItems)
		{
			if (shopItem.IsPopulated)
			{
				bool ownsItem = _inventoryManager.OwnsItem(shopItem.StoreItem.Profile);
				shopItem.Populate(shopItem.StoreItem, ownsItem);
			}
		}
	}

	private void OnShopItemClick(ItemShopItem shopItem)
	{
		_purchaseItemPopup.Show(shopItem.StoreItem, _storeId);
	}
}
