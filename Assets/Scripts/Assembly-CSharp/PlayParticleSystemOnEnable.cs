using UnityEngine;

public class PlayParticleSystemOnEnable : MonoBehaviour
{
	private ParticleSystem _particleSystem;

	private void Awake()
	{
		_particleSystem = GetComponent<ParticleSystem>();
	}

	private void OnEnable()
	{
		_particleSystem.Play();
	}

	private void OnDisable()
	{
		_particleSystem.Stop();
	}
}
