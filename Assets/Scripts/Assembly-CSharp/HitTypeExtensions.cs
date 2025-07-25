public static class HitTypeExtensions
{
	public static bool IsProjectile(this HitType hitType)
	{
		return hitType == HitType.Projectile;
	}

	public static bool IsRaycast(this HitType hitType)
	{
		return hitType == HitType.Raycast;
	}

	public static bool IsContinuous(this HitType hitType)
	{
		return hitType == HitType.Continuous;
	}

	public static bool IsMelee(this HitType hitType)
	{
		return hitType == HitType.Melee;
	}
}
