public struct UpdateRespawnPointSignal
{
	public BoltEntity PlayerEntity;

	public string SpawnPointId;

	public string LoadoutHeroClass;

	public UpdateRespawnPointSignal(BoltEntity playerEntity, string spawnPointId, string loadoutHeroClass)
	{
		PlayerEntity = playerEntity;
		SpawnPointId = spawnPointId;
		LoadoutHeroClass = loadoutHeroClass;
	}
}
