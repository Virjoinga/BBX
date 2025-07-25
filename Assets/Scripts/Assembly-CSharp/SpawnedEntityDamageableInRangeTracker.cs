using UnityEngine;

public class SpawnedEntityDamageableInRangeTracker<TState> : SpawnedEntityDamageableEnteredTracker<TState> where TState : ISpawnedEntity
{
	private void OnTriggerExit(Collider other)
	{
		if (base.entity.isAttached && base.entity.isOwner)
		{
			IDamageable componentInParent = other.gameObject.GetComponentInParent<IDamageable>();
			if (componentInParent != null)
			{
				_trackedDamageablesAffected.Remove(componentInParent);
			}
		}
	}
}
