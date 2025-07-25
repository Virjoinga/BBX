using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace BSCore
{
	public class InventoryManager
	{
		public class Factory : PlaceholderFactory<string, InventoryManager>
		{
		}

		private IInventoryService _inventoryService;

		private Dictionary<string, InventoryItem> _inventoryById = new Dictionary<string, InventoryItem>();

		private Dictionary<CurrencyType, int> _currencyByType = new Dictionary<CurrencyType, int>();

		private string _serviceId;

		public List<InventoryItem> Inventory => _inventoryById.Values.ToList();

		public IDictionary<CurrencyType, int> Currencies => new Dictionary<CurrencyType, int>(_currencyByType);

		private event Action _inventoryFetched;

		public event Action InventoryFetched
		{
			add
			{
				_inventoryFetched += value;
			}
			remove
			{
				_inventoryFetched -= value;
			}
		}

		private event Action _inventoryFetchFailed;

		public event Action InventoryFetchFailed
		{
			add
			{
				_inventoryFetchFailed += value;
			}
			remove
			{
				_inventoryFetchFailed -= value;
			}
		}

		private event Action _currencyFetched;

		public event Action CurrencyFetched
		{
			add
			{
				_currencyFetched += value;
			}
			remove
			{
				_currencyFetched -= value;
			}
		}

		[Inject]
		public InventoryManager(IInventoryService inventoryService, ProfileManager profileManager)
		{
			_inventoryService = inventoryService;
		}

		public InventoryManager(string serviceId, IInventoryService inventoryService, ProfileManager profileManager)
			: this(inventoryService, profileManager)
		{
			_serviceId = serviceId;
		}

		public void Fetch(Action<List<InventoryItem>, IDictionary<CurrencyType, int>> onSuccess, Action<FailureReasons> onFailure)
		{
			Action<Dictionary<string, InventoryItem>, IDictionary<CurrencyType, int>> onSuccess2 = delegate(Dictionary<string, InventoryItem> inventory, IDictionary<CurrencyType, int> currency)
			{
				OnInventoryFetched(inventory);
				OnCurrencyFetched(currency);
				onSuccess?.Invoke(Inventory, currency);
			};
			Action<FailureReasons> onFailure2 = delegate(FailureReasons reason)
			{
				this._inventoryFetchFailed?.Invoke();
				onFailure?.Invoke(reason);
			};
			_inventoryService.Fetch(_serviceId, onSuccess2, onFailure2);
		}

		public bool OwnsItem(string itemId)
		{
			return _inventoryById.ContainsKey(itemId);
		}

		public bool OwnsItem(BaseProfile profile)
		{
			return OwnsItem(profile.Id);
		}

		public bool CanAfford(BaseProfile profile, CurrencyType currencyType)
		{
			if (!profile.HasCurrencyTypeCost(currencyType))
			{
				return false;
			}
			return CanAfford(currencyType, profile.Cost[currencyType]);
		}

		public bool CanAfford(CurrencyType type, int amount)
		{
			int value = 0;
			if (_currencyByType.TryGetValue(type, out value))
			{
				return value >= amount;
			}
			return false;
		}

		public uint GetUsageCount(BaseProfile profile)
		{
			return GetUsageCount(profile.Id);
		}

		public uint GetUsageCount(string itemId)
		{
			uint result = 0u;
			if (_inventoryById.TryGetValue(itemId, out var value))
			{
				result = value.Count;
			}
			return result;
		}

		public void GrantItem(BaseProfile profile, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_inventoryService.GrantItem(profile, onSuccess, onFailure);
		}

		public void GrantCurrency(string playfabId, CurrencyType currencyType, int amount, Action onSuccess = null, Action<FailureReasons> onFailure = null)
		{
			_inventoryService.GrantCurrency(playfabId, currencyType, amount, onSuccess, onFailure);
		}

		public void ConsumeItem(string itemId, int amount, Action onSuccess, Action<FailureReasons> onFailure)
		{
			if (!_inventoryById.TryGetValue(itemId, out var item) || item.Count == 0)
			{
				onFailure(FailureReasons.NotEnoughUsesRemaining);
				return;
			}
			item.Count -= (uint)amount;
			Action<FailureReasons> onFailure2 = delegate(FailureReasons reason)
			{
				item.Count += (uint)amount;
				onFailure(reason);
			};
			_inventoryService.ConsumeItem(item.InstanceId, amount, onSuccess, onFailure2);
		}

		private void OnInventoryFetched(Dictionary<string, InventoryItem> items)
		{
			_inventoryById = items;
			this._inventoryFetched?.Invoke();
		}

		private void OnCurrencyFetched(IDictionary<CurrencyType, int> currencies)
		{
			_currencyByType = new Dictionary<CurrencyType, int>(currencies);
			this._currencyFetched?.Invoke();
		}
	}
}
