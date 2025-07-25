using BSCore;
using UnityEngine;
using Zenject;

public class PlayMusicOnAwake : MonoBehaviour
{
	[SerializeField]
	private AudioClip _musicClip;

	[Inject]
	private BackgroundMusicManager _backgroundMusicManager;

	private void Awake()
	{
		_backgroundMusicManager.PlayBackgroundMusic(_musicClip);
	}
}
