using UnityEngine;

public class MainMenuUILoader : MonoBehaviour
{
	private void Awake()
	{
		UIPrefabManager.Instantiate(UIPrefabIds.MainMenu, OnMainMenuCreated);
	}

	private void OnMainMenuCreated(GameObject uiGameobject)
	{
		UIPrefabManager.TryDestroyAllBut(UIPrefabIds.MainMenu);
	}
}
