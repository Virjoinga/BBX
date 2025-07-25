public class HealthPickupEntity : CollectOnWalkOverPickupEntity<IHealthPickupState>
{
	public static void SpawnPickup(PickupSpawnPoint spawnPoint, PickupType pickupType)
	{
		BasePickupEntity<IHealthPickupState>.SpawnPickup(BoltPrefabs.HealthPickupEntity, spawnPoint, pickupType);
	}
}
