using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplosiveProjectile : Projectile
{
	[SerializeField]
	private GameObject _explosiveImpactPrefab;

	protected override void Impact(RaycastHit hit)
	{
		if (_hasProcessedImpact)
		{
			return;
		}
		List<HitInfo> list = AreaEffectCalculator.GetAffectableInRange(_details.effectRadius, GenerateHitInfo(hit));
		if (_details.weaponProfile.Explosion.ClosestOnly && list.Count > 1)
		{
			if (_details.weaponProfile.Explosion.IgnoreSelf)
			{
				list = list.Where(delegate(HitInfo areaHit)
				{
					IPlayerController componentInParent = areaHit.collider.GetComponentInParent<IPlayerController>();
					return componentInParent == null || componentInParent.entity != PlayerController.LocalPlayer.entity;
				}).ToList();
			}
			if (list.Count > 0)
			{
				HitInfo hitInfo = list.OrderBy((HitInfo areaHit) => areaHit.distance).First();
				OnHit(hitInfo);
			}
		}
		else
		{
			foreach (HitInfo item in list)
			{
				OnHit(item);
			}
		}
		base.Impact(hit);
	}

	private void OnHit(HitInfo hitInfo)
	{
		if (_explosiveImpactPrefab != null && !BoltNetwork.IsServer)
		{
			SmartPool.Spawn(_explosiveImpactPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
		}
	}

	protected override GameObject TrySpawnImpactEffect(Vector3 position, Vector3 normal)
	{
		GameObject gameObject = base.TrySpawnImpactEffect(position, normal);
		Debug.Log($"[ExplosiveProjectile] Spawnign impact effect with radius: {_details.effectRadius}");
		if (_details.effectRadius > 0f)
		{
			gameObject.transform.localScale = Vector3.one * _details.effectRadius;
			DebugExtension.DebugWireSphere(gameObject.transform.position, Color.red, _details.effectRadius, 5f);
		}
		return gameObject;
	}
}
