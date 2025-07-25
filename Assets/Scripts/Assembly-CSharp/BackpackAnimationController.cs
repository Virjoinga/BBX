using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BackpackAnimationController : BaseAnimationController
{
	public enum Parameters
	{
		Open = 0,
		Close = 1
	}

	private AudioSource _audio;

	protected override void Awake()
	{
		base.Awake();
		_audio = GetComponent<AudioSource>();
		_audio.playOnAwake = false;
		_audio.Stop();
	}

	public void Open(bool playAudio)
	{
		ResetTrigger(Parameters.Close);
		SetTrigger(Parameters.Open);
		if (playAudio)
		{
			_audio.Play();
		}
	}

	public void Close()
	{
		SetTrigger(Parameters.Close);
	}
}
