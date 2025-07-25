using System.Collections.Generic;
using BSCore;
using UnityEngine;

public class DynamicEffectLoader : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> _pooledEffects = new List<GameObject>();

	[SerializeField]
	private int _minPoolSize = 5;

	[SerializeField]
	private int _maxPoolSize = 5;

	[SerializeField]
	private int _allocationBlockSize = 5;

	private DelayedAction.DelayedActionHandle _delayedActionHandle;

	public List<GameObject> PooledEffects => _pooledEffects;

	public int MinPoolSize => _minPoolSize;

	public int MaxPoolSize => _maxPoolSize;

	public int AllocationBlockSize => _allocationBlockSize;

	private void Awake()
	{
		_delayedActionHandle = DelayedAction.RunWhen(() => MonoBehaviourSingleton<DynamicEffectPools>.IsInstantiated, delegate
		{
			DynamicEffectPools.LoadWeaponEffects(this);
		});
	}

	private void OnDestroy()
	{
		_delayedActionHandle.Kill();
	}
}
