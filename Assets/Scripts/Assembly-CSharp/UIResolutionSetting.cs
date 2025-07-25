public class UIResolutionSetting : UISettingsCarousel
{
	public int ActiveIndex { get; set; }

	protected override void Setup()
	{
	}

	public override bool IsDirty()
	{
		return base.CurrentIndex != ActiveIndex;
	}

	public override void ResetToSavedValue()
	{
		SetCarouselValue(ActiveIndex);
	}

	public override void SaveToDataStore()
	{
	}

	public override void SetDefaultValue()
	{
	}
}
