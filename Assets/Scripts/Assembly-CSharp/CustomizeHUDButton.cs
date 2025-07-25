using BSCore;
using UnityEngine;

public class CustomizeHUDButton : UIBaseButtonClickHandler
{
	[SerializeField]
	private UISettingsManager _settings;

	private bool _openedInMainMenu;

	private UIPrefabIds _prefabId = UIPrefabIds.CustomizableMainMenuHUD;

	protected override void OnClick()
	{
		if (!CustomizableHUDController.HasInstance)
		{
			_openedInMainMenu = true;
			UIPrefabManager.Instantiate(_prefabId, OnCustomizableHUDCreated, interactive: true, 100);
		}
		else
		{
			OnCustomizableHUDCreated(null);
		}
	}

	private void OnCustomizableHUDCreated(GameObject uiGameobject)
	{
		CustomizableHUDController.Instance.ToggleCustomization(isOn: true);
		CustomizableHUDController.Instance.Closed += OnClosed;
		_settings.Hide(ignoreMouseLock: true);
		MonoBehaviourSingleton<MouseLockToggle>.Instance.ReleaseCursor();
		MonoBehaviourSingleton<MouseLockToggle>.Instance.PreventLock(prevent: true);
		if (PlayerController.HasLocalPlayer)
		{
			PlayerController.LocalPlayer.LocalInputBlocked = true;
		}
	}

	private void OnClosed()
	{
		CustomizableHUDController.Instance.Closed -= OnClosed;
		CustomizableHUDController.Instance.ToggleCustomization(isOn: false);
		if (_openedInMainMenu)
		{
			UIPrefabManager.Destroy(_prefabId);
		}
		else
		{
			MonoBehaviourSingleton<MouseLockToggle>.Instance.PreventLock(prevent: false);
			MonoBehaviourSingleton<MouseLockToggle>.Instance.TryLockCursor();
			if (PlayerController.HasLocalPlayer)
			{
				PlayerController.LocalPlayer.LocalInputBlocked = false;
			}
		}
		_openedInMainMenu = false;
	}
}
