public struct PlayerRequestedRespawnSignal
{
	public BoltEntity PlayerEntity;

	public PlayerRequestedRespawnSignal(BoltEntity playerEntity)
	{
		PlayerEntity = playerEntity;
	}
}
