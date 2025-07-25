using UnityEngine;

public class AmmoClipPickupEntity : BasePickupEntity<IAmmoClipPickupState>
{
	public static void SpawnPickup(Vector3 position)
	{
		BasePickupEntity<IAmmoClipPickupState>.SpawnPickup(BoltPrefabs.AmmoClipPickupEntity, position, Quaternion.identity, PickupType.AmmoClip);
	}
}
