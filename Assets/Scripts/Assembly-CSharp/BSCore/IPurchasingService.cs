using System;

namespace BSCore
{
	public interface IPurchasingService
	{
		void Purchase(BaseProfile profile, CurrencyType type, int price, string storeId, Action onSuccess, Action<FailureReasons> onFailure);
	}
}
