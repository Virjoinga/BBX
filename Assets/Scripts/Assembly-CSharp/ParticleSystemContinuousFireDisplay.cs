using UnityEngine;

public class ParticleSystemContinuousFireDisplay : BaseContinuousFireDisplay
{
	[SerializeField]
	private ParticleSystem _particleSystem;

	[SerializeField]
	private AudioSource _audioSource;

	private void Start()
	{
		_particleSystem.Stop();
		if (_audioSource != null)
		{
			_audioSource.Stop();
		}
	}

	private void OnDisable()
	{
		_particleSystem.Stop();
		if (_audioSource != null)
		{
			_audioSource.Stop();
		}
	}

	public override void Toggle(bool isOn)
	{
		if (isOn && !_particleSystem.isPlaying)
		{
			_particleSystem.Play();
			if (_audioSource != null)
			{
				_audioSource.Play();
			}
		}
		else if (!isOn && _particleSystem.isPlaying)
		{
			_particleSystem.Stop();
			if (_audioSource != null)
			{
				_audioSource.Stop();
			}
		}
	}
}
