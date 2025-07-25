using DG.Tweening;
using UnityEngine;
using Zenject;

public class MenuSlideOutUI : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private RectTransform _topBar;

	[SerializeField]
	private RectTransform _bottomBar;

	[SerializeField]
	private RectTransform _leftPanel;

	[SerializeField]
	private RectTransform _rightPanel;

	private float _slideDuration = 0.5f;

	[Inject]
	private void Construct()
	{
		_signalBus.Subscribe<SlideOutMenuUISignal>(SlideOutUI);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<SlideOutMenuUISignal>(SlideOutUI);
	}

	private void SlideOutUI()
	{
		_topBar.DOAnchorPosY(90f, _slideDuration);
		_bottomBar.DOAnchorPosY(-90f, _slideDuration);
		_leftPanel.DOAnchorPosX(-550f, _slideDuration);
		_rightPanel.DOAnchorPosX(450f, _slideDuration);
	}
}
