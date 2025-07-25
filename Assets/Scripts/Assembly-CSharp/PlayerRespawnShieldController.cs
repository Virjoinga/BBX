using System.Collections;
using UnityEngine;

public class PlayerRespawnShieldController : BaseEntityBehaviour<IPlayerState>
{
	[SerializeField]
	private ObjectFader _respawnShieldDisplay;

	private float _fadeInDuration = 0.5f;

	private float _fadeOutDuration = 1f;

	protected override void Start()
	{
		base.Start();
		_respawnShieldDisplay.SetFadeLevel(0f);
		_respawnShieldDisplay.gameObject.SetActive(value: false);
	}

	protected override void OnControllerOrRemoteAttached()
	{
		base.state.AddCallback("IsShielded", OnIsShieldedUpdated);
	}

	private void OnIsShieldedUpdated()
	{
		StopAllCoroutines();
		StartCoroutine(FadeShieldTo(base.state.IsShielded));
	}

	private IEnumerator FadeShieldTo(bool toOn)
	{
		if (!_respawnShieldDisplay.gameObject.activeInHierarchy && toOn)
		{
			_respawnShieldDisplay.gameObject.SetActive(value: true);
		}
		float start = (toOn ? 0f : 1f);
		float end = (toOn ? 1f : 0f);
		float num = (toOn ? _fadeInDuration : _fadeOutDuration);
		float step = 1f / num;
		for (float t = 0f; t <= 1f; t += Time.deltaTime * step)
		{
			_respawnShieldDisplay.SetFadeLevel(Mathf.SmoothStep(start, end, t));
			yield return null;
		}
		_respawnShieldDisplay.SetFadeLevel(end);
		if (_respawnShieldDisplay.gameObject.activeInHierarchy && !toOn)
		{
			_respawnShieldDisplay.gameObject.SetActive(value: false);
		}
	}
}
