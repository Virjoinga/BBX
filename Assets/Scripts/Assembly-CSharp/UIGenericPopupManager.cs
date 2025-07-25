using UnityEngine;
using UnityEngine.Events;

public static class UIGenericPopupManager
{
	private static bool _popupActive;

	public static void ShowPopup(GenericPopupDetails popupDetails)
	{
		if (_popupActive)
		{
			ClosePopup();
		}
		_popupActive = true;
		UIPrefabManager.Instantiate(UIPrefabIds.GenericPopup, delegate(GameObject go)
		{
			OnPopupCreated(go, popupDetails);
		}, interactive: true, 10);
	}

	private static void OnPopupCreated(GameObject uiGameobject, GenericPopupDetails popupDetails)
	{
		if (!(uiGameobject == null))
		{
			uiGameobject.GetComponent<UIGenericPopup>().Show(popupDetails, ClosePopup);
		}
	}

	public static void ShowConfirmPopup(string message, UnityAction confirmAction, bool allowCloseButtons = false)
	{
		ShowPopup(new GenericPopupDetails
		{
			Message = message,
			Button1Details = new GenericButtonDetails("OK", confirmAction, UIConstants.GreenButtonColor),
			AllowCloseButtons = allowCloseButtons
		});
	}

	public static void ShowYesNoPopup(string message, UnityAction yesAction, UnityAction noAction, bool allowCloseButtons = false)
	{
		ShowPopup(new GenericPopupDetails
		{
			Message = message,
			Button1Details = new GenericButtonDetails("No", noAction, UIConstants.RedButtonColor),
			Button2Details = new GenericButtonDetails("Yes", yesAction, UIConstants.GreenButtonColor),
			AllowCloseButtons = allowCloseButtons
		});
	}

	private static void ClosePopup()
	{
		_popupActive = false;
		UIPrefabManager.Destroy(UIPrefabIds.GenericPopup);
	}
}
