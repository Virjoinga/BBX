using UnityEngine;

public class WeaponIsFiringReset : StateMachineBehaviour
{
	private float _startTime;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_startTime = Time.realtimeSinceStartup;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (Time.realtimeSinceStartup - _startTime >= stateInfo.length)
		{
			animator.SetBool("IsFiring", value: false);
		}
	}
}
