using UnityEngine;
using Zenject;

public class ZenjectInstantiater
{
	private DiContainer _zenjectContainer;

	[Inject]
	public ZenjectInstantiater(DiContainer zenjectContainer)
	{
		_zenjectContainer = zenjectContainer;
	}

	public GameObject Instantiate(GameObject prefab)
	{
		return Instantiate(prefab, null);
	}

	public GameObject Instantiate(GameObject prefab, Transform parentTransform)
	{
		return _zenjectContainer.InstantiatePrefab(prefab, parentTransform);
	}

	public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parentTransform)
	{
		return _zenjectContainer.InstantiatePrefab(prefab, position, rotation, parentTransform);
	}

	public T Instantiate<T>(T prefab) where T : MonoBehaviour
	{
		return Instantiate(prefab, null);
	}

	public T Instantiate<T>(T prefab, Transform parentTransform) where T : MonoBehaviour
	{
		return Instantiate(prefab.gameObject, parentTransform).GetComponent<T>();
	}

	public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parentTransform) where T : MonoBehaviour
	{
		return Instantiate(prefab.gameObject, position, rotation, parentTransform).GetComponent<T>();
	}
}
