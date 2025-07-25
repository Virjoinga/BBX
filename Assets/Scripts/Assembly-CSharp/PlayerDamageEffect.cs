using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public class PlayerDamageEffect : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private Volume _postProcessVolume;

	private float _maxHealth = 100f;

	private float _cachedHealth;

	private TweenerCore<float, float, FloatOptions> _tweeningHandle;

	private float _maxEffectWeight = 0.77f;

	private float _minEffectWeight = 0.6f;

	private IEnumerator Start()
	{
		_signalBus.Subscribe<PlayerHealthChangedSignal>(OnPlayerHealthChangedSignal);
		yield return new WaitUntil(() => PlayerController.HasLocalPlayer);
		_maxHealth = PlayerController.LocalPlayer.LoadoutController.TryGetModifiedHealth();
		_cachedHealth = _maxHealth;
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<PlayerHealthChangedSignal>(OnPlayerHealthChangedSignal);
	}

	private void OnPlayerHealthChangedSignal(PlayerHealthChangedSignal playerHealthChangedSignal)
	{
		_maxHealth = playerHealthChangedSignal.MaxHealth;
		KillAllTweens();
		float updatedHealth = playerHealthChangedSignal.UpdatedHealth;
		if (updatedHealth <= 0f || updatedHealth >= _maxHealth)
		{
			_tweeningHandle = DOTween.To(() => _postProcessVolume.weight, delegate(float newWeight)
			{
				_postProcessVolume.weight = newWeight;
			}, 0f, 0.5f);
			return;
		}
		if (updatedHealth < _cachedHealth)
		{
			_tweeningHandle = DOTween.To(() => _postProcessVolume.weight, delegate(float newWeight)
			{
				_postProcessVolume.weight = newWeight;
			}, _maxEffectWeight, 0.5f).OnComplete(delegate
			{
				SetWeightForCurrentHealth(updatedHealth);
			});
		}
		else
		{
			SetWeightForCurrentHealth(updatedHealth);
		}
		_cachedHealth = updatedHealth;
	}

	private void SetWeightForCurrentHealth(float currentHealth)
	{
		float num = currentHealth / _maxHealth;
		float endValue;
		if (num > 0.5f)
		{
			endValue = 0f;
		}
		else
		{
			float t = Mathf.InverseLerp(0f, 0.5f, num);
			endValue = Mathf.Lerp(_maxEffectWeight, _minEffectWeight, t);
		}
		_tweeningHandle = DOTween.To(() => _postProcessVolume.weight, delegate(float newWeight)
		{
			_postProcessVolume.weight = newWeight;
		}, endValue, 0.5f);
	}

	private void KillAllTweens()
	{
		if (_tweeningHandle != null && _tweeningHandle.IsPlaying())
		{
			_tweeningHandle.Kill();
		}
	}
}
