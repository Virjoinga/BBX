using Constants;
using UnityEngine;

public class RandomDeflectedLaunchPoint : DeflectedLaunchPoint
{
	private float _spreadAmount => _currentSpread;

	protected override void DeflectForward(ref Vector3 forward, int shotCount)
	{
		if (!(_spreadAmount <= 0f))
		{
			forward = Match.GetSpreadForward(forward, base.transform.up, base.transform.right, _spreadAmount, shotCount);
		}
	}
}
