using System;
using System.Collections.Generic;
using UnityEngine;

namespace BSCore.Chat
{
	public abstract class BaseChatRoom
	{
		private const int CACHED_MESSAGE_LIMIT = 50;

		protected readonly string _color;

		protected readonly ChatClient _chatClient;

		protected readonly GameConfigData _gameConfigData;

		protected Queue<string> _cachedMessages = new Queue<string>();

		public abstract string Id { get; }

		public List<string> CachedMessages => new List<string>(_cachedMessages);

		private event Action<string> _messageReceived;

		public event Action<string> MessageReceived
		{
			add
			{
				_messageReceived += value;
			}
			remove
			{
				_messageReceived -= value;
			}
		}

		public BaseChatRoom(ChatClient chatClient, GameConfigData gameConfigData)
		{
			_chatClient = chatClient;
			_gameConfigData = gameConfigData;
		}

		public BaseChatRoom(Color color, ChatClient chatClient, GameConfigData gameConfigData)
		{
			_color = ColorToHex(color);
			_chatClient = chatClient;
			_gameConfigData = gameConfigData;
		}

		protected void RaiseMessageReceived(string displayName, string message)
		{
			message = displayName + "<color=#" + _color + ">" + message + "</color>";
			_cachedMessages.Enqueue(message);
			if (_cachedMessages.Count > 50)
			{
				_cachedMessages.Dequeue();
			}
			this._messageReceived?.Invoke(message);
		}

		protected string ColorToHex(Color color)
		{
			return ColorUtility.ToHtmlStringRGB(color);
		}
	}
}
