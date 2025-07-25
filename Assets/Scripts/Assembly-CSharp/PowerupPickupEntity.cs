public class PowerupPickupEntity : CollectOnWalkOverPickupEntity<IPowerupPickupState>
{
	public static void SpawnPickup(PickupSpawnPoint spawnPoint, PickupType pickupType)
	{
		BasePickupEntity<IPowerupPickupState>.SpawnPickup(BoltPrefabs.PowerupPickupEntity, spawnPoint, pickupType);
	}
}
