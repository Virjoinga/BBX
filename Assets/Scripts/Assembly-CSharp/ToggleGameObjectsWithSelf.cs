using UnityEngine;

public class ToggleGameObjectsWithSelf : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _gameObjects;

	private void Start()
	{
		ToggleGameObjects(base.gameObject.activeInHierarchy);
	}

	private void OnEnable()
	{
		ToggleGameObjects(isActive: true);
	}

	private void OnDisable()
	{
		ToggleGameObjects(isActive: false);
	}

	private void ToggleGameObjects(bool isActive)
	{
		GameObject[] gameObjects = _gameObjects;
		for (int i = 0; i < gameObjects.Length; i++)
		{
			gameObjects[i].SetActive(isActive);
		}
	}
}
