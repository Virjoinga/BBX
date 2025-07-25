using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BSCore;
using BSCore.Chat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ChatController : MonoBehaviour
{
	public static string GENERAL_CHANNEL = "general";

	private static readonly Regex PRIVATE_MESSAGE_REGEX = new Regex("/[tw] ([a-zA-Z][a-zA-Z0-9]+) (.*)");

	private static readonly Regex GROUP_MESSAGE_REGEX = new Regex("/g (.*)");

	private const int MESSAGE_CHAR_LIMIT = 100;

	private static readonly Regex _disallowedChatCharacters = new Regex("[^\\u0000-\\u007F]");

	[Inject]
	private ChatClient _chatClient;

	[Inject]
	private ChatCommandController _chatCommandController;

	[SerializeField]
	private TMP_InputField _chatInput;

	[SerializeField]
	private Button _chatSendButton;

	[SerializeField]
	private List<ChatLogController> _chatLogs;

	[SerializeField]
	private ActiveUI _activeElements;

	[SerializeField]
	private Button _activationOverlay;

	[SerializeField]
	private Button _deactivationOverlay;

	private PublicChatRoom _groupChatRoom;

	private SystemChatRoom _systemChatRoom;

	private PrivateChatRoom _privateChatRoom;

	private PublicChatRoom _generalChatRoom;

	private void Awake()
	{
		_chatClient.Authenticated += OnChatConnected;
		_chatClient.Disconnected += OnChatDisconnected;
	}

	private void Start()
	{
		_systemChatRoom = _chatClient.SystemChatRoom;
		_privateChatRoom = _chatClient.PrivateChatRoom;
		_generalChatRoom = _chatClient.JoinChannel(GENERAL_CHANNEL);
		SubscribeAllLogsToRooms(_systemChatRoom, _privateChatRoom);
		PublicChatRoom[] array = _chatClient.ChatRooms.Values.ToArray();
		BaseChatRoom[] rooms = array;
		SubscribeAllLogsToRooms(rooms);
		_chatClient.ChannelJoined += OnChannelJoined;
		if (_chatClient.IsConnected)
		{
			OnChatConnected();
		}
		_chatSendButton?.onClick.AddListener(OnChatSendClicked);
		_chatInput.onSubmit.AddListener(OnChatInputSubmit);
		_activationOverlay?.onClick.AddListener(OnActivationOverlayClicked);
		_deactivationOverlay?.onClick.AddListener(Deactivate);
		Deactivate();
	}

	private void Update()
	{
		if (!ActiveUI.Manager.IsInputSupressed || !(ActiveUI.Manager.LastActiveUI != _activeElements))
		{
			_chatInput.text = _disallowedChatCharacters.Replace(_chatInput.text, string.Empty);
			bool keyDown = Input.GetKeyDown(KeyCode.Escape);
			if (Input.GetKeyDown(KeyCode.Return))
			{
				Activate();
			}
			else if (keyDown || Input.GetKeyDown(KeyCode.Return))
			{
				Deactivate();
			}
		}
	}

	private void OnDestroy()
	{
		_chatClient.ChannelJoined -= OnChannelJoined;
		UnsubscribeAllLogsFromRooms(false, _privateChatRoom, _systemChatRoom, _generalChatRoom);
		_chatClient.Authenticated -= OnChatConnected;
		_chatClient.Disconnected -= OnChatDisconnected;
	}

	private void OnChannelJoined(PublicChatRoom room)
	{
		Debug.Log("[ChatController] Joined channel: " + room.Id);
		string text = ((room.Id == GENERAL_CHANNEL) ? room.Id : "Group");
		_systemChatRoom.DisplayMessage("Joined channel " + text);
		SubscribeAllLogsToRooms(room);
		if (room.Id == GENERAL_CHANNEL)
		{
			_generalChatRoom = room;
		}
	}

	private void OnChatConnected()
	{
		_systemChatRoom.DisplayMessage("Connected to chat");
	}

	private void OnChatDisconnected()
	{
		_systemChatRoom.DisplayMessage("Lost connection to chat");
	}

	private void SubscribeAllLogsToRooms(params BaseChatRoom[] rooms)
	{
		foreach (ChatLogController chatLog in _chatLogs)
		{
			chatLog.SubscribeToRooms(rooms);
		}
	}

	private void UnsubscribeAllLogsFromRooms(bool updateMemberList, params BaseChatRoom[] rooms)
	{
		foreach (ChatLogController chatLog in _chatLogs)
		{
			if (chatLog != null)
			{
				chatLog.UnSubscribeFromRooms(updateMemberList, rooms);
			}
		}
	}

	private void OnChatSendClicked()
	{
		OnChatInputSubmit(_chatInput.text);
	}

	private void OnChatInputSubmit(string message)
	{
		if (string.IsNullOrEmpty(message))
		{
			return;
		}
		_chatInput.text = "";
		message = message.Trim();
		if (message[0] == '/')
		{
			_chatCommandController.HandleChatCommand(message);
			return;
		}
		if (!_chatClient.IsConnected)
		{
			_systemChatRoom.DisplayMessage("Cannot send message. You are not connected to the chat server");
			return;
		}
		message = _disallowedChatCharacters.Replace(message, string.Empty);
		if (message.Length > 100)
		{
			message = message.Remove(100);
		}
		if (!HandleGroupMessage(message))
		{
			_generalChatRoom.SendMessage(message);
		}
	}

	private bool HandlePrivateMessage(string message)
	{
		Match match = PRIVATE_MESSAGE_REGEX.Match(message);
		if (match.Success)
		{
			string value = match.Groups[1].Value;
			string value2 = match.Groups[2].Value;
			if (_chatClient.TryGetServiceIdByNicknameFromAnyChannel(value, out var serviceId))
			{
				_privateChatRoom.SendMessage(serviceId, value2);
			}
			else if (!string.IsNullOrEmpty(value2))
			{
				_systemChatRoom.DisplayMessage($"Could not find a player by the name of {value}.");
			}
		}
		return match.Success;
	}

	private bool HandleGroupMessage(string message)
	{
		Match match = GROUP_MESSAGE_REGEX.Match(message);
		if (match.Success)
		{
			if (_groupChatRoom == null)
			{
				_systemChatRoom.DisplayMessage("You are not in a group");
				return true;
			}
			message = match.Groups[1].Value;
			_groupChatRoom.SendMessage(message);
		}
		return match.Success;
	}

	private void OnActivationOverlayClicked()
	{
		Activate();
	}

	private void Activate()
	{
		_activationOverlay?.gameObject.SetActive(value: false);
		_deactivationOverlay?.gameObject.SetActive(value: true);
		_activeElements?.Show();
	}

	private void Deactivate()
	{
		_activationOverlay?.gameObject.SetActive(value: true);
		_deactivationOverlay?.gameObject.SetActive(value: false);
		_activeElements?.Hide();
		_chatInput.text = "";
	}
}
