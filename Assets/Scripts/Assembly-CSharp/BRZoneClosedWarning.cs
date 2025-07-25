using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class BRZoneClosedWarning : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private RectTransform _rectTransform;

	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private float _minScale = 0.7f;

	[SerializeField]
	private float _scaleTime = 0.5f;

	private bool _isShowingWarning;

	private Tweener _tweener;

	private void Start()
	{
		_text.enabled = false;
		_signalBus.Subscribe<EnterExitClosedZoneSignal>(OnEnterExitClosedZone);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<EnterExitClosedZoneSignal>(OnEnterExitClosedZone);
	}

	private void OnEnterExitClosedZone(EnterExitClosedZoneSignal enterExitClosedZoneSignal)
	{
		if (enterExitClosedZoneSignal.InClosedZone)
		{
			TryShowWarning();
		}
		else
		{
			TryHideWarning();
		}
	}

	private void TryShowWarning()
	{
		if (!_isShowingWarning)
		{
			_tweener.Kill();
			_rectTransform.DOScale(0f, 0f);
			_text.enabled = true;
			_tweener = _rectTransform.DOScale(1f, _scaleTime).OnComplete(delegate
			{
				_tweener = _rectTransform.DOScale(_minScale, _scaleTime).SetLoops(-1, LoopType.Yoyo);
			});
			_isShowingWarning = true;
		}
	}

	private void TryHideWarning()
	{
		if (_isShowingWarning)
		{
			_tweener.Kill();
			_tweener = _rectTransform.DOScale(0f, _scaleTime).OnComplete(delegate
			{
				_text.enabled = false;
			});
			_isShowingWarning = false;
		}
	}
}
