using UnityEngine;

public class SelfBuffOnlyFiringWeapon : FiringWeapon
{
	public override FireResults Fire(bool isFiring, Vector3 aimPoint, int serverFrame, int shotCount)
	{
		if (!isFiring)
		{
			return FireResults.Empty;
		}
		return GenerateFireResults(_launchPoint.transform.position, Vector3.zero, 1);
	}

	protected override FireResults GenerateFireResults(Vector3 position, Vector3 forward, int ammoUsed)
	{
		FireResults result = base.GenerateFireResults(position, forward, ammoUsed);
		result.HitType = HitType.SelfBuff;
		return result;
	}
}
