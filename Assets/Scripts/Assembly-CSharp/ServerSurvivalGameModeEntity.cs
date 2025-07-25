using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using BSCore.Constants.Config;
using Bolt;
using UnityEngine;
using Zenject;

public class ServerSurvivalGameModeEntity : ServerBaseGameModeEntity<ISurvivalGameModeState>
{
	public class SurvivalPlayerData
	{
		public BoltEntity PlayerEntity;

		public string DisplayName;

		public bool HasLoaded;

		public bool IsAlive;

		public float Points;

		public float TimeJoined;

		public BoltConnection PlayerConnection { get; private set; }

		public float TimeInMatch => BoltNetwork.ServerTime - TimeJoined;

		public SurvivalPlayerData(BoltEntity playerEntity)
		{
			PlayerEntity = playerEntity;
			PlayerConnection = PlayerEntity.controller;
			DisplayName = playerEntity.GetState<IPlayerState>().DisplayName;
			Points = 0f;
			IsAlive = true;
			TimeJoined = BoltNetwork.ServerTime;
		}
	}

	[Inject]
	private ProfileManager _profileManager;

	[Inject]
	private StatisticsManager _statisticsManager;

	private SurvivalConfigData _config;

	private readonly Dictionary<NetworkId, SurvivalPlayerData> _playersByNetworkId = new Dictionary<NetworkId, SurvivalPlayerData>();

	private readonly List<SurvivalEnemy> _spawnedEnemies = new List<SurvivalEnemy>();

	private Coroutine _spawnEnemySwarmRoutine;

	public static int Wave { get; private set; }

	private float _longestTimeInMatch => _playersByNetworkId.Values.Max((SurvivalPlayerData p) => p.TimeInMatch);

	public MatchState MatchState => (MatchState)base.state.MatchState;

	public override void Attached()
	{
		base.Attached();
		GameModeEntityHelper.AmmoClipsAreLimited = true;
		if (base.entity.isOwner)
		{
			_config = _configManager.Get<SurvivalConfigData>(DataKeys.Survival);
			_config.Setup();
			_config.Pickups.Init(_profileManager);
			base.state.AddCallback("Map", OnMapUpdated);
			SurvivalEnemy.Damaged += OnEnemyDamaged;
			SurvivalEnemy.Died += OnEnemyDied;
			_signalBus.Subscribe<PlayerRequestedRespawnSignal>(OnPlayerRequestedRespawn);
		}
	}

	public override void Detached()
	{
		if (base.entity.isOwner)
		{
			SurvivalEnemy.Damaged -= OnEnemyDamaged;
			SurvivalEnemy.Died -= OnEnemyDied;
			_signalBus.Unsubscribe<PlayerRequestedRespawnSignal>(OnPlayerRequestedRespawn);
		}
	}

	public void PlayerConnected(BoltEntity playerEntity)
	{
		_playersByNetworkId.Add(playerEntity.networkId, new SurvivalPlayerData(playerEntity));
		int index = -1;
		for (int i = 0; i < base.state.Players.Length; i++)
		{
			if (base.state.Players[i].EntityId == null)
			{
				index = i;
				break;
			}
		}
		IPlayerState playerState = playerEntity.GetState<IPlayerState>();
		playerState.AmmoClips = _config.StartingAmmoClips;
		base.state.Players[index].Name = playerState.DisplayName;
		base.state.Players[index].Points = 0;
		base.state.Players[index].EntityId = playerEntity;
	}

	public void PlayerDisconnected(BoltEntity playerEntity)
	{
		bool flag = false;
		if (_playersByNetworkId.TryGetValue(playerEntity.networkId, out var value))
		{
			flag = value.IsAlive;
			_playersByNetworkId.Remove(playerEntity.networkId);
		}
		if (base.state.TryGetPlayerByEntity(playerEntity, out var player))
		{
			if (flag)
			{
				UpdateSurvivalStatisticForPlayer(value.PlayerEntity, Mathf.FloorToInt(value.Points));
			}
			player.Name = string.Empty;
			player.Points = 0;
			player.EntityId = null;
		}
	}

	protected override void OnPlayerLoaded(PlayerLoadedSignal playerLoadedSignal)
	{
		if (_playersByNetworkId.ContainsKey(playerLoadedSignal.PlayerEntity.networkId))
		{
			_playersByNetworkId[playerLoadedSignal.PlayerEntity.networkId].HasLoaded = true;
		}
		if (ShouldStart())
		{
			StartCoroutine(SurvivalMatchStateCycle());
		}
	}

	protected override void OnPlayerDied(PlayerDiedSignal playerDiedSignal)
	{
		if (_playersByNetworkId.TryGetValue(playerDiedSignal.victim.entity.networkId, out var value))
		{
			value.IsAlive = false;
			PlayerController victim = playerDiedSignal.victim;
			IPlayerState playerState = victim.state;
			GameModeEntityHelper.DropAllItems(playerState, victim.transform.position, victim.transform.rotation, playerState.Loadouts[0].Weapons[0].Id);
			UpdateSurvivalStatisticForPlayer(victim.entity, Mathf.FloorToInt(value.Points));
		}
	}

