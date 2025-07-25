using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsManager : BB2ActiveUI
{
	[SerializeField]
	private Button _applyButton;

	[SerializeField]
	private Button _resetToDefaultsButton;

	[SerializeField]
	private Image _applyButtonImage;

	[SerializeField]
	private Color _unsavedChangesApplyButtonColor;

	[SerializeField]
	private GameObject _leaveMatchButton;

	[SerializeField]
	private GameObject _exitGameButton;

	[SerializeField]
	private ResolutionWindowModeController _resolutionWindowModeController;

	private UISettingBase[] _settings;

	private Color _defaultApplyButtonColor;

	protected override void Awake()
	{
		base.Awake();
		_settings = (from setting in GetComponentsInChildren<UISettingBase>(includeInactive: true)
			where setting.MobileFriendly
			select setting).ToArray();
		_defaultApplyButtonColor = _applyButtonImage.color;
		_applyButton.onClick.AddListener(Save);
		_resetToDefaultsButton.onClick.AddListener(OnClickedResetToDefaults);
	}

	protected override void Start()
	{
		base.Start();
		UISettingBase[] settings = _settings;
		foreach (UISettingBase obj in settings)
		{
			obj.Init();
			obj.SettingChanged += OnSettingChanged;
		}
	}

	private void OnSettingChanged()
	{
		if (_settings.Any((UISettingBase setting) => setting.IsDirty()))
		{
			_applyButtonImage.color = _unsavedChangesApplyButtonColor;
		}
		else
		{
			_applyButtonImage.color = _defaultApplyButtonColor;
		}
	}

	private void Save()
	{
		UISettingBase[] settings = _settings;
		foreach (UISettingBase uISettingBase in settings)
		{
			if (uISettingBase.IsDirty())
			{
				uISettingBase.SaveToDataStore();
			}
		}
		_resolutionWindowModeController.TryApplyResolutionWindowSettings();
		_applyButtonImage.color = _defaultApplyButtonColor;
	}

	private void ResetToSaved()
	{
		UISettingBase[] settings = _settings;
		for (int i = 0; i < settings.Length; i++)
		{
			settings[i].ResetToSavedValue();
		}
		_applyButtonImage.color = _defaultApplyButtonColor;
		Hide();
	}

	private void OnClickedResetToDefaults()
	{
		UIGenericPopupManager.ShowYesNoPopup("Are you sure you want to reset to default settings?", ResetToDefaults, null);
	}

	private void ResetToDefaults()
	{
		UISettingBase[] settings = _settings;
		for (int i = 0; i < settings.Length; i++)
		{
			settings[i].SetDefaultValue();
		}
		_resolutionWindowModeController.ResetToDefaults();
		_applyButtonImage.color = _defaultApplyButtonColor;
	}

	public override void Show(bool forceShow = false, bool ignoreMouseRelease = false)
	{
		_leaveMatchButton.SetActive(ConnectionManager.IsConnected);
		_exitGameButton.SetActive(!ConnectionManager.IsConnected);
		base.Show(forceShow, ignoreMouseRelease);
	}

	public override void Hide(bool ignoreMouseLock = false)
	{
		if (_settings.Any((UISettingBase setting) => setting.IsDirty()))
		{
			UIGenericPopupManager.ShowPopup(new GenericPopupDetails
			{
				Message = "You have unsaved changed. Would you like to apply them now or discard them?",
				Button1Details = new GenericButtonDetails("Discard", ResetToSaved, UIConstants.RedButtonColor),
				Button2Details = new GenericButtonDetails("Apply", SaveAndClose, UIConstants.GreenButtonColor),
				AllowCloseButtons = false
			});
		}
		else
		{
			base.Hide();
		}
	}

	private void SaveAndClose()
	{
		Save();
		Hide();
	}
}
