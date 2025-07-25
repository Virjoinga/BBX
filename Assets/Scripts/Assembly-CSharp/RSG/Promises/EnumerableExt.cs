using System;
using System.Collections.Generic;

namespace RSG.Promises
{
	public static class EnumerableExt
	{
		public static void Each<T>(this IEnumerable<T> source, Action<T> fn)
		{
			foreach (T item in source)
			{
				fn(item);
			}
		}

		public static void Each<T>(this IEnumerable<T> source, Action<T, int> fn)
		{
			int num = 0;
			foreach (T item in source)
			{
				fn(item, num);
				num++;
			}
		}

		public static IEnumerable<T> FromItems<T>(params T[] items)
		{
			for (int i = 0; i < items.Length; i++)
			{
				yield return items[i];
			}
		}
	}
}
