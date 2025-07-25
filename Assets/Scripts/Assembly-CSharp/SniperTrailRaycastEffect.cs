using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SniperTrailRaycastEffect : PooledEffectController, IRaycastEffect
{
	public void Display(Vector3 forward, float forwardVelocity)
	{
		StartCoroutine(TracerRoutine(forward));
	}

	protected override void OnEnable()
	{
		if (_audioSource != null)
		{
			_audioSource.Stop();
			_audioSource.Play();
		}
	}

	private IEnumerator TracerRoutine(Vector3 forward)
	{
		yield return null;
		if (!Physics.Raycast(new Ray(base.transform.position, forward), out var hitInfo, 500f, LayerMaskConfig.GroundLayers))
		{
			hitInfo.point = base.transform.position + forward * 300f;
		}
		base.transform.position = hitInfo.point;
		yield return new WaitForSeconds(_lifetime);
		Despawn();
	}
}
