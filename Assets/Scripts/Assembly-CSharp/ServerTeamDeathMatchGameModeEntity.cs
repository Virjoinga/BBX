using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BSCore;
using BSCore.Constants.Config;
using Bolt;
using MatchMaking;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(GameModeEntityHelper))]
public class ServerTeamDeathMatchGameModeEntity : ServerBaseGameModeEntity<ITeamDeathMatchGameModeState>
{
	public class TDMPlayerData
	{
		public string EntityId;

		public string DisplayName;

		public TeamId Team;

		public PlatformType Platform;

		public bool HasLoaded;

		public bool HasTeam;

		public bool SetupCompleted;

		public string ActiveHeroClass;

		public BoltEntity PlayerEntity;

		public NetworkId NetworkId;

		public bool IsRespawning;

		public float CanRespawnTime;

		public string SelectedRespawnPointId;

		public int CurrentWeaponIndex;

		public string RespawnUpdatedHeroClass;

		public DelayedAction.DelayedActionHandle ForcedRespawnAction;

		public int KillStreakEXPBonus;

		public int KillStreakCurrencyBonus;

		public PlayerProfile PlayerProfile { get; private set; }

		public bool IsReady
		{
			get
			{
				if (HasLoaded && HasTeam && SetupCompleted)
				{
					return PlayerEntity != null;
				}
				return false;
			}
		}

		public BoltConnection PlayerConnection { get; private set; }

		public TDMPlayerData(string entityId)
		{
			EntityId = entityId;
		}

		public void SetTeam(TeamId team)
		{
			Team = team;
			HasTeam = true;
		}

		public void SetBoltEntity(BoltEntity playerEntity)
		{
			PlayerEntity = playerEntity;
			NetworkId = PlayerEntity.networkId;
			PlayerConnection = PlayerEntity.controller;
		}

		public void SetPlayerProfile(PlayerProfile playerProfile)
		{
			PlayerProfile = playerProfile;
			ActiveHeroClass = PlayerProfile.LoadoutManager.EquippedHeroClass.ToString();
		}

		public void SetRespawnData(float canRespawnTime, string selectedRespawnPointId, int currentWeaponIndex, DelayedAction.DelayedActionHandle forcedRespawnAction)
		{
			CanRespawnTime = canRespawnTime;
			SelectedRespawnPointId = selectedRespawnPointId;
			CurrentWeaponIndex = currentWeaponIndex;
			ForcedRespawnAction = forcedRespawnAction;
			IsRespawning = true;
		}

		public void ClearRespawnData()
		{
			CanRespawnTime = 0f;
			SelectedRespawnPointId = string.Empty;
			CurrentWeaponIndex = 0;
			ForcedRespawnAction = default(DelayedAction.DelayedActionHandle);
			IsRespawning = false;
			RespawnUpdatedHeroClass = string.Empty;
		}
	}

	private const float MATCH_START_DELAY = 3f;

	private const int SERVER_FRAME_BUFFER = 10;

	[Inject]
	private StatisticsManager _statisticsManager;

	[Inject]
	private InventoryManager _inventoryManager;

	[Inject]
	private ProfileManager _profileManager;

	[SerializeField]
	private DebugTDMPlayersController _debugPlayerController;

	[Inject]
	private TeamDeathMatchHelper _teamDeathMatchHelper;

	private TeamDeathMatchConfigData _config;

	private Dictionary<TeamId, int> _teamsToScore = new Dictionary<TeamId, int>();

	private Dictionary<TeamId, int> _teamsToPlayerCount = new Dictionary<TeamId, int>();

	private Dictionary<string, TDMPlayerData> _playersData = new Dictionary<string, TDMPlayerData>();

	private int _playerIndex;

	public MatchState MatchState => (MatchState)base.state.MatchState;

	private List<TDMPlayerData> _readyPlayers => _playersData.Values.Where((TDMPlayerData p) => p.IsReady).ToList();

	private void Awake()
	{
		if (PlayfabServerManagement.IsInitializedAndReady)
		{
			_teamDeathMatchHelper.GetMatchPlayers(PlayfabServerManagement.SessionId, "teamDeathMatch", OnMatchPlayersFetched);
		}
		else
		{
			_debugPlayerController.TeamSelected += DebugGivePlayerTeam;
		}
	}

	private void DebugGivePlayerTeam(string entityId, TeamId team)
	{
		if (_playersData.ContainsKey(entityId))
		{
			TDMPlayerData tDMPlayerData = _playersData[entityId];
			AddPlayerToTeam(tDMPlayerData, team);
			if (!tDMPlayerData.SetupCompleted && tDMPlayerData.HasLoaded)
			{
				SetupPlayer(tDMPlayerData);
			}
		}
		else
		{
			Debug.LogError("[ServerTeamDeathMatchGameModeEntity] Unable to set team for player " + entityId);
		}
	}

