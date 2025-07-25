using System.Collections;
using UnityEngine;

public class DelayedParticleSystemPlay : MonoBehaviour
{
	private ParticleSystem _particleSystem;

	private void Reset()
	{
		ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
		main.playOnAwake = false;
	}

	private void Awake()
	{
		_particleSystem = GetComponent<ParticleSystem>();
		ParticleSystem.MainModule main = _particleSystem.main;
		main.playOnAwake = false;
	}

	private IEnumerator Start()
	{
		yield return null;
		_particleSystem.Play();
	}
}
