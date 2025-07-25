using System;
using System.Collections;
using UnityEngine;

public class PooledEffectController : MonoBehaviour
{
	[SerializeField]
	private bool _useParticleSystemsLifetime = true;

	[SerializeField]
	protected float _lifetime = 1f;

	protected AudioSource _audioSource;

	private ParticleSystem[] _particleSystems;

	private Coroutine _despawnCoroutine;

	protected virtual void Awake()
	{
		_particleSystems = GetComponentsInChildren<ParticleSystem>();
		_audioSource = GetComponentInChildren<AudioSource>();
		if (_useParticleSystemsLifetime)
		{
			_lifetime = GetLongestEffect();
		}
	}

	protected virtual void OnEnable()
	{
		if (_despawnCoroutine != null)
		{
			StopCoroutine(_despawnCoroutine);
		}
		StopAll();
		PlayAll();
		_despawnCoroutine = StartCoroutine(DespawnAfterPlayComplete());
	}

	private IEnumerator DespawnAfterPlayComplete()
	{
		yield return new WaitForSeconds(_lifetime);
		Despawn();
	}

	protected virtual void Despawn()
	{
		SmartPool.Despawn(base.gameObject);
	}

	protected virtual void StopAll()
	{
		if (_audioSource != null)
		{
			_audioSource.Stop();
		}
		ForEachParticleSystem(Stop);
	}

	protected virtual void PlayAll()
	{
		if (!BoltNetwork.IsServer)
		{
			if (_audioSource != null)
			{
				_audioSource.Play();
			}
			ForEachParticleSystem(Play);
		}
	}

	protected virtual void ForEachParticleSystem(Action<ParticleSystem> callback)
	{
		ParticleSystem[] particleSystems = _particleSystems;
		foreach (ParticleSystem obj in particleSystems)
		{
			callback(obj);
		}
	}

	protected virtual void Stop(ParticleSystem particleSystem)
	{
		if (particleSystem.isPlaying)
		{
			particleSystem.Stop();
		}
	}

	protected virtual void Play(ParticleSystem particleSystem)
	{
		particleSystem.Play();
	}

	protected virtual float GetLongestEffect()
	{
		float playLength = 0f;
		if (_audioSource != null && _audioSource.clip != null)
		{
			playLength = _audioSource.clip.length;
		}
		ForEachParticleSystem(delegate(ParticleSystem particleSystem)
		{
			if (particleSystem.main.duration > playLength)
			{
				playLength = particleSystem.main.duration;
			}
		});
		return playLength;
	}
}
