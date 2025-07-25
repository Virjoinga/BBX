using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BSCore.Constants.Config;
using UnityEngine;
using Zenject;

namespace BSCore
{
	public class ConfigManager
	{
		private IConfigService _configService;

		private Dictionary<string, string> _data = new Dictionary<string, string>();

		public bool HasFetched { get; private set; }

		public IDictionary<string, string> Data => new Dictionary<string, string>(_data);

		private event Action _fetched;

		public event Action Fetched
		{
			add
			{
				_fetched += value;
			}
			remove
			{
				_fetched -= value;
			}
		}

		[Inject]
		public ConfigManager(IConfigService configService)
		{
			_configService = configService;
		}

		public void Fetch(Action onSuccess, Action<FailureReasons> onFailure)
		{
			Fetch(null, onSuccess, onFailure);
		}

		public void Fetch(List<string> keys, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_configService.Fetch(keys, onSuccessWrapper, onFailure);
			void onSuccessWrapper(Dictionary<string, string> data)
			{
				HasFetched = true;
				_data = data;
				onSuccess();
				this._fetched?.Invoke();
			}
		}

		public T Get<T>(DataKeys key) where T : class, new()
		{
			if (!typeof(T).IsSerializable && !typeof(ISerializable).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException($"[ConfigManager.Get<T>()] Type {typeof(T).GetType()} is not serializable.");
			}
			string text = Get(key, "");
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return JsonUtility.FromJson<T>(text);
		}

		public string Get(DataKeys key, string defaultValue = "")
		{
			string value = defaultValue;
			_data.TryGetValue(key.ToString(), out value);
			return value;
		}

		public int Get(DataKeys key, int defaultValue = 0)
		{
			int result = defaultValue;
			if (_data.TryGetValue(key.ToString(), out var value))
			{
				int.TryParse(value, out result);
			}
			return result;
		}

		public bool Get(DataKeys key, bool defaultValue = false)
		{
			return Get(key, 0) != 0;
		}

		public float Get(DataKeys key, float defaultValue = 0f)
		{
			float result = defaultValue;
			if (_data.TryGetValue(key.ToString(), out var value))
			{
				float.TryParse(value, out result);
			}
			return result;
		}

		public DateTime Get(DataKeys key, DateTime defaultValue)
		{
			return DateTimeExtensions.FromUnixTimeStamp(Get(key, 0));
		}
	}
}
