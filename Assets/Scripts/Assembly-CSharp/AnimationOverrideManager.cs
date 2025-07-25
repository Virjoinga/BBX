using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AnimationOverrideManager
{
	private static Dictionary<AnimatorOverrideController, int> _usageCounts = new Dictionary<AnimatorOverrideController, int>();

	private static Dictionary<string, AnimatorOverrideController> _resourcesByCombo = new Dictionary<string, AnimatorOverrideController>();

	public static AnimatorOverrideController Request(string classId, string weaponType)
	{
		string path = GetPath(classId, weaponType);
		Debug.Log("[AnimationOverrideManager] Requesting resource at path: " + path);
		if (!_resourcesByCombo.TryGetValue(path, out var value))
		{
			Debug.Log("[AnimationOverrideManager] Loading resource at path: " + path);
			value = Resources.Load<AnimatorOverrideController>(path);
			_resourcesByCombo.Add(path, value);
		}
		if (!_usageCounts.ContainsKey(value))
		{
			_usageCounts.Add(value, 0);
		}
		_usageCounts[value] += 1;
		return value;
	}

	public static void Release(string classId, string weaponType)
	{
		string path = GetPath(classId, weaponType);
		if (!_resourcesByCombo.TryGetValue(path, out var value) && _usageCounts.ContainsKey(value))
		{
			_usageCounts[value] -= 1;
			if (_usageCounts[value] <= 0)
			{
				_usageCounts.Remove(value);
				Resources.UnloadAsset(value);
				_resourcesByCombo.Remove(path);
			}
		}
	}

	private static string GetPath(string classId, string animationType)
	{
		string text = classId + "_" + animationType + "_AnimatorOverrides";
		if (SceneManager.GetActiveScene().name == "MainMenu")
		{
			text = "MainMenu/" + text;
		}
		return text;
	}
}
