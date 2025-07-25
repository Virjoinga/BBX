using BSCore;

public class CloseAllActiveUIsButton : UIBaseButtonClickHandler
{
	protected override void OnClick()
	{
		ActiveUI.Manager.CloseAllActiveUI();
	}
}
