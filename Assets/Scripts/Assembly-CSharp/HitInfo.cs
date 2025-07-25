using UnityEngine;

public struct HitInfo
{
	public int projectileId;

	public HitType hitType;

	public int launchServerFrame;

	public int hitServerFrame;

	public Vector3 origin;

	public Vector3 forward;

	public Vector3 forwardOnHit;

	public bool directHit;

	public Vector3 point;

	public Vector3 normal;

	public Quaternion projectileRotation;

	public Collider collider;

	public float distance;

	public string weaponId;

	public float chargeTime;

	public WeaponProfile.EffectData[] effects;

	public WeaponProfile weaponProfile;

	public BoltEntity hitEntity;

	public BoltHitbox hitBox;

	public bool isHeadshot;

	public bool isMelee;

	public MeleeWeapon.Direction meleeDirection;

	public override string ToString()
	{
		string text = ((collider != null) ? collider.name : "null");
		string text2 = ((hitEntity != null) ? hitEntity.name : "null");
		return $"[HitInfo] projectileId={projectileId}, hitType={hitType}, launchServerFrame={launchServerFrame}, " + $"origin={origin}, directHit={directHit}, point={point}, normal={normal}, collider={text}, " + string.Format("distance={0}, chargeTime={1}, weaponProfile={2}, hitEntity={3}", distance, chargeTime, weaponProfile?.Id ?? "null", text2);
	}
}
