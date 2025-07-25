using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Enum<T> where T : struct, IConvertible
{
	public static int Count
	{
		get
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			return Enum.GetNames(typeof(T)).Length;
		}
	}

	public static T1 Random<T1>() where T1 : struct, IConvertible
	{
		int count = Enum<T1>.Count;
		int index = UnityEngine.Random.Range(0, count);
		return (T1)Enum.GetValues(typeof(T1)).GetValue(index);
	}

	public static T Parse(string name)
	{
		return (T)Enum.Parse(typeof(T), name);
	}

	public static bool TryParse(string name, out T value)
	{
		return Enum.TryParse<T>(name, out value);
	}

	public static List<string> GetNames()
	{
		return Enum.GetNames(typeof(T)).ToList();
	}

	public static void ForEach(Action<T> action)
	{
		foreach (T value in Enum.GetValues(typeof(T)))
		{
			action(value);
		}
	}
}
