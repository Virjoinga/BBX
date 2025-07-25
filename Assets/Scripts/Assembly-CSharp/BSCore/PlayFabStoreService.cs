using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Zenject;

namespace BSCore
{
	public class PlayFabStoreService : PlayFabService, IStoreService
	{
		private ProfileManager _profileManager;

		[Inject]
		public PlayFabStoreService(ProfileManager profileManager)
		{
			_profileManager = profileManager;
		}

		public void Fetch(string storeId, Action<StoreData> onSuccess, Action<FailureReasons> onFailure)
		{
			GetStoreItemsRequest request = new GetStoreItemsRequest
			{
				StoreId = storeId
			};
			Action<GetStoreItemsResult> resultCallback = delegate(GetStoreItemsResult result)
			{
				List<StoreItem> list = new List<StoreItem>();
				foreach (PlayFab.ClientModels.StoreItem item2 in result.Store)
				{
					BaseProfile byId = _profileManager.GetById(item2.ItemId);
					if (byId == null)
					{
						Debug.LogErrorFormat("Error loading store item ({0}) for store {1}", item2.ItemId, result.StoreId);
					}
					else
					{
						StoreItem item = new StoreItem(byId, item2.VirtualCurrencyPrices);
						list.Add(item);
					}
				}
				StoreData obj = new StoreData(result.StoreId, list, result.MarketingData.DisplayName, result.MarketingData.Description);
				onSuccess(obj);
			};
			Action<PlayFabError> errorCallback = OnFailureCallback(delegate
			{
				Fetch(storeId, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			});
			PlayFabClientAPI.GetStoreItems(request, resultCallback, errorCallback);
		}
	}
}
