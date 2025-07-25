using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using Constants;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CrosshairController : MonoBehaviour
{
	private enum CrosshairColor
	{
		Neutral = 0,
		Teammate = 1,
		Enemy = 2
	}

	[Serializable]
	public class CrosshairSpriteByType
	{
		public Match.CrosshairType CrosshairType;

		public Sprite Sprite;
	}

	[Inject]
	private SignalBus _signalBus;

	[Inject]
	protected GameConfigData _gameConfigData;

	[SerializeField]
	private Image _crosshairImage;

	[SerializeField]
	private List<CrosshairSpriteByType> _crosshairSpriteByTypes;

	[SerializeField]
	private CanvasGroup _hitMarker;

	[SerializeField]
	private Vector2 _spreadIndicatorSizeRange = Vector2.one;

	[SerializeField]
	private AudioSource _hitMarkerSource;

	private Tweener _hitMarkerTween;

	private float _hitMarkerTweenTime = 0.3f;

	private CrosshairColor _currentCrosshairColor;

	private IEnumerator Start()
	{
		_hitMarker.alpha = 0f;
		_signalBus.Subscribe<ActiveWeaponSlotUpdatedSignal>(OnActiveWeaponSlotUpdated);
		_signalBus.Subscribe<DamagableDamagedSignal>(OnPlayerDamaged);
		_signalBus.Subscribe<WeaponCurrentSpreadUpdated>(OnWeaponCurrentSpreadUpdated);
		yield return new WaitUntil(() => PlayerController.HasLocalPlayer && PlayerController.LocalPlayer.LoadoutController.Outfit != null && PlayerController.LocalPlayer.WeaponHandler.ActiveWeaponProfile != null);
		WeaponProfile activeWeaponProfile = PlayerController.LocalPlayer.WeaponHandler.ActiveWeaponProfile;
		UpdateIconForProfile(activeWeaponProfile);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<ActiveWeaponSlotUpdatedSignal>(OnActiveWeaponSlotUpdated);
		_signalBus.Unsubscribe<DamagableDamagedSignal>(OnPlayerDamaged);
		_signalBus.Unsubscribe<WeaponCurrentSpreadUpdated>(OnWeaponCurrentSpreadUpdated);
	}

	private void OnActiveWeaponSlotUpdated(ActiveWeaponSlotUpdatedSignal activeWeaponSlotUpdatedSignal)
	{
		WeaponProfile profile = activeWeaponSlotUpdatedSignal.Profile;
		UpdateIconForProfile(profile);
		_crosshairImage.transform.localScale = Vector3.one;
	}

	private void UpdateIconForProfile(WeaponProfile profile)
	{
		CrosshairSpriteByType crosshairSpriteByType = _crosshairSpriteByTypes.FirstOrDefault((CrosshairSpriteByType x) => x.CrosshairType == profile.Crosshair);
		if (crosshairSpriteByType != null)
		{
			_crosshairImage.overrideSprite = crosshairSpriteByType.Sprite;
		}
		else
		{
			Debug.LogError($"[CrosshairController] No type option setup for crosshair type {profile.Crosshair}");
		}
	}

	private void OnPlayerDamaged(DamagableDamagedSignal signal)
	{
		if (PlayerController.HasLocalPlayer && signal.Attacker == PlayerController.LocalPlayer.entity)
		{
			ShowHitMarker();
		}
	}

	private void OnWeaponCurrentSpreadUpdated(WeaponCurrentSpreadUpdated signal)
	{
		_crosshairImage.transform.localScale = Vector3.one * Mathf.Lerp(_spreadIndicatorSizeRange.x, _spreadIndicatorSizeRange.y, signal.PercentToMax);
	}

	private void ShowHitMarker()
	{
		if (_hitMarkerTween != null && _hitMarkerTween.IsActive())
		{
			_hitMarkerTween.Kill();
		}
		_hitMarkerSource.PlayOneShot(_hitMarkerSource.clip);
		_hitMarkerTween = _hitMarker.DOFade(1f, _hitMarkerTweenTime).OnComplete(delegate
		{
			_hitMarkerTween = _hitMarker.DOFade(0f, _hitMarkerTweenTime);
		});
	}

	private void FixedUpdate()
	{
		if (Camera.main == null)
		{
			return;
		}
		if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out var hitInfo, 500f, LayerMaskConfig.HitableLayers))
		{
			PlayerController componentInParent = hitInfo.collider.GetComponentInParent<PlayerController>();
			if (componentInParent != null)
			{
				if (componentInParent.IsLocal)
				{
					TrySetCrosshairColor(CrosshairColor.Neutral);
				}
				else if (componentInParent.IsLocalPlayerTeammate)
				{
					TrySetCrosshairColor(CrosshairColor.Teammate);
				}
				else
				{
					TrySetCrosshairColor(CrosshairColor.Enemy);
				}
				return;
			}
		}
		TrySetCrosshairColor(CrosshairColor.Neutral);
	}

	private void TrySetCrosshairColor(CrosshairColor crosshairColor)
	{
		if (crosshairColor != _currentCrosshairColor)
		{
			_currentCrosshairColor = crosshairColor;
			switch (_currentCrosshairColor)
			{
			case CrosshairColor.Neutral:
				_crosshairImage.color = Color.white;
				break;
			case CrosshairColor.Teammate:
				_crosshairImage.color = _gameConfigData.TeammateColor;
				break;
			case CrosshairColor.Enemy:
				_crosshairImage.color = _gameConfigData.EnemyColor;
				break;
			default:
				_crosshairImage.color = Color.white;
				break;
			}
		}
	}
}
