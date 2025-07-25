using System.Collections.Generic;
using System.Linq;
using BSCore.Chat;
using NodeClient;
using TMPro;
using UnityEngine;
using Zenject;

public class PlayerStatusDisplay : MonoBehaviour
{
	[Inject]
	private ChatClient _chatClient;

	[SerializeField]
	private TextMeshProUGUI _inLobby;

	[SerializeField]
	private TextMeshProUGUI _inMatchmaking;

	[SerializeField]
	private TextMeshProUGUI _inMatch;

	private PublicChatRoom _generalChatRoom;

	private void Start()
	{
		_generalChatRoom = _chatClient.JoinChannel(ChatController.GENERAL_CHANNEL);
		_generalChatRoom.RoomMembersChanged += OnRoomMembersChanged;
		_chatClient.Disconnected += OnChatDisconnected;
	}

	private void OnRoomMembersChanged()
	{
		IEnumerable<PlayerCrumb> playersInRoom = _generalChatRoom.PlayersInRoom;
		Debug.Log(string.Format("[PlayerStatusDisplay] Players in room: {0}\n{1}", playersInRoom.Count(), string.Join("\n", playersInRoom.Select((PlayerCrumb p) => p.ToString()))));
		_inLobby.text = playersInRoom.Count((PlayerCrumb p) => p.Status == PlayerStatus.InLobby).ToString();
		_inMatchmaking.text = playersInRoom.Count((PlayerCrumb p) => p.Status == PlayerStatus.InMatchmaking).ToString();
		_inMatch.text = playersInRoom.Count((PlayerCrumb p) => p.Status == PlayerStatus.InMatch).ToString();
	}

	private void OnChatDisconnected()
	{
		_inLobby.text = "0";
		_inMatchmaking.text = "0";
		_inMatch.text = "0";
	}
}
