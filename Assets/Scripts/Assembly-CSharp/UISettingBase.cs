using System;
using BSCore;
using UnityEngine;
using Zenject;

public abstract class UISettingBase : MonoBehaviour
{
	[Inject]
	protected DataStoreManager _dataStoreManager;

	[SerializeField]
	private bool _mobileFriendly = true;

	[SerializeField]
	protected DataStoreKeys _dataStoreKey;

	public bool MobileFriendly => _mobileFriendly;

	private event Action _settingChanged;

	public event Action SettingChanged
	{
		add
		{
			_settingChanged += value;
		}
		remove
		{
			_settingChanged -= value;
		}
	}

	[Inject]
	private void Construct()
	{
		Setup();
	}

	protected abstract void Setup();

	public abstract void Init();

	public abstract bool IsDirty();

	public abstract void SetDefaultValue();

	public abstract void ResetToSavedValue();

	public abstract void SaveToDataStore();

	protected void RaiseSettingChanged()
	{
		this._settingChanged?.Invoke();
	}
}
