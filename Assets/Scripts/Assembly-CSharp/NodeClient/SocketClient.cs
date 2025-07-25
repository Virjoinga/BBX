using System;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using BestHTTP;
using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;
using BestHTTP.SocketIO.JsonEncoders;
using LitJson;
using UnityEngine;
using Zenject;

namespace NodeClient
{
	public class SocketClient
	{
		public enum ErrorType
		{
			ReconnectFailed = 0,
			ConnectFailed = 1,
			UserBanned = 2,
			MessageFailed = 3,
			CouldNotFindUser = 4,
			JoinChannelFailed = 5,
			Unknown = 6
		}

		private readonly SignalBus _signalBus;

		private readonly UserManager _userManager;

		private SocketManager _manager;

		private bool _managerStarted;

		private readonly Dictionary<string, ChannelCrumb> _channelsById = new Dictionary<string, ChannelCrumb>();

		private PlayerStatus _lastStatus;

		private readonly List<string> _channelJoinsInProgress = new List<string>();

		private readonly Dictionary<Action<string, object[]>, SocketIOCallback> _socketCallbacksByCallback = new Dictionary<Action<string, object[]>, SocketIOCallback>();

		private readonly Dictionary<string, SocketIOCallback> _responseCallbacksByEventName = new Dictionary<string, SocketIOCallback>();

		public bool IsConnected { get; private set; }

		public bool IsAuthenticated { get; private set; }

		public string ServiceId { get; private set; }

		private event Action _connected;

		public event Action Connected
		{
			add
			{
				_connected += value;
			}
			remove
			{
				_connected -= value;
			}
		}

		private event Action _disconnected;

		public event Action Disconnected
		{
			add
			{
				_disconnected += value;
			}
			remove
			{
				_disconnected -= value;
			}
		}

		private event Action _reconnecting;

		public event Action Reconnecting
		{
			add
			{
				_reconnecting += value;
			}
			remove
			{
				_reconnecting -= value;
			}
		}

		private event Action _reconnected;

		public event Action Reconnected
		{
			add
			{
				_reconnected += value;
			}
			remove
			{
				_reconnected -= value;
			}
		}

		private event Action _reconnectFailed;

		public event Action ReconnectFailed
		{
			add
			{
				_reconnectFailed += value;
			}
			remove
			{
				_reconnectFailed -= value;
			}
		}

		private event Action _authenticated;

		public event Action Authenticated
		{
			add
			{
				_authenticated += value;
			}
			remove
			{
				_authenticated -= value;
			}
		}

		private event Action<ErrorType> _error;

		public event Action<ErrorType> Error
		{
			add
			{
				_error += value;
			}
			remove
			{
				_error -= value;
			}
		}

		private event Action<ChannelCrumb> _channelJoined;

		public event Action<ChannelCrumb> ChannelJoined
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

		private event Action<string> _channelLeft;

		public event Action<string> ChannelLeft
		{
			add
			{
				_channelLeft += value;
			}
			remove
			{
				_channelLeft -= value;
			}
		}

		public static string ToJson(object obj)
		{
			JsonWriter jsonWriter = new JsonWriter();
			JsonMapper.ToJson(obj, jsonWriter);
			return jsonWriter.ToString();
		}

		public static T FromJson<T>(string json)
		{
			return JsonMapper.ToObject<T>(json);
		}

		public static T ConvertTo<T>(object obj)
		{
			return FromJson<T>(ToJson(obj));
		}

		public SocketClient(GameConfigData gameConfigData, SignalBus signalBus, UserManager userManager)
		{
			_signalBus = signalBus;
			_userManager = userManager;
			Setup(gameConfigData.ChatServerAddress);
		}

		private void RaiseConnected()
		{
			this._connected?.Invoke();
			_signalBus.Fire(new SocketConnectedSignal());
		}

		private void RaiseDisconnected()
		{
			this._disconnected?.Invoke();
			_signalBus.Fire(new SocketDisconnectedSignal());
		}

		private void RaiseReconnecting()
		{
			this._reconnecting?.Invoke();
			_signalBus.Fire(new SocketReconnectingSignal());
		}

		private void RaiseReconnected()
		{
			this._reconnected?.Invoke();
			_signalBus.Fire(new SocketReconnectedSignal());
		}

		private void RaiseReconnectFailed()
		{
			this._reconnectFailed?.Invoke();
			RaiseError(ErrorType.ReconnectFailed);
		}

		private void RaiseAuthenticated()
		{
			this._authenticated?.Invoke();
			_signalBus.Fire(new SocketAuthenticatedSignal());
		}