	private void OnMatchPlayersFetched(List<MatchedMember> matchPlayers)
	{
		foreach (MatchedMember matchPlayer in matchPlayers)
		{
			Debug.Log("[ServerTeamDeathMatchGameModeEntity] Setup Match Player " + JsonUtility.ToJson(matchPlayer));
			TeamId result = TeamId.Team1;
			Enum.TryParse<TeamId>(matchPlayer.teamId, out result);
			if (!_playersData.ContainsKey(matchPlayer.playerAttributes.entityId))
			{
				_playersData.Add(matchPlayer.playerAttributes.entityId, new TDMPlayerData(matchPlayer.playerAttributes.entityId));
			}
			if (!_playersData[matchPlayer.playerAttributes.entityId].HasTeam)
			{
				AddPlayerToTeam(_playersData[matchPlayer.playerAttributes.entityId], result);
			}
			Debug.Log($"[ServerTeamDeathMatchGameModeEntity] Adding Player {matchPlayer.playerAttributes.entityId} with team {result}");
		}
	}

	public override void Attached()
	{
		base.Attached();
		if (base.entity.isOwner)
		{
			_config = _configManager.Get<TeamDeathMatchConfigData>(DataKeys.TeamDeathMatch);
			base.state.AddCallback("Map", OnMapUpdated);
			if (PlayfabServerManagement.IsInitializedAndReady && !string.IsNullOrEmpty(PlayfabServerManagement.ServerId))
			{
				base.state.ServerId = PlayfabServerManagement.ServerId;
			}
			StartCoroutine(TrySetMapVariationRoutine());
			_signalBus.Subscribe<PlayerRequestedRespawnSignal>(OnPlayerRequestedRespawn);
			_signalBus.Subscribe<UpdateRespawnPointSignal>(OnUpdateRespawnPointRequested);
		}
	}

	public override void Detached()
	{
		base.Detached();
		if (base.entity.isOwner)
		{
			_signalBus.Unsubscribe<PlayerRequestedRespawnSignal>(OnPlayerRequestedRespawn);
			_signalBus.Unsubscribe<UpdateRespawnPointSignal>(OnUpdateRespawnPointRequested);
		}
	}

	private IEnumerator TrySetMapVariationRoutine()
	{
		yield return new WaitUntil(() => MatchMap.Loaded);
		if (MonoBehaviourSingleton<MapVariationManager>.IsInstantiated)
		{
			base.state.MapVariation = MonoBehaviourSingleton<MapVariationManager>.Instance.GetRandomMapVariation();
			Debug.Log($"ServerTeamDeathMatchGameModeEntity] Setting Map Varation {base.state.MapVariation}");
		}
	}

	private void OnMapUpdated()
	{
		_helper.LoadMap(base.state.Map);
	}

	public void PlayerConnected(BoltEntity playerEntity, PlayerProfile playerProfile, PlatformType platform)
	{
		Debug.Log($"[ServerTeamDeathMatchGameModeEntity] Player Connected: {playerProfile.Entity.Id}|{playerProfile.DisplayName} - {Time.realtimeSinceStartup}");
		if (!_playersData.ContainsKey(playerProfile.Entity.Id))
		{
			_playersData.Add(playerProfile.Entity.Id, new TDMPlayerData(playerProfile.Entity.Id));
		}
		_playersData[playerProfile.Entity.Id].SetBoltEntity(playerEntity);
		_playersData[playerProfile.Entity.Id].DisplayName = playerProfile.DisplayName;
		_playersData[playerProfile.Entity.Id].Platform = platform;
		_playersData[playerProfile.Entity.Id].SetPlayerProfile(playerProfile);
		if (!PlayfabServerManagement.IsInitializedAndReady)
		{
			_debugPlayerController.AddPlayer(playerProfile);
			_expectedPlayerCount++;
			base.state.ExpectedPlayerCount = _expectedPlayerCount;
		}
		if (base.state.MatchState == 0)
		{
			Debug.Log($"[ServerTeamDeathMatchGameModeEntity] TryStartPlayerLoading  {playerProfile.Entity.Id}|{playerProfile.DisplayName} - {Time.realtimeSinceStartup}");
			TryStartPlayerLoading();
		}
		if (base.state.MatchState == 1 || (_playerLoadingStarted && base.state.MatchState == 0))
		{
			Debug.Log($"[ServerTeamDeathMatchGameModeEntity] OnPlayerLateConnect  {playerProfile.Entity.Id}|{playerProfile.DisplayName} - {Time.realtimeSinceStartup}");
			OnPlayerLateConnect(playerProfile.Entity.Id);
		}
	}

