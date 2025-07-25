using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using BSCore.Constants.Config;
using Bolt;
using TMPro;
using UnityEngine;
using Zenject;

public class ServerBattleRoyaleGameModeEntity : ServerBaseGameModeEntity<IBattleRoyaleGameModeState>
{
	public class BRPlayer
	{
		public BoltEntity PlayerEntity;

		public string DisplayName;

		public bool HasLoaded;

		public int Kills;

		public bool IsAlive;

		public bool HasUsedSecondLife;

		public float DiedAt;

		public BoltConnection PlayerConnection { get; private set; }

		public BRPlayer(BoltEntity playerEntity)
		{
			PlayerEntity = playerEntity;
			PlayerConnection = PlayerEntity.controller;
			DisplayName = playerEntity.GetState<IPlayerState>().DisplayName;
			Kills = 0;
			IsAlive = true;
			HasUsedSecondLife = false;
		}
	}

	protected const float PLAYER_LOADING_TIMEOUT = 30f;

	[Inject]
	private ProfileManager _profileManager;

	[SerializeField]
	private GameObject _stopDropCollider;

	[SerializeField]
	private TMP_InputField _expectedPlayersInput;

	private BattleRoyaleConfigData _config;

	private Dictionary<NetworkId, BRPlayer> _playersByNetworkId = new Dictionary<NetworkId, BRPlayer>();

	private Dictionary<BRPlayer, Coroutine> _playerDeathTimers = new Dictionary<BRPlayer, Coroutine>();

	public MatchState MatchState => (MatchState)base.state.MatchState;

	private void Awake()
	{
		if (!CommandLineArgsManager.IsHeadlessMode())
		{
			_expectedPlayersInput.onSubmit.AddListener(OnExpectedPlayersUpdated);
			_expectedPlayersInput.text = "2";
		}
		else
		{
			Object.Destroy(_expectedPlayersInput.gameObject);
		}
	}

	public override void Attached()
	{
		base.Attached();
		if (base.entity.isOwner)
		{
			_config = _configManager.Get<BattleRoyaleConfigData>(DataKeys.BattleRoyale);
			base.state.AddCallback("Map", OnMapUpdated);
			StartCoroutine(SpawnItemsWhenMapLoaded());
			_signalBus.Subscribe<PlayerRequestedRespawnSignal>(OnPlayerRequestedSecondLife);
		}
		else
		{
			_expectedPlayersInput.gameObject.SetActive(value: false);
		}
	}

	public override void Detached()
	{
		base.Detached();
		if (base.entity.isOwner)
		{
			_signalBus.Unsubscribe<PlayerRequestedRespawnSignal>(OnPlayerRequestedSecondLife);
		}
	}

	private void OnExpectedPlayersUpdated(string value)
	{
		Debug.Log("[ServerBattleRoyaleGameModeEntity] Updating expected players to " + value);
		if (int.TryParse(value, out var result))
		{
			_expectedPlayerCount = result;
		}
	}

	public void TryStartPlayerLoading()
	{
		if (_playerLoadingStarted)
		{
			return;
		}
		if (PlayfabServerManagement.IsInitializedAndReady)
		{
			_expectedPlayerCount = PlayfabServerManagement.ExpectedPlayerCount;
		}
		else
		{
			int result = 1;
			if (int.TryParse(_expectedPlayersInput.text, out result))
			{
				_expectedPlayerCount = result;
			}
		}
		StartCoroutine(BattleRoyaleStateCycle());
		_playerLoadingStarted = true;
	}

	private IEnumerator BattleRoyaleStateCycle()
	{
		Debug.Log("[ServerBattleRoyaleGameModeEntity] Starting Battle Royale cycle. Waiting for players");
		yield return new WaitUntil(ShouldStartCheck);
		Debug.Log("[ServerBattleRoyaleGameModeEntity] Players loaded, giving it a bit longer...");
		yield return new WaitForSeconds(1.5f);
		Debug.Log("[ServerBattleRoyaleGameModeEntity] Starting Match");
		TryStartMatch();
		yield return new WaitForSeconds(2f);
		Debug.Log("[ServerBattleRoyaleGameModeEntity] Dropping stop drop collider");
		SetPlayersInputState(inputEnabled: true);
		_stopDropCollider.SetActive(value: false);
		yield return new WaitUntil(ShouldEndCheck);
		Debug.Log("[ServerBattleRoyaleGameModeEntity] Ending Match");
		yield return EndMatch();
	}

