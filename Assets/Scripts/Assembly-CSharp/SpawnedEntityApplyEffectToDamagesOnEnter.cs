using System.Collections.Generic;
using System.Linq;

public class SpawnedEntityApplyEffectToDamagesOnEnter<TState> : SpawnedEntityDamageableEnteredTracker<TState> where TState : ISpawnedEntity
{
	private void LateUpdate()
	{
		if (!base.entity.isOwner || !_shouldDealDamage)
		{
			return;
		}
		foreach (KeyValuePair<IDamageable, bool> item in _trackedDamageablesAffected.Where((KeyValuePair<IDamageable, bool> x) => !x.Value).ToList())
		{
			IDamageable key = item.Key;
			StatusEffectController component = key.entity.GetComponent<StatusEffectController>();
			if (component != null)
			{
				component.TryApplyEffect(_weaponId, _effectDatas, base._ownerEntity);
			}
			_trackedDamageablesAffected[key] = true;
		}
	}
}
