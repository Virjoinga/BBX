using UnityEngine;

public struct WeaponStateAtFrame
{
	public readonly WeaponProfile Profile;

	public readonly HitType HitType;

	public readonly Vector3 WeaponPosition;

	public readonly Vector3 WeaponForward;

	public readonly Vector3 WeaponUp;

	public readonly Vector3 WeaponRight;

	public readonly Vector3 HandlerPosition;

	public readonly Vector3 HandlerForward;

	public readonly int AmmoRemaining;

	public readonly int NextFireTime;

	public readonly bool IsReloading;

	public WeaponStateAtFrame(WeaponHandler handler, FiringWeapon weapon, int ammoRemaining, int nextFireTime, bool isReloading)
	{
		Profile = ((weapon != null) ? weapon.Profile : null);
		HitType = ((weapon != null) ? weapon.HitType : HitType.Raycast);
		HandlerPosition = handler.transform.position;
		HandlerForward = handler.transform.forward;
		if (weapon == null)
		{
			WeaponPosition = (WeaponForward = (WeaponUp = (WeaponRight = Vector3.zero)));
		}
		else
		{
			WeaponPosition = weapon.transform.position;
			WeaponForward = weapon.transform.forward;
			WeaponUp = weapon.transform.up;
			WeaponRight = weapon.transform.right;
		}
		AmmoRemaining = ammoRemaining;
		NextFireTime = nextFireTime;
		IsReloading = isReloading;
	}
}
