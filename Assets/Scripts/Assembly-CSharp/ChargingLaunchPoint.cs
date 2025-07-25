using UnityEngine;

public class ChargingLaunchPoint : LaunchPoint
{
	[SerializeField]
	protected ParticleSystem _chargeEffect;

	[SerializeField]
	protected float _minSize = 1f;

	[SerializeField]
	protected float _maxSize = 3f;

	[SerializeField]
	protected float _minRadius = 0.1f;

	[SerializeField]
	protected float _maxRadius = 0.4f;

	protected float _chargeTime;

	protected float _chargePercent;

	protected float _radius = 0.1f;

	public void SetChargePercent(float chargeTime, float chargePercent)
	{
		_chargeTime = chargeTime;
		_chargePercent = chargePercent;
		if (!(_chargeEffect == null))
		{
			if (_chargeEffect.isPlaying && chargePercent == 0f)
			{
				_chargeEffect.Stop();
			}
			else if (!_chargeEffect.isPlaying && chargePercent > 0f)
			{
				_chargeEffect.Play();
			}
			UpdateChargeEffect(chargePercent);
			_radius = Mathf.Lerp(_minRadius, _maxRadius, chargePercent);
		}
	}

	protected virtual void UpdateChargeEffect(float chargePercent)
	{
		if (!(_chargeEffect == null))
		{
			_chargeEffect.transform.localScale = Vector3.one * Mathf.Lerp(_minSize, _maxSize, chargePercent);
		}
	}

	protected override HitInfo GenerateHitInfo(RaycastHit hit, int serverFrame)
	{
		HitInfo result = base.GenerateHitInfo(hit, serverFrame);
		result.chargeTime = _chargeTime;
		return result;
	}

	protected override Projectile.LaunchDetails GenerateLaunchDetails(int projectileId, int serverFrame)
	{
		Projectile.LaunchDetails result = base.GenerateLaunchDetails(projectileId, serverFrame);
		result.chargeTime = _chargeTime;
		if (base.Profile.Explosion.Explodes)
		{
			result.effectRadius = base.Profile.CalculateRangeAtTime(result.chargeTime);
		}
		result.scale = ((_chargeEffect != null) ? _chargeEffect.transform.localScale.x : 1f);
		result.radius = _radius;
		return result;
	}

	protected override GameObject TrySpawnImpactEffect(RaycastHit hit)
	{
		GameObject gameObject = base.TrySpawnImpactEffect(hit);
		if (gameObject != null && base.Profile.Explosion.Explodes && base.Profile.Charge.CanCharge && base.Profile.Charge.RangeAtMax > 0f)
		{
			gameObject.transform.localScale = Vector3.one * base.Profile.CalculateRangeAtTime(_chargeTime);
		}
		return gameObject;
	}
}
