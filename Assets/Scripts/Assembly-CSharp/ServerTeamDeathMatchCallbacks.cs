using BSCore;
using BSCore.Constants.Config;

[BoltGlobalBehaviour(BoltNetworkModes.Host, new string[] { "TeamDeathMatchGameMode" })]
public class ServerTeamDeathMatchCallbacks : BaseServerGameModeCallbacks
{
	private ServerTeamDeathMatchGameModeEntity _gameModeEntity;

	protected override void SpawnGameModeEntity()
	{
		BoltEntity boltEntity = BoltNetwork.Instantiate(BoltPrefabs.TeamDeathMatchGameModeEntity);
		_gameModeEntity = boltEntity.GetComponent<ServerTeamDeathMatchGameModeEntity>();
		ITeamDeathMatchGameModeState state = boltEntity.GetState<ITeamDeathMatchGameModeState>();
		state.MatchState = 0;
		_signalBus.Fire(new MatchStateUpdatedSignal((MatchState)state.MatchState));
		TeamDeathMatchConfigData teamDeathMatchConfigData = _configManager.Get<TeamDeathMatchConfigData>(DataKeys.TeamDeathMatch);
		state.Map = teamDeathMatchConfigData.GetRandomMap();
	}

	protected override BoltEntity CreatePlayerEntity(PlayerProfile player)
	{
		BoltEntity boltEntity = BaseServerGameModeCallbacks.CreatePlayerEntity(player.DisplayName, player.LoadoutManager.GetEquippedLoadout(), _playerPrefabId, GameModeType.TeamDeathMatch, isSecondLife: false, _profileManager);
		IPlayerState state = boltEntity.GetState<IPlayerState>();
		LoadoutData defaultLoadout = player.LoadoutManager.GetDefaultLoadout(player.LoadoutManager.EquippedHeroClass);
		if (string.IsNullOrEmpty(state.Loadouts[0].MeleeWeapon.Id))
		{
			state.Loadouts[0].MeleeWeapon.Id = defaultLoadout.MeleeWeapon;
		}
		for (int i = 0; i < state.Loadouts[0].Weapons.Length; i++)
		{
			if (string.IsNullOrEmpty(state.Loadouts[0].Weapons[i].Id))
			{
				state.Loadouts[0].Weapons[i].Id = defaultLoadout.Weapons[i];
			}
		}
		state.Loadouts[0].ActiveWeapon = 0;
		state.InputEnabled = false;
		return boltEntity;
	}

	protected override bool AllowPlayer(PlayerProfile player)
	{
		return _gameModeEntity.MatchState == MatchState.Starting;
	}

	protected override void OnPlayerConnected(BoltEntity playerEntity, PlayerProfile playerProfile, PlatformType platform)
	{
		_gameModeEntity.PlayerConnected(playerEntity, playerProfile, platform);
	}

	protected override void OnPlayerDisconnected(BoltEntity playerEntity, PlayerProfile playerProfile)
	{
		_gameModeEntity.PlayerDisconnected(playerEntity, playerProfile);
	}
}
