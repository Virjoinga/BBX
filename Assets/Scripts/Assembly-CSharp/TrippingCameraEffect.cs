using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

public class TrippingCameraEffect : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private Volume _postProcessVolume;

	[SerializeField]
	private AudioReverbFilter _reverbFilter;

	[SerializeField]
	private float _colorShiftSpeed = 2f;

	[SerializeField]
	private float _lensDistortionSpeed = 2f;

	private ColorAdjustments _colorGradingEffect;

	private LensDistortion _lensDistortionEffect;

	private TweenerCore<float, float, FloatOptions> _colorGradingTweeningHandle;

	private TweenerCore<float, float, FloatOptions> _lensDistortionTweeningHandle;

	private void Start()
	{
		_postProcessVolume.profile.TryGet<ColorAdjustments>(out _colorGradingEffect);
		_postProcessVolume.profile.TryGet<LensDistortion>(out _lensDistortionEffect);
		_signalBus.Subscribe<TrippingEffectUpdatedSignal>(OnTrippingEffectUpdated);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<TrippingEffectUpdatedSignal>(OnTrippingEffectUpdated);
	}

	private void OnTrippingEffectUpdated(TrippingEffectUpdatedSignal signal)
	{
		if (signal.IsActive)
		{
			ShowEffect();
		}
		else
		{
			HideEffect();
		}
	}

	private void ShowEffect()
	{
		KillAllTweens();
		_postProcessVolume.weight = 1f;
		_reverbFilter.enabled = true;
		_colorGradingEffect.hueShift.value = -180f;
		_colorGradingTweeningHandle = DOTween.To(() => _colorGradingEffect.hueShift.value, delegate(float newValue)
		{
			_colorGradingEffect.hueShift.value = newValue;
		}, 180f, _colorShiftSpeed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
		_lensDistortionEffect.intensity.value = -0.7f;
		_lensDistortionTweeningHandle = DOTween.To(() => _lensDistortionEffect.intensity.value, delegate(float newValue)
		{
			_lensDistortionEffect.intensity.value = newValue;
		}, 0.7f, _lensDistortionSpeed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
	}

	private void HideEffect()
	{
		_postProcessVolume.weight = 0f;
		_reverbFilter.enabled = false;
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		if (_colorGradingTweeningHandle != null && _colorGradingTweeningHandle.IsPlaying())
		{
			_colorGradingTweeningHandle.Kill();
		}
		if (_lensDistortionTweeningHandle != null && _lensDistortionTweeningHandle.IsPlaying())
		{
			_lensDistortionTweeningHandle.Kill();
		}
	}
}
