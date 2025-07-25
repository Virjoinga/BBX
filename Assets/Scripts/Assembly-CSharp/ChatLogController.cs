using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using BSCore.Chat;
using NodeClient;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ChatLogController : HideWhileActiveUIOpen
{
	[Inject]
	private GameConfigData _gameConfigData;

	[Inject]
	private UserManager _userManager;

	[SerializeField]
	private ChatLogItem _logItemPrefab;

	[SerializeField]
	private RectTransform _layoutRoot;

	[SerializeField]
	private ChatPlayerItem _chatPlayerItemPrefab;

	[SerializeField]
	private RectTransform _chatPlayerListRoot;

	[SerializeField]
	private SmartPool _smartPool;

	private readonly List<BaseChatRoom> _subscribedRooms = new List<BaseChatRoom>();

	private readonly Queue<ChatLogItem> _messageQueue = new Queue<ChatLogItem>();

	private void OnEnable()
	{
		RebuildChatLogLayout();
		RebuildPlayerListLayout();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		foreach (BaseChatRoom subscribedRoom in _subscribedRooms)
		{
			subscribedRoom.MessageReceived -= OnMessageReceived;
			if (subscribedRoom is PublicChatRoom)
			{
				(subscribedRoom as PublicChatRoom).RoomMembersChanged -= OnRoomMembersChanged;
			}
		}
	}

	protected override void SetGameObjectsActiveState()
	{
		base.SetGameObjectsActiveState();
		if (!ActiveUI.Manager.IsActiveUIShown)
		{
			RebuildChatLogLayout();
			RebuildPlayerListLayout();
		}
	}

	public void SubscribeToRooms(params BaseChatRoom[] rooms)
	{
		foreach (BaseChatRoom baseChatRoom in rooms)
		{
			if (_subscribedRooms.Contains(baseChatRoom))
			{
				continue;
			}
			_subscribedRooms.Add(baseChatRoom);
			baseChatRoom.MessageReceived += OnMessageReceived;
			if (!(baseChatRoom is PublicChatRoom))
			{
				continue;
			}
			(baseChatRoom as PublicChatRoom).RoomMembersChanged += OnRoomMembersChanged;
			foreach (string cachedMessage in baseChatRoom.CachedMessages)
			{
				OnMessageReceived(cachedMessage);
			}
		}
		UpdateRoomMembersList();
	}

	public void UnSubscribeFromRooms(bool updateMemberList, params BaseChatRoom[] rooms)
	{
		foreach (BaseChatRoom baseChatRoom in rooms)
		{
			if (_subscribedRooms.Contains(baseChatRoom))
			{
				Debug.Log("[ChatLogController] Unsubscribing from events in room " + baseChatRoom.Id);
				_subscribedRooms.Remove(baseChatRoom);
				baseChatRoom.MessageReceived -= OnMessageReceived;
				if (baseChatRoom is PublicChatRoom)
				{
					(baseChatRoom as PublicChatRoom).RoomMembersChanged -= OnRoomMembersChanged;
				}
			}
		}
		if (updateMemberList)
		{
			UpdateRoomMembersList();
		}
	}

	private void OnMessageReceived(string formattedMessage)
	{
		ChatLogItem chatLogItem = SmartPool.Spawn(_logItemPrefab);
		chatLogItem.Init(formattedMessage);
		chatLogItem.transform.SetAsLastSibling();
		_messageQueue.Enqueue(chatLogItem);
		if (_messageQueue.Count > _smartPool.MaxPoolSize)
		{
			SmartPool.Despawn(_messageQueue.Dequeue().gameObject);
		}
		RebuildChatLogLayout();
	}

	private void RebuildChatLogLayout()
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(RebuildAfterFrame(_layoutRoot));
		}
	}

	private void OnRoomMembersChanged()
	{
		UpdateRoomMembersList();
	}

	private void UpdateRoomMembersList()
	{
		SmartPool.DespawnAllItems(_chatPlayerItemPrefab.gameObject);
		List<PlayerCrumb> list = RegenerateUniquePlayersInSubscribedRooms();
		Debug.Log("[ChatLogController] Unique players: " + string.Join(", ", list.Select((PlayerCrumb p) => p.ToString())));
		foreach (PlayerCrumb item in list)
		{
			ChatPlayerItem chatPlayerItem = SmartPool.Spawn(_chatPlayerItemPrefab);
			chatPlayerItem.Display(item, _gameConfigData, _userManager.CurrentUser.IsMe(item));
			chatPlayerItem.transform.SetAsLastSibling();
		}
		RebuildPlayerListLayout();
	}

	private List<PlayerCrumb> RegenerateUniquePlayersInSubscribedRooms()
	{
		List<PlayerCrumb> list = new List<PlayerCrumb>();
		foreach (BaseChatRoom subscribedRoom in _subscribedRooms)
		{
			if (!(subscribedRoom is PublicChatRoom))
			{
				continue;
			}
			foreach (PlayerCrumb item in (subscribedRoom as PublicChatRoom).PlayersInRoom)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
		}
		return list.OrderBy((PlayerCrumb p) => p.Nickname).ToList();
	}

	private void RebuildPlayerListLayout()
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(RebuildAfterFrame(_chatPlayerListRoot));
		}
	}

	private IEnumerator RebuildAfterFrame(RectTransform layoutRoot)
	{
		yield return new WaitForEndOfFrame();
		LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
	}
}
