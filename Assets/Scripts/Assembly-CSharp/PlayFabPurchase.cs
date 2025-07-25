using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabPurchase
{
	private Action _successCallback;

	private Action _failureCallback;

	public string ItemId { get; private set; }

	public string OrderId { get; private set; }

	private void RaiseFailureCallback()
	{
		_failureCallback();
	}

	private void RaiseSuccessCallback()
	{
		_successCallback();
	}

	public void StartPurchase(string itemId, string catalogName, Action successCallback, Action failureCallback)
	{
		ItemId = itemId;
		_successCallback = successCallback;
		_failureCallback = failureCallback;
		PlayFabClientAPI.StartPurchase(new StartPurchaseRequest
		{
			CatalogVersion = catalogName,
			Items = new List<ItemPurchaseRequest>
			{
				new ItemPurchaseRequest
				{
					ItemId = itemId,
					Quantity = 1u,
					Annotation = "Purchased via in-game store"
				}
			}
		}, delegate(StartPurchaseResult result)
		{
			OrderId = result.OrderId;
			Debug.Log("[PlayFabPurchase] Successfully started purchase with orderId: " + OrderId);
			PayForPurchase();
		}, delegate(PlayFabError error)
		{
			Debug.LogError("[PlayFabPurchase] Error: Start Purchase Failed: " + error.ErrorMessage + " - " + error.ErrorDetails);
			RaiseFailureCallback();
		});
	}

	private void PayForPurchase()
	{
		Debug.Log("[PlayFabPurchase] Starting Pay for Purchase with orderId: " + OrderId);
		PlayFabClientAPI.PayForPurchase(new PayForPurchaseRequest
		{
			OrderId = OrderId,
			ProviderName = "Steam",
			Currency = "RM"
		}, delegate
		{
			Debug.Log("[PlayFabPurchase] Successfully Paid for Purchase!");
		}, delegate(PlayFabError error)
		{
			Debug.LogError("[PlayFabPurchase] Error: Pay for Purchase Failed: " + error.ErrorMessage + " - " + error.ErrorDetails);
			RaiseFailureCallback();
		});
	}

	public void ConfirmPurchase()
	{
		Debug.Log("[PlayFabPurchase] Starting Confirm Purchase with orderId: " + OrderId);
		PlayFabClientAPI.ConfirmPurchase(new ConfirmPurchaseRequest
		{
			OrderId = OrderId
		}, delegate
		{
			Debug.Log("[PlayFabPurchase] Successfully Confirmed Purchase!");
			RaiseSuccessCallback();
		}, delegate(PlayFabError error)
		{
			Debug.LogError("[PlayFabPurchase] Error: ConfirmPurchase Failed: " + error.ErrorMessage + " - " + error.ErrorDetails);
			RaiseFailureCallback();
		});
	}
}
