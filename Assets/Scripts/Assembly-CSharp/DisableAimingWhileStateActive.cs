using UnityEngine;

public class DisableAimingWhileStateActive : PreExitStateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		if (animator != null)
		{
			WeaponAnimationController component = animator.GetComponent<WeaponAnimationController>();
			if (component != null)
			{
				component.DisableAiming();
			}
		}
	}
}
