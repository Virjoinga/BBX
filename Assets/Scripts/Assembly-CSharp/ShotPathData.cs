using UnityEngine;

public struct ShotPathData
{
	public Vector3 AimPosition;

	public Vector3 HitPosition;

	public bool HideShotPathBlockedIcon;

	public ShotPathData(Vector3 aimPosition, Vector3 hitPosition, bool hideShotPathBlockedIcon)
	{
		AimPosition = aimPosition;
		HitPosition = hitPosition;
		HideShotPathBlockedIcon = hideShotPathBlockedIcon;
	}
}
