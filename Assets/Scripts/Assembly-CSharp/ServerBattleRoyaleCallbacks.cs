using BSCore;

[BoltGlobalBehaviour(BoltNetworkModes.Host, new string[] { "BattleRoyaleGameMode" })]
public class ServerBattleRoyaleCallbacks : BaseServerGameModeCallbacks
{
	public const string DefaultMeleeWeapon = "krikketBat";

	private ServerBattleRoyaleGameModeEntity _gameModeEntity;

	protected override void SpawnGameModeEntity()
	{
		BoltEntity boltEntity = BoltNetwork.Instantiate(BoltPrefabs.BattleRoyaleGameModeEntity);
		_gameModeEntity = boltEntity.GetComponent<ServerBattleRoyaleGameModeEntity>();
		IBattleRoyaleGameModeState state = boltEntity.GetState<IBattleRoyaleGameModeState>();
		state.Map = CommandLineArgsManager.GetArg("-map", "BattleRoyale");
		state.MatchState = 0;
	}

	protected override bool AllowPlayer(PlayerProfile player)
	{
		return _gameModeEntity.MatchState == MatchState.Starting;
	}

	protected override void OnPlayerConnected(BoltEntity playerEntity, PlayerProfile playerProfile, PlatformType platform)
	{
		_gameModeEntity.PlayerConnected(playerEntity);
	}

	protected override void OnPlayerDisconnected(BoltEntity playerEntity, PlayerProfile playerProfile)
	{
		_gameModeEntity.PlayerDisconnected(playerEntity);
	}

	protected override BoltEntity CreatePlayerEntity(PlayerProfile player)
	{
		BoltEntity boltEntity = BaseServerGameModeCallbacks.CreatePlayerEntity(player.DisplayName, player.LoadoutManager.GetEquippedLoadout(), _playerPrefabId, GameModeType.BattleRoyale, isSecondLife: false, _profileManager);
		IPlayerState state = boltEntity.GetState<IPlayerState>();
		state.Loadouts[0].Weapons[1].Id = string.Empty;
		state.Loadouts[0].Weapons[2].Id = string.Empty;
		state.Loadouts[0].Weapons[0].Id = "krikketBat";
		state.Loadouts[0].ActiveWeapon = 0;
		return boltEntity;
	}
}
