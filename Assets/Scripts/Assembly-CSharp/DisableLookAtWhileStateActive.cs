using UnityEngine;

public class DisableLookAtWhileStateActive : PlayerStateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		GetPlayer(animator).DisableLookAt();
	}

	protected override void OnStateStartExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!PreExitStateMachineBehaviour.NextStateHas<DisableLookAtWhileStateActive>(animator, layerIndex))
		{
			GetPlayer(animator).EnableLookAt();
		}
	}
}
