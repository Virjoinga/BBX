using UnityEngine;

public static class IPlayermotorExtensions
{
	public static bool HasCollidedAbove(this CollisionFlags flags)
	{
		return (flags & CollisionFlags.Above & CollisionFlags.Above) != 0;
	}

	public static bool HasCollidedBelow(this CollisionFlags flags)
	{
		return (flags & CollisionFlags.Below & CollisionFlags.Below) != 0;
	}
}
