using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using Zenject;

public class MenuCharacterDisplayController : MonoBehaviourSingleton<MenuCharacterDisplayController>
{
	[Inject]
	private MenuLoadoutManager _menuLoadoutManager;

	[SerializeField]
	private LoadoutController _loadoutController;

	[SerializeField]
	private PlayerAnimationController _animationController;

	[SerializeField]
	private WeaponHandler _weaponHandler;

	[SerializeField]
	private float _moveDuration = 0.5f;

	[SerializeField]
	private Vector2 _gearUpLensShift = Vector2.zero;

	[SerializeField]
	private Transform _rotatableMount;

	[SerializeField]
	private Vector2 _emotionDelayRange = Vector2.zero;

	[SerializeField]
	private PlayerAnimationController.Parameter[] _emotionOptions = new PlayerAnimationController.Parameter[0];

	private TweenerCore<Vector2, Vector2, VectorOptions> _tweeningHandle;

	private Coroutine _randomEmotionRoutine;

	public LoadoutController LoadoutController => _loadoutController;

	public WeaponHandler WeaponHandler => _weaponHandler;

	private IEnumerator Start()
	{
		yield return new WaitUntil(() => _loadoutController.IsInitialized);
		_menuLoadoutManager.SetCurrentLoadout();
		StartCoroutine(EyeBlinkRoutine());
		_loadoutController.OutfitEquipped += OnOutfitEquipped;
		_animationController.AnimationControllerUpdated += SetOutfitLookAt;
	}

	public void SetOutfitLookAt()
	{
	}

	public void SwitchToGearUp(Vector3 positionerOrigin)
	{
		if (_tweeningHandle != null && _tweeningHandle.IsPlaying())
		{
			_tweeningHandle.Kill();
		}
		_tweeningHandle = DOTween.To(() => Camera.main.lensShift, delegate(Vector2 newLensShift)
		{
			Camera.main.lensShift = newLensShift;
		}, _gearUpLensShift, _moveDuration);
	}

	public void SwitchToHome()
	{
		if (_tweeningHandle != null && _tweeningHandle.IsPlaying())
		{
			_tweeningHandle.Kill();
		}
		_tweeningHandle = DOTween.To(() => Camera.main.lensShift, delegate(Vector2 newLensShift)
		{
			Camera.main.lensShift = newLensShift;
		}, new Vector2(0f, 0f), _moveDuration);
	}

	public void PresentBack()
	{
		_rotatableMount.DORotate(new Vector3(0f, -90f, 0f), 0.5f);
	}

	public void PresentFront()
	{
		_rotatableMount.DORotate(new Vector3(0f, 90f, 0f), 0.5f);
	}

	private IEnumerator EyeBlinkRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(2f, 10f));
			_animationController.SetTrigger(PlayerAnimationController.Parameter.Blink);
		}
	}

	private void OnOutfitEquipped()
	{
		if (_randomEmotionRoutine != null)
		{
			StopCoroutine(_randomEmotionRoutine);
		}
		_randomEmotionRoutine = StartCoroutine(RandomEmotionRoutine());
	}

	private IEnumerator RandomEmotionRoutine()
	{
		Debug.Log("[MenuCharacterDisplayController] Starting RandomEmotionRoutine");
		yield return null;
		_animationController.ApplyRootMotion = true;
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(_emotionDelayRange.x, _emotionDelayRange.y));
			bool hasMelee = WeaponHandler.HasActiveMelee;
			if (!WeaponHandler.HasActiveMelee)
			{
				continue;
			}
			string meLeeName = _loadoutController.MeleeWeapon.name;
			WeaponHandler.SheatheMeleeWeapon();
			yield return new WaitForSeconds(0.4f);
			PlayerAnimationController.Parameter trigger = _emotionOptions[Random.Range(0, _emotionOptions.Length)];
			_animationController.SetTrigger(trigger);
			yield return new MaxWaitUntil(delegate
			{
				AnimatorStateInfo currentAnimatorStateInfo2 = _animationController.GetCurrentAnimatorStateInfo(0);
				return (currentAnimatorStateInfo2.IsName("IdleStandingMotion1") || currentAnimatorStateInfo2.IsName("IdleStandingMotion2") || currentAnimatorStateInfo2.IsName("IdleStandingMotion3")) ? true : false;
			}, 3f);
			AnimatorStateInfo currentAnimatorStateInfo = _animationController.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName("IdleStandingMotion1") || currentAnimatorStateInfo.IsName("IdleStandingMotion2") || currentAnimatorStateInfo.IsName("IdleStandingMotion3"))
			{
				float length = currentAnimatorStateInfo.length;
				yield return new WaitForSeconds(length);
				if (!WeaponHandler.HasActiveWeapon && hasMelee && meLeeName == _loadoutController.MeleeWeapon.name)
				{
					WeaponHandler.DrawMeleeWeapon();
					yield return new WaitForSeconds(1f);
				}
			}
		}
	}
}
