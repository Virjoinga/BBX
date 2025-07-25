using BSCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PurchaseItemPopup : MonoBehaviour
{
	[Inject]
	private InventoryManager _inventoryManager;

	[Inject]
	private PurchasingManager _purchasingManager;

	[SerializeField]
	protected TextMeshProUGUI _displayNameText;

	[SerializeField]
	private TextMeshProUGUI _descriptionText;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private BuyItemButton _softCurrencyBuyButton;

	[SerializeField]
	private BuyItemButton _hardCurrencyBuyButton;

	[SerializeField]
	private Button _closeButton;

	[SerializeField]
	private GameObject _purchasingOverlay;

	private StoreItem _storeItem;

	private string _storeId;

	private void Awake()
	{
		_closeButton.onClick.AddListener(Close);
		_softCurrencyBuyButton.OnClicked += TryBuyItem;
		_hardCurrencyBuyButton.OnClicked += TryBuyItem;
	}

	public void Show(StoreItem storeItem, string storeId)
	{
		_storeItem = storeItem;
		_storeId = storeId;
		_displayNameText.text = _storeItem.Profile.Name;
		_descriptionText.text = _storeItem.Profile.Description;
		_icon.overrideSprite = _storeItem.Profile.Icon;
		_inventoryManager.CurrencyFetched += OnCurrencyFetched;
		if (_storeItem.HasCurrencyTypeCost(CurrencyType.S1))
		{
			_softCurrencyBuyButton.SetCurrencyCost(_storeItem.OverrideCosts[CurrencyType.S1]);
			_softCurrencyBuyButton.UpdateCanAfford(_inventoryManager.CanAfford(CurrencyType.S1, _storeItem.OverrideCosts[CurrencyType.S1]));
			_softCurrencyBuyButton.gameObject.SetActive(value: true);
		}
		else
		{
			_softCurrencyBuyButton.gameObject.SetActive(value: false);
		}
		if (_storeItem.HasCurrencyTypeCost(CurrencyType.H1))
		{
			_hardCurrencyBuyButton.SetCurrencyCost(_storeItem.OverrideCosts[CurrencyType.H1]);
			_hardCurrencyBuyButton.UpdateCanAfford(_inventoryManager.CanAfford(CurrencyType.H1, _storeItem.OverrideCosts[CurrencyType.H1]));
			_hardCurrencyBuyButton.gameObject.SetActive(value: true);
		}
		else
		{
			_hardCurrencyBuyButton.gameObject.SetActive(value: false);
		}
		_purchasingOverlay.SetActive(value: false);
		base.gameObject.SetActive(value: true);
	}

	private void OnCurrencyFetched()
	{
		if (_storeItem.Profile != null)
		{
			if (_storeItem.HasCurrencyTypeCost(CurrencyType.S1))
			{
				_softCurrencyBuyButton.UpdateCanAfford(_inventoryManager.CanAfford(CurrencyType.S1, _storeItem.OverrideCosts[CurrencyType.S1]));
			}
			if (_storeItem.HasCurrencyTypeCost(CurrencyType.H1))
			{
				_hardCurrencyBuyButton.UpdateCanAfford(_inventoryManager.CanAfford(CurrencyType.H1, _storeItem.OverrideCosts[CurrencyType.H1]));
			}
		}
	}

	private void TryBuyItem(BuyItemButton buyButton)
	{
		if (_storeItem.Profile != null)
		{
			int num = _storeItem.OverrideCosts[buyButton.Currency];
			if (!_inventoryManager.CanAfford(buyButton.Currency, num))
			{
				_ = buyButton.Currency;
				return;
			}
			_purchasingOverlay.SetActive(value: true);
			_inventoryManager.InventoryFetched += Close;
			_inventoryManager.InventoryFetchFailed += Close;
			_purchasingManager.PurchaseItem(_storeItem.Profile, buyButton.Currency, num, _storeId, OnPurchaseSuccess, OnPurchaseFailure);
		}
	}

	private void OnPurchaseSuccess()
	{
		Debug.Log("[PurchaseItemPopup] Purchased " + _storeItem.Profile.Id + " Succesfully");
	}

	private void OnPurchaseFailure(FailureReasons reason)
	{
		Debug.LogError($"[PurchaseItemPopup] Failed to purchase item {_storeItem.Profile.Id} - {reason}");
		_purchasingOverlay.SetActive(value: false);
	}

	private void Close()
	{
		_inventoryManager.InventoryFetched -= Close;
		_inventoryManager.InventoryFetchFailed -= Close;
		_inventoryManager.CurrencyFetched -= OnCurrencyFetched;
		base.gameObject.SetActive(value: false);
	}
}
