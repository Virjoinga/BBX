using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GloveBlasterBallEffect : MonoBehaviour
{
	[SerializeField]
	private AudioSource _audio;

	private void Reset()
	{
		_audio = GetComponent<AudioSource>();
		_audio.loop = false;
	}

	private void OnEnable()
	{
		_audio.Play();
	}

	private void OnDisable()
	{
		_audio.Stop();
	}
}
