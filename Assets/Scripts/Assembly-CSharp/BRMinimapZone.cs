using DG.Tweening;
using UnityEngine;

public class BRMinimapZone : MonoBehaviour
{
	[SerializeField]
	private int _zoneId;

	[SerializeField]
	private SpriteRenderer _closingIcon;

	private float _closingFadeLevel = 0.9f;

	private float _closingFadeTime = 0.9f;

	private Tweener _closingTween;

	public int ZoneId => _zoneId;

	public void ShowClosing()
	{
		_closingIcon.DOFade(0.25f, 0f);
		_closingIcon.gameObject.SetActive(value: true);
		_closingTween = _closingIcon.DOFade(_closingFadeLevel, _closingFadeTime).SetLoops(-1, LoopType.Yoyo);
	}

	public void ShowClosed()
	{
		_closingTween.Kill();
		_closingIcon.DOFade(1f, 1.5f);
	}
}
