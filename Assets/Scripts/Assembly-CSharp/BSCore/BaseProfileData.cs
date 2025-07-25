using System;
using UnityEngine;

namespace BSCore
{
	[Serializable]
	public class BaseProfileData
	{
		public int ReleaseVersion;

		public string Rarity = "Common";

		public static T FromJson<T>(string json) where T : BaseProfileData
		{
			return JsonUtility.FromJson<T>(json);
		}
	}
}
