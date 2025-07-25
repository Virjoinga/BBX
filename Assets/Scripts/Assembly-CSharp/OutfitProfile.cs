using System.Collections.Generic;
using BSCore;
using UnityEngine;

public class OutfitProfile : ProfileWithHeroClass
{
	public HeroClassProfile HeroClassProfile { get; private set; }

	public OutfitProfile(GameItem gameItem, Dictionary<string, BaseProfile> profiles)
		: base(gameItem)
	{
		if (profiles.TryGetValue(base.HeroClass.ToString(), out var value))
		{
			HeroClassProfile = value as HeroClassProfile;
		}
		else
		{
			Debug.LogError($"[OutfitProfile] Failed to find heroClass profile {base.HeroClass}");
		}
	}
}
