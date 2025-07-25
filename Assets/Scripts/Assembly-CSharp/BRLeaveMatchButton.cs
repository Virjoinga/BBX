using BSCore;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BRLeaveMatchButton : UIBaseButtonClickHandler
{
	[SerializeField]
	private bool _isVictoryScreen;

	protected override void OnClick()
	{
		UIPrefabManager.Instantiate(UIPrefabIds.LoadingOverlay, OnLoadingOverlayCreated, interactive: false, 11);
	}

	private void OnLoadingOverlayCreated(GameObject uiGameobject)
	{
		if (_isVictoryScreen)
		{
			UIPrefabManager.Destroy(UIPrefabIds.BRVictoryScreen);
		}
		else
		{
			UIPrefabManager.Destroy(UIPrefabIds.BRDeathScreen);
		}
		ConnectionManager.Shutdown();
		SceneManager.LoadSceneAsync("MainMenu");
	}
}
