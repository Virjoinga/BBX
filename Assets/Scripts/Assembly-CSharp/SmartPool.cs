using System.Collections.Generic;
using UnityEngine;

public class SmartPool : MonoBehaviour
{
	public const string Version = "1.02";

	private static Dictionary<GameObject, SmartPool> _Pools = new Dictionary<GameObject, SmartPool>();

	[SerializeField]
	private GameObject _prefab;

	[SerializeField]
	private bool _dontDestroy;

	[SerializeField]
	private bool _prepareAtStart;

	[SerializeField]
	private int _allocationBlockSize = 1;

	[SerializeField]
	private int _minPoolSize = 1;

	[SerializeField]
	private int _maxPoolSize = 1;

	[SerializeField]
	private PoolExceededMode _onMaxPoolSize;

	[SerializeField]
	private bool _autoCull = true;

	[SerializeField]
	private float _cullingSpeed = 1f;

	[SerializeField]
	private bool _debugLog;

	private Stack<GameObject> _mStock = new Stack<GameObject>();

	private List<GameObject> _mSpawned = new List<GameObject>();

	private float _mLastCullingTime;

	private string _poolName
	{
		get
		{
			if (!(_prefab != null))
			{
				return string.Empty;
			}
			return _prefab.name;
		}
	}

	public int InStock => _mStock.Count;

	public int Spawned => _mSpawned.Count;

	public int MaxPoolSize => _maxPoolSize;

