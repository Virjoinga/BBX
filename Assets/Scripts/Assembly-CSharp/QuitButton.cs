using BSCore;
using UnityEngine;

public class QuitButton : UIBaseButtonClickHandler
{
	protected override void OnClick()
	{
		Application.Quit();
	}
}
