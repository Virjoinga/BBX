using System.Linq;
using UnityEngine;

public abstract class PreExitStateMachineBehaviour : StateMachineBehaviour
{
	private bool _firstFrameHappened;

	private bool _lastFrameHappened;

	protected static bool NextStateHas<T>(Animator animator, int layerIndex) where T : PreExitStateMachineBehaviour
	{
		if (animator != null)
		{
			return animator.GetBehaviours(animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash, layerIndex).Any((StateMachineBehaviour smb) => smb is T);
		}
		return false;
	}

	protected static bool TryGetBehaviour<T>(Animator animator, int layerIndex, out T behaviour) where T : PreExitStateMachineBehaviour
	{
		behaviour = null;
		if (animator == null)
		{
			return false;
		}
		behaviour = animator.GetBehaviours(animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash, layerIndex).FirstOrDefault((StateMachineBehaviour smb) => smb is T) as T;
		return behaviour != null;
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_firstFrameHappened = false;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!animator.IsInTransition(layerIndex) && !_firstFrameHappened)
		{
			_firstFrameHappened = true;
		}
		if (animator.IsInTransition(layerIndex) && !_lastFrameHappened && _firstFrameHappened)
		{
			_lastFrameHappened = true;
			OnStateStartExit(animator, stateInfo, layerIndex);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_lastFrameHappened = false;
	}

	protected virtual void OnStateStartExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
