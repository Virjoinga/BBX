public class FollowLaunchPointExplosiveProjectile : ExplosiveProjectile
{
	protected override void FixedUpdate()
	{
		base.transform.position = _details.launchPointTransform.position;
		base.FixedUpdate();
	}
}
