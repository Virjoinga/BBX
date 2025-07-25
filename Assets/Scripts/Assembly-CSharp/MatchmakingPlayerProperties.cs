using System;
using UnityEngine;

[Serializable]
public class MatchmakingPlayerProperties
{
	public int Skill;

	public int PlayerCount;

	public string BuildVersion;

	public string DisplayName;

	public string OutfitId;

	public object Latency;

	public MatchmakingPlayerProperties(string displayName, string outfitId)
	{
		Skill = 1;
		PlayerCount = 1;
		BuildVersion = Application.version;
		DisplayName = displayName;
		OutfitId = outfitId;
		Latency = new object[1]
		{
			new
			{
				region = "EastUs",
				latency = 30
			}
		};
	}
}
