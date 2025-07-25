using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PerformanceFeed : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private EliminationPlate _eliminationPlatePrefab;

	[SerializeField]
	private RectTransform _layoutRoot;

	private void Start()
	{
		_signalBus.Subscribe<PlayerEliminatedSignal>(OnPlayerEliminated);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<PlayerEliminatedSignal>(OnPlayerEliminated);
	}

	private void OnPlayerEliminated(PlayerEliminatedSignal signal)
	{
		if (signal.LocalPlayerPerformedFinalBlow || signal.LocalPlayerAssisted)
		{
			EliminationPlate eliminationPlate = SmartPool.Spawn(_eliminationPlatePrefab);
			if (eliminationPlate != null)
			{
				eliminationPlate.Populate(signal.VictimName, signal.LocalPlayerPerformedFinalBlow);
				StartCoroutine(RebuildAfterFrame());
			}
			else
			{
				Debug.LogError("[PerformanceFeed] Failed to spawn elimination plate from smart pool");
			}
		}
	}

	private IEnumerator RebuildAfterFrame()
	{
		yield return new WaitForEndOfFrame();
		LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutRoot);
	}
}
