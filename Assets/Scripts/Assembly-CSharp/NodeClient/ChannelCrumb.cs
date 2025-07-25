using System;
using System.Collections.Generic;
using System.Linq;

namespace NodeClient
{
	[Serializable]
	public class ChannelCrumb
	{
		public string error;

		public string c;

		public List<PlayerCrumb> p;

		public bool IsError => !string.IsNullOrEmpty(error);

		public string Id => c;

		public List<PlayerCrumb> PlayerList => p;

		private event Action _updated;

		public event Action Updated
		{
			add
			{
				_updated += value;
			}
			remove
			{
				_updated -= value;
			}
		}

		private event Action<Message> _messageRecieved;

		public event Action<Message> MessageRecieved
		{
			add
			{
				_messageRecieved += value;
			}
			remove
			{
				_messageRecieved -= value;
			}
		}

		private void RaiseUpdated()
		{
			this._updated?.Invoke();
		}

		public void RaiseMessageReceived(Message message)
		{
			this._messageRecieved?.Invoke(message);
		}

		public void AddPlayer(PlayerCrumb player)
		{
			if (!PlayerList.Contains(player))
			{
				PlayerList.Add(player);
				RaiseUpdated();
			}
		}

		public void RemovePlayer(string playerId)
		{
			PlayerList.RemoveAll((PlayerCrumb p) => p.Id == playerId);
			RaiseUpdated();
		}

		public void UpdatePlayer(PlayerCrumb player)
		{
			int index = PlayerList.IndexOf(player);
			PlayerList[index] = player;
			RaiseUpdated();
		}

		public bool TryGetPlayerById(string playerId, out PlayerCrumb player)
		{
			player = PlayerList.FirstOrDefault((PlayerCrumb p) => p.Id == playerId);
			return player != null;
		}

		public PlayerCrumb GetPlayerById(string playerId)
		{
			TryGetPlayerById(playerId, out var player);
			return player;
		}
	}
}
