using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class MenuHallwayAnimatorController : MonoBehaviourSingleton<MenuHallwayAnimatorController>
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private MenuCamera _menuCamera;

	private Action _animationCompleted;

	private Outfit _outfit;

	protected override void Awake()
	{
		base.Awake();
		_animator.enabled = false;
	}

	public void StartRunJumpAnimation(Action animationCompleted)
	{
		_animationCompleted = animationCompleted;
		_animator.enabled = true;
		_animator.SetTrigger("StartRunJump");
		_menuCamera.StartCharacterFollow();
		_signalBus.Fire<SlideOutMenuUISignal>();
	}

	private void StartOutfitRunAnimation()
	{
	}

	private IEnumerator TransitionToRunAnimation()
	{
		for (float t = 0f; t <= 1f; t += Time.deltaTime * 2f)
		{
			_animator.SetFloat(PlayerAnimationController.Parameter.Speed.ToString(), t);
			yield return null;
		}
		_animator.SetFloat(PlayerAnimationController.Parameter.Speed.ToString(), 1f);
	}

	private void StartOutfitJump()
	{
	}

	private IEnumerator TransitionJumpAnimation()
	{
		_animator.SetFloat(PlayerAnimationController.Parameter.Speed.ToString(), 0f);
		_animator.SetFloat(PlayerAnimationController.Parameter.VerticalSpeed.ToString(), 1f);
		yield return new WaitForSeconds(1f);
		_animator.SetFloat(PlayerAnimationController.Parameter.VerticalSpeed.ToString(), 1f);
	}

	private void AnimationCompleted()
	{
		_animationCompleted?.Invoke();
	}
}
