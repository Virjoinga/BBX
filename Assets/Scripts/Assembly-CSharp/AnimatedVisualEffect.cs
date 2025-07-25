using UnityEngine;

public class AnimatedVisualEffect : BaseVisualEffect
{
	[SerializeField]
	private PlayerAnimationController.Parameter _animationProperty;

	private PlayerAnimationController _animationController;

	public override void Setup(Outfit outfit)
	{
		base.Setup(outfit);
		_animationController = GetComponentInParent<PlayerAnimationController>();
	}

	protected override void Show()
	{
		base.Show();
		if (_animationController != null)
		{
			_animationController.SetBool(_animationProperty, value: true);
		}
	}

	protected override void Hide()
	{
		base.Hide();
		if (_animationController != null)
		{
			_animationController.SetBool(_animationProperty, value: false);
		}
	}
}
