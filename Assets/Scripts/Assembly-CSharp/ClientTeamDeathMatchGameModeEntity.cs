using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using BSCore.Constants.Config;
using Bolt;
using UnityEngine;
using Zenject;

public class ClientTeamDeathMatchGameModeEntity : EntityEventListener<ITeamDeathMatchGameModeState>
{
	private const float FORCE_LEAVE_TIME = 30f;

	[Inject]
	private SignalBus _signalBus;

	[Inject]
	protected ConfigManager _configManager;

	[Inject]
	private ZenjectInstantiater _zenjectInstantiater;

	[SerializeField]
	private GameModeEntityHelper _helper;

	private TeamDeathMatchConfigData _config;

	public static string SERVERID { get; private set; }

	public static ClientTeamDeathMatchGameModeEntity TDMGameMode { get; private set; }

	public static bool HasTDMGameMode => TDMGameMode != null;

	public string Map { get; private set; }

	public float MinRespawnTime { get; private set; }

	public float MatchStartTime => base.state.MatchStartTime;

	public float MatchEndTime { get; private set; }

	public int ExpectedPlayerCount => base.state.ExpectedPlayerCount;

	private IEnumerator Start()
	{
		yield return new WaitUntil(() => PlayerController.HasLocalPlayer && PlayerController.LocalPlayer.HasTeamSet && MatchMap.Loaded);
		UIPrefabManager.Destroy(UIPrefabIds.TDMMatchLoadingScreen, 2f);
	}

	public override void Attached()
	{
		if (!base.entity.isOwner)
		{
			InstantiateMatchUI();
			_config = _configManager.Get<TeamDeathMatchConfigData>(DataKeys.TeamDeathMatch);
			MinRespawnTime = _config.MinRespawnTime;
			base.state.AddCallback("Map", OnMapUpdated);
			base.state.AddCallback("ServerId", OnServerIdUpdated);
			OnServerIdUpdated();
			base.state.AddCallback("MatchState", OnMatchStateUpdated);
			base.state.AddCallback("MatchStartTime", OnMatchStartTimeUpdated);
			base.state.AddCallback("Players[]", OnPlayersUpdated);
			base.state.AddCallback("ExpectedPlayerCount", OnExpectedPlayerCountUpdated);
			base.state.AddCallback("PlayersLoaded", OnPlayersLoadedUpdated);
			StartCoroutine(TryApplyMapVariationRoutine());
			OnPlayersUpdated();
			TDMGameMode = this;
			Debug.Log("[ClientTeamDeathMatchGameModeEntity] Attached state.MatchState = " + base.state.MatchState);
			if (base.state.MatchState == 1)
			{
				_signalBus.Fire(new MatchStateUpdatedSignal(MatchState.Active));
			}
		}
	}

	public override void Detached()
	{
		TDMGameMode = null;
	}

	private void InstantiateMatchUI()
	{
		UIPrefabManager.Instantiate(UIPrefabIds.TDMMatchHud);
		UIPrefabManager.Instantiate(UIPrefabIds.DamageNumbers, interactive: false);
	}

	private IEnumerator TryApplyMapVariationRoutine()
	{
		yield return new WaitUntil(() => MatchMap.Loaded);
		base.state.AddCallback("MapVariation", TryApplyMapVariation);
		TryApplyMapVariation();
	}

	private void TryApplyMapVariation()
	{
		if (MonoBehaviourSingleton<MapVariationManager>.IsInstantiated)
		{
			MonoBehaviourSingleton<MapVariationManager>.Instance.ApplyMapVariation(base.state.MapVariation);
		}
	}

	private void OnMapUpdated()
	{
		Map = base.state.Map;
		_helper.LoadMap(base.state.Map);
	}

	private void OnServerIdUpdated()
	{
		SERVERID = base.state.ServerId;
	}

	private void OnMatchStartTimeUpdated()
	{
		float matchEndTime = (MatchEndTime = base.state.MatchStartTime + _config.TimeLimit);
		_signalBus.Fire(new MatchTimesUpdatedSignal
		{
			MatchStartTime = base.state.MatchStartTime,
			MatchEndTime = matchEndTime
		});
	}

	private void OnMatchStateUpdated()
	{
		MatchState matchState = (MatchState)base.state.MatchState;
		if (matchState != MatchState.Active && matchState == MatchState.Complete)
		{
			OnMatchComplete();
		}
		_signalBus.Fire(new MatchStateUpdatedSignal(matchState));
	}

	private void OnMatchComplete()
	{
		if (PlayerController.HasLocalPlayer)
		{
			UIPrefabManager.Instantiate(UIPrefabIds.TDMAARScreen, OnTDMAARScreenCreated, interactive: true, 0, useParent: false);
		}
		StartCoroutine(ForceLeaveRoutine());
	}

	private void OnTDMAARScreenCreated(GameObject uiGameobject)
	{
		if (!(uiGameobject == null))
		{
			uiGameobject.GetComponent<TDMResultsScreen>().Populate(base.state.Players.ToList());
			UIPrefabManager.Destroy(UIPrefabIds.TDMMatchHud);
			UIPrefabManager.Destroy(UIPrefabIds.DamageNumbers);
		}
	}

	private IEnumerator ForceLeaveRoutine()
	{
		yield return new WaitForSeconds(30f);
		UIPrefabManager.Instantiate(UIPrefabIds.LoadingOverlay, OnLoadingOverlayCreated, interactive: false, 11);
	}

	private void OnLoadingOverlayCreated(GameObject uiGameobject)
	{
		ConnectionManager.Shutdown();
	}

	private void OnPlayersUpdated()
	{
		List<TDMPlayerState> players = base.state.Players.Where((TDMPlayerState p) => !string.IsNullOrEmpty(p.EntityId)).ToList();
		_signalBus.Fire(new TDMPlayersUpdatedSignal
		{
			Players = players
		});
		TeamId allyTeam = TeamId.Team1;
		if (PlayerController.HasLocalPlayer)
		{
			allyTeam = (TeamId)PlayerController.LocalPlayer.state.Team;
		}
		TeamId opponentTeam = ((allyTeam != TeamId.Team1) ? TeamId.Team1 : TeamId.Team2);
		int allyTeamScore = base.state.Players.Where((TDMPlayerState p) => p.Team == (int)opponentTeam).Sum((TDMPlayerState p) => p.Deaths);
		int opponentTeamScore = base.state.Players.Where((TDMPlayerState p) => p.Team == (int)allyTeam).Sum((TDMPlayerState p) => p.Deaths);
		_signalBus.Fire(new TDMScoresUpdatedSignal
		{
			AllyTeamScore = allyTeamScore,
			OpponentTeamScore = opponentTeamScore
		});
	}

	private void OnExpectedPlayerCountUpdated()
	{
		_signalBus.Fire(new MatchExpectedPlayerCountUpdatedSignal
		{
			ExpectedPlayerCount = base.state.ExpectedPlayerCount
		});
	}

	private void OnPlayersLoadedUpdated()
	{
		_signalBus.Fire(new MatchPlayersLoadedUpdatedSignal
		{
			PlayersLoaded = base.state.PlayersLoaded
		});
	}
}
