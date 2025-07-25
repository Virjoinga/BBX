using System;
using System.Collections;
using UnityEngine;

namespace BSCore
{
	public static class CoroutineExtensions
	{
		public static IEnumerator RunPeriodically(Action func, float delay, float period)
		{
			for (float t = 0f; t < delay; t += Time.deltaTime)
			{
				yield return null;
			}
			while (true)
			{
				func();
				for (float t = 0f; t < period; t += Time.deltaTime)
				{
					yield return null;
				}
			}
		}
	}
}
