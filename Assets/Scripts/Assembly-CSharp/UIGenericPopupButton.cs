using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIGenericPopupButton : MonoBehaviour
{
	[SerializeField]
	private Button _button;

	[SerializeField]
	private TextMeshProUGUI _buttonText;

	[SerializeField]
	private Image _colorableButtonImage;

	public void Populate(GenericButtonDetails buttonDetails, UnityAction closeAction)
	{
		_buttonText.text = buttonDetails.Text;
		_colorableButtonImage.color = buttonDetails.Color;
		if (buttonDetails.Action != null)
		{
			_button.onClick.AddListener(buttonDetails.Action);
		}
		_button.onClick.AddListener(closeAction);
		base.gameObject.SetActive(value: true);
	}
}
