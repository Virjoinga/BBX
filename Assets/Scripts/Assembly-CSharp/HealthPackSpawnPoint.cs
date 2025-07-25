using UnityEngine;

public class HealthPackSpawnPoint : PlayerSpawnPoint
{
	[SerializeField]
	private HealthPackType _healthPackType;

	public HealthPackType HealthPackType => _healthPackType;
}
