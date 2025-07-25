using System;
using BSCore;
using NodeClient;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatPlayerItem : MonoBehaviour
{
	[Serializable]
	private struct StatusToIcon
	{
		public PlayerStatus status;

		public Sprite icon;
	}

	[SerializeField]
	private StatusToIcon[] _statusToIcons = new StatusToIcon[2];

	[SerializeField]
	private TextMeshProUGUI _playerName;

	[SerializeField]
	private Image _icon;

	public PlayerCrumb PlayerCrumb { get; private set; }

	public void Display(PlayerCrumb playerCrumb, GameConfigData gameConfigData, bool isMe)
	{
		PlayerCrumb = playerCrumb;
		Color color = gameConfigData.OtherMessageNameColor;
		if (isMe)
		{
			color = gameConfigData.MyMessageNameColor;
		}
		else if (playerCrumb.IsAdmin)
		{
			color = gameConfigData.AdminMessageNameColor;
		}
		_playerName.text = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + playerCrumb.Nickname + "</color>";
		_icon.sprite = StatusToString(playerCrumb.Status);
	}

	private Sprite StatusToString(PlayerStatus status)
	{
		StatusToIcon[] statusToIcons = _statusToIcons;
		for (int i = 0; i < statusToIcons.Length; i++)
		{
			StatusToIcon statusToIcon = statusToIcons[i];
			if (statusToIcon.status == status)
			{
				return statusToIcon.icon;
			}
		}
		return null;
	}
}
