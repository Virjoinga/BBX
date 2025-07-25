using System;

namespace BSCore
{
	public abstract class BaseIAPManager
	{
		protected class PurchaseData
		{
			public IAPProfile profile;

			public Action onSuccess;

			public Action<FailureReasons> onFailure;

			public PurchaseData(IAPProfile profile, Action onSuccess, Action<FailureReasons> onFailure)
			{
				this.profile = profile;
				this.onSuccess = onSuccess;
				this.onFailure = onFailure;
			}
		}

		protected PurchaseData _currentPurchase;

		public void BuyProduct(IAPProfile profile, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_currentPurchase = new PurchaseData(profile, onSuccess, onFailure);
			ProcessCurrentPurchase();
		}

		protected abstract void ProcessCurrentPurchase();
	}
}
