using System;

[Serializable]
public class GroundHazardData
{
	public string Id;

	public WeaponProfile.EffectData effect;

	public GroundHazardData(GroundHazardProfileData data)
	{
		Id = data.Id;
		effect = new WeaponProfile.EffectData(data.effect);
	}
}
