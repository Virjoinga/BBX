using UnityEngine;

public class DisableLayerWhileStateActive : PlayerStateMachineBehaviour
{
	[SerializeField]
	private int[] _layersToDisable = new int[1] { 1 };

	[SerializeField]
	private float _disableDuration = 0.125f;

	public int[] LayersToDisable => _layersToDisable;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		GetPlayer(animator).DisableLayers(_layersToDisable, _disableDuration);
	}

	protected override void OnStateStartExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!PreExitStateMachineBehaviour.NextStateHas<DisableLayerWhileStateActive>(animator, layerIndex))
		{
			GetPlayer(animator).EnableLayers(_layersToDisable);
		}
	}
}
