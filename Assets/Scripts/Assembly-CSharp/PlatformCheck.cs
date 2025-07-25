using UnityEngine;

public static class PlatformCheck
{
	public static PlatformType GetPlatform()
	{
		if (Application.isMobilePlatform)
		{
			return PlatformType.Mobile;
		}
		if (Application.isConsolePlatform)
		{
			return PlatformType.Console;
		}
		return PlatformType.PC;
	}
}
