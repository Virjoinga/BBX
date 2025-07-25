using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemButton : MonoBehaviour
{
	[SerializeField]
	private Button _button;

	[SerializeField]
	private CurrencyType _currencyType;

	[SerializeField]
	private TextMeshProUGUI _currencyAmount;

	[SerializeField]
	private Color _canAffordColor = Color.black;

	[SerializeField]
	private Color _canNotAffordColor = Color.red;

	public CurrencyType Currency => _currencyType;

	private event Action<BuyItemButton> _onClicked;

	public event Action<BuyItemButton> OnClicked
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

	public void SetCurrencyCost(int cost)
	{
		_currencyAmount.text = cost.ToString();
	}

	public void UpdateCanAfford(bool canAfford)
	{
		_currencyAmount.color = (canAfford ? _canAffordColor : _canNotAffordColor);
	}
}
