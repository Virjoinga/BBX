using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSCore
{
	public class Enum<T> where T : struct, IConvertible
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
			TryParse(name, out var value);
			return value;
		}

		public static bool TryParse(string name, out T value)
		{
			value = First();
			try
			{
				value = (T)Enum.Parse(typeof(T), name);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static T First()
		{
			return (T)Enum.GetValues(typeof(T)).GetValue(0);
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
}
