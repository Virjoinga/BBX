using System;

namespace BSCore
{
	public interface IStoreService
	{
		void Fetch(string storeId, Action<StoreData> onSuccess, Action<FailureReasons> onFailure);
	}
}
