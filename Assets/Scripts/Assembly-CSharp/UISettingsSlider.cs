using BSCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsSlider : UISettingBase
{
	[SerializeField]
	private TMP_InputField _valueInput;

	[SerializeField]
	private Slider _slider;

	[SerializeField]
	[Range(0f, 4f)]
	private int _decimalPlaces;

	[SerializeField]
	private float _displayMinValue;

	[SerializeField]
	private float _displayMaxValue = 1f;

	[SerializeField]
	private bool _showPercentDisplay;

	[SerializeField]
	private bool _saveImmediately;

	private DataStoreFloat _dataStore;

	private float _cachedValue;

	protected override void Setup()
	{
		_dataStore = _dataStoreManager.GetStore<DataStoreFloat, float>(_dataStoreKey);
	}

	public override void Init()
	{
		_slider.onValueChanged.AddListener(OnValueChanged);
		_valueInput.onEndEdit.AddListener(OnTextInputValueChanged);
		_cachedValue = _dataStore.Value;
		ResetToSavedValue();
		OnValueChanged(_dataStore.Value);
	}

	private void OnValueChanged(float value)
	{
		UpdateValueDisplay(value);
		RaiseSettingChanged();
	}

	private void OnTextInputValueChanged(string newValue)
	{
		if (newValue.Substring(newValue.Length - 1) == "%")
		{
			newValue = newValue.Remove(newValue.Length - 1, 1);
		}
		if (float.TryParse(newValue, out var result))
		{
			float t = Mathf.InverseLerp(_displayMinValue, _displayMaxValue, result);
			float num = Mathf.Lerp(_slider.minValue, _slider.maxValue, t);
			SetSliderValue(num);
			OnValueChanged(num);
		}
	}

	private void UpdateValueDisplay(float value)
	{
		float t = Mathf.InverseLerp(_slider.minValue, _slider.maxValue, value);
		string text = Mathf.Lerp(_displayMinValue, _displayMaxValue, t).ToString($"F{_decimalPlaces}");
		if (_showPercentDisplay)
		{
			text += "%";
		}
		_valueInput.SetTextWithoutNotify(text);
		if (_saveImmediately)
		{
			_dataStore.Value = value;
		}
	}

	private void SetSliderValue(float value)
	{
		float value2 = Mathf.Clamp(value, _slider.minValue, _slider.maxValue);
		_slider.value = value2;
	}

	public override void SetDefaultValue()
	{
		_dataStore.SetToDefault();
		SetSliderValue(_dataStore.Value);
	}

	public override void ResetToSavedValue()
	{
		if (_saveImmediately)
		{
			_dataStore.Value = _cachedValue;
		}
		SetSliderValue(_dataStore.Value);
	}

	public override void SaveToDataStore()
	{
		_dataStore.Value = _slider.value;
		_cachedValue = _dataStore.Value;
	}

	public override bool IsDirty()
	{
		if (_dataStore == null)
		{
			return false;
		}
		if (_dataStore.Value == _slider.value)
		{
			return _dataStore.Value != _cachedValue;
		}
		return true;
	}
}
