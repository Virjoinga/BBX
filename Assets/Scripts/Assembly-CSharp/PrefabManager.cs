using System.Collections.Generic;
using UnityEngine;

public static class PrefabManager
{
	private static Dictionary<GameObject, int> _usageCounts = new Dictionary<GameObject, int>();

	private static Dictionary<string, GameObject> _resourcesByItemId = new Dictionary<string, GameObject>();

	public static GameObject Request(ProfileWithHeroClass profile, string parentPath = "")
	{
		if (!string.IsNullOrEmpty(parentPath) && !parentPath.EndsWith("/"))
		{
			parentPath += "/";
		}
		string pathForItemType = GetPathForItemType(profile);
		return RequestViaPath(parentPath + profile.Id, pathForItemType);
	}

	public static T Request<T>(ProfileWithHeroClass profile, string parentPath = "") where T : MonoBehaviour
	{
		GameObject gameObject = Request(profile, parentPath);
		if (!(gameObject != null))
		{
			return null;
		}
		return gameObject.GetComponent<T>();
	}

	private static string GetPathForItemType(ProfileWithHeroClass profile)
	{
		_ = profile.ItemType;
		return profile.Id;
	}

	public static void Release(ProfileWithHeroClass profile)
	{
		if (profile != null)
		{
			Release(profile.Id);
		}
	}

	public static void Release(string itemId)
	{
		if (_resourcesByItemId.TryGetValue(itemId, out var value) && _usageCounts.ContainsKey(value))
		{
			_usageCounts[value] -= 1;
			if (_usageCounts[value] <= 0)
			{
				_usageCounts.Remove(value);
				_resourcesByItemId.Remove(itemId);
			}
		}
	}

	public static GameObject RequestViaPath(string path)
	{
		string[] array = path.Split('/');
		return RequestViaPath(array[array.Length - 1], path);
	}

	public static T RequestViaPath<T>(string path) where T : MonoBehaviour
	{
		string[] array = path.Split('/');
		GameObject gameObject = RequestViaPath(array[array.Length - 1], path);
		if (!(gameObject != null))
		{
			return null;
		}
		return gameObject.GetComponent<T>();
	}

	private static GameObject RequestViaPath(string itemId, string path)
	{
		if (!_resourcesByItemId.TryGetValue(itemId, out var value))
		{
			Debug.Log("[PrefabManager] Loading resource at path: " + path);
			value = Resources.Load<GameObject>(path);
			if (value == null)
			{
				Debug.LogError("[PrefabManager] Resource id " + itemId + " at path " + path + " could not be found. FIX IT! FIX IT! FIX IT! FIX IT! FIX IT!");
				return null;
			}
			_resourcesByItemId.Add(itemId, value);
		}
		if (!_usageCounts.ContainsKey(value))
		{
			_usageCounts.Add(value, 0);
		}
		_usageCounts[value] += 1;
		return value;
	}
}
