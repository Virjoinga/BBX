using System;
using BSCore;

public class ProfileWithHeroClass : BaseProfile
{
	[Serializable]
	public class ProfileWithHeroClassData : BaseProfileData
	{
		public string heroClass;
	}

	public HeroClass HeroClass { get; private set; }

	public ProfileWithHeroClass(GameItem gameItem)
		: base(gameItem)
	{
	}

	protected override void DeserializeData(string json)
	{
		_profileData = BaseProfileData.FromJson<ProfileWithHeroClassData>(json);
	}

	protected override void ParseCustomData()
	{
		base.ParseCustomData();
		Enum<HeroClass>.TryParse((_profileData as ProfileWithHeroClassData).heroClass, out var value);
		HeroClass = value;
	}
}
