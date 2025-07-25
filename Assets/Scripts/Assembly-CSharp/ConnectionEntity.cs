using BSCore;

public struct ConnectionEntity
{
	public BoltEntity Entity;

	public PlayerProfile Profile;

	public ConnectionEntity(BoltEntity boltEntity, PlayerProfile playerProfile)
	{
		Entity = boltEntity;
		Profile = playerProfile;
	}
}
