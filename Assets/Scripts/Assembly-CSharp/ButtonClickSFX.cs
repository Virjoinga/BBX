using BSCore;
using UnityEngine;

public class ButtonClickSFX : UIBaseButtonClickHandler
{
	[SerializeField]
	private AudioClip _clickSFXOverride;

	protected override void OnClick()
	{
		if (_clickSFXOverride == null)
		{
			MonoBehaviourSingleton<UISFXManager>.Instance.PlayDefaultButtonClick();
		}
		else
		{
			MonoBehaviourSingleton<UISFXManager>.Instance.PlayButtonClick(_clickSFXOverride);
		}
	}
}
