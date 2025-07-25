using BSCore;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsToggle : UISettingBase
{
	[SerializeField]
	private GameObject _onDisplay;

	[SerializeField]
	private GameObject _offDisplay;

	private DataStoreBool _dataStore;

	private Toggle _toggle;

	private bool _wasOn;

	protected override void Setup()
	{
		_dataStore = _dataStoreManager.GetStore<DataStoreBool, bool>(_dataStoreKey);
	}

	public override void Init()
	{
		_toggle = GetComponentInChildren<Toggle>();
		_toggle.onValueChanged.AddListener(OnValueChanged);
		ResetToSavedValue();
	}

	private void OnValueChanged(bool isOn)
	{
		UpdateValueDisplay(isOn);
		if (isOn != _wasOn)
		{
			RaiseSettingChanged();
			_wasOn = isOn;
		}
	}

	private void UpdateValueDisplay(bool value)
	{
		_onDisplay.SetActive(value);
		_offDisplay.SetActive(!value);
	}

	public override bool IsDirty()
	{
		if (_dataStore == null)
		{
			return false;
		}
		return _dataStore.Value != _toggle.isOn;
	}

	public override void ResetToSavedValue()
	{
		_wasOn = _dataStore.Value;
		_toggle.isOn = _dataStore.Value;
		UpdateValueDisplay(_toggle.isOn);
	}

	public override void SaveToDataStore()
	{
		_dataStore.Value = _toggle.isOn;
	}

	public override void SetDefaultValue()
	{
		_dataStore.SetToDefault();
		_wasOn = _dataStore.Value;
		_toggle.isOn = _dataStore.Value;
	}
}
