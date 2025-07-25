using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Zenject;

namespace BSCore
{
	public class ClientPlayFabInventoryService : BasePlayFabInventoryService
	{
		[Inject]
		public ClientPlayFabInventoryService(ProfileManager profileManager)
			: base(profileManager)
		{
		}

		public override void Fetch(string serviceId, Action<Dictionary<string, InventoryItem>, IDictionary<CurrencyType, int>> onSuccess, Action<FailureReasons> onFailure)
		{
			if (!_profileManager.HasFetched)
			{
				DelayedAction.RunWhen(() => _profileManager.HasFetched, delegate
				{
					Fetch(serviceId, onSuccess, onFailure);
				});
				return;
			}
			GetUserInventoryRequest request = new GetUserInventoryRequest();
			Action<GetUserInventoryResult> resultCallback = delegate(GetUserInventoryResult result)
			{
				Dictionary<string, InventoryItem> arg = ParseInventory(serviceId, result.Inventory);
				Dictionary<CurrencyType, int> arg2 = ParseCurrency(result.VirtualCurrency);
				onSuccess(arg, arg2);
			};
			Action<PlayFabError> errorCallback = OnFailureCallback(delegate
			{
				Fetch(serviceId, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure?.Invoke(reason);
			});
			PlayFabClientAPI.GetUserInventory(request, resultCallback, errorCallback);
		}

		public override void ConsumeItem(string instanceId, int usesToConsume, Action onSuccess, Action<FailureReasons> onFailure)
		{
			ConsumeItemRequest request = new ConsumeItemRequest
			{
				ItemInstanceId = instanceId,
				ConsumeCount = usesToConsume
			};
			Action<ConsumeItemResult> resultCallback = delegate
			{
				onSuccess();
			};
			Action<PlayFabError> errorCallback = OnFailureCallback(delegate
			{
				ConsumeItem(instanceId, usesToConsume, onSuccess, onFailure);
			}, onFailure);
			PlayFabClientAPI.ConsumeItem(request, resultCallback, errorCallback);
		}

		private Dictionary<string, InventoryItem> ParseInventory(string serviceId, List<ItemInstance> itemInstances)
		{
			Dictionary<string, InventoryItem> dictionary = new Dictionary<string, InventoryItem>();
			foreach (ItemInstance itemInstance in itemInstances)
			{
				if (!dictionary.TryGetValue(itemInstance.ItemId, out var value))
				{
					BaseProfile byId = _profileManager.GetById(itemInstance.ItemId);
					if (byId == null)
					{
						Debug.LogWarningFormat("[InventoryService] {0} has {1} in their inventory, but could not find a profile for it", serviceId, itemInstance.ItemId);
						continue;
					}
					value = new InventoryItem(new InventoryItem.Data
					{
						Id = itemInstance.ItemId,
						InstanceId = itemInstance.ItemInstanceId,
						Profile = byId,
						Count = (itemInstance.RemainingUses.HasValue ? ((uint)itemInstance.RemainingUses.Value) : 0u)
					});
					dictionary.Add(value.Id, value);
				}
			}
			return dictionary;
		}
	}
}