	private void OnPlayerLateConnect(string entityId)
	{
		if (!PlayfabServerManagement.IsInitializedAndReady)
		{
			return;
		}
		_teamDeathMatchHelper.GetMatchPlayers(PlayfabServerManagement.SessionId, "teamDeathMatch", delegate(List<MatchedMember> matchPlayers)
		{
			MatchedMember matchedMember = matchPlayers.Where((MatchedMember p) => p.playerAttributes.entityId == entityId).FirstOrDefault();
			if (matchedMember != null)
			{
				TeamId result = TeamId.Team1;
				Enum.TryParse<TeamId>(matchedMember.teamId, out result);
				TDMPlayerData tDMPlayerData = _playersData[matchedMember.playerAttributes.entityId];
				if (!tDMPlayerData.HasTeam)
				{
					AddPlayerToTeam(tDMPlayerData, result);
					_expectedPlayerCount++;
					base.state.ExpectedPlayerCount = _expectedPlayerCount;
					Debug.Log($"[ServerTeamDeathMatchGameModeEntity] Adding Player {matchedMember.playerAttributes.entityId} with team {result}");
				}
				if (!tDMPlayerData.SetupCompleted && tDMPlayerData.HasLoaded)
				{
					SetupPlayer(tDMPlayerData);
				}
			}
		});
	}

	public void PlayerDisconnected(BoltEntity playerEntity, PlayerProfile playerProfile)
	{
		Debug.Log("[ServerTeamDeathMatchGameModeEntity] Player Disconnected: " + playerProfile.Entity.Id);
		if (_playersData.ContainsKey(playerProfile.Entity.Id))
		{
			RemovePlayerFromTeam(_playersData[playerProfile.Entity.Id]);
			_playersData.Remove(playerProfile.Entity.Id);
		}
		if (!PlayfabServerManagement.IsInitializedAndReady)
		{
			_debugPlayerController.RemovePlayer(playerProfile.Entity.Id);
		}
		for (int i = 0; i < base.state.Players.Length; i++)
		{
			if (base.state.Players[i].NetworkId == playerEntity.networkId)
			{
				base.state.Players[i].Disconnected = true;
			}
		}
	}

	private void TryStartPlayerLoading()
	{
		if (!_playerLoadingStarted)
		{
			_expectedPlayerCount = 1;
			if (PlayfabServerManagement.IsInitializedAndReady)
			{
				_expectedPlayerCount = PlayfabServerManagement.ExpectedPlayerCount;
			}
			base.state.ExpectedPlayerCount = _expectedPlayerCount;
			StartCoroutine(TeamDeathMatchStateCycle());
			_playerLoadingStarted = true;
		}
	}

	protected override void OnPlayerLoaded(PlayerLoadedSignal playerLoadedSignal)
	{
		TDMPlayerData tDMPlayerData = _playersData.Values.Where((TDMPlayerData x) => x.NetworkId == playerLoadedSignal.PlayerEntity.networkId).First();
		if (tDMPlayerData != null)
		{
			if (!tDMPlayerData.HasLoaded)
			{
				Debug.Log($"[ServerTeamDeathMatchGameModeEntity] Player Loaded: {tDMPlayerData.EntityId}|{tDMPlayerData.DisplayName} - {Time.realtimeSinceStartup}");
				tDMPlayerData.HasLoaded = true;
				if (!tDMPlayerData.SetupCompleted && tDMPlayerData.HasTeam)
				{
					SetupPlayer(tDMPlayerData);
					return;
				}
				Debug.LogError("[ServerTeamDeathMatchGameModeEntity] Player (" + tDMPlayerData.EntityId + "|" + tDMPlayerData.DisplayName + ") is loaded but does not have team set. **Ignore if not Playfab Build**");
			}
		}
		else
		{
			Debug.LogError($"[ServerTeamDeathMatchGameModeEntity] Unable to find Entry for player with network id: {playerLoadedSignal.PlayerEntity.networkId}");
		}
	}