	private void Awake()
	{
		if (_prepareAtStart && _poolName.Length == 0)
		{
			Debug.LogWarning("SmartPool: Missing PoolName for pool belonging to '" + base.gameObject.name + "'!");
		}
		if (_dontDestroy)
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void OnEnable()
	{
		if (_prefab != null)
		{
			if (!HasPool(_prefab))
			{
				AddPool(this);
			}
		}
		else if (_prepareAtStart)
		{
			Debug.LogError("SmartPool: Pool '" + _poolName + "' is missing it's prefab!");
		}
	}

	private void Start()
	{
		if (_prepareAtStart)
		{
			Prepare();
		}
	}

	private void LateUpdate()
	{
		if (_autoCull && Time.time - _mLastCullingTime > _cullingSpeed)
		{
			_mLastCullingTime = Time.time;
			Cull(smartCull: true);
		}
	}

	private void OnDisable()
	{
		if (!_dontDestroy)
		{
			Clear();
			if (_Pools.Remove(_prefab) && _debugLog)
			{
				Debug.Log("SmartPool: Removing " + _poolName + " from the pool dictionary!");
			}
		}
	}

	private void OnDestroy()
	{
		_Pools.Remove(_prefab);
	}

	private void Reset()
	{
		_prefab = null;
		_dontDestroy = false;
		_allocationBlockSize = 1;
		_minPoolSize = 1;
		_maxPoolSize = 1;
		_onMaxPoolSize = PoolExceededMode.Ignore;
		_debugLog = false;
		_autoCull = true;
		_cullingSpeed = 1f;
		_mLastCullingTime = 0f;
	}

	private void Clear()
	{
		if (_debugLog)
		{
			Debug.Log("SmartPool (" + _poolName + "): Clearing all instances of " + _prefab.name);
		}
		foreach (GameObject item in _mSpawned)
		{
			Object.Destroy(item);
		}
		_mSpawned.Clear();
		foreach (GameObject item2 in _mStock)
		{
			Object.Destroy(item2);
		}
		_mStock.Clear();
	}

	public void Cull()
	{
		Cull(smartCull: false);
	}

	public void Cull(bool smartCull)
	{
		int num = (smartCull ? Mathf.Min(_allocationBlockSize, _mStock.Count - _maxPoolSize) : (_mStock.Count - _maxPoolSize));
		if (_debugLog && num > 0)
		{
			Debug.Log("SmartPool (" + _poolName + "): Culling " + num + " items");
		}
		while (num-- > 0)
		{
			Object.Destroy(_mStock.Pop());
		}
	}

	public void DespawnItem(GameObject item)
	{
		if (!item)
		{
			if (_debugLog)
			{
				Debug.LogWarning("SmartPool (" + _poolName + ").DespawnItem: item is null!");
			}
			return;
		}
		if (IsSpawned(item))
		{
			item.SetActive(value: false);
			item.name = _prefab.name + "_stock";
			_mSpawned.Remove(item);
			_mStock.Push(item);
			if (_debugLog)
			{
				Debug.Log("SmartPool (" + _poolName + "): Despawning '" + item.name);
			}
			return;
		}
		Object.Destroy(item);
		if (_debugLog)
		{
			Debug.LogWarning("SmartPool (" + _poolName + "): Cant Despawn" + item.name + "' because it's not managed by this pool! However, SmartPool destroyed it!");
		}
	}

	public void DespawnAllItems()
	{
		while (_mSpawned.Count > 0)
		{
			DespawnItem(_mSpawned[0]);
		}
	}

	public void KillItem(GameObject item)
	{
		if (!item)
		{
			if (_debugLog)
			{
				Debug.LogWarning("SmartPool (" + _poolName + ").KillItem: item is null!");
			}
		}
		else
		{
			_mSpawned.Remove(item);
			Object.Destroy(item);
		}
	}

	public bool IsManagedObject(GameObject item)
	{
		if (!item)
		{
			if (_debugLog)
			{
				Debug.LogWarning("SmartPool (" + _poolName + ").IsManagedObject: item is null!");
			}
			return false;
		}
		if (_mSpawned.Contains(item) || _mStock.Contains(item))
		{
			return true;
		}
		return false;
	}

	public bool IsSpawned(GameObject item)
	{
		if (!item)
		{
			if (_debugLog)
			{
				Debug.LogWarning("SmartPool (" + _poolName + ").IsSpawned: item is null!");
			}
			return false;
		}
		return _mSpawned.Contains(item);
	}

	private void Populate(int no)
	{
		while (no > 0)
		{
			GameObject gameObject = Object.Instantiate(_prefab, base.transform);
			gameObject.SetActive(value: false);
			gameObject.name = _prefab.name + "_stock";
			_mStock.Push(gameObject);
			no--;
		}
		if (_debugLog)
		{
			Debug.Log("SmartPool (" + _poolName + "): Instantiated " + _mStock.Count + " instances of " + _prefab.name);
		}
	}

	public void Prepare(GameObject prefab, int minPoolSize, int maxPoolSize, int allocationBlockSize)
	{
		_prefab = prefab;
		_minPoolSize = minPoolSize;
		_maxPoolSize = maxPoolSize;
		_allocationBlockSize = allocationBlockSize;
		AddPool(this);
		Prepare();
	}

	public void Prepare()
	{
		Clear();
		_mStock = new Stack<GameObject>(_minPoolSize);
		Populate(_minPoolSize);
	}

	public GameObject SpawnItem()
	{
		GameObject gameObject = null;
		if (InStock == 0 && (Spawned < _maxPoolSize || _onMaxPoolSize == PoolExceededMode.Ignore))
		{
			Populate(_allocationBlockSize);
		}
		if (InStock > 0)
		{
			gameObject = _mStock.Pop();
			if (_debugLog)
			{
				Debug.Log("SmartPool (" + _poolName + "): Spawning item, taking it from the stock!");
			}
		}
		else if (_onMaxPoolSize == PoolExceededMode.ReUse)
		{
			gameObject = _mSpawned[0];
			_mSpawned.RemoveAt(0);
			if (_debugLog)
			{
				Debug.Log("SmartPool (" + _poolName + "): Spawning item, reusing an existing item!");
			}
		}
		else if (_debugLog)
		{
			Debug.Log("SmartPool (" + _poolName + "): MaxPoolSize exceeded, nothing was spawned!");
		}
		if (gameObject != null)
		{
			_mSpawned.Add(gameObject);
			gameObject.SetActive(value: true);
			gameObject.name = $"{_prefab.name}_clone ({_mStock.Count})";
			gameObject.transform.localPosition = Vector3.zero;
		}
		return gameObject;
	}

	public static void AddPool(SmartPool pool)
	{
		_Pools.Add(pool._prefab, pool);
		if (pool._debugLog)
		{
			Debug.Log("SmartPool: Adding '" + pool._poolName + "' to the pool dictionary!");
		}
	}

	public static bool HasPool(GameObject prefab)
	{
		return _Pools.ContainsKey(prefab);
	}

	public static void Cull(GameObject prefab)
	{
		Cull(prefab, smartCull: false);
	}

	public static void Cull(GameObject prefab, bool smartCull)
	{
		if (TryGetPoolByPrefab(prefab, out var p))
		{
			p.Cull();
		}
	}

	public static void Despawn(GameObject item)
	{
		if ((bool)item)
		{
			SmartPool poolByItem = GetPoolByItem(item);
			if (poolByItem != null)
			{
				poolByItem.DespawnItem(item);
			}
			else
			{
				Object.Destroy(item);
			}
		}
	}

	public static void DespawnAllItems(GameObject prefab)
	{
		if (TryGetPoolByPrefab(prefab, out var p))
		{
			p.DespawnAllItems();
		}
	}

	public static SmartPool GetPoolByItem(GameObject item)
	{
		foreach (SmartPool value in _Pools.Values)
		{
			if (value.IsManagedObject(item))
			{
				return value;
			}
		}
		return null;
	}

	public static bool TryGetPoolByPrefab(GameObject prefab, out SmartPool p)
	{
		return _Pools.TryGetValue(prefab, out p);
	}

	public static void Kill(GameObject item)
	{
		if ((bool)item)
		{
			SmartPool poolByItem = GetPoolByItem(item);
			if (poolByItem != null)
			{
				poolByItem.KillItem(item);
			}
			else
			{
				Object.Destroy(item);
			}
		}
	}

	public static void Prepare(GameObject prefab)
	{
		if (TryGetPoolByPrefab(prefab, out var p))
		{
			p.Prepare();
		}
	}

	public static GameObject Spawn(GameObject prefab)
	{
		if (_Pools.TryGetValue(prefab, out var value))
		{
			return value.SpawnItem();
		}
		return null;
	}

	public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = Spawn(prefab);
		if (gameObject != null)
		{
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
		}
		return gameObject;
	}

	public static T Spawn<T>(T prefab) where T : MonoBehaviour
	{
		GameObject gameObject = Spawn(prefab.gameObject);
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent<T>();
	}

	public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : MonoBehaviour
	{
		GameObject gameObject = Spawn(prefab.gameObject, position, rotation);
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent<T>();
	}
}
