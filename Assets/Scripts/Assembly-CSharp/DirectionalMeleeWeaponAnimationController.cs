public class DirectionalMeleeWeaponAnimationController : BaseMeleeWeaponAnimationController
{
	private enum SpeedParams
	{
		ForwardSpeed = 0,
		BackwardSpeed = 1,
		LeftSpeed = 2,
		RightSpeed = 3,
		StandingSpeed = 4
	}

	public override void Setup()
	{
		_animator.SetFloat(SpeedParams.ForwardSpeed.ToString(), _meleeWeapon.Profile.Melee.Forward.AnimationSpeedMultiplier);
		_animator.SetFloat(SpeedParams.BackwardSpeed.ToString(), _meleeWeapon.Profile.Melee.Backward.AnimationSpeedMultiplier);
		_animator.SetFloat(SpeedParams.LeftSpeed.ToString(), _meleeWeapon.Profile.Melee.Left.AnimationSpeedMultiplier);
		_animator.SetFloat(SpeedParams.RightSpeed.ToString(), _meleeWeapon.Profile.Melee.Right.AnimationSpeedMultiplier);
		_animator.SetFloat(SpeedParams.StandingSpeed.ToString(), _meleeWeapon.Profile.Melee.Backward.AnimationSpeedMultiplier);
	}

	protected override void TriggerMeleeAnim(WeaponProfile.MeleeOptionData meleeOption)
	{
		_animator.SetTrigger(meleeOption.AnimParam.ToString());
	}
}
