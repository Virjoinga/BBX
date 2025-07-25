using UnityEngine;

public interface IDamageable
{
	BoltEntity entity { get; }

	Collider HurtCollider { get; }

	Transform DamageNumberSpawn { get; }

	int Team { get; }

	void TakeDamage(HitInfo hitInfo, float damage, BoltEntity attacker);

	void TakeDamage(HitInfo hitInfo, float damage, BoltEntity attacker, WeaponProfile.EffectData[] effects);
}
