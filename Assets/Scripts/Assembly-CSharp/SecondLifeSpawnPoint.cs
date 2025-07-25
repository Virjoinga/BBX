using UnityEngine;

public class SecondLifeSpawnPoint : PlayerSpawnPoint
{
	[SerializeField]
	private BRZoneEntity _zone;

	[SerializeField]
	private BRZoneEntity _otherZone;

	public BRZoneEntity Zone => _zone;

	public BRZoneEntity OtherZone => _otherZone;
}
