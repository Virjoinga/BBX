using BSCore;
using UnityEngine;

public class BRSpectateButton : UIBaseButtonClickHandler
{
	[SerializeField]
	private BRSecondLifeButton _secondLifeButton;

	protected override void OnClick()
	{
		ClientBattleRoyaleCallbacks.IsSpectating = true;
		_secondLifeButton.gameObject.SetActive(value: false);
		base.gameObject.SetActive(value: false);
	}
}
