using System;
using BSCore;
using NodeClient;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendPlate : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _nameText;

	[SerializeField]
	private Button _inviteToGroupButton;

	[SerializeField]
	private Button _removeFriendButton;

	[SerializeField]
	private Image _offlineDisplay;

	[SerializeField]
	private Image _inLobbyDisplay;

	[SerializeField]
	private Image _inMatchmakingDisplay;

	[SerializeField]
	private Image _inMatchDisplay;

	[SerializeField]
	private Image _steamTagDisplay;

	private PlayerStatus _status = PlayerStatus.Offline;

	public string FriendId { get; private set; }

	public string Displayname => _nameText.text;

	public bool GroupButtonEnabled
	{
		set
		{
			_inviteToGroupButton.interactable = value;
		}
	}

	private event Action<string> _removeButtonClicked;

	public event Action<string> RemoveButtonClicked
	{
		add
		{
			_removeButtonClicked += value;
		}
		remove
		{
			_removeButtonClicked -= value;
		}
	}

	private event Action<FriendPlate> _inviteToGroupClicked;

	public event Action<FriendPlate> InviteToGroupClicked
	{
		add
		{
			_inviteToGroupClicked += value;
		}
		remove
		{
			_inviteToGroupClicked -= value;
		}
	}

	private void Start()
	{
		_removeFriendButton.onClick.AddListener(RemoveFriend);
		_inviteToGroupButton.onClick.AddListener(InviteToGroup);
	}

	public void Populate(Friend friend, GroupManager groupManager)
	{
		_nameText.text = friend.DisplayName;
		FriendId = friend.ServiceId;
		_steamTagDisplay.gameObject.SetActive(friend.IsSteam);
		UpdateStatus(friend.Status);
		UpdateGroupAvailability(groupManager.CanInvite(friend.ServiceId));
	}

	public void UpdateStatus(PlayerStatus status)
	{
		_status = status;
		_offlineDisplay.gameObject.SetActive(_status == PlayerStatus.Offline);
		_inLobbyDisplay.gameObject.SetActive(_status == PlayerStatus.InLobby);
		_inMatchmakingDisplay.gameObject.SetActive(_status == PlayerStatus.InMatchmaking);
		_inMatchDisplay.gameObject.SetActive(_status == PlayerStatus.InMatch);
	}

	public void UpdateGroupAvailability(bool canInvite)
	{
		_inviteToGroupButton.interactable = canInvite && _status != PlayerStatus.Offline;
	}

	private void RemoveFriend()
	{
		this._removeButtonClicked?.Invoke(FriendId);
	}

	private void InviteToGroup()
	{
		this._inviteToGroupClicked?.Invoke(this);
	}
}
