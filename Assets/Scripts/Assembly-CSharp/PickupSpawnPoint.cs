using UnityEngine;

public class PickupSpawnPoint : SpawnPoint
{
	[SerializeField]
	private bool _isIndoors;

	[SerializeField]
	private PickupType _pickupType = PickupType.RandomPowerup;

	public bool IsIndoors => _isIndoors;

	public PickupType PickupType => _pickupType;
}
