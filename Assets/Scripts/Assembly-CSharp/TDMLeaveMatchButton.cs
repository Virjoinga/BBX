using BSCore;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TDMLeaveMatchButton : UIBaseButtonClickHandler
{
	protected override void OnClick()
	{
		UIPrefabManager.Instantiate(UIPrefabIds.LoadingOverlay, OnLoadingOverlayCreated, interactive: false, 11);
	}

	private void OnLoadingOverlayCreated(GameObject uiGameobject)
	{
		UIPrefabManager.Destroy(UIPrefabIds.TDMAARScreen);
		UIPrefabManager.sceneLoad = "MainMenu";
		ConnectionManager.Shutdown();
		SceneManager.LoadSceneAsync("MainMenu");
	}
}
