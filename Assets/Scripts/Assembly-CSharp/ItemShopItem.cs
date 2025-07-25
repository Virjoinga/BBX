using System;
using BSCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopItem : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _displayNameText;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private Button _button;

	[SerializeField]
	private GameObject _softCurrencyCostDisplay;

	[SerializeField]
	private GameObject _hardCurrencyCostDisplay;

	[SerializeField]
	private TextMeshProUGUI _softCurrencyCostText;

	[SerializeField]
	private TextMeshProUGUI _hardCurrencyCostText;

	[SerializeField]
	private GameObject _ownsItemOverlay;

	public StoreItem StoreItem { get; private set; }

	public bool IsPopulated { get; private set; }

	private event Action<ItemShopItem> _onClicked;

	public event Action<ItemShopItem> OnClicked
	{
		add
		{
			_onClicked += value;
		}
		remove
		{
			_onClicked -= value;
		}
	}

	private void Awake()
	{
		_button.onClick.AddListener(delegate
		{
			this._onClicked?.Invoke(this);
		});
	}

	public void Populate(StoreItem storeItem, bool ownsItem)
	{
		StoreItem = storeItem;
		_displayNameText.text = StoreItem.Profile.Name;
		_icon.overrideSprite = StoreItem.Profile.Icon;
		if (storeItem.HasCurrencyTypeCost(CurrencyType.S1))
		{
			_softCurrencyCostText.text = storeItem.OverrideCosts[CurrencyType.S1].ToString();
			_softCurrencyCostDisplay.SetActive(value: true);
		}
		else
		{
			_softCurrencyCostDisplay.SetActive(value: false);
		}
		if (storeItem.HasCurrencyTypeCost(CurrencyType.H1))
		{
			_hardCurrencyCostText.text = storeItem.OverrideCosts[CurrencyType.H1].ToString();
			_hardCurrencyCostDisplay.SetActive(value: true);
		}
		else
		{
			_hardCurrencyCostDisplay.SetActive(value: false);
		}
		_ownsItemOverlay.SetActive(ownsItem);
		_button.interactable = !ownsItem;
		IsPopulated = true;
	}

	public void Clear()
	{
		IsPopulated = false;
	}
}
