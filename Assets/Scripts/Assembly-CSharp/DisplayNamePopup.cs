using BSCore;
using RSG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DisplayNamePopup : MonoBehaviour
{
	[Inject]
	private SteamAbstractionLayer _steamAbstractionLayer;

	[Inject]
	private UserManager _userManager;

	[SerializeField]
	private Button _submitButton;

	[SerializeField]
	private TMP_InputField _inputField;

	[SerializeField]
	private TextMeshProUGUI _feedbackText;

	private Promise _promise;

	private void Start()
	{
		_submitButton.onClick.AddListener(delegate
		{
			TrySetDisplayName(_inputField.text);
		});
		_inputField.onSubmit.AddListener(TrySetDisplayName);
		_feedbackText.text = string.Empty;
		_inputField.text = _userManager.StripInvalidCharacters(_userManager.CurrentUser.DisplayName);
		_inputField.text = _userManager.StripInvalidCharacters(_steamAbstractionLayer.GetDisplayName());
		if (!_userManager.IsValidDisplayName(_inputField.text))
		{
			OnFailure(FailureReasons.DisplayNameInvalid);
		}
	}

	public void SetPromise(Promise promise)
	{
		_promise = promise;
	}

	private void TrySetDisplayName(string displayName)
	{
		_feedbackText.text = "Checking...";
		_submitButton.interactable = false;
		_inputField.interactable = false;
		if (_userManager.IsValidDisplayName(displayName))
		{
			_userManager.UpdateDisplayName(displayName, OnSuccess, OnFailure);
		}
		else
		{
			OnFailure(FailureReasons.DisplayNameInvalid);
		}
	}

	private void OnSuccess()
	{
		_promise.Resolve();
		Object.Destroy(base.gameObject);
	}

	private void OnFailure(FailureReasons failureReason)
	{
		switch (failureReason)
		{
		case FailureReasons.DisplayNameNotAvailable:
			_feedbackText.text = "That display name is unavailable.";
			break;
		case FailureReasons.DisplayNameInvalid:
			_feedbackText.text = "That display name is invalid.";
			break;
		default:
			_feedbackText.text = "Failed to update display name. Please try again.";
			break;
		}
		_submitButton.interactable = true;
		_inputField.interactable = true;
		_inputField.ActivateInputField();
		_inputField.Select();
	}
}
