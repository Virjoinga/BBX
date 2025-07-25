using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIGenericPopup : MonoBehaviour
{
	[SerializeField]
	private Button[] _closeButtons = new Button[0];

	[SerializeField]
	private GameObject _closeButtonImage;

	[SerializeField]
	private TextMeshProUGUI _messageText;

	[SerializeField]
	private UIGenericPopupButton _button1;

	[SerializeField]
	private UIGenericPopupButton _button2;

	[SerializeField]
	private UIGenericPopupButton _button3;

	private void OnEnable()
	{
		BringToFront();
	}

	private void BringToFront()
	{
		base.transform.SetAsLastSibling();
	}

	public void Show(GenericPopupDetails popupDetails, UnityAction closeAction)
	{
		SetupCloseButtons(popupDetails.AllowCloseButtons, closeAction);
		_messageText.text = popupDetails.Message;
		TryPopuplateButtons(popupDetails, closeAction);
	}

	private void SetupCloseButtons(bool allowCloseButtons, UnityAction closeAction)
	{
		if (allowCloseButtons)
		{
			Button[] closeButtons = _closeButtons;
			for (int i = 0; i < closeButtons.Length; i++)
			{
				closeButtons[i].onClick.AddListener(closeAction);
			}
			_closeButtonImage.SetActive(value: true);
		}
		else
		{
			_closeButtonImage.SetActive(value: false);
		}
	}

	private void TryPopuplateButtons(GenericPopupDetails popupDetails, UnityAction closeAction)
	{
		if (!popupDetails.Button1Details.IsInitialized)
		{
			Debug.LogError("[UIGenericPopup] Trying to create a generic popup with no buttons defined");
			return;
		}
		_button1.Populate(popupDetails.Button1Details, closeAction);
		if (popupDetails.Button2Details.IsInitialized)
		{
			_button2.Populate(popupDetails.Button2Details, closeAction);
		}
		else
		{
			_button2.gameObject.SetActive(value: false);
		}
		if (popupDetails.Button3Details.IsInitialized)
		{
			_button3.Populate(popupDetails.Button3Details, closeAction);
		}
		else
		{
			_button3.gameObject.SetActive(value: false);
		}
	}
}
