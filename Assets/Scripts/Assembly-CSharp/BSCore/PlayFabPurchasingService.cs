using System;
using PlayFab;
using PlayFab.ClientModels;

namespace BSCore
{
	public class PlayFabPurchasingService : PlayFabService, IPurchasingService
	{
		public void Purchase(BaseProfile profile, CurrencyType type, int price, string storeId = "", Action onSuccess = null, Action<FailureReasons> onFailure = null)
		{
			PurchaseItemRequest purchaseItemRequest = new PurchaseItemRequest
			{
				ItemId = profile.Id,
				VirtualCurrency = type.ToString(),
				Price = price,
				CatalogVersion = profile.CatalogVersion
			};
			if (!string.IsNullOrEmpty(storeId))
			{
				purchaseItemRequest.StoreId = storeId;
			}
			Action<PlayFabError> errorCallback = OnFailureCallback(delegate
			{
				Purchase(profile, type, price, storeId, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			});
			PlayFabClientAPI.PurchaseItem(purchaseItemRequest, delegate
			{
				onSuccess();
			}, errorCallback);
		}
	}
}
