using System.Collections;
using UnityEngine;

public class ParticleHitDetectingContinuousFireLaunchPoint : ContinuousFireLaunchPoint
{
	[SerializeField]
	private ParticleHitDetector _detector;

	protected override void Awake()
	{
		base.Awake();
		_detector.CollisionDetected += OnCollisionDetected;
	}

	public override void UpdateProperties(WeaponProfile profile)
	{
		base.UpdateProperties(profile);
		_detector.UpdateProperties(profile);
	}

	protected override IEnumerator ContinuousFireRoutine()
	{
		_detector.ClearHitDetectionCache();
		yield return base.ContinuousFireRoutine();
	}

	protected override void ClearHitDetectionCache()
	{
		base.ClearHitDetectionCache();
		_detector.ClearHitDetectionCache();
	}

	private void OnCollisionDetected(GameObject victimGO, ParticleCollisionEvent collision)
	{
		Vector3 normalized = collision.velocity.normalized;
		Collider collider = collision.colliderComponent as Collider;
		IDamageable componentInParent = victimGO.GetComponentInParent<IDamageable>();
		HitInfo obj = new HitInfo
		{
			origin = collision.intersection - normalized * Radius * 2f,
			forward = normalized,
			launchServerFrame = _serverFrame,
			collider = collider,
			hitEntity = componentInParent?.entity,
			point = collision.intersection,
			normal = collision.normal,
			weaponProfile = base.Profile,
			weaponId = base.Profile.Id,
			hitType = HitType.Continuous
		};
		_onHit(obj);
	}
}
