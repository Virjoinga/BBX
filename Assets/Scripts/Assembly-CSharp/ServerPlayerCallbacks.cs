using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Host, new string[] { "OfflineMovementTest" })]
public class ServerPlayerCallbacks : GlobalEventListener
{
	private Dictionary<BoltConnection, BoltEntity> _entitiesByConnection = new Dictionary<BoltConnection, BoltEntity>();

	public override void BoltStartDone()
	{
		StartCoroutine(CreateDummiesWhenSpawnManagerReady());
	}

	public override void Connected(BoltConnection connection)
	{
		Debug.Log("[ConnectionManager] Connection established, creating player for it");
		StartCoroutine(CreatePlayerWhenSpawnManagerReady(connection));
	}

	private IEnumerator CreateDummiesWhenSpawnManagerReady()
	{
		yield return new WaitUntil(() => MonoBehaviourSingleton<SpawnManager>.Instance != null);
		for (int num = 0; num < BoltTestInitializer.Instance.Dummies; num++)
		{
			CreatePlayerEntity((num % 2 == 0) ? "oliver" : "riggs");
		}
	}

	private IEnumerator CreatePlayerWhenSpawnManagerReady(BoltConnection connection)
	{
		yield return new WaitUntil(() => MonoBehaviourSingleton<SpawnManager>.Instance != null);
		BoltEntity boltEntity = CreatePlayerEntity("riggs");
		boltEntity.AssignControl(connection);
		_entitiesByConnection.Add(connection, boltEntity);
	}

	private BoltEntity CreatePlayerEntity(string character)
	{
		PlayerSpawnPoint randomPlayerSpawnPoint = MonoBehaviourSingleton<SpawnManager>.Instance.GetRandomPlayerSpawnPoint();
		Debug.Log($"[ServerPlayerCallbacks] Spawning new player at {randomPlayerSpawnPoint.Position}");
		BoltEntity boltEntity = BoltNetwork.Instantiate(BoltPrefabs.Player, randomPlayerSpawnPoint.Position, randomPlayerSpawnPoint.Rotation);
		IPlayerState state = boltEntity.GetState<IPlayerState>();
		state.Loadouts[0].Outfit = character;
		state.Damageable.MaxHealth = 200f;
		state.Damageable.Health = 200f;
		state.InputEnabled = true;
		return boltEntity;
	}

	public override void Disconnected(BoltConnection connection)
	{
		if (_entitiesByConnection.TryGetValue(connection, out var value))
		{
			BoltNetwork.Destroy(value.gameObject);
		}
	}
}