		private void RaiseError(ErrorType errorType)
		{
			this._error?.Invoke(errorType);
			_signalBus.Fire(new SocketErrorSignal(errorType));
		}

		private void RaiseChannelJoined(ChannelCrumb channel)
		{
			this._channelJoined?.Invoke(channel);
			_signalBus.Fire(new SocketChannelJoinedSignal(channel));
		}

		private void RaiseChannelLeft(string channelId)
		{
			this._channelLeft?.Invoke(channelId);
			_signalBus.Fire(new SocketChannelLeftSignal(channelId));
		}

		private void Setup(string address)
		{
			if (string.IsNullOrEmpty(address) || !address.EndsWith("/socket.io/"))
			{
				Debug.LogError("[SocketClient] '" + address + "' is not a Socket.IO address (must end with '/socket.io/'). Please fix.");
				return;
			}
			SocketOptions options = new SocketOptions
			{
				ReconnectionAttempts = 5,
				Reconnection = true,
				Timeout = TimeSpan.FromMilliseconds(10000.0),
				AutoConnect = false
			};
			_manager = new SocketManager(new Uri(address), options)
			{
				Encoder = new LitJsonEncoder()
			};
			JsonMapper.RegisterImporter((double input) => (int)(input + 0.5));
			HTTPUpdateDelegator.OnBeforeApplicationQuit = OnBeforeApplicationQuit(HTTPUpdateDelegator.OnBeforeApplicationQuit);
			_manager.Socket.On(SocketIOEventTypes.Connect, OnServerConnect);
			_manager.Socket.On(SocketIOEventTypes.Disconnect, OnServerDisconnect);
			_manager.Socket.On(SocketIOEventTypes.Error, OnError);
			_manager.Socket.On(EventNames.Reconnect, OnReconnect);
			_manager.Socket.On(EventNames.Reconnecting, OnReconnecting);
			_manager.Socket.On(EventNames.ReconnectAttempt, OnReconnectAttempt);
			_manager.Socket.On(EventNames.ReconnectFailed, OnReconnectFailed);
			_manager.Socket.On(EventNames.LoggedIn, OnAuthenticated);
			_manager.Socket.On(EventNames.JoinedChannel, OnJoinedChannel);
			_manager.Socket.On(EventNames.LeftChannel, OnLeftChannel);
			_manager.Socket.On(EventNames.PlayerAdded, OnPlayerAdded);
			_manager.Socket.On(EventNames.PlayerRemoved, OnPlayerRemoved);
			_manager.Socket.On(EventNames.PlayerUpdated, OnPlayerUpdated);
			_manager.Open();
			_managerStarted = true;
		}

		private void Cleanup()
		{
			_manager.Socket.Off(SocketIOEventTypes.Connect, OnServerConnect);
			_manager.Socket.Off(SocketIOEventTypes.Disconnect, OnServerDisconnect);
			_manager.Socket.Off(SocketIOEventTypes.Error, OnError);
			_manager.Socket.Off(EventNames.Reconnect, OnReconnect);
			_manager.Socket.Off(EventNames.Reconnecting, OnReconnecting);
			_manager.Socket.Off(EventNames.ReconnectAttempt, OnReconnectAttempt);
			_manager.Socket.Off(EventNames.ReconnectFailed, OnReconnectFailed);
			_manager.Socket.Off(EventNames.LoggedIn, OnAuthenticated);
			_manager.Socket.Off(EventNames.JoinedChannel, OnJoinedChannel);
			_manager.Socket.Off(EventNames.LeftChannel, OnLeftChannel);
			_manager.Socket.Off(EventNames.PlayerAdded, OnPlayerAdded);
			_manager.Socket.Off(EventNames.PlayerRemoved, OnPlayerRemoved);
			_manager.Socket.Off(EventNames.PlayerUpdated, OnPlayerUpdated);
			_manager.Close();
			_managerStarted = false;
		}

		private Func<bool> OnBeforeApplicationQuit(Func<bool> currentMethod)
		{
			return delegate
			{
				Cleanup();
				return currentMethod == null || currentMethod();
			};
		}

		private void OnServerConnect(Socket socket, Packet packet, object[] args)
		{
			IsConnected = true;
			RaiseConnected();
			if (_userManager.CurrentUser != null && _userManager.IsValidDisplayName(_userManager.CurrentUser.DisplayName))
			{
				Authenticate(_userManager.CurrentUser.Id, _userManager.CurrentUser.SessionTicket);
			}
			else
			{
				_userManager.UserPropertiesFilled += onUserFetched;
			}
			void onUserFetched()
			{
				Authenticate(_userManager.CurrentUser.Id, _userManager.CurrentUser.SessionTicket);
				_userManager.UserPropertiesFilled -= onUserFetched;
			}
		}

