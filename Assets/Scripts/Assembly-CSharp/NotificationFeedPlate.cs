using System.Collections;
using BSCore;
using TMPro;
using UnityEngine;

public class NotificationFeedPlate : FadeableUI
{
	[SerializeField]
	private TextMeshProUGUI _message;

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

	public void Populate(string message)
	{
		StopAllCoroutines();
		base.transform.SetAsLastSibling();
		_message.text = message;
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
