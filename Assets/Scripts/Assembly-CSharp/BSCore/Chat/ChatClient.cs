using System;
using System.Collections.Generic;
using NodeClient;
using UnityEngine;

namespace BSCore.Chat
{
	public class ChatClient
	{
		public readonly Dictionary<string, PublicChatRoom> ChatRooms = new Dictionary<string, PublicChatRoom>();

		private readonly SocketClient _socketClient;

		private readonly GameConfigData _gameConfigData;

		public bool IsConnected => _socketClient.IsConnected;

		public bool IsAuthenticated => _socketClient.IsAuthenticated;

		public SystemChatRoom SystemChatRoom { get; private set; }

		public PrivateChatRoom PrivateChatRoom { get; private set; }

		public event Action Connected
		{
			add
			{
				_socketClient.Connected += value;
			}
			remove
			{
				_socketClient.Connected -= value;
			}
		}

		public event Action Disconnected
		{
			add
			{
				_socketClient.Disconnected += value;
			}
			remove
			{
				_socketClient.Disconnected -= value;
			}
		}

		public event Action Reconnected
		{
			add
			{
				_socketClient.Reconnected += value;
			}
			remove
			{
				_socketClient.Reconnected -= value;
			}
		}

		public event Action Authenticated
		{
			add
			{
				_socketClient.Authenticated += value;
			}
			remove
			{
				_socketClient.Authenticated -= value;
			}
		}

		private event Action<PublicChatRoom> _channelJoined;

		public event Action<PublicChatRoom> ChannelJoined
		{
			add
			{
				_channelJoined += value;
			}
			remove
			{
				_channelJoined -= value;
			}
		}

		public event Action<string> ChannelLeft
		{
			add
			{
				_socketClient.ChannelLeft += value;
			}
			remove
			{
				_socketClient.ChannelLeft -= value;
			}
		}

		public ChatClient(SocketClient socketClient, GameConfigData gameConfigData)
		{
			_socketClient = socketClient;
			_gameConfigData = gameConfigData;
			SystemChatRoom = new SystemChatRoom(this, _gameConfigData);
			PrivateChatRoom = new PrivateChatRoom(this, _gameConfigData);
			_socketClient.ChannelJoined += OnChannelJoined;
			_socketClient.ChannelLeft += OnChannelLeft;
			Init();
		}

		private void RaiseChannelJoined(PublicChatRoom channel)
		{
			this._channelJoined?.Invoke(channel);
		}

		public void Disconnect()
		{
			Cleanup();
		}

		public PublicChatRoom JoinChannel(string channelId)
		{
			Debug.Log("[ChatClient] Joining channel " + channelId);
			if (!ChatRooms.TryGetValue(channelId, out var value))
			{
				if (_socketClient.TryGetChannelById(channelId, out var channel))
				{
					value = new PublicChatRoom(this, channel, _gameConfigData, _socketClient.ServiceId);
				}
				else
				{
					value = new PublicChatRoom(this, channelId, _gameConfigData, _socketClient.ServiceId);
					if (channelId != ChatController.GENERAL_CHANNEL)
					{
						DelayedAction.RunWhen(() => _socketClient.IsAuthenticated, delegate
						{
							_socketClient.JoinChannel(channelId);
						});
					}
				}
			}
			return value;
		}

		public void LeaveChannel(string channelId)
		{
			_socketClient.LeaveChannel(channelId);
		}

		public void SendPublicMessage(string channelId, string message)
		{
			PublicMessageRequest publicMessageRequest = new PublicMessageRequest
			{
				c = channelId,
				m = message
			};
			_socketClient.Emit(EventNames.PublicMessage, publicMessageRequest);
		}

		public void SendPrivateMessage(string recipientId, string message)
		{
			PrivateMessageRequest privateMessageRequest = new PrivateMessageRequest
			{
				i = recipientId,
				m = message
			};
			_socketClient.Emit(EventNames.PrivateMessage, privateMessageRequest);
		}

		public void DisplaySystemMessage(string text)
		{
			SystemChatRoom.OnMessageReceived(new SystemMessage(text));
		}

		public bool TryGetServiceIdByNicknameFromAnyChannel(string nickname, out string serviceId)
		{
			return _socketClient.TryGetServiceIdByNicknameFromAnyChannel(nickname, out serviceId);
		}

		public bool TryGetChannelById(string roomId, out PublicChatRoom room)
		{
			return ChatRooms.TryGetValue(roomId, out room);
		}

		private void Init()
		{
			_socketClient.On(EventNames.PublicMessage, OnPublicMessage);
			_socketClient.On(EventNames.PrivateMessage, OnPrivateMessage);
		}

		private void Cleanup()
		{
			_socketClient.Off(EventNames.PublicMessage, OnPublicMessage);
			_socketClient.Off(EventNames.PrivateMessage, OnPrivateMessage);
		}

		private void OnChannelJoined(ChannelCrumb channelCrumb)
		{
			if (ChatRooms.TryGetValue(channelCrumb.Id, out var value))
			{
				value.AssignChannel(channelCrumb);
			}
			else
			{
				value = new PublicChatRoom(this, channelCrumb, _gameConfigData, _socketClient.ServiceId);
				ChatRooms.Add(channelCrumb.Id, value);
			}
			Debug.Log("[ChatClient] Joined channel " + channelCrumb.Id);
			RaiseChannelJoined(value);
		}

		private void OnChannelLeft(string channelId)
		{
			if (ChatRooms.TryGetValue(channelId, out var value))
			{
				value.RemoveChannel();
			}
		}

		private void OnPublicMessage(string payload, object[] args)
		{
			if (args == null || args.Length == 0)
			{
				Debug.LogWarning("[ChatClient] Received public message with incomplete data");
				return;
			}
			PublicMessageCrumb publicMessageCrumb = SocketClient.ConvertTo<PublicMessageCrumb>(args[0]);
			if (_socketClient.TryGetChannelById(publicMessageCrumb.channelId, out var channel))
			{
				string senderNickname = "";
				PlayerCrumb player;
				if (string.IsNullOrEmpty(publicMessageCrumb.senderId))
				{
					Debug.LogErrorFormat("[ChatClient] Received public message, but senderId was null: {0}", publicMessageCrumb);
				}
				else if (channel.TryGetPlayerById(publicMessageCrumb.senderId, out player))
				{
					senderNickname = player.Nickname;
				}
				channel.RaiseMessageReceived(publicMessageCrumb.ToMessage(senderNickname, _socketClient.ServiceId));
			}
			else
			{
				Debug.LogWarning("[ChatClient] Received public message for a channel not subscribed to.");
			}
		}

		private void OnPrivateMessage(string payload, object[] args)
		{
			PrivateMessage message = SocketClient.ConvertTo<PrivateMessageCrumb>(args[0]).ToMessage(_socketClient.ServiceId);
			PrivateChatRoom.OnMessageReceived(message);
		}
	}
}
