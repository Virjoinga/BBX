using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugLoginPopup : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField _inputField;

	[SerializeField]
	private Button _submitButton;

	[SerializeField]
	private TextMeshProUGUI _feedbackText;

	private event Action<string> _onDebugIdEntered;

	public void Listen(Action<string> onDebugIdEntered)
	{
		this._onDebugIdEntered = onDebugIdEntered;
	}

	private void Start()
	{
		_feedbackText.text = string.Empty;
		_submitButton.onClick.AddListener(delegate
		{
			RaiseIdEntered(_inputField.text);
		});
		_inputField.onSubmit.AddListener(RaiseIdEntered);
	}

	private void RaiseIdEntered(string debugId)
	{
		_feedbackText.text = "Submitting...";
		if (string.IsNullOrEmpty(debugId))
		{
			_feedbackText.text = "You forgot something...";
			return;
		}
		_feedbackText.text = "Logging In...";
		this._onDebugIdEntered?.Invoke(debugId);
		UnityEngine.Object.Destroy(base.gameObject, 0.5f);
	}
}
