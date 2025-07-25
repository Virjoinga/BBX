using UnityEngine;

public static class ColorExtensions
{
	public static string ToHexString(this Color color)
	{
		return ColorUtility.ToHtmlStringRGB(color);
	}
}
