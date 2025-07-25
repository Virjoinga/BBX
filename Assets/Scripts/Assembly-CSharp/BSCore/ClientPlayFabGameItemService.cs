using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Zenject;

namespace BSCore
{
	public class ClientPlayFabGameItemService : BasePlayFabGameItemService
	{
		[Inject]
		public ClientPlayFabGameItemService(BaseProfileFactory profileFactory, GameConfigData configData)
			: base(profileFactory, configData)
		{
		}

		public override void Fetch(string catalogName, Action<Dictionary<string, BaseProfile>> onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest
			{
				CatalogVersion = catalogName
			}, errorCallback: OnFailureCallback(delegate
			{
				Fetch(catalogName, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: OnSuccessWrapper(onSuccess));
		}

		private Action<GetCatalogItemsResult> OnSuccessWrapper(Action<Dictionary<string, BaseProfile>> onSuccess)
		{
			return delegate(GetCatalogItemsResult result)
			{
				List<GameItem> list = new List<GameItem>();
				foreach (CatalogItem item in result.Catalog)
				{
					if (IsValidItemType(item.ItemClass))
					{
						GameItem gameItem = ConvertToGameItem(item);
						if (IncludeInCurrentVersion(gameItem))
						{
							list.Add(gameItem);
						}
					}
				}
				onSuccess(_profileFactory.GenerateProfiles(list));
			};
		}

		private GameItem ConvertToGameItem(CatalogItem catalogItem)
		{
			GameItem.Data data = new GameItem.Data
			{
				CatalogVersion = catalogItem.CatalogVersion,
				Id = catalogItem.ItemId,
				ItemType = Enum<ItemType>.Parse(catalogItem.ItemClass),
				Tags = catalogItem.Tags,
				IsDefault = (catalogItem.Tags != null && catalogItem.Tags.Contains("default")),
				Description = catalogItem.Description,
				Name = catalogItem.DisplayName,
				Icon = catalogItem.ItemImageUrl,
				CustomData = catalogItem.CustomData,
				UsageCount = (catalogItem.Consumable.UsageCount.HasValue ? ((int)catalogItem.Consumable.UsageCount.Value) : (-1)),
				UsageLifespan = (catalogItem.Consumable.UsagePeriod.HasValue ? ((int)catalogItem.Consumable.UsagePeriod.Value) : (-1)),
				Cost = GenerateCostDictionary(catalogItem.VirtualCurrencyPrices),
				CurrencyBundle = GenerateCurrencyBundles(catalogItem)
			};
			if (catalogItem.Bundle != null && catalogItem.Bundle.BundledItems != null && catalogItem.Bundle.BundledItems.Count > 0)
			{
				data.BundledItems = catalogItem.Bundle.BundledItems;
			}
			return new GameItem(data);
		}

		private Dictionary<CurrencyType, int> GenerateCostDictionary(Dictionary<string, uint> prices)
		{
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			foreach (KeyValuePair<string, uint> price in prices)
			{
				dictionary.Add(Enum<CurrencyType>.Parse(price.Key), (int)price.Value);
			}
			return dictionary;
		}

		private Dictionary<CurrencyType, uint> GenerateCurrencyBundles(CatalogItem catalogItem)
		{
			if (catalogItem.Bundle != null && catalogItem.Bundle.BundledVirtualCurrencies != null)
			{
				Dictionary<CurrencyType, uint> dictionary = new Dictionary<CurrencyType, uint>();
				{
					foreach (KeyValuePair<string, uint> bundledVirtualCurrency in catalogItem.Bundle.BundledVirtualCurrencies)
					{
						CurrencyType key = (CurrencyType)Enum.Parse(typeof(CurrencyType), bundledVirtualCurrency.Key);
						uint value = uint.Parse(bundledVirtualCurrency.Value.ToString());
						dictionary.Add(key, value);
					}
					return dictionary;
				}
			}
			return null;
		}
	}
}
