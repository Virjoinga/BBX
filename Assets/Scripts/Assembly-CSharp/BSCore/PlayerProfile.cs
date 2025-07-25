using System.Collections.Generic;
using BSCore.Constants;
using PlayFab.ClientModels;
using Zenject;

namespace BSCore
{
	public class PlayerProfile
	{
		public struct Data
		{
			public string serviceId;

			public string sessionTicket;

			public string displayName;

			public bool hasUniversalAccount;

			public EntityKey entity;
		}

		private Dictionary<string, string> _userData = new Dictionary<string, string>();

		private Dictionary<string, string> _readOnlyData = new Dictionary<string, string>();

		private Dictionary<StatisticKey, int> _statistics = new Dictionary<StatisticKey, int>();

		public string Id { get; private set; }

		public bool IsAdmin { get; private set; }

		public string SessionTicket { get; private set; }

		public string DisplayName { get; set; }

		public IDictionary<string, string> PlayerData { get; private set; }

		public bool DataPopulated { get; private set; }

		public bool HasDisplayName => !string.IsNullOrEmpty(DisplayName);

		public bool HasUniversalAccount { get; private set; }

		public bool HasMinimumData
		{
			get
			{
				if (DataPopulated)
				{
					return HasDisplayName;
				}
				return false;
			}
		}

		public EntityKey Entity { get; private set; }

		public PlayerLoadoutManager LoadoutManager { get; private set; }

		public PlayerProfile(Data profileData, DiContainer container)
		{
			Id = profileData.serviceId;
			SessionTicket = profileData.sessionTicket;
			DisplayName = profileData.displayName;
			HasUniversalAccount = profileData.hasUniversalAccount;
			Entity = profileData.entity;
			LoadoutManager = new PlayerLoadoutManager(container);
		}

		public void OnDataFetched(Dictionary<string, string> data, Dictionary<string, string> readOnlyData)
		{
			_userData = data;
			_readOnlyData = readOnlyData;
			IsAdmin = GetData(User.DataKeys.isAdmin, defaultValue: false);
			LoadoutManager.PopulateLoadouts(data);
		}

		public void OnStatisticsFetched(Dictionary<StatisticKey, int> statistics)
		{
			_statistics = statistics;
		}

		public bool HasData(User.DataKeys key)
		{
			if (!_userData.ContainsKey(key.ToString()))
			{
				return _readOnlyData.ContainsKey(key.ToString());
			}
			return true;
		}

		public string GetData(User.DataKeys key, string defaultValue = "")
		{
			if (!_userData.TryGetValue(key.ToString(), out var value) && !_readOnlyData.TryGetValue(key.ToString(), out value))
			{
				return defaultValue;
			}
			return value;
		}

		public int GetData(User.DataKeys key, int defautlValue = 0)
		{
			if (!int.TryParse(GetData(key, "0"), out var result))
			{
				return defautlValue;
			}
			return result;
		}

		public float GetData(User.DataKeys key, float defautlValue = 0f)
		{
			if (!float.TryParse(GetData(key, "0.0"), out var result))
			{
				return defautlValue;
			}
			return result;
		}

		public bool GetData(User.DataKeys key, bool defaultValue = false)
		{
			string text = GetData(key, defaultValue.ToString()).ToLower();
			if (!(text == "true"))
			{
				return text == "1";
			}
			return true;
		}

		public bool HasStatistic(StatisticKey key)
		{
			return _statistics.ContainsKey(key);
		}

		public int GetStatistic(StatisticKey key)
		{
			if (!_statistics.TryGetValue(key, out var value))
			{
				return 0;
			}
			return value;
		}
	}
}