	private void SetupPlayer(TDMPlayerData playerData)
	{
		Debug.Log("Setup for player " + playerData.DisplayName + " ; id : " + playerData.EntityId);
		int num = _playerIndex;
		int num2 = base.state.Players.ToList().FindIndex((TDMPlayerState p) => p.EntityId == playerData.EntityId);
		if (num2 > 0)
		{
			num = num2;
		}
		base.state.Players[num].EntityId = playerData.EntityId;
		base.state.Players[num].NetworkId = playerData.NetworkId;
		base.state.Players[num].Team = (int)playerData.Team;
		base.state.Players[num].DisplayName = playerData.DisplayName;
		base.state.Players[num].Platform = (int)playerData.Platform;
		base.state.Players[num].BoltEntity = playerData.PlayerEntity;
		base.state.Players[num].Disconnected = false;
		try
		{
			if (playerData.PlayerEntity.TryFindState<IPlayerState>(out var playerState))
			{
				Debug.Log($"[ServerTeamDeathMatchGameModeEntity] Setting Player {playerData.EntityId}|{playerState.DisplayName} to Team {(int)playerData.Team}");
				PlayerSpawnPoint randomPlayerSpawnPointForTeam = MonoBehaviourSingleton<SpawnManager>.Instance.GetRandomPlayerSpawnPointForTeam(playerData.Team, forceTeamSpawn: true);
				playerData.PlayerEntity.transform.position = randomPlayerSpawnPointForTeam.Position;
				playerData.PlayerEntity.transform.rotation = randomPlayerSpawnPointForTeam.Rotation;
				playerState.Team = (int)playerData.Team;
				base.state.Players[num].Loadout = playerState.Loadouts[0].Serialize();
				SetPlayerInputState(playerState, inputEnabled: true);
				playerData.SetupCompleted = true;
			}
			else
			{
				Debug.LogError("[ServerTeamDeathMatchGameModeEntity] Failed to get Player State for player " + playerData.EntityId + "|" + playerState.DisplayName + ". Team is not set on Player State!");
			}
		}
		catch (Exception ex)
		{
			Debug.Log("[ServerTeamDeathMatchGameModeEntity] Exception Setting Up Player " + playerData.EntityId + " / " + playerData.DisplayName + ". Error - " + ex.ToString());
		}
		if (num == _playerIndex)
		{
			_playerIndex++;
		}
		if (base.state.MatchState == 1)
		{
			StartCoroutine(ResetPlayersForMatchStarted(playerData.EntityId));
		}
	}

	private IEnumerator TeamDeathMatchStateCycle()
	{
		Debug.Log($"[ServerTeamDeathMatchGameModeEntity] Starting Team Death cycle. Waiting for players. - {Time.realtimeSinceStartup}");
		base.state.MatchStartTime = BoltNetwork.ServerTime + _config.LoadingTimeout;
		yield return new WaitUntil(ShouldStartCheck);
		Debug.Log($"[ServerTeamDeathMatchGameModeEntity] Resetting Players For Match Start - {Time.realtimeSinceStartup}");
		ResetPlayersForMatchStart();
		yield return new WaitUntil(() => BoltNetwork.ServerTime > base.state.MatchStartTime);
		Debug.Log($"[ServerTeamDeathMatchGameModeEntity] Starting Match - {Time.realtimeSinceStartup}");
		StartMatch();
		yield return new WaitUntil(ShouldEndCheck);
		Debug.Log($"[ServerTeamDeathMatchGameModeEntity] Ending Match - {Time.realtimeSinceStartup}");
		_teamDeathMatchHelper.EndMatch(PlayfabServerManagement.SessionId);
		yield return EndMatch();
	}

	private bool ShouldStartCheck()
	{
		if (_playersData.Count < _expectedPlayerCount || !_playersData.Values.All((TDMPlayerData p) => p.IsReady))
		{
			return BoltNetwork.ServerTime > base.state.MatchStartTime;
		}
		return true;
	}

	private IEnumerator ResetPlayersForMatchStarted(string entityId)
	{
		Debug.LogError("[ServerTeamDeathMatchGameModeEntity] ResetPlayersForMatchStarted for " + entityId);
		yield return new WaitUntil(() => _playersData.ContainsKey(entityId) && _playersData[entityId].IsReady);
		TDMPlayerData tDMPlayerData = _playersData[entityId];
		Debug.LogError("[ServerTeamDeathMatchGameModeEntity] ResetPlayersForMatchStarted for " + tDMPlayerData.EntityId + "|" + tDMPlayerData.DisplayName + " start");
		if (tDMPlayerData.PlayerEntity.TryFindState<IPlayerState>(out var playerState))
		{
			PlayerController component = tDMPlayerData.PlayerEntity.GetComponent<PlayerController>();
			if (component != null)
			{
				component.TryCancelEmote();
			}
			else
			{
				Debug.LogError("[ServerTeamDeathMatchGameModeEntity] Failed to get PlayerController for player " + tDMPlayerData.EntityId + "|" + tDMPlayerData.DisplayName);
			}
			playerState.InputEnabled = true;
			playerState.Loadouts[0].ActiveWeapon = 0;
			playerState.Damageable.Health = playerState.Damageable.MaxHealth;
			int serverFrame = BoltNetwork.ServerFrame;
			for (int num = 0; num < playerState.Loadouts[0].Weapons.Length; num++)
			{
				playerState.Loadouts[0].Weapons[num].RemainingAmmo = playerState.Loadouts[0].Weapons[num].MaxAmmo;
				playerState.Loadouts[0].Weapons[num].NextFireFrame = serverFrame;
				playerState.Loadouts[0].Weapons[num].InCooldownUntil = serverFrame - 10;
			}
			playerState.NextMeleeTime = serverFrame;
			PlayerSpawnPoint randomPlayerSpawnPointForTeam = MonoBehaviourSingleton<SpawnManager>.Instance.GetRandomPlayerSpawnPointForTeam(tDMPlayerData.Team, forceTeamSpawn: true);
			tDMPlayerData.PlayerEntity.transform.position = randomPlayerSpawnPointForTeam.Position;
			tDMPlayerData.PlayerEntity.transform.rotation = randomPlayerSpawnPointForTeam.Rotation;
		}
		else
		{
			Debug.LogError("[ServerTeamDeathMatchGameModeEntity] ResetPlayersForMatchStarted Failed to get Player State for player " + tDMPlayerData.EntityId + "|" + tDMPlayerData.DisplayName + ". Unable to reset!");
		}
		yield return null;
		_signalBus.Fire(new MatchStateUpdatedSignal((MatchState)base.state.MatchState));
	}

