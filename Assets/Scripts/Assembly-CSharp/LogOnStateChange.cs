using UnityEngine;

public class LogOnStateChange : PreExitStateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		Debug.LogError("[LogOnStateChange] OnStateEnter: " + animator.GetNextAnimatorClipInfo(layerIndex)[0].clip.name);
	}

	protected override void OnStateStartExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateStartExit(animator, stateInfo, layerIndex);
		Debug.LogError("[LogOnStateChange] OnStateStartExit: " + animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.name);
	}
}
