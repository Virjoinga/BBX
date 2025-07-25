using UnityEngine;

public class TNTEntity : SpawnedEntityStickyExplosion<ITNTEntity>
{
	[SerializeField]
	private Collider _collider;

	private void OnTriggerStay(Collider collider)
	{
		if (base.entity.isAttached && base.entity.isOwner && !base.state.IsExploded)
		{
			IDamageable componentInParent = collider.gameObject.GetComponentInParent<IDamageable>();
			if (componentInParent != null && componentInParent.entity.TryFindState<IPlayerState>(out var playerState) && !playerState.IsShielded && playerState.Team != base._ownerPlayerState.Team && (!base._ownerIsPlayer || playerState.Team != base._ownerPlayerState.Team || !(componentInParent.entity != base._ownerEntity)))
			{
				HitInfo hitInfo = new HitInfo
				{
					directHit = true,
					point = base.transform.position,
					normal = collider.transform.position,
					collider = collider,
					weaponProfile = base._weaponProfile,
					weaponId = base._weaponProfile.Id,
					hitEntity = componentInParent.entity
				};
				Explode(hitInfo);
				_collider.enabled = false;
			}
		}
	}
}
