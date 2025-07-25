using System;
using System.Collections;
using UnityEngine;

public static class MonoBehaviourExtensions
{
	public static IEnumerator PerformOverDuration(this MonoBehaviour behaviour, float duration, Action<float> onTick, bool smooth = false)
	{
		float step = 1f / duration;
		for (float t = 0f; t <= 1f; t += Time.deltaTime * step)
		{
			if (smooth)
			{
				onTick(Mathf.SmoothStep(0f, 1f, t));
			}
			else
			{
				onTick(t);
			}
			yield return null;
		}
		onTick(1f);
	}
}
