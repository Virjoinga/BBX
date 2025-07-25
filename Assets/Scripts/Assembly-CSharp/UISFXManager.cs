using UnityEngine;

public class UISFXManager : MonoBehaviourSingleton<UISFXManager>
{
	[SerializeField]
	private AudioSource _audioSource;

	[SerializeField]
	private AudioClip _defaultButtonClickSFX;

	public void PlayDefaultButtonClick()
	{
		PlayButtonClick(_defaultButtonClickSFX);
	}

	public void PlayButtonClick(AudioClip audioClip)
	{
		_audioSource.PlayOneShot(audioClip);
	}
}
