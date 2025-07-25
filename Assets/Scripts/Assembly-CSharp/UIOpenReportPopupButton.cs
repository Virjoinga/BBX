using BSCore;

public class UIOpenReportPopupButton : UIBaseButtonClickHandler
{
	protected override void OnClick()
	{
		UIPrefabManager.Instantiate(UIPrefabIds.UserReportPopup, interactive: true, 11);
	}
}
