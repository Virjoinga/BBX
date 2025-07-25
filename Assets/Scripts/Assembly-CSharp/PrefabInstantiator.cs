using Bolt;
using UnityEngine;
using Zenject;

public class PrefabInstantiator : IPrefabPool
{
	private DiContainer _diContainer;

	public PrefabInstantiator(DiContainer diContainer)
	{
		_diContainer = diContainer;
	}

	public GameObject LoadPrefab(PrefabId prefabId)
	{
		return PrefabDatabase.Find(prefabId);
	}

	public GameObject Instantiate(PrefabId prefabId, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = LoadPrefab(prefabId);
		if (gameObject == null)
		{
			Debug.LogError($"[PrefabInstantiator] Error isntantiating prefabId {prefabId}. Could not find prefab");
			return null;
		}
		return _diContainer.InstantiatePrefab(gameObject, position, rotation, null);
	}

	public void Destroy(GameObject gameObject)
	{
		Object.Destroy(gameObject);
	}
}
