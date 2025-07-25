using UnityEngine;

public class ParticleSizeChargingLaunchPoint : ChargingLaunchPoint
{
	[SerializeField]
	protected ParticleSystem[] _sizeChangingEffects;

	[SerializeField]
	private float _impaceEffectMinScale;

	[SerializeField]
	private float _impaceEffectMaxScale;

	protected override void UpdateChargeEffect(float chargePercent)
	{
		float num = Mathf.Lerp(_minSize, _maxSize, chargePercent);
		ParticleSystem[] sizeChangingEffects = _sizeChangingEffects;
		for (int i = 0; i < sizeChangingEffects.Length; i++)
		{
			ParticleSystem.MainModule main = sizeChangingEffects[i].main;
			main.startSize = num;
		}
	}

	protected override GameObject TrySpawnImpactEffect(RaycastHit hit)
	{
		GameObject gameObject = base.TrySpawnImpactEffect(hit);
		if (gameObject != null)
		{
			gameObject.transform.localScale = Vector3.one * Mathf.Lerp(_impaceEffectMinScale, _impaceEffectMaxScale, _chargePercent);
		}
		return gameObject;
	}
}
