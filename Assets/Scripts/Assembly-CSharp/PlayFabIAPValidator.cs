using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public static class PlayFabIAPValidator
{
	public static void ValidateGPPurchase(string currencyCode, int purchasePrice, string jsonReceipt, string signature, Action successCallback, Action failCallback)
	{
		ValidateGooglePlayPurchaseRequest request = new ValidateGooglePlayPurchaseRequest
		{
			CurrencyCode = currencyCode,
			PurchasePrice = (uint)purchasePrice,
			ReceiptJson = jsonReceipt,
			Signature = signature
		};
		Action<ValidateGooglePlayPurchaseResult> resultCallback = delegate
		{
			Debug.Log("[PlayFabIAPValidator] Google IAP Validated!");
			successCallback?.Invoke();
		};
		PlayFabClientAPI.ValidateGooglePlayPurchase(request, resultCallback, OnValidationFailed(failCallback));
	}

	public static void ValidateIOSPurchase(string currencyCode, int purchasePrice, string receipt, Action successCallback, Action failCallback)
	{
		ValidateIOSReceiptRequest request = new ValidateIOSReceiptRequest
		{
			CurrencyCode = currencyCode,
			PurchasePrice = purchasePrice,
			ReceiptData = receipt
		};
		Action<ValidateIOSReceiptResult> resultCallback = delegate
		{
			Debug.Log("[PlayFabIAPValidator] iOS IAP Validated!");
			successCallback?.Invoke();
		};
		PlayFabClientAPI.ValidateIOSReceipt(request, resultCallback, OnValidationFailed(failCallback));
	}

	private static Action<PlayFabError> OnValidationFailed(Action onFailure)
	{
		return delegate(PlayFabError error)
		{
			Debug.LogErrorFormat("[PlayFabIAPValidator] Playfab IAP Validation Failed. Code {0} - Message: {1} - Details: {2}", error.Error, error.ErrorMessage, error.ErrorDetails);
			onFailure?.Invoke();
		};
	}
}
