using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundWhenEnabled : MonoBehaviour
{
	[SerializeField]
	private AudioSource _audioSource;

	private void OnEnable()
	{
		_audioSource.Play();
	}

	private void OnDisable()
	{
		_audioSource.Stop();
	}
}
