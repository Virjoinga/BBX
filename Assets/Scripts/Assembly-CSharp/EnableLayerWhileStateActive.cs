using UnityEngine;

public class EnableLayerWhileStateActive : PlayerStateMachineBehaviour
{
	[SerializeField]
	private int _layerToDisable;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		GetPlayer(animator).EnableLayer(_layerToDisable);
	}

	protected override void OnStateStartExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!PreExitStateMachineBehaviour.NextStateHas<EnableAimingWhileStateActive>(animator, layerIndex))
		{
			GetPlayer(animator).DisableLayer(_layerToDisable);
		}
	}
}
