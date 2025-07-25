using System.Collections;
using Bolt;
using UnityEngine;

public abstract class ClientBaseGameModeCallbacks : GlobalEventListener
{
	protected abstract bool _matchIsActive { get; }

	protected virtual IEnumerator Start()
	{
		yield return new WaitUntil(() => PlayerController.HasLocalPlayer && MatchMap.Loaded && _matchIsActive);
		UIPrefabManager.Destroy(UIPrefabIds.LoadingOverlay);
		UIPrefabManager.Destroy(UIPrefabIds.TDMMatchLoadingScreen, 2f);
	}
}
