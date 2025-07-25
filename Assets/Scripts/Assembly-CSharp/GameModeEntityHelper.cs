using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeEntityHelper : MonoBehaviour
{
	public static bool AmmoClipsAreLimited;

	private string _loadedMap = string.Empty;

	public static void DropAllItems(IPlayerState state, Vector3 position, Quaternion rotation, string defaultMeleeWeapon)
	{
		DropExistingItem(state.Loadouts[0].Weapons[1].Id, position, rotation);
		state.Loadouts[0].Weapons[1].Id = string.Empty;
		DropExistingItem(state.Loadouts[0].Weapons[2].Id, position + Vector3.right * 1f, rotation);
		state.Loadouts[0].Weapons[2].Id = string.Empty;
		if (state.Loadouts[0].Weapons[0].Id != defaultMeleeWeapon)
		{
			DropExistingItem(state.Loadouts[0].Weapons[0].Id, position + Vector3.left * 1f, rotation);
			state.Loadouts[0].Weapons[0].Id = defaultMeleeWeapon;
		}
	}

	public static void DropExistingItem(string itemId, Vector3 position, Quaternion rotation)
	{
		if (BoltNetwork.IsConnected && !string.IsNullOrEmpty(itemId))
		{
			WeaponPickupEntity.SpawnPickup(itemId, position);
		}
	}

	public void LoadMap(string map)
	{
		string mapSceneName = map + "Map";
		SceneManager.sceneLoaded += SetMapSceneAsActive;
		if (map != _loadedMap)
		{
			_loadedMap = map;
			SceneManager.LoadSceneAsync(mapSceneName, LoadSceneMode.Additive);
		}
		void SetMapSceneAsActive(Scene scene, LoadSceneMode mode)
		{
			if (scene.name == mapSceneName)
			{
				SceneManager.SetActiveScene(scene);
				SceneManager.sceneLoaded -= SetMapSceneAsActive;
			}
		}
	}
}