	private void UpdateSurvivalStatisticForPlayer(BoltEntity entity, int points)
	{
		BaseServerGameModeCallbacks.EntitiesByConnection.TryGetValue(entity.controller, out var value);
		Dictionary<StatisticKey, int> updates = new Dictionary<StatisticKey, int>
		{
			{
				StatisticKey.LifetimeSurvival,
				points
			},
			{
				StatisticKey.WeeklySurvial,
				points
			},
			{
				StatisticKey.DailySurvival,
				points
			}
		};
		_statisticsManager.UpdatePlayerStatistics(value.Profile, updates);
	}

	private IEnumerator SurvivalMatchStateCycle()
	{
		base.state.MatchState = 1;
		Debug.Log("[ServerSurvivalGameModeEntity] Player(s) loaded, giving it a bit longer...");
		yield return new WaitForSeconds(1.5f);
		Debug.Log("[ServerSurvivalGameModeEntity] Starting Match");
		SpawnPickups();
		base.state.MatchStartTime = BoltNetwork.ServerTime;
		_spawnEnemySwarmRoutine = StartCoroutine(SpawnEnemySwarmRoutine());
		yield return new WaitUntil(ShouldEnd);
		Debug.Log("[ServerSurvivalGameModeEntity] Ending Match");
		StopCoroutine(_spawnEnemySwarmRoutine);
		_spawnEnemySwarmRoutine = null;
		yield return EndMatch();
	}

	private bool ShouldStart()
	{
		if (base.state.MatchState == 0)
		{
			return _playersByNetworkId.Where((KeyValuePair<NetworkId, SurvivalPlayerData> kvp) => kvp.Value.HasLoaded).Count() >= 1;
		}
		return false;
	}

	private bool ShouldEnd()
	{
		return _playersByNetworkId.Where((KeyValuePair<NetworkId, SurvivalPlayerData> kvp) => kvp.Value.IsAlive).Count() < 1;
	}

	private IEnumerator SpawnEnemySwarmRoutine()
	{
		while (true)
		{
			float spawnInterval = CalculateSpawnInterval();
			List<SurvivalEnemyConfigData> enemyConfigs = CalculateEnemiesToSpawn();
			Debug.Log($"[ServerSurvivalGameModeEntity] Starting Wave {Wave} (Spawning {enemyConfigs.Count} enemies)");
			int numSpawned = 0;
			base.state.EnemiesToKill = enemyConfigs.Count;
			base.state.Wave = Wave;
			while (numSpawned < enemyConfigs.Count)
			{
				if (_spawnedEnemies.Count < _config.MaxEnemies)
				{
					SpawnEnemy(enemyConfigs[numSpawned]);
					numSpawned++;
					yield return new WaitForSeconds(spawnInterval);
				}
				yield return null;
			}
			Debug.Log($"[ServerSurvivalGameModeEntity] All enemies in wave {Wave} spawned");
			yield return new WaitWhile(() => _spawnedEnemies.Count > 0);
			Debug.Log($"[ServerSurvivalGameModeEntity] Wave {Wave} complete, wating for {_config.IntermissionLength}s for next wave");
			yield return new WaitForSeconds(_config.IntermissionLength);
			Wave++;
		}
	}

	private List<SurvivalEnemyConfigData> CalculateEnemiesToSpawn()
	{
		int num = _config.StartingEnemies + Wave * _config.WaveModifiers.Count;
		SurvivalEnemyConfigData item = _config.Enemies[0];
		List<SurvivalEnemyConfigData> list = _config.Enemies.Where((SurvivalEnemyConfigData ec) => ec.StartWave <= Wave && ec.Percentage > 0).ToList();
		int count = list.Count;
		for (int num2 = 0; num2 < count; num2++)
		{
			int num3 = Mathf.FloorToInt((float)(num * list[num2].Percentage) / 100f);
			for (int num4 = 0; num4 < num3; num4++)
			{
				list.Add(list[num2]);
			}
		}
		if (list.Count < num)
		{
			for (int num5 = list.Count; num5 <= num; num5++)
			{
				list.Add(item);
			}
		}
		list.Shuffle(new System.Random());
		return list;
	}

	private float CalculateSpawnInterval()
	{
		return Mathf.Max(4f - 0.06f * (float)Wave, 1f);
	}

	private void SpawnEnemy(SurvivalEnemyConfigData enemyConfig)
	{
		PlayerSpawnPoint enemySpawnPoint = SurvivalSpawnManager.GetEnemySpawnPoint();
		SurvivalEnemy.SurvivalEnemyAttachToken token = new SurvivalEnemy.SurvivalEnemyAttachToken(enemyConfig.Name);
		BoltEntity boltEntity = BoltNetwork.Instantiate(BoltPrefabs.SurvivalEnemy, token, enemySpawnPoint.Position, enemySpawnPoint.Rotation);
		SurvivalEnemy component = boltEntity.GetComponent<SurvivalEnemy>();
		ISurvivalEnemy survivalEnemy = boltEntity.GetState<ISurvivalEnemy>();
		Damageable damageable = survivalEnemy.Damageable;
		float health = (survivalEnemy.Damageable.MaxHealth = CalculateScaledHealth(enemyConfig.Health));
		damageable.Health = health;
		enemyConfig.Damage = CalculateScaledDamage(enemyConfig.Damage);
		enemyConfig.Speed = CalculateScaledSpeed(enemyConfig.Speed);
		component.SetConfig(enemyConfig);
		_spawnedEnemies.Add(component);
	}

