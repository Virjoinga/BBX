using System.Collections.Generic;
using System.Linq;

public class SpawnedEntityApplyEffectToDamageablesInRange<TState> : SpawnedEntityDamageableInRangeTracker<TState> where TState : ISpawnedEntity
{
	private void LateUpdate()
	{
		if (!base.entity.isOwner || !_shouldDealDamage)
		{
			return;
		}
		foreach (KeyValuePair<IDamageable, bool> item in _trackedDamageablesAffected.Where((KeyValuePair<IDamageable, bool> x) => !x.Value).ToList())
		{
			IDamageable damageable = item.Key;
			StatusEffectController component = damageable.entity.GetComponent<StatusEffectController>();
			if (component != null)
			{
				component.ApplyEffectWhile(_weaponId, _effectDatas, base._ownerEntity, conditional);
			}
			_trackedDamageablesAffected[damageable] = true;
			bool conditional()
			{
				if (this != null && base.entity != null && base.entity.isAttached && _shouldDealDamage)
				{
					return _trackedDamageablesAffected.ContainsKey(damageable);
				}
				return false;
			}
		}
	}
}
