using System.Collections;
using UnityEngine;

public class TriggerAfterSeconds : PreExitStateMachineBehaviour
{
	[SerializeField]
	private PlayerAnimationController.Parameter _trigger;

	[SerializeField]
	private float _length = 4f;

	private Coroutine _timeoutCoroutine;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		_timeoutCoroutine = animator.GetComponent<PlayerAnimationController>().StartCoroutine(TriggerAfterSecondsRoutine(animator));
	}

	protected override void OnStateStartExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (_timeoutCoroutine != null)
		{
			animator.GetComponent<PlayerAnimationController>().StopCoroutine(_timeoutCoroutine);
			_timeoutCoroutine = null;
		}
	}

	private IEnumerator TriggerAfterSecondsRoutine(Animator animator)
	{
		yield return new WaitForSeconds(_length);
		animator.SetTrigger(_trigger.ToString());
		_timeoutCoroutine = null;
	}
}
