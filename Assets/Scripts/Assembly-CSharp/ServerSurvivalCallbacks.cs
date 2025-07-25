using BSCore;
using BSCore.Constants.Config;

[BoltGlobalBehaviour(BoltNetworkModes.Host, new string[] { "SurvivalGameMode" })]
public class ServerSurvivalCallbacks : BaseServerGameModeCallbacks
{
	private ServerSurvivalGameModeEntity _gameModeEntity;

	protected override bool AllowPlayer(PlayerProfile player)
	{
		if (_gameModeEntity.MatchState != MatchState.Complete)
		{
			return BaseServerGameModeCallbacks.EntitiesByConnection.Count < 20;
		}
		return false;
	}

	protected override BoltEntity CreatePlayerEntity(PlayerProfile player)
	{
		LoadoutData defaultLoadout = player.LoadoutManager.GetDefaultLoadout(player.LoadoutManager.EquippedHeroClass);
		LoadoutData equippedLoadout = player.LoadoutManager.GetEquippedLoadout();
		for (int i = 0; i < equippedLoadout.Weapons.Length; i++)
		{
			equippedLoadout.Weapons[i] = string.Empty;
		}
		if (string.IsNullOrEmpty(equippedLoadout.MeleeWeapon))
		{
			equippedLoadout.MeleeWeapon = defaultLoadout.MeleeWeapon;
		}
		BoltEntity boltEntity = BaseServerGameModeCallbacks.CreatePlayerEntity(player.DisplayName, equippedLoadout, _playerPrefabId, GameModeType.Survival, isSecondLife: false, _profileManager);
		boltEntity.GetState<IPlayerState>().Team = 1;
		return boltEntity;
	}

	protected override void OnPlayerConnected(BoltEntity playerEntity, PlayerProfile playerProfile, PlatformType platform)
	{
		_gameModeEntity.PlayerConnected(playerEntity);
	}

	protected override void OnPlayerDisconnected(BoltEntity playerEntity, PlayerProfile playerProfile)
	{
		_gameModeEntity.PlayerDisconnected(playerEntity);
	}

	protected override void SpawnGameModeEntity()
	{
		BoltEntity boltEntity = BoltNetwork.Instantiate(BoltPrefabs.SurvivalGameModeEntity);
		_gameModeEntity = boltEntity.GetComponent<ServerSurvivalGameModeEntity>();
		ISurvivalGameModeState state = boltEntity.GetState<ISurvivalGameModeState>();
		state.MatchState = 0;
		SurvivalConfigData survivalConfigData = _configManager.Get<SurvivalConfigData>(DataKeys.Survival);
		state.Map = survivalConfigData.GetRandomMap();
	}
}
