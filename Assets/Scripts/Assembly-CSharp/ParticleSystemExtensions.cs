using UnityEngine;

public static class ParticleSystemExtensions
{
	public static void Stop(this ParticleSystem[] particleSystems)
	{
		if (particleSystems == null || particleSystems.Length == 0)
		{
			return;
		}
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			if (particleSystem != null)
			{
				particleSystem.Stop();
			}
		}
	}

	public static void Play(this ParticleSystem[] particleSystems)
	{
		if (particleSystems == null || particleSystems.Length == 0)
		{
			return;
		}
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			if (particleSystem != null)
			{
				particleSystem.Play();
			}
		}
	}
}
