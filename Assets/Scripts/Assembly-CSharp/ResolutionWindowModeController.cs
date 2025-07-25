using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResolutionWindowModeController : MonoBehaviour
{
	private const string DEFAULT_WINDOWMODE = "Borderless";

	[SerializeField]
	private UIResolutionSetting _resolutionSetting;

	[SerializeField]
	private UIResolutionSetting _windowModeSetting;

	private Dictionary<string, Resolution> _resolutionOptionsByDisplayKey = new Dictionary<string, Resolution>();

	private Dictionary<string, FullScreenMode> _windowModeByDisplayKey = new Dictionary<string, FullScreenMode>
	{
		{
			"Fullscreen",
			FullScreenMode.ExclusiveFullScreen
		},
		{
			"Borderless",
			FullScreenMode.FullScreenWindow
		},
		{
			"Windowed",
			FullScreenMode.Windowed
		}
	};

	private void Awake()
	{
		_resolutionOptionsByDisplayKey = new Dictionary<string, Resolution>();
		Resolution[] resolutions = Screen.resolutions;
		for (int i = 0; i < resolutions.Length; i++)
		{
			Resolution value = resolutions[i];
			string key = $"{value.width} x {value.height}";
			if (!_resolutionOptionsByDisplayKey.ContainsKey(key))
			{
				_resolutionOptionsByDisplayKey.Add(key, value);
			}
		}
		_resolutionSetting.SetOptions(_resolutionOptionsByDisplayKey.Keys.ToArray());
		string value2 = $"{Screen.width} x {Screen.height}";
		int num = Array.IndexOf(_resolutionSetting.Options, value2);
		if (num < 0)
		{
			num = _resolutionOptionsByDisplayKey.Count - 1;
		}
		_resolutionSetting.ActiveIndex = num;
		_resolutionSetting.ResetToSavedValue();
		_windowModeSetting.SetOptions(_windowModeByDisplayKey.Keys.ToArray());
		string value3 = _windowModeByDisplayKey.FirstOrDefault((KeyValuePair<string, FullScreenMode> x) => x.Value == Screen.fullScreenMode).Key;
		if (string.IsNullOrEmpty(value3))
		{
			value3 = "Borderless";
		}
		int activeIndex = Array.IndexOf(_windowModeSetting.Options, value3);
		_windowModeSetting.ActiveIndex = activeIndex;
		_windowModeSetting.ResetToSavedValue();
	}

	public void TryApplyResolutionWindowSettings()
	{
		if (_resolutionSetting.IsDirty() || _windowModeSetting.IsDirty())
		{
			ApplyResolutionWindowSettings();
		}
	}

	public void ResetToDefaults()
	{
		string value = $"{Display.main.systemWidth} x {Display.main.systemHeight}";
		int num = Array.IndexOf(_resolutionSetting.Options, value);
		if (num <= 0)
		{
			value = $"{Screen.currentResolution.width} x {Screen.currentResolution.height}";
			num = Array.IndexOf(_resolutionSetting.Options, value);
		}
		_resolutionSetting.ActiveIndex = num;
		_resolutionSetting.ResetToSavedValue();
		KeyValuePair<string, FullScreenMode> keyValuePair = _windowModeByDisplayKey.FirstOrDefault((KeyValuePair<string, FullScreenMode> x) => x.Value == FullScreenMode.FullScreenWindow);
		int activeIndex = Array.IndexOf(_windowModeSetting.Options, keyValuePair.Key);
		_windowModeSetting.ActiveIndex = activeIndex;
		_windowModeSetting.ResetToSavedValue();
		ApplyResolutionWindowSettings();
	}

	private void ApplyResolutionWindowSettings()
	{
		Resolution resolution = _resolutionOptionsByDisplayKey[_resolutionSetting.CurrentOption];
		FullScreenMode fullScreenMode = _windowModeByDisplayKey[_windowModeSetting.CurrentOption];
		Screen.SetResolution(resolution.width, resolution.height, fullScreenMode);
		Debug.Log($"Updated Screen W:{resolution.width} H:{resolution.height} | WindowMode:{fullScreenMode}");
		_resolutionSetting.ActiveIndex = _resolutionSetting.CurrentIndex;
		_windowModeSetting.ActiveIndex = _windowModeSetting.CurrentIndex;
	}
}
