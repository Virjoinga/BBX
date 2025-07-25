using BSCore;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveMatchButton : UIBaseButtonClickHandler
{
	[SerializeField]
	private BB2ActiveUI _activeUI;

	protected override void OnClick()
	{
		_activeUI.Hide();
		UIPrefabManager.Instantiate(UIPrefabIds.LoadingOverlay, OnLoadingOverlayCreated, interactive: false, 11);
	}

	private void OnLoadingOverlayCreated(GameObject uiGameobject)
	{
		UIPrefabManager.sceneLoad = "MainMenu";
		UIPrefabManager.Destroy(UIPrefabIds.BRMatchHud);
		ConnectionManager.Shutdown();
		SceneManager.LoadSceneAsync("MainMenu");
	}
}
