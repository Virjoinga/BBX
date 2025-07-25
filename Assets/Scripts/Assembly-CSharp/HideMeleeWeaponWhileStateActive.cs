using UnityEngine;

public class HideMeleeWeaponWhileStateActive : PlayerStateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
	}

	protected override void OnStateStartExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
