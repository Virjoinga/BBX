using System;
using BSCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendRequestPlate : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _nameText;

	[SerializeField]
	private GameObject _approveRejectButtonsGroup;

	[SerializeField]
	private Button _cancelRequestButton;

	[SerializeField]
	private Button _approveRequestButton;

	[SerializeField]
	private Button _rejectRequestButton;

	private Friend _friend;

	private event Action<string> _removeFriend;

	public event Action<string> RemoveFriend
	{
		add
		{
			_removeFriend += value;
		}
		remove
		{
			_removeFriend -= value;
		}
	}

	private event Action<string> _approveFriend;

	public event Action<string> ApproveFriend
	{
		add
		{
			_approveFriend += value;
		}
		remove
		{
			_approveFriend -= value;
		}
	}

	private void Awake()
	{
		_cancelRequestButton.onClick.AddListener(delegate
		{
			this._removeFriend?.Invoke(_friend.ServiceId);
		});
		_rejectRequestButton.onClick.AddListener(delegate
		{
			this._removeFriend?.Invoke(_friend.ServiceId);
		});
		_approveRequestButton.onClick.AddListener(delegate
		{
			this._approveFriend?.Invoke(_friend.ServiceId);
		});
	}

	public void Populate(Friend friend)
	{
		_friend = friend;
		_nameText.text = _friend.DisplayName;
		_cancelRequestButton.gameObject.SetActive(!_friend.IsPendingConfirmation);
		_approveRejectButtonsGroup.SetActive(_friend.IsPendingConfirmation);
	}
}
