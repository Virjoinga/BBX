public static class BoltEntityExtensions
{
	public static bool IsControllerOnly(this BoltEntity entity)
	{
		if (entity.isControlled)
		{
			return !entity.isOwner;
		}
		return false;
	}
}
