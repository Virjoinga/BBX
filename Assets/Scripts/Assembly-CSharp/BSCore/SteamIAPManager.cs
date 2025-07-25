using UnityEngine;

namespace BSCore
{
	public class SteamIAPManager : BaseIAPManager
	{
		private PlayFabPurchase _purchase;

		private SteamAbstractionLayer _steamAbstractionLayer;

		public SteamIAPManager(SteamAbstractionLayer steamAbstractionLayer)
		{
			_steamAbstractionLayer = steamAbstractionLayer;
			_steamAbstractionLayer.PurchaseAuthorizeResponse += OnSteamPurchaseAuthorizeResponse;
		}

		~SteamIAPManager()
		{
			_steamAbstractionLayer.PurchaseAuthorizeResponse -= OnSteamPurchaseAuthorizeResponse;
		}

		protected override void ProcessCurrentPurchase()
		{
			_purchase = new PlayFabPurchase();
			_purchase.StartPurchase(_currentPurchase.profile.Id, _currentPurchase.profile.CatalogVersion, OnPurchaseSuccess, delegate
			{
				_currentPurchase.onFailure(FailureReasons.Unknown);
			});
		}

		private void OnSteamPurchaseAuthorizeResponse(bool isAuthorized)
		{
			if (!isAuthorized)
			{
				_currentPurchase.onFailure(FailureReasons.UserCancelledPurchase);
			}
			else if (_purchase == null)
			{
				Debug.LogError("[SteamIAPManager] Got steam authorization response but there is no existing valid purchase");
			}
			else
			{
				_purchase.ConfirmPurchase();
			}
		}

		private void OnPurchaseSuccess()
		{
			_currentPurchase.onSuccess();
		}
	}
}
