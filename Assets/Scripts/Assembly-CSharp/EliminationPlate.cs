using System.Collections;
using BSCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EliminationPlate : FadeableUI
{
	[SerializeField]
	private TextMeshProUGUI _eliminatedPlayerName;

	[SerializeField]
	private Image _eliminationIcon;

	[SerializeField]
	private Sprite _assistSprite;

	[SerializeField]
	private Sprite _finalBlowSprite;

	[SerializeField]
	private float _fadeInDuration = 0.2f;

	[SerializeField]
	private float _fadeOutDuration = 0.3f;

	[SerializeField]
	private float _duration = 3f;

	private void OnEnable()
	{
		FadeOut(0f);
	}

	public void Populate(string eliminatedPlayerName, bool isFinalBlow)
	{
		StopAllCoroutines();
		base.transform.SetAsLastSibling();
		_eliminatedPlayerName.text = eliminatedPlayerName;
		_eliminationIcon.sprite = (isFinalBlow ? _finalBlowSprite : _assistSprite);
		StartCoroutine(ShowFadeEffect());
	}

	private IEnumerator ShowFadeEffect()
	{
		yield return FadeToRoutine(1f, _fadeInDuration, null);
		yield return new WaitForSeconds(_duration);
		yield return FadeToRoutine(0f, _fadeOutDuration, null);
		SmartPool.Despawn(base.gameObject);
	}
}
