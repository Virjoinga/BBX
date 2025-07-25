using System;

namespace BSCore
{
	public abstract class BaseDataStoreService<T>
	{
		protected T _value;

		public string Key { get; private set; }

		public T Default { get; protected set; }

		public T Value
		{
			get
			{
				return _value;
			}
			set
			{
				ref T value2 = ref _value;
				object obj = value;
				if (!value2.Equals(obj))
				{
					_value = value;
					Persist();
					this._changed?.Invoke(_value);
				}
			}
		}

		protected event Action<T> _changed;

		public event Action<T> Changed
		{
			add
			{
				_changed += value;
			}
			remove
			{
				_changed -= value;
			}
		}

		public BaseDataStoreService(string key, T defaultValue)
		{
			Key = key;
			Default = defaultValue;
			_value = Default;
			Fetch();
		}

		public void Listen(Action<T> callback)
		{
			Changed += callback;
		}

		public void ListenAndInvoke(Action<T> callback)
		{
			Listen(callback);
			callback(Value);
		}

		public void Unlisten(Action<T> callback)
		{
			Changed -= callback;
		}

		public void SetToDefault()
		{
			Value = Default;
		}

		public void SetNewDefault(T value)
		{
			Default = value;
		}

		protected abstract void Persist();

		protected abstract void Fetch();
	}
}