	protected override void OnPlayerLoaded(PlayerLoadedSignal playerLoadedSignal)
	{
		if (_playersByNetworkId.ContainsKey(playerLoadedSignal.PlayerEntity.networkId))
		{
			_playersByNetworkId[playerLoadedSignal.PlayerEntity.networkId].HasLoaded = true;
		}
	}

	private bool ShouldStartCheck()
	{
		_playerLoadingTimeoutTimer += Time.deltaTime;
		if (_playersByNetworkId.Count < _expectedPlayerCount || !_playersByNetworkId.Values.All((BRPlayer p) => p.HasLoaded))
		{
			return _playerLoadingTimeoutTimer > 30f;
		}
		return true;
	}

	private void TryStartMatch()
	{
		if (base.state.MatchState == 0)
		{
			base.state.RemainingPlayers = _playersByNetworkId.Count;
			base.state.RemainingFirstLifePlayers = _playersByNetworkId.Count;
			base.state.MatchState = 1;
			_signalBus.Fire(new MatchStateUpdatedSignal((MatchState)base.state.MatchState));
			MonoBehaviourSingleton<BRZonesController>.Instance.StartZoneClosures();
		}
	}

	private bool ShouldEndCheck()
	{
		if (base.state.RemainingPlayers <= 1)
		{
			return base.state.MatchState == 1;
		}
		return false;
	}

	protected override void UpdateMatchEndState()
	{
		base.state.MatchState = 2;
		_signalBus.Fire(new MatchStateUpdatedSignal((MatchState)base.state.MatchState));
		SetPlayersInputState(inputEnabled: false);
	}

	public void PlayerConnected(BoltEntity playerEntity)
	{
		Debug.Log("Adding BR Player " + playerEntity.networkId);
		_playersByNetworkId.Add(playerEntity.networkId, new BRPlayer(playerEntity));
		if (playerEntity.TryFindState<IPlayerState>(out var playerState))
		{
			playerState.InputEnabled = base.state.MatchState == 1;
		}
		UpdateRemainingPlayers();
		TryStartPlayerLoading();
	}

	public void PlayerDisconnected(BoltEntity playerEntity)
	{
		Debug.Log("Removing BR Player " + playerEntity.networkId);
		_playersByNetworkId.Remove(playerEntity.networkId);
		UpdateRemainingPlayers();
	}

	protected override void OnPlayerDied(PlayerDiedSignal playerDiedSignal)
	{
		BRPlayer value;
		bool flag = _playersByNetworkId.TryGetValue(playerDiedSignal.attacker.networkId, out value);
		BRPlayer value2;
		bool flag2 = _playersByNetworkId.TryGetValue(playerDiedSignal.victim.entity.networkId, out value2);
		value.Kills++;
		value2.IsAlive = false;
		value2.DiedAt = BoltNetwork.ServerTime;
		if (!value2.HasUsedSecondLife)
		{
			GameModeEntityHelper.DropAllItems(playerDiedSignal.victim.state, playerDiedSignal.victim.transform.position, playerDiedSignal.victim.transform.rotation, "krikketBat");
			playerDiedSignal.victim.state.Loadouts[0].Weapons[1].Id = string.Empty;
			playerDiedSignal.victim.state.Loadouts[0].Weapons[2].Id = string.Empty;
			playerDiedSignal.victim.state.Loadouts[0].Weapons[0].Id = string.Empty;
			if (!flag || !flag2)
			{
				string text = "";
				if (!flag)
				{
					text += "attacker";
					if (!flag2)
					{
						text += " or ";
					}
				}
				if (!flag2)
				{
					text += "victim";
				}
				Debug.LogError("[ServerBattleRoyaleGameModeEntity] Could not find " + text);
				return;
			}
		}
		UpdateRemainingPlayers();
		if (!value2.HasUsedSecondLife && base.state.RemainingFirstLifePlayers >= _config.SecondLife.MinPlayers)
		{
			value2.PlayerEntity.GetState<IPlayerState>().CanSecondLife = true;
			_playerDeathTimers.Add(value2, StartCoroutine(DestroyPlayerAfterDelay(_config.SecondLife.TimeLimit, value2)));
		}
		else
		{
			StartCoroutine(DestroyPlayerAfterDelay(3f, value2));
		}
	}

	private IEnumerator DestroyPlayerAfterDelay(float time, BRPlayer player)
	{
		yield return new WaitForSeconds(time);
		if (player.PlayerEntity != null && player.PlayerEntity.gameObject != null)
		{
			BoltNetwork.Destroy(player.PlayerEntity.gameObject);
		}
		if (_playerDeathTimers.ContainsKey(player))
		{
			_playerDeathTimers.Remove(player);
		}
	}