		private void OnServerDisconnect(Socket socket, Packet packet, object[] args)
		{
			IsConnected = false;
			IsAuthenticated = false;
			RaiseDisconnected();
			foreach (string key in _channelsById.Keys)
			{
				RaiseChannelLeft(key);
			}
			_channelsById.Clear();
		}

		private void OnError(Socket socket, Packet packet, object[] args)
		{
			RaiseError(ErrorType.Unknown);
		}

		private void OnReconnect(Socket socket, Packet packet, object[] args)
		{
			RaiseReconnected();
		}

		private void OnReconnecting(Socket socket, Packet packet, object[] args)
		{
			IsConnected = false;
			RaiseReconnecting();
		}

		private void OnReconnectAttempt(Socket socket, Packet packet, object[] args)
		{
		}

		private void OnReconnectFailed(Socket socket, Packet packet, object[] args)
		{
			RaiseReconnectFailed();
		}

		private void OnAuthenticated(Socket socket, Packet packet, object[] args)
		{
			IsAuthenticated = true;
			RaiseAuthenticated();
			UpdateStatus(_lastStatus);
		}

		private void OnJoinedChannel(Socket socket, Packet packet, object[] args)
		{
			ChannelCrumb channelCrumb = ConvertTo<ChannelCrumb>(args[0]);
			_channelJoinsInProgress.Remove(channelCrumb.Id);
			if (channelCrumb.IsError)
			{
				RaiseError(ErrorType.JoinChannelFailed);
				return;
			}
			Debug.Log("[SocketClient] Joined channel: " + channelCrumb.Id);
			_channelsById.Add(channelCrumb.Id, channelCrumb);
			RaiseChannelJoined(channelCrumb);
		}

		private void OnLeftChannel(Socket socket, Packet packet, object[] args)
		{
			LeftChannel leftChannel = ConvertTo<LeftChannel>(args[0]);
			if (_channelsById.ContainsKey(leftChannel.channelId))
			{
				_channelsById.Remove(leftChannel.channelId);
				RaiseChannelLeft(leftChannel.channelId);
			}
		}

		private void OnPlayerAdded(Socket socket, Packet packet, object[] args)
		{
			PlayerJoinedChannel playerJoinedChannel = ConvertTo<PlayerJoinedChannel>(args[0]);
			if (_channelsById.TryGetValue(playerJoinedChannel.channelId, out var value))
			{
				value.AddPlayer(playerJoinedChannel.player);
			}
		}

		private void OnPlayerRemoved(Socket socket, Packet packet, object[] args)
		{
			PlayerLeftChannel playerLeftChannel = ConvertTo<PlayerLeftChannel>(args[0]);
			if (_channelsById.TryGetValue(playerLeftChannel.channelId, out var value))
			{
				value.RemovePlayer(playerLeftChannel.playerId);
			}
		}

		private void OnPlayerUpdated(Socket socket, Packet packet, object[] args)
		{
			UpdatePlayerInChannel updatePlayerInChannel = ConvertTo<UpdatePlayerInChannel>(args[0]);
			if (_channelsById.TryGetValue(updatePlayerInChannel.channelId, out var value))
			{
				value.UpdatePlayer(updatePlayerInChannel.player);
			}
		}

		public void On(string eventName, Action<string, object[]> callback)
		{
			_socketCallbacksByCallback.Add(callback, delegate(Socket socket, Packet packet, object[] args)
			{
				Debug.Log("[SocketClient] Received event " + eventName + ":\n" + packet.Payload);
				callback(packet.Payload, args);
			});
			_manager.Socket.On(eventName, _socketCallbacksByCallback[callback]);
		}

		public void Off(string eventName, Action<string, object[]> callback)
		{
			if (_socketCallbacksByCallback.TryGetValue(callback, out var value))
			{
				_socketCallbacksByCallback.Remove(callback);
				_manager.Socket.Off(eventName, value);
			}
		}

		public void Once(string eventName, Action<string, object[]> callback)
		{
			_manager.Socket.Once(eventName, delegate(Socket socket, Packet packet, object[] args)
			{
				Debug.Log("[SocketClient] Received event " + eventName + ":\n" + packet.Payload);
				callback(packet.Payload, args);
			});
		}

