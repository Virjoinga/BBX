using UnityEngine;

public abstract class PlayerStateMachineBehaviour : PreExitStateMachineBehaviour
{
	protected PlayerAnimationController GetPlayer(Animator animator)
	{
		return animator.GetComponentInParent<PlayerAnimationController>();
	}
}