	private void ResetPlayersForMatchStart()
	{
		MonoBehaviourSingleton<SpawnManager>.Instance.ResetSpawnPointCooldowns();
		foreach (TDMPlayerData readyPlayer in _readyPlayers)
		{
			if (readyPlayer.PlayerEntity.TryFindState<IPlayerState>(out var playerState))
			{
				PlayerController component = readyPlayer.PlayerEntity.GetComponent<PlayerController>();
				if (component != null)
				{
					component.TryCancelEmote();
				}
				else
				{
					Debug.LogError("[ServerTeamDeathMatchGameModeEntity] Failed to get PlayerController for player " + readyPlayer.EntityId + "|" + readyPlayer.DisplayName);
				}
				playerState.InputEnabled = false;
				playerState.Loadouts[0].ActiveWeapon = 0;
				playerState.Damageable.Health = playerState.Damageable.MaxHealth;
				int serverFrame = BoltNetwork.ServerFrame;
				for (int i = 0; i < playerState.Loadouts[0].Weapons.Length; i++)
				{
					playerState.Loadouts[0].Weapons[i].RemainingAmmo = playerState.Loadouts[0].Weapons[i].MaxAmmo;
					playerState.Loadouts[0].Weapons[i].NextFireFrame = serverFrame;
					playerState.Loadouts[0].Weapons[i].InCooldownUntil = serverFrame - 10;
				}
				playerState.NextMeleeTime = serverFrame;
				PlayerSpawnPoint randomPlayerSpawnPointForTeam = MonoBehaviourSingleton<SpawnManager>.Instance.GetRandomPlayerSpawnPointForTeam(readyPlayer.Team, forceTeamSpawn: true);
				readyPlayer.PlayerEntity.transform.position = randomPlayerSpawnPointForTeam.Position;
				readyPlayer.PlayerEntity.transform.rotation = randomPlayerSpawnPointForTeam.Rotation;
			}
			else
			{
				Debug.LogError("[ServerTeamDeathMatchGameModeEntity] Failed to get Player State for player " + readyPlayer.EntityId + "|" + readyPlayer.DisplayName + ". Unable to reset!");
			}
		}
		base.state.PlayersLoaded = true;
		base.state.MatchStartTime = BoltNetwork.ServerTime + 3f;
	}

	private void StartMatch()
	{
		base.state.MatchState = 1;
		_signalBus.Fire(new MatchStateUpdatedSignal((MatchState)base.state.MatchState));
		SetPlayersInputState(inputEnabled: true);
	}

	private bool ShouldEndCheck()
	{
		if (base.state.MatchState == 1)
		{
			if (!TimeLimitReached() && !ScoreLimitReached())
			{
				return NotEnoughPlayersLeft();
			}
			return true;
		}
		return false;
	}

	private bool TimeLimitReached()
	{
		return BoltNetwork.ServerTime > base.state.MatchStartTime + _config.TimeLimit;
	}

	private bool ScoreLimitReached()
	{
		foreach (int value in _teamsToScore.Values)
		{
			if (value >= _config.KillLimit)
			{
				return true;
			}
		}
		return false;
	}

	private bool NotEnoughPlayersLeft()
	{
		foreach (int value in _teamsToPlayerCount.Values)
		{
			if (value <= 0)
			{
				return true;
			}
		}
		return false;
	}

	protected override void UpdateMatchEndState()
	{
		base.state.MatchState = 2;
		_signalBus.Fire(new MatchStateUpdatedSignal((MatchState)base.state.MatchState));
		SetPlayersInputState(inputEnabled: false);
		AwardMatchEndRewards();
	}