		public void Emit(string eventName, params object[] args)
		{
			_manager.Socket.Emit(eventName, args);
		}

		public void OnResponse<TResponse>(string eventName) where TResponse : ISocketClientResponse
		{
			_responseCallbacksByEventName.Add(eventName, delegate(Socket socket, Packet packet, object[] args)
			{
				Debug.Log("[SocketClient] Received event " + eventName + ":\n" + packet.Payload);
				SocketClientRequestTracker.HandleResponse(ConvertTo<TResponse>(args[0]));
			});
			_manager.Socket.On(eventName, _responseCallbacksByEventName[eventName]);
		}

		public void OffResponse<TResponse>(string eventName) where TResponse : ISocketClientResponse
		{
			if (_responseCallbacksByEventName.TryGetValue(eventName, out var value))
			{
				_responseCallbacksByEventName.Remove(eventName);
				_manager.Socket.Off(eventName, value);
			}
		}

		public void Request<TRequest, TResponse>(string eventName, TRequest request, Action<TRequest, TResponse> onResponse, Action<string> onFailure) where TRequest : ISocketClientRequest where TResponse : ISocketClientResponse
		{
			Action<ISocketClientRequest, ISocketClientResponse> onResponse2 = delegate(ISocketClientRequest req, ISocketClientResponse res)
			{
				onResponse((TRequest)req, (TResponse)res);
			};
			SocketClientRequestTracker.RegisterRequest(request, onResponse2, onFailure);
			try
			{
				Emit(eventName, request);
			}
			catch (Exception ex)
			{
				onFailure(ex.Message);
			}
		}

		public void Request<TRequest, TResponse>(string eventName, TRequest request, Action<TResponse> onResponse, Action<string> onFailure) where TRequest : ISocketClientRequest where TResponse : ISocketClientResponse
		{
			Action<ISocketClientRequest, ISocketClientResponse> onResponse2 = delegate(ISocketClientRequest req, ISocketClientResponse res)
			{
				onResponse((TResponse)res);
			};
			SocketClientRequestTracker.RegisterRequest(request, onResponse2, onFailure);
			try
			{
				Emit(eventName, request);
			}
			catch (Exception ex)
			{
				onFailure(ex.Message);
			}
		}

		public bool TryGetServiceIdByNicknameFromAnyChannel(string nickname, out string serviceId)
		{
			serviceId = "";
			foreach (ChannelCrumb value in _channelsById.Values)
			{
				PlayerCrumb playerCrumb = value.PlayerList.FirstOrDefault((PlayerCrumb p) => p.Nickname.ToLower() == nickname);
				if (playerCrumb != null)
				{
					serviceId = playerCrumb.Id;
					return true;
				}
			}
			return false;
		}

		public void Authenticate(string serviceId, string ticket)
		{
			Debug.Log("[SocketClient] Authenticating with server (serviceId: " + serviceId + ")");
			ServiceId = serviceId;
			Dictionary<string, object> dictionary = new Dictionary<string, object> { { "SessionTicket", ticket } };
			_manager.Socket.Emit(EventNames.Login, dictionary);
		}

		public void Disconnect()
		{
			Debug.Log("[SocketClient] Disconnect called");
			if (_managerStarted)
			{
				Cleanup();
			}
		}

		public bool TryGetChannelById(string channelId, out ChannelCrumb channel)
		{
			return _channelsById.TryGetValue(channelId, out channel);
		}

		public void UpdateStatus(PlayerStatus status)
		{
			Debug.Log($"[SocketClient] Updating status to {status}");
			_manager.Socket.Emit(EventNames.UpdatePlayerStatus, new UpdateStatusRequest(status));
			_lastStatus = status;
		}

		public void JoinChannel(string channelId)
		{
			if (!_channelJoinsInProgress.Contains(channelId))
			{
				_channelJoinsInProgress.Add(channelId);
				Debug.Log("[SocketClient] Subscribing(" + EventNames.JoinChannel + ") to channel: " + channelId);
				JoinChannelRequest joinChannelRequest = new JoinChannelRequest
				{
					c = channelId
				};
				_manager.Socket.Emit(EventNames.JoinChannel, joinChannelRequest);
			}
		}

		public void LeaveChannel(string channelId)
		{
			Debug.Log("[SocketClient] Unsubscribing(" + EventNames.JoinChannel + ") from channel: " + channelId);
			LeaveChannelRequest leaveChannelRequest = new LeaveChannelRequest
			{
				c = channelId
			};
			_manager.Socket.Emit(EventNames.LeaveChannel, leaveChannelRequest);
		}
	}
}
