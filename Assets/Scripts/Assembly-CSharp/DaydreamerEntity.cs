using UnityEngine;

public class DaydreamerEntity : DeployableEntity<IDaydreamer>
{
	[SerializeField]
	private GameObject _detonationEffect;

	private void OnTriggerStay(Collider collider)
	{
		if (!base.entity.isAttached || !base.entity.isOwner)
		{
			return;
		}
		IDamageable componentInParent = collider.gameObject.GetComponentInParent<IDamageable>();
		if (componentInParent != null && componentInParent.entity.TryFindState<IPlayerState>(out var playerState) && !playerState.IsShielded && playerState.Team != base._ownerPlayerState.Team && (!base._ownerIsPlayer || playerState.Team != base._ownerPlayerState.Team || !(componentInParent.entity != base._ownerEntity)))
		{
			StatusEffectController component = componentInParent.entity.GetComponent<StatusEffectController>();
			if (component != null)
			{
				component.TryApplyEffect(base._weaponProfile.Id, base._weaponProfile.Effects, base._ownerEntity);
				BoltNetwork.Destroy(base.gameObject);
			}
		}
	}

	private void OnDestroy()
	{
		if (_detonationEffect != null && !BoltNetwork.IsServer)
		{
			SmartPool.Spawn(_detonationEffect, base.transform.position, base.transform.rotation);
		}
	}
}
