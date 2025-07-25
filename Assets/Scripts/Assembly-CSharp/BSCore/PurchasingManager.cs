using System;
using Zenject;

namespace BSCore
{
	public class PurchasingManager
	{
		private IPurchasingService _purchasingService;

		private ProfileManager _profileManager;

		private InventoryManager _inventoryManager;

		[Inject]
		public PurchasingManager(IPurchasingService purchasingServce, ProfileManager profileManager, InventoryManager inventoryManager)
		{
			_purchasingService = purchasingServce;
			_profileManager = profileManager;
			_inventoryManager = inventoryManager;
		}

		public void PurchaseItem(string profileId, CurrencyType type, int price, Action onSuccess, Action<FailureReasons> onFailure)
		{
			BaseProfile byId = _profileManager.GetById(profileId);
			_purchasingService.Purchase(byId, type, price, string.Empty, onSuccess, onFailure);
		}

		public void PurchaseItem(string profileId, CurrencyType type, int price, string storeId, Action onSuccess, Action<FailureReasons> onFailure)
		{
			BaseProfile byId = _profileManager.GetById(profileId);
			_purchasingService.Purchase(byId, type, price, storeId, onSuccess, onFailure);
		}

		public void PurchaseItem(BaseProfile profile, CurrencyType type, int price, Action onSuccess, Action<FailureReasons> onFailure)
		{
			PurchaseItem(profile, type, price, string.Empty, onSuccess, onFailure);
		}

		public void PurchaseItem(BaseProfile profile, CurrencyType type, int price, string storeId, Action onSuccess, Action<FailureReasons> onFailure)
		{
			Action onSuccess2 = delegate
			{
				_inventoryManager.Fetch(null, null);
				onSuccess();
			};
			_purchasingService.Purchase(profile, type, price, storeId, onSuccess2, onFailure);
		}
	}
}
