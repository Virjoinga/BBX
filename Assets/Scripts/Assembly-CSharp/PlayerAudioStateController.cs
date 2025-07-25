using System.Collections;
using UnityEngine;

public class PlayerAudioStateController : MonoBehaviour
{
	[SerializeField]
	private AudioSource _audio;

	[SerializeField]
	private AudioClip _weaponPickupAudio;

	private bool _wasGrounded = true;

	private AudioClip _landAudio;

	private AudioClip _walkAudio;

	private AudioClip _emoteAudio;

	private bool _audioOverriden;

	[SerializeField]
	private float _landAudioMinFallTime = 0.25f;

	private float _lastJumpTime;

	public void Init(Outfit outfit)
	{
		_landAudio = outfit.LandAudio;
		_walkAudio = outfit.WalkAudio;
		_emoteAudio = outfit.EmoteAudio;
	}

	public void PlayAudioOverride(AudioClip clip)
	{
		StopAllCoroutines();
		_audio.Stop();
		_audio.clip = clip;
		_audio.Play();
		StartCoroutine(WaitForClipToComplete(clip));
	}

	public void SetAudioState(float speed, float strafeSpeed, bool isGrounded)
	{
		if (!_audioOverriden)
		{
			if (!_wasGrounded && isGrounded && Time.realtimeSinceStartup - _lastJumpTime > _landAudioMinFallTime)
			{
				_audio.Stop();
				_audio.loop = false;
				_audio.clip = _landAudio;
				_audio.Play();
			}
			else if (isGrounded && (Mathf.Abs(speed) > 0.1f || Mathf.Abs(strafeSpeed) > 0.1f) && !_audio.isPlaying)
			{
				_audio.Stop();
				_audio.loop = true;
				_audio.clip = _walkAudio;
				_audio.Play();
			}
			else if ((Mathf.Abs(speed) <= 0.1f || !isGrounded) && _audio.clip != _landAudio)
			{
				_audio.Stop();
			}
			if (_wasGrounded && !isGrounded)
			{
				_lastJumpTime = Time.realtimeSinceStartup;
			}
			_wasGrounded = isGrounded;
		}
	}

	public void PlayEmoteMusic()
	{
		if (!(_emoteAudio == null))
		{
			_audioOverriden = true;
			_audio.Stop();
			_audio.loop = false;
			_audio.clip = _emoteAudio;
			_audio.Play();
			StartCoroutine(WaitForClipToComplete(_emoteAudio));
		}
	}

	public void PlayWeaponPickupSFX()
	{
		if (!_audioOverriden)
		{
			_audioOverriden = true;
			_audio.Stop();
			_audio.loop = false;
			_audio.clip = _weaponPickupAudio;
			_audio.Play();
			StartCoroutine(WaitForClipToComplete(_weaponPickupAudio));
		}
	}

	private IEnumerator WaitForClipToComplete(AudioClip clip)
	{
		if (!(clip == null))
		{
			_audioOverriden = true;
			yield return new WaitForSeconds(clip.length);
			_audioOverriden = false;
		}
	}
}
