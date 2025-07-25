using UnityEngine;

public static class IPlayerHitCommandInputExtensions
{
	public static string ToStr(this IPlayerHitCommandInput input)
	{
		string text = ((input.Victim != null) ? input.Victim.name : "null");
		return $"[IPlayerHitCommandInput] Point={input.Point}, Origin={input.Origin}, Forward={input.Forward}, Victim={text}, " + $"HitServerFrame={input.HitServerFrame}, LaunchServerFrame={input.LaunchServerFrame}, HitType={(HitType)input.HitType}, " + $"WeaponId={input.WeaponId}, ChargeTime={input.ChargeTime}";
	}

	public static HitInfo ToHitInfo(this IPlayerHitCommandInput input)
	{
		return new HitInfo
		{
			hitType = (HitType)input.HitType,
			directHit = (input.Victim != null),
			launchServerFrame = input.LaunchServerFrame,
			hitServerFrame = input.HitServerFrame,
			origin = input.Origin,
			forward = input.Forward,
			forwardOnHit = input.ForwardOnHit,
			normal = input.Normal,
			projectileRotation = input.ProjectileRotation,
			point = input.Point,
			distance = Vector3.Distance(input.Origin, input.Point),
			collider = ((input.Victim != null) ? input.Victim.GetComponent<IDamageable>().HurtCollider : null),
			weaponId = input.WeaponId,
			chargeTime = input.ChargeTime,
			hitEntity = input.Victim,
			isMelee = input.IsMelee,
			meleeDirection = (MeleeWeapon.Direction)input.MeleeDirection
		};
	}

	public static IPlayerHitCommandInput ToHitCommandInput(this HitInfo hit)
	{
		IPlayerHitCommandInput playerHitCommandInput = PlayerHitCommand.Create();
		playerHitCommandInput.HitType = (int)hit.hitType;
		playerHitCommandInput.Origin = hit.origin;
		playerHitCommandInput.Point = hit.point;
		playerHitCommandInput.Forward = hit.forward;
		playerHitCommandInput.ForwardOnHit = hit.forwardOnHit;
		playerHitCommandInput.Normal = hit.normal;
		playerHitCommandInput.ProjectileRotation = hit.projectileRotation;
		playerHitCommandInput.LaunchServerFrame = hit.launchServerFrame;
		playerHitCommandInput.HitServerFrame = hit.hitServerFrame;
		playerHitCommandInput.Victim = hit.hitEntity;
		if (hit.collider != null)
		{
			IDamageable componentInParent = hit.collider.gameObject.GetComponentInParent<IDamageable>();
			if (componentInParent != null)
			{
				playerHitCommandInput.Victim = componentInParent.entity;
			}
		}
		playerHitCommandInput.WeaponId = hit.weaponId;
		playerHitCommandInput.ChargeTime = hit.chargeTime;
		playerHitCommandInput.IsMelee = hit.isMelee;
		playerHitCommandInput.MeleeDirection = (int)hit.meleeDirection;
		return playerHitCommandInput;
	}

	public static bool IsValidInput(this IPlayerHitCommandInput input, Outfit outfit)
	{
		return true;
	}

	public static bool IsMelee(this IPlayerHitCommandInput input)
	{
		return ((HitType)input.HitType).IsMelee();
	}

	public static bool IsProjectile(this IPlayerHitCommandInput input)
	{
		return ((HitType)input.HitType).IsProjectile();
	}

	public static bool IsRaycast(this IPlayerHitCommandInput input)
	{
		return ((HitType)input.HitType).IsRaycast();
	}

	public static bool IsContinuous(this IPlayerHitCommandInput input)
	{
		return ((HitType)input.HitType).IsContinuous();
	}
}
