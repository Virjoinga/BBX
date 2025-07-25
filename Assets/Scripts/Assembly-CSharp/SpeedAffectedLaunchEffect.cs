using UnityEngine;

public class SpeedAffectedLaunchEffect : PooledEffectController, IRaycastEffect
{
	private float _velocity;

	public void Display(Vector3 endPosition, float forwardVelocity)
	{
		_velocity = forwardVelocity;
	}

	private void FixedUpdate()
	{
		base.transform.position += base.transform.forward * _velocity * Time.fixedDeltaTime;
	}
}
