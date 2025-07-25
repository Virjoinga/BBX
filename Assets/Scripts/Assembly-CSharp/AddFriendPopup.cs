using System;
using BSCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AddFriendPopup : MonoBehaviour
{
	[Inject]
	private ClientFriendsManager _friendsManager;

	[SerializeField]
	private Button _addFriendButton;

	[SerializeField]
	private TMP_InputField _inputField;

	[SerializeField]
	private TextMeshProUGUI _errorText;

	[SerializeField]
	private Button[] _closeButons;

	private event Action<bool> _closingPopup;

	public event Action<bool> ClosingPopup
	{
		add
		{
			_closingPopup += value;
		}
		remove
		{
			_closingPopup -= value;
		}
	}

	private void Awake()
	{
		_addFriendButton.onClick.AddListener(TryAddFriend);
		Button[] closeButons = _closeButons;
		for (int i = 0; i < closeButons.Length; i++)
		{
			closeButons[i].onClick.AddListener(delegate
			{
				ClosePopup(requestSent: false);
			});
		}
	}

	private void OnEnable()
	{
		_inputField.text = string.Empty;
	}

	private void TryAddFriend()
	{
		_errorText.text = string.Empty;
		if (string.IsNullOrEmpty(_inputField.text))
		{
			_errorText.text = "Please Enter A Display Name";
		}
		else
		{
			_friendsManager.SendFriendRequestByName(_inputField.text, FriendRequestSent, FailedToGetServiceId);
		}
	}

	private void FriendRequestSent()
	{
		Debug.Log("[AddFriendPopup] Friend Request Sent");
		ClosePopup(requestSent: true);
	}

	private void FailedToGetServiceId(FailureReasons failureReason)
	{
		Debug.Log($"[AddFriendPopup] Failed to Get service Id {failureReason}");
		_errorText.text = "User with display name not found";
	}

	private void ClosePopup(bool requestSent)
	{
		_errorText.text = string.Empty;
		this._closingPopup?.Invoke(requestSent);
		base.gameObject.SetActive(value: false);
	}
}
