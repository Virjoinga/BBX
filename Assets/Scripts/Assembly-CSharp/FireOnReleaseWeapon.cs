using UnityEngine;

public class FireOnReleaseWeapon : FiringWeapon
{
	private bool _fireButtonHeld;

	protected override void OnDisable()
	{
		base.OnDisable();
		_fireButtonHeld = false;
	}

	public override FireResults Fire(bool isFiring, Vector3 aimPoint, int serverFrame, int shotCount)
	{
		if (!_fireButtonHeld && isFiring)
		{
			FireHoldStarted();
		}
		FireResults result = FireResults.Empty;
		if (_fireButtonHeld && !isFiring)
		{
			result = base.Fire(isFiring: true, aimPoint, serverFrame, shotCount);
			FireHoldReleased();
		}
		_fireButtonHeld = isFiring;
		return result;
	}

	protected virtual void FireHoldStarted()
	{
	}

	protected virtual void FireHoldReleased()
	{
	}
}
