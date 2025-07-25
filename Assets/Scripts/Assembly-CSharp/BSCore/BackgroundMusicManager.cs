using System.Collections;
using UnityEngine;
using Zenject;

namespace BSCore
{
	[RequireComponent(typeof(AudioSource))]
	public class BackgroundMusicManager : MonoBehaviour
	{
		[Inject]
		private SignalBus _signalBus;

		[SerializeField]
		private AudioSource _audioSource;

		[SerializeField]
		private float _fadeDuration = 0.75f;

		private void Reset()
		{
			_audioSource = GetComponent<AudioSource>();
		}

		private void Awake()
		{
			_signalBus.Subscribe<TrippingEffectUpdatedSignal>(OnTrippingEffectUpdated);
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<TrippingEffectUpdatedSignal>(OnTrippingEffectUpdated);
		}

		private void OnTrippingEffectUpdated(TrippingEffectUpdatedSignal signal)
		{
			if (signal.IsActive)
			{
				StartCoroutine(FadeTo(0f, 0.75f));
			}
			else
			{
				StartCoroutine(FadeTo(1f, 0.75f));
			}
		}

		public void PlayBackgroundMusic(AudioClip clip)
		{
			if (!(clip == _audioSource.clip))
			{
				StopAllCoroutines();
				StartCoroutine(FadeToNextClip(clip));
			}
		}

		private IEnumerator FadeToNextClip(AudioClip clip)
		{
			yield return FadeTo(0f, _fadeDuration);
			_audioSource.Stop();
			_audioSource.clip = clip;
			_audioSource.Play();
			yield return FadeTo(1f, _fadeDuration);
		}

		private IEnumerator FadeTo(float volume, float duration)
		{
			float start = _audioSource.volume;
			float step = 1f / duration;
			for (float t = 0f; t <= 1f; t += Time.deltaTime * step)
			{
				_audioSource.volume = Mathf.Clamp01(Mathf.SmoothStep(start, volume, t));
				yield return null;
			}
		}
	}
}