	private void OnPlayerRequestedSecondLife(PlayerRequestedRespawnSignal signal)
	{
		BoltEntity playerEntity = signal.PlayerEntity;
		if (_playersByNetworkId.TryGetValue(playerEntity.networkId, out var value))
		{
			if (_playerDeathTimers.ContainsKey(value))
			{
				StopCoroutine(_playerDeathTimers[value]);
				_playerDeathTimers.Remove(value);
			}
			IPlayerState playerState = signal.PlayerEntity.GetState<IPlayerState>();
			if (!value.IsAlive && !value.HasUsedSecondLife && playerState.CanSecondLife && base.state.MatchState == 1)
			{
				ChangeToSecondLife(value);
				UpdateRemainingPlayers();
			}
		}
	}

	private void ChangeToSecondLife(BRPlayer player)
	{
		LoadoutData newLoadout = new LoadoutData
		{
			Outfit = "zombieHuggable",
			MeleeWeapon = "zombieHuggableMelee"
		};
		BoltEntity playerEntity = player.PlayerEntity;
		PlayerSpawnPoint playerSpawnPoint = BRSpawnManager.Get2ndLifeSpawnPoint();
		Vector3 vector = playerSpawnPoint.Position;
		if (Physics.Raycast(vector, Vector3.down, out var hitInfo, 500f, LayerMaskConfig.GroundLayers))
		{
			vector = hitInfo.point;
		}
		Debug.Log($"[BaseServerGameModeCallbacks] Spawning new player at {vector}");
		playerEntity.transform.position = vector;
		playerEntity.transform.rotation = playerSpawnPoint.Rotation;
		OutfitProfile byId = _profileManager.GetById<OutfitProfile>(newLoadout.Outfit);
		IPlayerState playerState = playerEntity.GetState<IPlayerState>();
		playerState.Loadouts[0].FromLoadoutData(newLoadout);
		playerState.Loadouts[0].ActiveWeapon = 0;
		playerState.IsSecondLife = true;
		playerState.CanSecondLife = false;
		playerState.Damageable.Health = byId.HeroClassProfile.Health;
		playerState.Damageable.MaxHealth = byId.HeroClassProfile.Health;
		StartCoroutine(EnableInputAfterSecondLifeSpawn(playerState));
		player.IsAlive = true;
		player.HasUsedSecondLife = true;
	}

	private IEnumerator EnableInputAfterSecondLifeSpawn(IPlayerState state)
	{
		yield return new WaitForSeconds(2.667f);
		state.InputEnabled = true;
	}

	private void UpdateRemainingPlayers()
	{
		base.state.RemainingPlayers = _playersByNetworkId.Values.Count((BRPlayer p) => p.IsAlive);
		base.state.RemainingFirstLifePlayers = _playersByNetworkId.Values.Count((BRPlayer p) => p.IsAlive && !p.HasUsedSecondLife);
	}

	private void SetPlayersInputState(bool inputEnabled)
	{
		foreach (BRPlayer value in _playersByNetworkId.Values)
		{
			if (value.IsAlive && value.PlayerEntity.TryFindState<IPlayerState>(out var playerState))
			{
				SetPlayerInputState(playerState, inputEnabled);
			}
		}
	}

	private void OnMapUpdated()
	{
		_helper.LoadMap(base.state.Map);
	}

	private IEnumerator SpawnItemsWhenMapLoaded()
	{
		yield return new WaitUntil(() => MonoBehaviourSingleton<SpawnManager>.IsInstantiated && MonoBehaviourSingleton<BRZonesController>.IsInstantiated);
		_config.Pickups.Init(_profileManager);
		MonoBehaviourSingleton<BRZonesController>.Instance.SetConfigData(_config);
		SpawnPickups();
	}

	private void SpawnPickups()
	{
		SpawnItemPickups();
		SpawnHealthPickups();
	}

	private void SpawnItemPickups()
	{
		foreach (string itemPickup in _config.Pickups.GetItemPickups())
		{
			PickupSpawnPoint nextPickupSpawnPoint = MonoBehaviourSingleton<SpawnManager>.Instance.GetNextPickupSpawnPoint();
			WeaponPickupEntity.SpawnPickup(itemPickup, nextPickupSpawnPoint.Position);
		}
	}

	private void SpawnHealthPickups()
	{
	}
}
