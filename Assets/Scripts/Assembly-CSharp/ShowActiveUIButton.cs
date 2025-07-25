using BSCore;
using UnityEngine;

public class ShowActiveUIButton : UIBaseButtonClickHandler
{
	[SerializeField]
	private BB2ActiveUI _activeUI;

	protected override void OnClick()
	{
		_activeUI.Show();
	}
}
