using System.Collections;
using Bolt;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Host)]
public class ServerPickupCallbacks : GlobalEventListener
{
	private PickupConfig _pickupConfig;

	private IEnumerator Start()
	{
		_pickupConfig = SceneContextHelper.ResolveZenjectBinding<PickupConfig>();
		Debug.Log("[ServerPickupCallbacks] Start");
		yield return new WaitUntil(() => MonoBehaviourSingleton<SpawnManager>.IsInstantiated && _pickupConfig.IsInstantiated);
		Debug.Log("[ServerPickupCallbacks] SpawnManager and PickupConfig instantiated");
		SpawnPickups();
	}

	private void SpawnPickups()
	{
		Debug.Log("[ServerPickupCallbacks] Spawning pickups");
		foreach (PickupSpawnPoint pickupSpawnPoint in MonoBehaviourSingleton<SpawnManager>.Instance.PickupSpawnPoints)
		{
			switch (pickupSpawnPoint.PickupType)
			{
			case PickupType.RandomPowerup:
				CratePickupEntity.SpawnPickup(pickupSpawnPoint, 1, _pickupConfig.GetRandomType());
				break;
			case PickupType.HealthSmall:
			case PickupType.HealthLarge:
				HealthPickupEntity.SpawnPickup(pickupSpawnPoint, pickupSpawnPoint.PickupType);
				break;
			case PickupType.SpeedBoost:
			case PickupType.DamageBoost:
			case PickupType.DamageShield:
				PowerupPickupEntity.SpawnPickup(pickupSpawnPoint, pickupSpawnPoint.PickupType);
				break;
			default:
				Debug.LogError($"[ServerPickupCallbacks] No case for spawning pickup of type {pickupSpawnPoint.PickupType}");
				break;
			}
		}
	}
}