	private void AwardMatchEndRewards()
	{
		List<TDMPlayerState> source = base.state.Players.Where((TDMPlayerState p) => !string.IsNullOrEmpty(p.EntityId)).ToList();
		List<TDMPlayerState> source2 = source.Where((TDMPlayerState p) => p.Team == 1).ToList();
		int num = source.Where((TDMPlayerState p) => p.Team == 2).ToList().Sum((TDMPlayerState p) => p.Deaths);
		int num2 = source2.Sum((TDMPlayerState p) => p.Deaths);
		bool flag = num == num2;
		TeamId teamId = ((num > num2) ? TeamId.Team1 : TeamId.Team2);
		foreach (TDMPlayerData player in _readyPlayers)
		{
			if (!BaseServerGameModeCallbacks.EntitiesByConnection.TryGetValue(player.PlayerConnection, out var value))
			{
				continue;
			}
			TDMPlayerState tDMPlayerState = source.Where((TDMPlayerState p) => p.EntityId == player.EntityId).FirstOrDefault();
			if (tDMPlayerState == null)
			{
				continue;
			}
			TryUpdateKillStreakRewards(player, tDMPlayerState.KillStreak);
			int num3 = _config.Rewards.BaseCurrency;
			int num4 = _config.Rewards.BaseEXP;
			int num5;
			if (tDMPlayerState.Team == (int)teamId)
			{
				num5 = ((!flag) ? 1 : 0);
				if (num5 != 0)
				{
					num4 += _config.Rewards.WinEXP;
					num3 += _config.Rewards.WinCurrency;
				}
			}
			else
			{
				num5 = 0;
			}
			num4 += tDMPlayerState.Kills * _config.Rewards.KillEXP;
			num4 += tDMPlayerState.Assists * _config.Rewards.AssistEXP;
			num4 += player.KillStreakEXPBonus;
			num3 += tDMPlayerState.Kills * _config.Rewards.KillCurrency;
			num3 += tDMPlayerState.Assists * _config.Rewards.AssistCurrency;
			num3 += player.KillStreakCurrencyBonus;
			Dictionary<StatisticKey, int> dictionary = new Dictionary<StatisticKey, int>
			{
				{
					StatisticKey.Experience,
					num4
				},
				{
					StatisticKey.Kills,
					tDMPlayerState.Kills
				},
				{
					StatisticKey.KillsWeekly,
					tDMPlayerState.Kills
				},
				{
					StatisticKey.KillsMonthly,
					tDMPlayerState.Kills
				},
				{
					StatisticKey.Assists,
					tDMPlayerState.Assists
				},
				{
					StatisticKey.AssistsWeekly,
					tDMPlayerState.Assists
				},
				{
					StatisticKey.AssistsMonthly,
					tDMPlayerState.Assists
				}
			};
			if (num5 != 0)
			{
				dictionary.Add(StatisticKey.Wins, 1);
				dictionary.Add(StatisticKey.WinsWeekly, 1);
				dictionary.Add(StatisticKey.WinsMonthly, 1);
			}
			_statisticsManager.UpdatePlayerStatistics(value.Profile, dictionary);
			_inventoryManager.GrantCurrency(value.Profile.Id, CurrencyType.S1, num3);
		}
	}

	private void SetPlayersInputState(bool inputEnabled)
	{
		foreach (TDMPlayerData readyPlayer in _readyPlayers)
		{
			if (readyPlayer.PlayerEntity.TryFindState<IPlayerState>(out var playerState))
			{
				SetPlayerInputState(playerState, inputEnabled);
			}
		}
	}

	private void AddPlayerToTeam(TDMPlayerData playerData, TeamId teamId)
	{
		playerData.SetTeam(teamId);
		if (!_teamsToPlayerCount.ContainsKey(teamId))
		{
			_teamsToPlayerCount.Add(teamId, 0);
		}
		_teamsToPlayerCount[teamId]++;
	}

	private void RemovePlayerFromTeam(TDMPlayerData playerData)
	{
		if (_teamsToPlayerCount.ContainsKey(playerData.Team))
		{
			_teamsToPlayerCount[playerData.Team]--;
		}
	}

