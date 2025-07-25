using System.Collections.Generic;
using System.Linq;

public struct BoltHit
{
	public BoltHitboxBody Body { get; private set; }

	public List<BoltHitbox> HitBoxes { get; private set; }

	public BoltHitbox ClosestHitbox { get; private set; }

	public float Distance { get; private set; }

	public IDamageable Damageable => Body.GetComponent<IDamageable>();

	public bool IncludesHeadshot { get; private set; }

	public BoltHit(BoltPhysicsHit hit)
	{
		Body = hit.body;
		HitBoxes = new List<BoltHitbox> { hit.hitbox };
		ClosestHitbox = hit.hitbox;
		Distance = hit.distance;
		IncludesHeadshot = HitBoxes.Any((BoltHitbox hb) => hb.hitboxType == BoltHitboxType.Head);
	}

	public void Add(BoltPhysicsHit hit)
	{
		HitBoxes.Add(hit.hitbox);
		if (hit.distance < Distance)
		{
			ClosestHitbox = hit.hitbox;
			Distance = hit.distance;
		}
		IncludesHeadshot = HitBoxes.Any((BoltHitbox hb) => hb.hitboxType == BoltHitboxType.Head);
	}
}
