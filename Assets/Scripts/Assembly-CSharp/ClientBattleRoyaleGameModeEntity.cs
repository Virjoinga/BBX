using System.Collections;
using UnityEngine;

public class ClientBattleRoyaleGameModeEntity : ClientBaseGameModeEntity<IBattleRoyaleGameModeState>
{
	private const float FORCE_LEAVE_TIME = 30f;

	[SerializeField]
	private GameObject _stopDropCollider;

	public override void Attached()
	{
		if (!base.entity.isOwner)
		{
			InstantiateMatchUI();
			base.state.AddCallback("Map", OnMapUpdated);
			base.state.AddCallback("MatchState", OnMatchStateUpdated);
			base.state.AddCallback("RemainingPlayers", OnRemainingPlayersUpdated);
			base.state.AddCallback("RemainingFirstLifePlayers", OnRemainingPlayersUpdated);
		}
	}

	private void InstantiateMatchUI()
	{
		UIPrefabManager.Instantiate(UIPrefabIds.BRMatchHud);
	}

	private void OnMapUpdated()
	{
		_helper.LoadMap(base.state.Map);
	}

	private void OnMatchStateUpdated()
	{
		MatchState matchState = (MatchState)base.state.MatchState;
		switch (matchState)
		{
		case MatchState.Active:
			StartCoroutine(OnMatchStarted());
			break;
		case MatchState.Complete:
			OnMatchComplete();
			break;
		}
		ClientBaseGameModeEntity<IBattleRoyaleGameModeState>.MatchState = matchState;
		_signalBus.Fire(new MatchStateUpdatedSignal(matchState));
	}

	private IEnumerator OnMatchStarted()
	{
		yield return new WaitForSeconds(2f);
		_stopDropCollider.SetActive(value: false);
	}

	private void OnMatchComplete()
	{
		if (PlayerController.HasLocalPlayer && PlayerController.LocalPlayer.IsAlive)
		{
			UIPrefabManager.Instantiate(UIPrefabIds.BRVictoryScreen, OnVictoryScreenCreated);
		}
		StartCoroutine(ForceLeaveRoutine());
	}

	private void OnVictoryScreenCreated(GameObject uiGameobject)
	{
		UIPrefabManager.Destroy(UIPrefabIds.BRMatchHud);
	}

	private IEnumerator ForceLeaveRoutine()
	{
		yield return new WaitForSeconds(30f);
		UIPrefabManager.Instantiate(UIPrefabIds.LoadingOverlay, OnLoadingOverlayCreated, interactive: false, 11);
	}

	private void OnLoadingOverlayCreated(GameObject uiGameobject)
	{
		if (PlayerController.HasLocalPlayer && PlayerController.LocalPlayer.IsAlive)
		{
			UIPrefabManager.Destroy(UIPrefabIds.BRVictoryScreen);
		}
		else
		{
			UIPrefabManager.Destroy(UIPrefabIds.BRDeathScreen);
		}
		ConnectionManager.Shutdown();
	}

	private void OnRemainingPlayersUpdated()
	{
		ClientBaseGameModeEntity<IBattleRoyaleGameModeState>.RemainingPlayers = base.state.RemainingPlayers;
		ClientBaseGameModeEntity<IBattleRoyaleGameModeState>.RemainingFirstLifePlayers = base.state.RemainingFirstLifePlayers;
		_signalBus.Fire(default(BattleRoyaleRemainingPlayersUpdatedSignal));
	}
}