	protected override void OnPlayerDied(PlayerDiedSignal playerDiedSignal)
	{
		TDMPlayerState tDMPlayerState = base.state.Players.FirstOrDefault((TDMPlayerState x) => x.NetworkId == playerDiedSignal.attacker.networkId);
		TDMPlayerState tDMPlayerState2 = base.state.Players.FirstOrDefault((TDMPlayerState x) => x.NetworkId == playerDiedSignal.victim.entity.networkId);
		TeamId teamId = TeamId.Neutral;
		if (tDMPlayerState != null)
		{
			if (tDMPlayerState.NetworkId != playerDiedSignal.victim.entity.networkId)
			{
				tDMPlayerState.Kills++;
				tDMPlayerState.KillStreak++;
			}
			teamId = (TeamId)tDMPlayerState.Team;
		}
		if (tDMPlayerState2 == null)
		{
			return;
		}
		tDMPlayerState2.Deaths++;
		int killStreak = tDMPlayerState2.KillStreak;
		if (tDMPlayerState2.KillStreak > 0)
		{
			tDMPlayerState2.KillStreak = 0;
		}
		tDMPlayerState2.BoltEntity.GetComponent<IDamageable>().HurtCollider.enabled = false;
		IPlayerState playerState = playerDiedSignal.victim.entity.GetState<IPlayerState>();
		playerState.InputEnabled = false;
		int activeWeapon = playerState.Loadouts[0].ActiveWeapon;
		playerState.Loadouts[0].ActiveWeapon = -1;
		int serverFrame = BoltNetwork.ServerFrame;
		for (int num = 0; num < playerState.Loadouts[0].Weapons.Length; num++)
		{
			playerState.Loadouts[0].Weapons[num].RemainingAmmo = playerState.Loadouts[0].Weapons[num].MaxAmmo;
			playerState.Loadouts[0].Weapons[num].NextFireFrame = serverFrame;
			playerState.Loadouts[0].Weapons[num].InCooldownUntil = serverFrame - 10;
		}
		playerState.NextMeleeTime = serverFrame;
		if (teamId != TeamId.Neutral)
		{
			if (!_teamsToScore.ContainsKey(teamId))
			{
				_teamsToScore.Add(teamId, 0);
			}
			_teamsToScore[teamId]++;
			Debug.Log($"[ServerTeamDeathMatchGameModeEntity] Team {teamId} now has {_teamsToScore[teamId]} score");
		}
		PlayerSpawnPoint randomPlayerSpawnPointForTeam = MonoBehaviourSingleton<SpawnManager>.Instance.GetRandomPlayerSpawnPointForTeam((TeamId)playerState.Team);
		float canRespawnTime = BoltNetwork.ServerTime + _config.MinRespawnTime;
		float forceRespawnTime = BoltNetwork.ServerTime + _config.ForcedRespawnTime;
		tDMPlayerState2.NextRespawnPointId = randomPlayerSpawnPointForTeam.UniqueId;
		tDMPlayerState2.CanRespawnTime = canRespawnTime;
		tDMPlayerState2.ForceRespawnTime = forceRespawnTime;
		TDMPlayerData tDMPlayerData = _playersData.Values.FirstOrDefault((TDMPlayerData x) => x.NetworkId == playerDiedSignal.victim.entity.networkId);
		if (tDMPlayerData != null)
		{
			DelayedAction.DelayedActionHandle forcedRespawnAction = DelayedAction.RunAfterSeconds(_config.ForcedRespawnTime, delegate
			{
				RespawnPlayer(playerDiedSignal.victim.entity);
			});
			tDMPlayerData.SetRespawnData(canRespawnTime, randomPlayerSpawnPointForTeam.UniqueId, activeWeapon, forcedRespawnAction);
			TryUpdateKillStreakRewards(tDMPlayerData, killStreak);
		}
		else
		{
			Debug.LogError($"[ServerTeamDeathMatchGameModeEntity] Failed to get TDMPlayer for Victim {playerDiedSignal.victim.entity.networkId}");
		}
		StringBuilder stringBuilder = new StringBuilder();
		AssistTracker component = tDMPlayerState2.BoltEntity.GetComponent<AssistTracker>();
		foreach (TDMPlayerState player in base.state.Players)
		{
			if (player.BoltEntity != null && component.DidPlayerAssist(player.BoltEntity))
			{
				player.Assists++;
				stringBuilder.Append(player.EntityId + ",");
			}
		}
		PlayerDied playerDied = PlayerDied.Create(GlobalTargets.AllClients, ReliabilityModes.ReliableOrdered);
		playerDied.Attacker = playerDiedSignal.attacker;
		playerDied.AssistingPlayerIds = stringBuilder.ToString();
		playerDied.VictimName = tDMPlayerState2.DisplayName;
		playerDied.Victim = playerDiedSignal.victim.entity;
		playerDied.Send();
	}

	private void TryUpdateKillStreakRewards(TDMPlayerData playerData, int killStreak)
	{
		int num = 0;
		int num2 = 0;
		TeamDeathMatchConfigData.KillStreakReward[] killStreakRewards = _config.Rewards.KillStreakRewards;
		for (int i = 0; i < killStreakRewards.Length; i++)
		{
			TeamDeathMatchConfigData.KillStreakReward killStreakReward = killStreakRewards[i];
			if (killStreak >= killStreakReward.StreakAmount)
			{
				num = Mathf.Max(num, killStreakReward.EXPBonus);
				num2 = Mathf.Max(num2, killStreakReward.CurrencyBonus);
			}
		}
		if (num > 0)
		{
			playerData.KillStreakEXPBonus += num;
		}
		if (num2 > 0)
		{
			playerData.KillStreakCurrencyBonus += num2;
		}
	}

