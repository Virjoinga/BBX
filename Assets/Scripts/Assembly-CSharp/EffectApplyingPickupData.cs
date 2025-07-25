public class EffectApplyingPickupData : PickupData
{
	public WeaponProfile.EffectData Effect;

	public EffectApplyingPickupData(EffectApplyingPickupConfigData data)
		: base(data)
	{
		Effect = new WeaponProfile.EffectData(data.effect);
	}
}