	private float CalculateScaledHealth(float baseHealth)
	{
		return baseHealth + _config.PlayerCountModifiers.Health * (float)_playersByNetworkId.Count + _config.WaveModifiers.Health * (float)Wave;
	}

	private float CalculateScaledDamage(float baseDamage)
	{
		return baseDamage + _config.PlayerCountModifiers.Damage * (float)_playersByNetworkId.Count + _config.WaveModifiers.Damage * (float)Wave;
	}

	private float CalculateScaledSpeed(float baseSpeed)
	{
		return baseSpeed + _config.WaveModifiers.Speed * (float)Wave;
	}

	private void OnEnemyDamaged(SurvivalEnemy.SurvivalEnemyDamageInfo damageInfo)
	{
		float points = damageInfo.Damage * damageInfo.Enemy.Config.PointModifier;
		AwardPoints(damageInfo.Attacker, points);
	}

	private void OnEnemyDied(SurvivalEnemy enemy, BoltEntity attacker)
	{
		base.state.EnemiesToKill--;
		float num = _config.PointsPerKill * enemy.Config.PointModifier + (float)Wave * _config.WaveModifiers.Points;
		if (enemy.state.IsAttacking && !enemy.IsAttacking(attacker))
		{
			num += _config.AssistBonusPoints * enemy.Config.PointModifier + (float)Wave * _config.WaveModifiers.Points;
		}
		AwardPoints(attacker, num);
		_spawnedEnemies.Remove(enemy);
	}

	private void AwardPoints(BoltEntity attacker, float points)
	{
		if (base.state.TryGetPlayerByEntity(attacker, out var player) && _playersByNetworkId.TryGetValue(attacker.networkId, out var value))
		{
			value.Points += points;
			player.Points = Mathf.FloorToInt(value.Points);
		}
	}

	protected override IEnumerator EndMatch()
	{
		Wave = 0;
		base.state.MatchState = 2;
		_signalBus.Fire(new MatchStateUpdatedSignal((MatchState)base.state.MatchState));
		yield return new WaitForSeconds(5f);
		HealthPickupEntity[] array = UnityEngine.Object.FindObjectsOfType<HealthPickupEntity>();
		for (int i = 0; i < array.Length; i++)
		{
			BoltNetwork.Destroy(array[i].gameObject);
		}
		WeaponPickupEntity[] array2 = UnityEngine.Object.FindObjectsOfType<WeaponPickupEntity>();
		for (int i = 0; i < array2.Length; i++)
		{
			BoltNetwork.Destroy(array2[i].gameObject);
		}
		foreach (SurvivalEnemy spawnedEnemy in _spawnedEnemies)
		{
			spawnedEnemy.DestroySelf();
		}
		_spawnedEnemies.Clear();
		base.state.MatchState = 0;
		_signalBus.Fire(new MatchStateUpdatedSignal((MatchState)base.state.MatchState));
	}

	protected override void UpdateMatchEndState()
	{
	}

	private void OnMapUpdated()
	{
		_helper.LoadMap(base.state.Map);
	}

	private void SpawnPickups()
	{
		SpawnItemPickups();
	}

	private void SpawnItemPickups()
	{
		foreach (string itemPickup in _config.Pickups.GetItemPickups())
		{
			PickupSpawnPoint nextPickupSpawnPoint = MonoBehaviourSingleton<SpawnManager>.Instance.GetNextPickupSpawnPoint();
			WeaponPickupEntity.SpawnPickup(itemPickup, nextPickupSpawnPoint.Position);
		}
	}

	private void OnPlayerRequestedRespawn(PlayerRequestedRespawnSignal signal)
	{
		if (MatchState == MatchState.Complete)
		{
			Debug.Log("[ServerTeamDeathMatchGameModeEntity] Stopping Respawn Player. Match has ended");
			return;
		}
		BoltEntity playerEntity = signal.PlayerEntity;
		if (_playersByNetworkId.TryGetValue(playerEntity.networkId, out var value) && !value.IsAlive)
		{
			PlayerSpawnPoint randomPlayerSpawnPoint = MonoBehaviourSingleton<SpawnManager>.Instance.GetRandomPlayerSpawnPoint();
			PlayerController component = playerEntity.GetComponent<PlayerController>();
			component.Spawn(randomPlayerSpawnPoint);
			component.state.Damageable.Health = component.state.Damageable.MaxHealth;
			component.state.Loadouts[0].ActiveWeapon = 0;
			component.state.Loadouts[0].Weapons[1].RemainingAmmo = 0;
			component.state.Loadouts[0].Weapons[2].RemainingAmmo = 0;
			value.Points = 0f;
			if (base.state.TryGetPlayerByEntity(playerEntity, out var player))
			{
				player.Points = 0;
			}
		}
	}
}