	private void OnUpdateRespawnPointRequested(UpdateRespawnPointSignal signal)
	{
		TDMPlayerData tDMPlayerData = _playersData.Values.FirstOrDefault((TDMPlayerData x) => x.NetworkId == signal.PlayerEntity.networkId);
		if (tDMPlayerData == null || !tDMPlayerData.IsRespawning)
		{
			Debug.LogError("[ServerTeamDeathMatchGameModeEntity] Failed to update respawn data for entity " + signal.PlayerEntity.name + ".");
			return;
		}
		if (!string.IsNullOrEmpty(signal.SpawnPointId))
		{
			tDMPlayerData.SelectedRespawnPointId = signal.SpawnPointId;
		}
		if (signal.LoadoutHeroClass != tDMPlayerData.ActiveHeroClass)
		{
			tDMPlayerData.RespawnUpdatedHeroClass = signal.LoadoutHeroClass;
		}
	}

	private void OnPlayerRequestedRespawn(PlayerRequestedRespawnSignal signal)
	{
		RespawnPlayer(signal.PlayerEntity);
	}

	private void RespawnPlayer(BoltEntity entity)
	{
		if (MatchState == MatchState.Complete)
		{
			Debug.Log("[ServerTeamDeathMatchGameModeEntity] Stopping Respawn Player. Match has ended");
			return;
		}
		TDMPlayerData tDMPlayerData = _playersData.Values.FirstOrDefault((TDMPlayerData x) => x.NetworkId == entity.networkId);
		if (tDMPlayerData == null)
		{
			Debug.LogError("[ServerTeamDeathMatchGameModeEntity] Stopping Respawn Player. No tdm player found !");
			return;
		}
		if (!tDMPlayerData.IsRespawning)
		{
			Debug.LogError("[ServerTeamDeathMatchGameModeEntity] Stopping Respawn Player. Player is not currently respawning");
			return;
		}
		if (BoltNetwork.ServerTime < tDMPlayerData.CanRespawnTime)
		{
			Debug.LogError($"[ServerTeamDeathMatchGameModeEntity] Stopping Respawn Player. They cannot respawn too early! ServerTime {BoltNetwork.ServerTime} | CanRespawnTime {tDMPlayerData.CanRespawnTime}");
			return;
		}
		tDMPlayerData.ForcedRespawnAction.Kill();
		if (!string.IsNullOrEmpty(tDMPlayerData.RespawnUpdatedHeroClass) && Enum.TryParse<HeroClass>(tDMPlayerData.RespawnUpdatedHeroClass, out var result))
		{
			SwitchActiveHeroClass(tDMPlayerData, result);
		}
		IPlayerState playerState = entity.GetState<IPlayerState>();
		PlayerSpawnPoint spawnPointById = MonoBehaviourSingleton<SpawnManager>.Instance.GetSpawnPointById(tDMPlayerData.SelectedRespawnPointId, tDMPlayerData.Team);
		if (spawnPointById != null)
		{
			entity.transform.position = spawnPointById.Position;
			entity.transform.rotation = spawnPointById.Rotation;
		}
		else
		{
			Debug.LogError("[ServerTeamDeathMatchGameModeEntity] Got Null Spawn Point! Player with respawn at same location as fallback");
		}
		entity.GetComponent<IDamageable>().HurtCollider.enabled = true;
		playerState.Damageable.Health = playerState.Damageable.MaxHealth;
		playerState.InputEnabled = true;
		playerState.Loadouts[0].ActiveWeapon = tDMPlayerData.CurrentWeaponIndex;
		for (int num = 0; num < playerState.Loadouts[0].Weapons.Length; num++)
		{
			if (!string.IsNullOrEmpty(playerState.Loadouts[0].Weapons[num].Id))
			{
				playerState.Loadouts[0].Weapons[num].RemainingAmmo = playerState.Loadouts[0].Weapons[num].MaxAmmo;
			}
		}
		tDMPlayerData.ClearRespawnData();
		StartCoroutine(ShieldOverTime(playerState));
	}

	private void SwitchActiveHeroClass(TDMPlayerData playerData, HeroClass updatedHeroClass)
	{
		LoadoutData loadoutForClass = playerData.PlayerProfile.LoadoutManager.GetLoadoutForClass(updatedHeroClass, saveAsWell: false);
		IPlayerState playerState = playerData.PlayerEntity.GetState<IPlayerState>();
		playerState.Loadouts[0].FromLoadoutData(loadoutForClass);
		float maxHealth = playerState.Loadouts[0].TryGetModifiedHealth(_profileManager);
		playerState.Damageable.MaxHealth = maxHealth;
		playerState.Loadouts[0].ActiveWeapon = 0;
		base.state.Players.FirstOrDefault((TDMPlayerState x) => x.NetworkId == playerData.NetworkId).Loadout = playerState.Loadouts[0].Serialize();
		playerData.CurrentWeaponIndex = 0;
		playerData.ActiveHeroClass = updatedHeroClass.ToString();
	}

	private IEnumerator ShieldOverTime(IPlayerState playerState)
	{
		playerState.IsShielded = true;
		yield return new WaitForSeconds(_config.RespawnShieldTime);
		playerState.IsShielded = false;
	}
}
