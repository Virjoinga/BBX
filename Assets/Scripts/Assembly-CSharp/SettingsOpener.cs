using BSCore;
using UnityEngine;

public class SettingsOpener : MonoBehaviour
{
	private const int SETTINGS_LAYER = 9;

	private BB2ActiveUI _activeUI;

	private void Update()
	{
		if (BSCoreInput.GetButtonDown(Option.Settings))
		{
			if (_activeUI == null)
			{
				UIPrefabManager.Instantiate(UIPrefabIds.SettingsPopup, OnUICreated, interactive: true, 9);
			}
			else if (!_activeUI.IsActive)
			{
				_activeUI.Show();
			}
		}
	}

	private void OnUICreated(GameObject uiGameobject)
	{
		if (!(uiGameobject == null))
		{
			_activeUI = uiGameobject.GetComponent<BB2ActiveUI>();
			if (!_activeUI.IsActive)
			{
				_activeUI.Show();
			}
		}
	}
}
