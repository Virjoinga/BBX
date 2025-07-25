using System.Collections.Generic;
using UnityEngine;

public class DynamicEffectPools : MonoBehaviourSingleton<DynamicEffectPools>
{
	private readonly Dictionary<GameObject, SmartPool> _effectPoolsByPrefab = new Dictionary<GameObject, SmartPool>();

	public static void LoadWeaponEffects(DynamicEffectLoader loader)
	{
		MonoBehaviourSingleton<DynamicEffectPools>.Instance.LoadWeaponEffectsInternal(loader);
	}

	private void LoadWeaponEffectsInternal(DynamicEffectLoader loader)
	{
		if (loader == null || loader.PooledEffects == null || loader.PooledEffects.Count == 0)
		{
			return;
		}
		loader.PooledEffects.ForEach(delegate(GameObject prefab)
		{
			if (!_effectPoolsByPrefab.ContainsKey(prefab))
			{
				Transform obj = new GameObject("Effect Pool - " + prefab.name).transform;
				obj.SetParent(base.transform);
				SmartPool smartPool = obj.gameObject.AddComponent<SmartPool>();
				smartPool.Prepare(prefab, loader.MinPoolSize, loader.MaxPoolSize, loader.AllocationBlockSize);
				_effectPoolsByPrefab.Add(prefab, smartPool);
			}
		});
	}
}
