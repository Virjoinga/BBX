using System;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
	public static T Random<T>(this IList<T> list)
	{
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static T Random<T>(this List<T> list)
	{
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static void Shuffle<T>(this IList<T> list, System.Random rng)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = rng.Next(num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}
}
