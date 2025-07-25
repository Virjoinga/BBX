using System;
using System.Collections.Generic;
using NodeClient;
using UnityEngine;
using Zenject;

namespace BSCore.Chat
{
	public class PublicChatRoom : BaseChatRoom
	{
		public class Factory : PlaceholderFactory<string, Color, ChatClient, SignalBus, GameConfigData, PublicChatRoom>
		{
		}

		private readonly string _serviceId;

		private readonly string _myNameColor;

		private readonly string _adminNameColor;

		private readonly string _otherNameColor;

		private ChannelCrumb _channel;

		private string _channelId;

		public IEnumerable<PlayerCrumb> PlayersInRoom => _channel?.PlayerList ?? new List<PlayerCrumb>();

		public override string Id => _channel?.Id ?? _channelId;

		private event Action _roomMembersChanged;

		public event Action RoomMembersChanged
		{
			add
			{
				_roomMembersChanged += value;
			}
			remove
			{
				_roomMembersChanged -= value;
			}
		}

		public PublicChatRoom(ChatClient chatClient, string channelId, GameConfigData gameConfigData, string serviceId)
			: base(gameConfigData.PublicMessageColor, chatClient, gameConfigData)
		{
			_channelId = channelId;
			_serviceId = serviceId;
			_myNameColor = ColorToHex(gameConfigData.MyMessageNameColor);
			_adminNameColor = ColorToHex(gameConfigData.AdminMessageNameColor);
			_otherNameColor = ColorToHex(gameConfigData.OtherMessageNameColor);
		}

		public PublicChatRoom(ChatClient chatClient, ChannelCrumb channel, GameConfigData gameConfigData, string serviceId)
			: this(chatClient, channel.Id, gameConfigData, serviceId)
		{
			AssignChannel(channel);
		}

		~PublicChatRoom()
		{
			if (_channel != null)
			{
				_channel.MessageRecieved -= OnMessageReceived;
				_channel.Updated -= RaiseRoomMembersChanged;
			}
		}

		protected void RaiseRoomMembersChanged()
		{
			this._roomMembersChanged?.Invoke();
		}

		public void AssignChannel(ChannelCrumb channelCrumb)
		{
			Debug.Log("[PublicChatRoom] Channel assigned, updating players in room");
			_channel = channelCrumb;
			if (_channel != null)
			{
				_channel.MessageRecieved += OnMessageReceived;
				_channel.Updated += RaiseRoomMembersChanged;
			}
			RaiseRoomMembersChanged();
		}

		public void RemoveChannel()
		{
			Debug.Log("[PublicChatRoom] Channel removed, updating players in room");
			_channel = null;
			RaiseRoomMembersChanged();
		}

		public void SendMessage(string message)
		{
			_chatClient.SendPublicMessage(Id, message);
		}

		public void OnMessageReceived(Message message)
		{
			PublicMessage publicMessage = new PublicMessage(message.senderId, message.message, message.senderNickname, message.channelId, message.isAdmin, _serviceId);
			string text = publicMessage.senderNickname;
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogError("[PublicChatRoom] Message sent, but the senderNickname was an empty string");
				text = "Name Missing";
			}
			string text2 = _otherNameColor;
			if (publicMessage.isMe)
			{
				text2 = _myNameColor;
			}
			else if (publicMessage.isAdmin)
			{
				text2 = _adminNameColor;
			}
			RaiseMessageReceived("<font=\"PepsiFont\"><color=#" + text2 + ">" + text + "</color></font>: ", publicMessage.message);
		}
	}
}
