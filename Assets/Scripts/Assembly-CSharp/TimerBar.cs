using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
	[SerializeField]
	private Image _bar;

	private TweenerCore<float, float, FloatOptions> _tween;

	public void StartTimer(float duration)
	{
		SetValue(1f);
		_tween = DOTween.To(GetValue, SetValue, 0f, duration);
	}

	private void OnDisable()
	{
		if (_tween != null && _tween.IsPlaying())
		{
			_tween.Kill();
		}
	}

	private float GetValue()
	{
		return _bar.fillAmount;
	}

	private void SetValue(float value)
	{
		_bar.fillAmount = value;
	}
}
