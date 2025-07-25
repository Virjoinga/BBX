using BSCore;
using TMPro;
using UnityEngine;
using Zenject;

public class CurrencyDisplay : MonoBehaviour
{
	[Inject]
	private InventoryManager _inventoryManager;

	[SerializeField]
	private CurrencyType _currencyType;

	[SerializeField]
	private TextMeshProUGUI _currencyText;

	private void Start()
	{
		_inventoryManager.CurrencyFetched += UpdateCurrencyDisplay;
		UpdateCurrencyDisplay();
	}

	private void OnDestroy()
	{
		_inventoryManager.CurrencyFetched -= UpdateCurrencyDisplay;
	}

	private void UpdateCurrencyDisplay()
	{
		int value = 0;
		_inventoryManager.Currencies.TryGetValue(_currencyType, out value);
		_currencyText.text = value.ToString();
	}
}
