using System;
using System.Collections;
using UnityEngine;

namespace BSCore
{
	[RequireComponent(typeof(CanvasGroup))]
	public class FadeableUI : MonoBehaviour
	{
		private CanvasGroup _canvasGroupBacker;

		protected CanvasGroup _canvasGroup
		{
			get
			{
				if (_canvasGroupBacker == null)
				{
					_canvasGroupBacker = GetComponent<CanvasGroup>();
				}
				return _canvasGroupBacker;
			}
		}

		private void Reset()
		{
			_canvasGroupBacker = GetComponent<CanvasGroup>();
		}

		public void Stop()
		{
			StopAllCoroutines();
		}

		public void FadeOut(float duration, Action onComplete)
		{
			FadeOut(duration, isInteractable: false, onComplete);
		}

		public void FadeOut(float duration, bool isInteractable = false, Action onComplete = null)
		{
			StopAllCoroutines();
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(FadeOutRoutine(duration, onComplete));
				SetInteractibleState(isInteractable);
			}
		}

		public void FadeIn(float duration, Action onComplete)
		{
			FadeIn(duration, isInteractable: true, onComplete);
		}

		public void FadeIn(float duration, bool isInteractable = true, Action onComplete = null)
		{
			StopAllCoroutines();
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(FadeInRoutine(duration, onComplete));
				SetInteractibleState(isInteractable);
			}
		}

		public void FadeTo(float desiredOpacity, float duration, bool isInteractable, Action onComplete)
		{
			StopAllCoroutines();
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(FadeToRoutine(desiredOpacity, duration, onComplete));
				SetInteractibleState(isInteractable);
			}
		}

		protected IEnumerator FadeOutRoutine(float duration, Action onComplete)
		{
			yield return StartCoroutine(FadeToFrom(_canvasGroup.alpha, 0f, duration, onComplete));
		}

		protected IEnumerator FadeInRoutine(float duration, Action onComplete)
		{
			yield return StartCoroutine(FadeToFrom(_canvasGroup.alpha, 1f, duration, onComplete));
		}

		public IEnumerator FadeToRoutine(float desiredOpacity, float duration, Action onComplete)
		{
			yield return StartCoroutine(FadeToFrom(_canvasGroup.alpha, desiredOpacity, duration, onComplete));
		}

		protected IEnumerator FadeToFrom(float startOpacity, float desiredOpacity, float duration, Action onComplete)
		{
			if (duration > 0f)
			{
				float step = 1f / duration;
				for (float t = 0f; t <= 1f; t += Time.deltaTime * step)
				{
					_canvasGroup.alpha = Mathf.SmoothStep(startOpacity, desiredOpacity, t);
					yield return null;
				}
			}
			_canvasGroup.alpha = desiredOpacity;
			onComplete?.Invoke();
		}

		private void SetInteractibleState(bool isInteractable)
		{
			_canvasGroup.interactable = isInteractable;
			_canvasGroup.blocksRaycasts = isInteractable;
		}
	}
}
