using System.Collections.Generic;
using UnityEngine;

public class WeaponAudioPlayer : MonoBehaviour
{
	[SerializeField]
	private AudioSource _fireAudioSource;

	[SerializeField]
	private AudioSource _reloadAudioSource;

	[SerializeField]
	private AudioSource _chargeAudioSource;

	[SerializeField]
	private AudioClip _fireClip;

	[SerializeField]
	private AudioClip _chargeClip;

	[SerializeField]
	private AudioClip _reloadClip;

	[SerializeField]
	private AudioClip[] _extraFireClips;

	private List<AudioClip> _fireClips = new List<AudioClip>();

	private void Start()
	{
		_fireClips.Add(_fireClip);
		AudioClip[] extraFireClips = _extraFireClips;
		foreach (AudioClip item in extraFireClips)
		{
			_fireClips.Add(item);
		}
	}

	public void PlayFire()
	{
		PlayClip(_fireAudioSource, GetFireClip());
	}

	public void PlayContinuousFire()
	{
		PlayClip(_fireAudioSource, GetFireClip(), loop: true);
	}

	public void StopContinuousFire()
	{
		_fireAudioSource.Stop();
	}

	public void PlayCharge(bool loop)
	{
		PlayClip(_chargeAudioSource, _chargeClip, loop);
	}

	public void StopCharge()
	{
		_chargeAudioSource.Stop();
	}

	public void PlayReload()
	{
		PlayClip(_reloadAudioSource, _reloadClip, loop: true);
	}

	public void StopReload()
	{
		_reloadAudioSource.Stop();
	}

	private void PlayClip(AudioSource audioSource, AudioClip clip, bool loop = false)
	{
		if (!BoltNetwork.IsServer)
		{
			audioSource.Stop();
			audioSource.clip = clip;
			audioSource.loop = loop;
			audioSource.Play();
		}
	}

	private AudioClip GetFireClip()
	{
		if (_extraFireClips == null || _extraFireClips.Length == 0)
		{
			return _fireClip;
		}
		return _fireClips.Random();
	}
}
