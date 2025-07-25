using System.Collections.Generic;
using BSCore;
using Bolt;
using UdpKit;
using UnityEngine;
using Zenject;

public abstract class BaseServerGameModeCallbacks : GlobalEventListener
{
	public static readonly Dictionary<BoltConnection, ConnectionEntity> EntitiesByConnection = new Dictionary<BoltConnection, ConnectionEntity>();

	protected ProfileManager _profileManager;

	protected SignalBus _signalBus;

	protected ConfigManager _configManager;

	private ServerUserAuthenticationManager _authmanager;

	protected PrefabId _playerPrefabId = BoltPrefabs.Player;

	protected virtual void Start()
	{
		ResolveZenjectBindings();
		SpawnGameModeEntity();
	}

	protected virtual void ResolveZenjectBindings()
	{
		_profileManager = SceneContextHelper.ResolveZenjectBinding<ProfileManager>();
		_signalBus = SceneContextHelper.ResolveZenjectBinding<SignalBus>();
		_authmanager = SceneContextHelper.ResolveZenjectBinding<ServerUserAuthenticationManager>();
		_configManager = SceneContextHelper.ResolveZenjectBinding<ConfigManager>();
	}

	protected abstract void SpawnGameModeEntity();

	protected abstract bool AllowPlayer(PlayerProfile player);

	protected abstract BoltEntity CreatePlayerEntity(PlayerProfile player);

	protected abstract void OnPlayerConnected(BoltEntity playerEntity, PlayerProfile playerProfile, PlatformType platformType);

	protected abstract void OnPlayerDisconnected(BoltEntity playerEntity, PlayerProfile playerProfile);

	public override void Connected(BoltConnection connection)
	{
		ServerConnectToken serverConnectToken = connection.ConnectToken as ServerConnectToken;
		CreatePlayerEntity(serverConnectToken.GetData(), connection);
	}

	protected virtual void CreatePlayerEntity(ServerConnectToken.Data data, BoltConnection connection)
	{
		_authmanager.AuthenticateUser(data.serviceToken, onPlayerAuthSuccess, onPlayerAuthFailed);
		void onPlayerAuthFailed(FailureReasons reason)
		{
			Debug.LogError($"[BaseServerGameModeCallbacks] Failed to authenticate user with serviceToken {data.serviceToken}. Reason: {reason}");
			connection.Disconnect(new DisconnectReasonToken
			{
				reason = DisconnectReasonToken.Reason.AuthenticationFailed
			});
		}
		void onPlayerAuthSuccess(PlayerProfile player)
		{
			BoltEntity boltEntity = CreatePlayerEntity(player);
			boltEntity.AssignControl(connection);
			EntitiesByConnection.Add(connection, new ConnectionEntity(boltEntity, player));
			if (PlayfabServerManagement.IsInitializedAndReady)
			{
				PlayfabServerManagement.PlayerConnected(player.Id);
			}
			OnPlayerConnected(boltEntity, player, data.Platform);
		}
	}

	public static BoltEntity CreatePlayerEntity(string displayName, LoadoutData loadoutData, PrefabId playerPrefabId, GameModeType gameModeType, bool isSecondLife, ProfileManager _profileManager)
	{
		PlayerController.AttachToken token = new PlayerController.AttachToken(gameModeType);
		PlayerSpawnPoint randomPlayerSpawnPoint = MonoBehaviourSingleton<SpawnManager>.Instance.GetRandomPlayerSpawnPoint();
		Debug.Log($"[BaseServerGameModeCallbacks] Spawning new player at {randomPlayerSpawnPoint.Position}");
		BoltEntity boltEntity = BoltNetwork.Instantiate(playerPrefabId, token, randomPlayerSpawnPoint.Position, randomPlayerSpawnPoint.Rotation);
		IPlayerState state = boltEntity.GetState<IPlayerState>();
		state.GameModeType = (int)gameModeType;
		state.DisplayName = displayName;
		state.Loadouts[0].FromLoadoutData(loadoutData);
		float num = state.Loadouts[0].TryGetModifiedHealth(_profileManager);
		state.Damageable.MaxHealth = num;
		state.Damageable.Health = num;
		state.Damageable.DamageReduction = 0f;
		state.InputEnabled = true;
		state.IsSecondLife = isSecondLife;
		return boltEntity;
	}

	public override void Disconnected(BoltConnection connection)
	{
		if (EntitiesByConnection.TryGetValue(connection, out var value))
		{
			OnPlayerDisconnected(value.Entity, value.Profile);
			if (PlayfabServerManagement.IsInitializedAndReady)
			{
				PlayfabServerManagement.PlayerDisconnected(value.Profile.Id);
			}
			EntitiesByConnection.Remove(connection);
			BoltNetwork.Destroy(value.Entity.gameObject);
		}
	}

	public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
	{
		Debug.Log($"[BaseServerGameModeCallbacks] Bolt Shutting Down - {disconnectReason}");
	}
}
