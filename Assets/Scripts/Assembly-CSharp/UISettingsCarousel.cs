using BSCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsCarousel : UISettingBase
{
	[SerializeField]
	private TextMeshProUGUI _value;

	[SerializeField]
	private Button _forwardButton;

	[SerializeField]
	private Button _backButton;

	[SerializeField]
	private GameObject _greyOut;

	private DataStoreInt _dataStore;

	protected string[] _options;

	public int CurrentIndex { get; private set; }

	public string CurrentOption => _options[CurrentIndex];

	public string[] Options => (string[])_options.Clone();

	protected override void Setup()
	{
		_dataStore = _dataStoreManager.GetStore<DataStoreInt, int>(_dataStoreKey);
		_dataStore.Listen(DataStoreChanged);
	}

	private void Awake()
	{
		_forwardButton.onClick.AddListener(ClickedForward);
		_backButton.onClick.AddListener(ClickedBack);
	}

	private void OnDestroy()
	{
		if (_dataStore != null)
		{
			_dataStore.Unlisten(DataStoreChanged);
		}
	}

	public override void Init()
	{
		ResetToSavedValue();
	}

	private void ClickedForward()
	{
		int num = CurrentIndex + 1;
		if (num > _options.Length - 1)
		{
			num = 0;
		}
		CurrentIndex = num;
		OnValueChanged(CurrentIndex);
	}

	private void ClickedBack()
	{
		int num = CurrentIndex - 1;
		if (num < 0)
		{
			num = _options.Length - 1;
		}
		CurrentIndex = num;
		OnValueChanged(CurrentIndex);
	}

	private void DataStoreChanged(int newValue)
	{
		SetCarouselValue(newValue);
		SaveToDataStore();
	}

	private void OnValueChanged(int index)
	{
		if (_options != null && index >= 0 && _options.Length > index && _options[index] != null)
		{
			_value.text = _options[index];
			RaiseSettingChanged();
		}
	}

	public void SetCarouselValue(int newIndex)
	{
		CurrentIndex = newIndex;
		OnValueChanged(CurrentIndex);
	}

	public void SetOptions(string[] options)
	{
		_options = options;
	}

	public void SetNewDefaultValue(int newDefault)
	{
		_dataStore.SetNewDefault(newDefault);
	}

	public void SetGreyOutState(bool isActive)
	{
		_greyOut.SetActive(isActive);
	}

	public override void SetDefaultValue()
	{
		_dataStore.SetToDefault();
		SetCarouselValue(_dataStore.Value);
	}

	public override void ResetToSavedValue()
	{
		SetCarouselValue(_dataStore.Value);
	}

	public override void SaveToDataStore()
	{
		_dataStore.Value = CurrentIndex;
	}

	public override bool IsDirty()
	{
		if (_dataStore == null)
		{
			return false;
		}
		return _dataStore.Value != CurrentIndex;
	}
}
