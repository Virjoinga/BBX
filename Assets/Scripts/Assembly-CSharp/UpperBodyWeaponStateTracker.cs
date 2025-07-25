using UnityEngine;

public class UpperBodyWeaponStateTracker : PlayerStateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		GetPlayer(animator).RaiseUpperBodyAnimationStateUpdated(isActive: true);
	}

	protected override void OnStateStartExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!PreExitStateMachineBehaviour.NextStateHas<UpperBodyWeaponStateTracker>(animator, layerIndex))
		{
			GetPlayer(animator).RaiseUpperBodyAnimationStateUpdated(isActive: false);
		}
	}
}
