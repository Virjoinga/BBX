using System;
using System.Collections.Generic;
using System.Text;

namespace BSCore
{
	public static class DictionaryExtensions
	{
		public static string ToJsonString(this IDictionary<string, string> dict)
		{
			StringBuilder stringBuilder = new StringBuilder("{\n");
			foreach (KeyValuePair<string, string> item in dict)
			{
				stringBuilder.AppendLine("    \"" + item.Key + "\": \"" + item.Value + "\"");
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public static void Merge(this Dictionary<string, string> a, Dictionary<string, string> b, bool overwrite = false)
		{
			foreach (KeyValuePair<string, string> item in b)
			{
				if (a.ContainsKey(item.Key))
				{
					if (overwrite)
					{
						a[item.Key] = b[item.Key];
					}
				}
				else
				{
					a.Add(item.Key, item.Value);
				}
			}
		}

		public static void Merge(this IDictionary<string, string> a, IDictionary<string, string> b, bool overwrite = false)
		{
			foreach (KeyValuePair<string, string> item in b)
			{
				if (a.ContainsKey(item.Key))
				{
					if (overwrite)
					{
						a[item.Key] = b[item.Key];
					}
				}
				else
				{
					a.Add(item.Key, item.Value);
				}
			}
		}

		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
		{
			if (!dictionary.TryGetValue(key, out var value))
			{
				return defaultValue;
			}
			return value;
		}

		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider)
		{
			if (!dictionary.TryGetValue(key, out var value))
			{
				return defaultValueProvider();
			}
			return value;
		}

		public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
		{
			if (!dictionary.TryGetValue(key, out var value))
			{
				return defaultValue;
			}
			return value;
		}

		public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider)
		{
			if (!dictionary.TryGetValue(key, out var value))
			{
				return defaultValueProvider();
			}
			return value;
		}
	}
}
