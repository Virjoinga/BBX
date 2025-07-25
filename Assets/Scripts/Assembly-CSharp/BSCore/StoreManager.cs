using System;
using System.Collections.Generic;
using Zenject;

namespace BSCore
{
	public class StoreManager
	{
		private IStoreService _storeService;

		private Dictionary<string, StoreData> _storesById = new Dictionary<string, StoreData>();

		public Dictionary<string, StoreData> StoresById => new Dictionary<string, StoreData>(_storesById);

		[Inject]
		public StoreManager(IStoreService storeService)
		{
			_storeService = storeService;
		}

		public void Fetch(string storeId, Action<StoreData> onSuccess, Action<FailureReasons> onFailure)
		{
			Action<StoreData> onSuccess2 = delegate(StoreData store)
			{
				if (_storesById.ContainsKey(store.ID))
				{
					_storesById[store.ID] = store;
				}
				else
				{
					_storesById.Add(store.ID, store);
				}
				onSuccess?.Invoke(store);
			};
			_storeService.Fetch(storeId, onSuccess2, onFailure);
		}
	}
}
