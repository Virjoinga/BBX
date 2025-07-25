using Zenject;

public class UIGraphicSetting : UISettingsCarousel
{
	[Inject]
	private GraphicSettingsManager _graphicSettingsManager;

	protected override void Setup()
	{
		SetOptions(_graphicSettingsManager.GetOptions(_dataStoreKey).ToArray());
		base.Setup();
	}
}
