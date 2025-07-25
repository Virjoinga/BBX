using BSCore;
using UnityEngine;

public class ActiveUIFadeOnOpen : ActiveUIStateChangedBase
{
	[SerializeField]
	private FadeableUI[] _objectsToFade = new FadeableUI[0];

	[SerializeField]
	private float _fadeDuration = 0.1f;

	protected override void OnActiveUIShown()
	{
		FadeableUI[] objectsToFade = _objectsToFade;
		for (int i = 0; i < objectsToFade.Length; i++)
		{
			objectsToFade[i].FadeOut(_fadeDuration);
		}
	}

	protected override void OnActiveUIHidden()
	{
		FadeableUI[] objectsToFade = _objectsToFade;
		for (int i = 0; i < objectsToFade.Length; i++)
		{
			objectsToFade[i].FadeIn(_fadeDuration);
		}
	}
}
