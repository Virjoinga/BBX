using System;
using UnityEngine;

namespace BSCore
{
	public static class ColorExtensions
	{
		public static string ToHexString(this Color color)
		{
			return BitConverter.ToString(new byte[4]
			{
				Convert.ToByte(Mathf.RoundToInt(color.r * 255f)),
				Convert.ToByte(Mathf.RoundToInt(color.g * 255f)),
				Convert.ToByte(Mathf.RoundToInt(color.b * 255f)),
				Convert.ToByte(Mathf.RoundToInt(color.a * 255f))
			}).Replace("-", "");
		}
	}
}
