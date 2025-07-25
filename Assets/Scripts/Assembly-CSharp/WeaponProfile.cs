using System;
using BSCore;
using BSCore.Constants.Config;
using Bolt;
using Constants;
using UnityEngine;

public class WeaponProfile : ProfileWithHeroClass
{
	[Serializable]
	public class WeaponProfileData : ProfileWithHeroClassData
	{
		[Serializable]
		public class MeleeOptionProfileData
		{
			public float aMult;

			public float mDelay;

			public float mDur;

			public float mDist;

			public float mSpeed;

			public bool liftWeapon;

			public float dMinHeight;

			public float damage;

			public float dDelay;

			public float dActive;

			public bool dDuringFall;

			public float recovery;

			public float cooldown;

			public KnockbackProfileData knockback;

			public EffectProfileData effect;
		}

		[Serializable]
		public class MeleeProfileData
		{
			public MeleeOptionProfileData standing;

			public MeleeOptionProfileData forward;

			public MeleeOptionProfileData backward;

			public MeleeOptionProfileData left;

			public MeleeOptionProfileData right;

			public MeleeOptionProfileData downward;
		}

		[Serializable]
		public class BoosterProfileData
		{
			public float ammo;

			public float damage;

			public float explosion;

			public float melee;
		}

		[Serializable]
		public class CharacterMultiplierProfileData
		{
			public float speed;

			public float health;

			public float specialCooldown;
		}

		[Serializable]
		public class AmmoRechargeProfileData
		{
			public float delay;

			public int recoveryRate;
		}

		[Serializable]
		public class ChargeProfileData
		{
			public float time;

			public float damageAtMax;

			public float projectileSpeedAtMax;

			public float rangeAtMax;

			public bool singleAmmoCharge;
		}

		[Serializable]
		public class EffectProfileData
		{
			public string effectType;

			public string statusType;

			public float chance;

			public float delay;

			public float duration;

			public float minSpin;

			public float maxSpin;

			public float modifier;

			public float range;

			public float spinDown;

			public float spinUp;

			public bool inverseForAlly;

			public bool reducedByTenacity;
		}

		[Serializable]
		public class ExplosionProfileData
		{
			public float minDamage;

			public float maxDamage;

			public float range;

			public float maxDamageRange;

			public bool closestOnly;

			public bool ignoreSelf;
		}

		[Serializable]
		public class LockingProfileData
		{
			public float angle;

			public float distance;

			public float radius;
		}

		[Serializable]
		public class ScoreModifierProfileData
		{
			[Serializable]
			public class DamageData
			{
				public float min;

				public float max;
			}

			[Serializable]
			public class CooldownData
			{
				public float min;

				public float max;
			}

			public DamageData damage;

			public CooldownData cooldown;

			public ExplosionProfileData explosion;
		}

		[Serializable]
		public class ZoomProfileData
		{
			public float max;

			public float speed;
		}

		[Serializable]
		public class SpreadProfileData
		{
			public bool isAoE;

			public float amount;

			public float falloffRange;

			public float falloffDamage;

			public float min;

			public int shotsToMax;

			public float spreadLossDelay;

			public float spreadRecoveryTime;
		}

		[Serializable]
		public class SpawnedEntityProfileData
		{
			public int maxDeployed;

			public int health;

			public float duration;

			public bool doesDamageOnHit;

			public bool spawnOnDirectHit;

			public bool spawnOnFire;
		}

		[Serializable]
		public class CriticalProfileData
		{
			public float extraDamage;
		}

		[Serializable]
		public class ReloadProfileData
		{
			public float time;

			public bool waitForCooldown;

			public bool inBackground;

			public float delay;
		}

		[Serializable]
		public class KnockbackProfileData
		{
			public float force;

			public bool breaksMelee;
		}

		[Serializable]
		public class SpinupProfileData
		{
			public float min;

			public float timeToFull;
		}

		[Serializable]
		public class ProjectileProfileData
		{
			public float speed;

			public float turnSpeed;

			public float travelTime;

			public string movement;

			public float arcMultiplier;
		}

		[Serializable]
		public class EquipmentProfileOverrideData
		{
			public float weaponDamageMajorEffectPercentage;

			public float weaponDamageMinorEffectPercentage;
		}

		public string animationType;

		public string meleeType;

		public string weaponType;

		public MeleeProfileData melee;

		public BoosterProfileData booster;

		public CharacterMultiplierProfileData characterMultiplier;

		public int clipSize;

		public float cooldown;

		public float reloadDelay;

		public ChargeProfileData charge;

		public AmmoRechargeProfileData ammoRecharge;

		public float damage;

		public EffectProfileData[] effects;

		public EffectProfileData[] selfEffects;

		public ExplosionProfileData explosion;

		public LockingProfileData locking;

		public float radiation;

		public float range;

		public float reloadTime;

		public ScoreModifierProfileData scoreModifier;

		public SpreadProfileData spread;

		public ZoomProfileData zoom;

		public int volleySize;

		public float volleyDelay;

		public SpawnedEntityProfileData spawnedEntity;

		public string crosshair;

		public bool hideShotPathBlockedIcon;

		public CriticalProfileData critical;

		public float damageInterval;

		public ReloadProfileData reload;

		public KnockbackProfileData knockback;

		public SpinupProfileData spinup;

		public ProjectileProfileData projectile;

		public EquipmentProfileOverrideData equipmentOverrides;
	}

	public class MeleeOptionData
	{
		public float AnimationSpeedMultiplier { get; private set; }

		public float MovementDelay { get; private set; }

		public float MovementDuration { get; private set; }

		public int MovementDurationFrames { get; private set; }

		public float MovementDistance { get; private set; }

		public float MovementSpeed { get; private set; }

		public MeleeWeapon.Direction MovementDirection { get; private set; }

		public bool LiftWeapon { get; private set; }

		public float DownwardAttackLoopMinHeight { get; private set; }

		public PlayerAnimationController.Parameter AnimParam { get; private set; }

		public bool UntilGrounded { get; private set; }

		public float Damage { get; private set; }

		public float DamageDelay { get; private set; }

		public float DamageActive { get; private set; }

		public bool DealDamageDuringDownwardFall { get; private set; }

		public float Recovery { get; private set; }

		public float Cooldown { get; private set; }

		public int CooldownFrames { get; private set; }

		public KnockbackData Knockback { get; private set; }

		public EffectData Effect { get; private set; }

		public MeleeOptionData(WeaponProfileData.MeleeOptionProfileData data, MeleeWeapon.Direction direction, PlayerAnimationController.Parameter animParam, float frameDeltaTime)
		{
			AnimationSpeedMultiplier = ((data.aMult > 0f) ? data.aMult : 1f);
			MovementDelay = data.mDelay;
			MovementDuration = data.mDur;
			MovementDurationFrames = Mathf.RoundToInt(MovementDuration / frameDeltaTime);
			MovementDistance = data.mDist;
			if (data.mSpeed > 0f)
			{
				MovementSpeed = data.mSpeed;
			}
			else
			{
				MovementSpeed = MovementDistance / MovementDuration;
			}
			MovementDirection = direction;
			LiftWeapon = data.liftWeapon;
			DownwardAttackLoopMinHeight = data.dMinHeight;
			Damage = data.damage;
			DamageDelay = data.dDelay;
			DamageActive = data.dActive;
			DealDamageDuringDownwardFall = data.dDuringFall;
			Recovery = data.recovery;
			Cooldown = data.cooldown;
			CooldownFrames = Mathf.RoundToInt(Cooldown / frameDeltaTime);
			AnimParam = animParam;
			UntilGrounded = MovementDuration == 0f;
			if (data.knockback != null)
			{
				data.knockback.force = Mathf.Max(7f, data.knockback.force);
			}
			Knockback = new KnockbackData(data.knockback);
			Effect = new EffectData(data.effect);
		}

		public Vector3 GetVelocity(Transform transform)
		{
			switch (MovementDirection)
			{
			case MeleeWeapon.Direction.Forward:
				return transform.forward * MovementSpeed;
			case MeleeWeapon.Direction.Backward:
				return -transform.forward * MovementSpeed;
			case MeleeWeapon.Direction.Left:
				return -transform.right * MovementSpeed;
			case MeleeWeapon.Direction.Right:
				return transform.right * MovementSpeed;
			case MeleeWeapon.Direction.Downward:
				return -transform.up * MovementSpeed;
			default:
				return Vector3.zero;
			}
		}

		public Vector2 GetInput(float horizontal, float vertical)
		{
			Vector2 result = new Vector2(0f, 0f);
			if (horizontal > 0.1f)
			{
				result.x = 1f;
			}
			else if (horizontal < -0.1f)
			{
				result.x = -1f;
			}
			if (vertical > 0.1f)
			{
				result.y = 1f;
			}
			else if (vertical < -0.1f)
			{
				result.y = -1f;
			}
			return result;
		}
	}

	public class MeleeData
	{
		public MeleeOptionData Standing { get; private set; }

		public MeleeOptionData Forward { get; private set; }

		public MeleeOptionData Backward { get; private set; }

		public MeleeOptionData Left { get; private set; }

		public MeleeOptionData Right { get; private set; }

		public MeleeOptionData Downward { get; private set; }

		public MeleeData(WeaponProfileData.MeleeProfileData data, float frameDeltaTime)
		{
			if (data.standing != null)
			{
				Standing = new MeleeOptionData(data.standing, MeleeWeapon.Direction.Standing, PlayerAnimationController.Parameter.UseMeleeStanding, frameDeltaTime);
				Forward = new MeleeOptionData(data.forward, MeleeWeapon.Direction.Forward, PlayerAnimationController.Parameter.UseMeleeForward, frameDeltaTime);
				Backward = new MeleeOptionData(data.backward, MeleeWeapon.Direction.Backward, PlayerAnimationController.Parameter.UseMeleeBackward, frameDeltaTime);
				Left = new MeleeOptionData(data.left, MeleeWeapon.Direction.Left, PlayerAnimationController.Parameter.UseMeleeLeft, frameDeltaTime);
				Right = new MeleeOptionData(data.right, MeleeWeapon.Direction.Right, PlayerAnimationController.Parameter.UseMeleeRight, frameDeltaTime);
				Downward = new MeleeOptionData(data.downward, MeleeWeapon.Direction.Downward, PlayerAnimationController.Parameter.UseMeleeDownward, frameDeltaTime);
			}
		}

		public MeleeOptionData GetOptionByDirection(MeleeWeapon.Direction direction)
		{
			switch (direction)
			{
			case MeleeWeapon.Direction.Forward:
				return Forward;
			case MeleeWeapon.Direction.Backward:
				return Backward;
			case MeleeWeapon.Direction.Left:
				return Left;
			case MeleeWeapon.Direction.Right:
				return Right;
			case MeleeWeapon.Direction.Downward:
				return Downward;
			default:
				return Standing;
			}
		}
	}

	public class BoosterData
	{
		public float Ammo { get; private set; }

		public float Damage { get; private set; }

		public float Explosion { get; private set; }

		public float Melee { get; private set; }

		public BoosterData(WeaponProfileData.BoosterProfileData data)
		{
			Ammo = data.ammo;
			Damage = data.damage;
			Explosion = data.explosion;
			Melee = data.melee;
		}
	}

	public class CharacterMultiplierData
	{
		public float Speed { get; private set; }

		public float Health { get; private set; }

		public float SpecialCooldown { get; private set; }

		public CharacterMultiplierData(WeaponProfileData.CharacterMultiplierProfileData data)
		{
			Speed = data.speed;
			Health = data.health;
			SpecialCooldown = data.specialCooldown;
		}
	}

	public class ChargeData
	{
		public float Time { get; private set; }

		public float ProjectileSpeedAtMax { get; private set; }

		public float RangeAtMax { get; private set; }

		public float AmmoIncrementTime { get; private set; }

		public bool CanCharge { get; private set; }

		public float DamageAtMax { get; private set; }

		public bool SingleAmmoCharge { get; private set; }

		public ChargeData(WeaponProfileData.ChargeProfileData data, int clipSize)
		{
			Time = data.time;
			ProjectileSpeedAtMax = data.projectileSpeedAtMax;
			RangeAtMax = data.rangeAtMax;
			AmmoIncrementTime = Time / (float)(clipSize - 1);
			CanCharge = Time > 0f;
			DamageAtMax = data.damageAtMax;
			SingleAmmoCharge = data.singleAmmoCharge;
		}

		public override string ToString()
		{
			return $"[ChargeData] (CanCharge: {CanCharge}, Time: {Time}, ProjectileSpeedAtMax: {ProjectileSpeedAtMax}, RangeAtMax: {RangeAtMax}, AmmoIncrementTime: {AmmoIncrementTime}, DamageAtMax: {DamageAtMax})";
		}
	}

	public class AmmoRechargeData
	{
		public float Delay { get; private set; }

		public int RecoveryRate { get; private set; }

		public bool CanRechargeAmmo { get; private set; }

		public AmmoRechargeData(WeaponProfileData.AmmoRechargeProfileData data)
		{
			Delay = data.delay;
			RecoveryRate = data.recoveryRate;
			CanRechargeAmmo = RecoveryRate > 0;
		}
	}

	[Serializable]
	public class EffectData
	{
		public Match.EffectType EffectType { get; private set; }

		public Match.StatusType StatusType { get; private set; }

		public float Chance { get; private set; }

		public float Delay { get; private set; }

		public float Duration { get; private set; }

		public float MinSpin { get; private set; }

		public float MaxSpin { get; private set; }

		public float Modifier { get; private set; }

		public float Range { get; private set; }

		public float SpinDown { get; private set; }

		public float SpinUp { get; private set; }

		public bool InverseForAlly { get; private set; }

		public bool ReducedByTenacity { get; private set; }

		public EffectData(WeaponProfileData.EffectProfileData data)
		{
			if (data != null)
			{
				if (!string.IsNullOrEmpty(data.effectType) && Enum<Match.EffectType>.TryParse(data.effectType, out var value))
				{
					EffectType = value;
				}
				if (!string.IsNullOrEmpty(data.statusType) && Enum<Match.StatusType>.TryParse(data.statusType, out var value2))
				{
					StatusType = value2;
				}
				Chance = data.chance;
				Delay = data.delay;
				Duration = data.duration;
				MinSpin = data.minSpin;
				MaxSpin = data.maxSpin;
				Modifier = data.modifier;
				Range = data.range;
				SpinDown = data.spinDown;
				SpinUp = data.spinUp;
				InverseForAlly = data.inverseForAlly;
				ReducedByTenacity = data.reducedByTenacity;
			}
		}
	}

	public class ExplosionData
	{
		public bool Explodes { get; private set; }

		public float MinDamage { get; private set; }

		public float MaxDamage { get; private set; }

		public float Range { get; private set; }

		public float MaxDamageRange { get; private set; }

		public bool ClosestOnly { get; private set; }

		public bool IgnoreSelf { get; private set; }

		public ExplosionData(WeaponProfileData.ExplosionProfileData data)
		{
			if (!(data.range <= 0f))
			{
				MinDamage = data.minDamage;
				MaxDamage = data.maxDamage;
				Range = data.range;
				MaxDamageRange = data.maxDamageRange;
				Explodes = true;
				ClosestOnly = data.closestOnly;
				IgnoreSelf = data.ignoreSelf;
			}
		}
	}

	public class LockingData
	{
		public bool CanLock { get; private set; }

		public float Angle { get; private set; }

		public float Distance { get; private set; }

		public float Radius { get; private set; }

		public LockingData(WeaponProfileData.LockingProfileData data)
		{
			if (!(data.angle <= 0f))
			{
				Angle = data.angle;
				Distance = data.distance;
				Radius = data.radius;
				CanLock = true;
			}
		}
	}

	public class DamageData
	{
		public float Min { get; private set; }

		public float Max { get; private set; }

		public DamageData(float min, float max)
		{
			Min = min;
			Max = max;
		}
	}

	public class CooldownData
	{
		public float Min { get; private set; }

		public float Max { get; private set; }

		public CooldownData(float min, float max)
		{
			Min = min;
			Max = max;
		}
	}

	public class ScoreModifierData
	{
		public DamageData Damage { get; private set; }

		public CooldownData Cooldown { get; private set; }

		public ExplosionData Explosion { get; private set; }

		public ScoreModifierData(WeaponProfileData.ScoreModifierProfileData data)
		{
			if (data != null)
			{
				if (data.damage != null)
				{
					Damage = new DamageData(data.damage.min, data.damage.max);
				}
				if (data.cooldown != null)
				{
					Cooldown = new CooldownData(data.cooldown.min, data.cooldown.max);
				}
				if (data.explosion != null)
				{
					Explosion = new ExplosionData(data.explosion);
				}
			}
		}
	}

	public class ZoomData
	{
		public bool CanZoom { get; private set; }

		public float Max { get; private set; } = 2f;

		public ZoomData(WeaponProfileData.ZoomProfileData data)
		{
			if (data.max > 0f)
			{
				Max = data.max;
			}
			CanZoom = true;
		}
	}

	public class SpreadData
	{
		public bool IsAoE { get; private set; }

		public float Amount { get; private set; }

		public float Min { get; private set; }

		public float SpreadIncreasePerFire { get; private set; }

		public float SpreadLossPerSecond { get; private set; }

		public float SpreadLossDelay { get; private set; }

		public float FalloffRange { get; private set; }

		public float FalloffDamage { get; private set; }

		public bool IsSpread { get; private set; }

		public bool IsVariableSpread { get; private set; }

		public SpreadData(WeaponProfileData.SpreadProfileData data, float cooldown)
		{
			if (data != null)
			{
				IsAoE = data.isAoE;
				Amount = data.amount;
				Min = data.min;
				if (data.shotsToMax > 0)
				{
					IsVariableSpread = true;
					SpreadIncreasePerFire = (Amount - Min) / (float)data.shotsToMax;
					SpreadLossPerSecond = (Amount - Min) / data.spreadRecoveryTime;
					SpreadLossDelay = data.spreadLossDelay;
				}
				FalloffRange = data.falloffRange;
				FalloffDamage = data.falloffDamage;
				IsSpread = Amount > 0f;
			}
		}
	}

	public class SpawnedEntityData
	{
		public bool SpawnsEntity { get; private set; }

		public float Duration { get; private set; }

		public bool DoesDamageOnHit { get; private set; }

		public bool SpawnOnDirectHit { get; private set; }

		public bool SpawnOnFire { get; private set; }

		public bool IsDeployable { get; private set; }

		public int MaxDeployedCount { get; private set; }

		public int Health { get; private set; }

		public SpawnedEntityData(WeaponProfileData.SpawnedEntityProfileData data)
		{
			Duration = data.duration;
			DoesDamageOnHit = data.doesDamageOnHit;
			SpawnOnDirectHit = data.spawnOnDirectHit;
			SpawnOnFire = data.spawnOnFire;
			SpawnsEntity = Duration > 0f;
			MaxDeployedCount = data.maxDeployed;
			IsDeployable = MaxDeployedCount > 0;
			Health = ((data.health <= 0) ? 1 : data.health);
		}
	}

	public class CriticalData
	{
		public bool CanCritical { get; private set; }

		public float ExtraDamage { get; private set; }

		public CriticalData(WeaponProfileData.CriticalProfileData data)
		{
			ExtraDamage = data.extraDamage;
			CanCritical = ExtraDamage > 0f;
		}
	}

	public class ReloadData
	{
		public float Time { get; private set; }

		public int TimeFrames { get; private set; }

		public float Delay { get; private set; }

		public int DelayFrames { get; private set; }

		public bool WaitForCooldown { get; private set; }

		public bool InBackground { get; private set; }

		public ReloadData(WeaponProfileData.ReloadProfileData data, float cooldown, float secondsPerFrame)
		{
			Time = data.time;
			TimeFrames = Mathf.RoundToInt(Time / secondsPerFrame);
			WaitForCooldown = data.waitForCooldown;
			InBackground = data.inBackground;
			if (WaitForCooldown)
			{
				Delay = cooldown;
			}
			else
			{
				Delay = ((data.delay > 0f) ? data.delay : 0.5f);
			}
			DelayFrames = Mathf.RoundToInt(Delay / secondsPerFrame);
		}
	}

	public class KnockbackData
	{
		public float Force { get; private set; }

		public bool BreaksMelee { get; private set; }

		public bool HasKnockback { get; private set; }

		public KnockbackData(WeaponProfileData.KnockbackProfileData data)
		{
			if (data == null)
			{
				HasKnockback = false;
				return;
			}
			Force = data.force;
			BreaksMelee = data.breaksMelee;
			HasKnockback = Force > 0f || BreaksMelee;
		}
	}

	public class SpinupData
	{
		public bool SpinsUp { get; private set; }

		public float Min { get; private set; }

		public int MinFrames { get; private set; }

		public float TimeToFull { get; private set; }

		public int FramesToFull { get; private set; }

		public SpinupData(WeaponProfileData.SpinupProfileData data, float frameDeltaTime)
		{
			if (data != null && data.timeToFull > 0f)
			{
				Debug.Log($"[WeaponProfile] Setting up Spinup data. frameDeltaTime: {frameDeltaTime}");
				Min = data.min;
				MinFrames = Mathf.RoundToInt(Min / frameDeltaTime);
				TimeToFull = data.timeToFull;
				FramesToFull = Mathf.RoundToInt(TimeToFull / frameDeltaTime);
				SpinsUp = true;
			}
		}
	}

	public class ProjectileData
	{
		public float Speed { get; private set; }

		public float TurnSpeed { get; private set; }

		public float TravelTime { get; private set; }

		public Projectile.MovementType MovementType { get; private set; }

		public float ArcMultiplier { get; private set; }

		public ProjectileData(WeaponProfileData.ProjectileProfileData data)
		{
			Speed = data.speed;
			TurnSpeed = data.turnSpeed;
			TravelTime = data.travelTime;
			if (Enum<global::Projectile.MovementType>.TryParse(data.movement, out var value))
			{
				MovementType = value;
			}
			else
			{
				MovementType = global::Projectile.MovementType.Linear;
			}
			ArcMultiplier = ((data.arcMultiplier > 0f) ? data.arcMultiplier : 1f);
		}
	}

	public class EquipmentOverideData
	{
		public float WeaponDamageMajorEffectPercentage { get; private set; }

		public float WeaponDamageMinorEffectPercentage { get; private set; }

		public EquipmentOverideData(WeaponProfileData.EquipmentProfileOverrideData data)
		{
			WeaponDamageMajorEffectPercentage = data.weaponDamageMajorEffectPercentage;
			WeaponDamageMinorEffectPercentage = data.weaponDamageMinorEffectPercentage;
		}
	}

	private float _selfDamageModifier = 0.5f;

	public GameItem GameItem { get; private set; }

	public string AnimationType { get; private set; }

	public string MeleeType { get; private set; }

	public string WeaponType { get; private set; }

	public MeleeData Melee { get; private set; }

	public BoosterData Booster { get; private set; }

	public CharacterMultiplierData CharacterMultiplier { get; private set; }

	public int ClipSize { get; private set; }

	public float Cooldown { get; private set; }

	public int CooldownFrames { get; private set; }

	public ChargeData Charge { get; private set; }

	public AmmoRechargeData AmmoRecharge { get; private set; }

	public float Damage { get; private set; }

	public EffectData[] Effects { get; private set; } = new EffectData[0];

	public EffectData[] SelfEffects { get; private set; } = new EffectData[0];

	public ExplosionData Explosion { get; private set; }

	public LockingData Locking { get; private set; }

	public float Radiation { get; private set; }

	public float Range { get; private set; }

	public ScoreModifierData ScoreModifier { get; private set; }

	public SpreadData Spread { get; private set; }

	public ZoomData Zoom { get; private set; }

	public bool CanZoom { get; private set; }

	public bool IsMelee { get; private set; }

	public int VolleySize { get; private set; } = 1;

	public float VolleyDelay { get; private set; }

	public SpawnedEntityData SpawnedEntity { get; private set; }

	public Match.CrosshairType Crosshair { get; private set; } = Match.CrosshairType.Simple;

	public bool HideShotPathBlockedIcon { get; private set; }

	public CriticalData Critical { get; private set; }

	public float DamageInterval { get; private set; }

	public ReloadData Reload { get; private set; }

	public KnockbackData Knockback { get; private set; }

	public SpinupData Spinup { get; private set; }

	public ProjectileData Projectile { get; private set; }

	public EquipmentOverideData EquipmentOverrides { get; private set; }

	public WeaponProfile(GameItem gameItem, ConfigManager configManager)
		: base(gameItem)
	{
		GameItem = gameItem;
		if (configManager != null)
		{
			_selfDamageModifier = configManager.Get(DataKeys.selfDamageModifier, 0.5f);
		}
	}

	protected override void DeserializeData(string json)
	{
		_profileData = BaseProfileData.FromJson<WeaponProfileData>(json);
	}

	protected override void ParseCustomData()
	{
		float num = 1f / (float)BoltRuntimeSettings.instance.GetConfigCopy().framesPerSecond;
		base.ParseCustomData();
		WeaponProfileData weaponProfileData = _profileData as WeaponProfileData;
		AnimationType = weaponProfileData.animationType;
		MeleeType = weaponProfileData.meleeType;
		WeaponType = weaponProfileData.weaponType;
		IsMelee = base.ItemType == ItemType.meleeWeapon;
		Booster = new BoosterData(weaponProfileData.booster);
		CharacterMultiplier = new CharacterMultiplierData(weaponProfileData.characterMultiplier);
		ClipSize = weaponProfileData.clipSize;
		Cooldown = weaponProfileData.cooldown;
		CooldownFrames = Mathf.RoundToInt(Cooldown / num);
		Melee = new MeleeData(weaponProfileData.melee, num);
		Charge = new ChargeData(weaponProfileData.charge, ClipSize);
		AmmoRecharge = new AmmoRechargeData(weaponProfileData.ammoRecharge);
		Damage = weaponProfileData.damage;
		if (weaponProfileData.effects != null && weaponProfileData.effects.Length != 0)
		{
			Effects = new EffectData[weaponProfileData.effects.Length];
			for (int i = 0; i < weaponProfileData.effects.Length; i++)
			{
				Effects[i] = new EffectData(weaponProfileData.effects[i]);
			}
		}
		if (weaponProfileData.selfEffects != null && weaponProfileData.selfEffects.Length != 0)
		{
			SelfEffects = new EffectData[weaponProfileData.selfEffects.Length];
			for (int j = 0; j < weaponProfileData.selfEffects.Length; j++)
			{
				SelfEffects[j] = new EffectData(weaponProfileData.selfEffects[j]);
			}
		}
		Explosion = new ExplosionData(weaponProfileData.explosion);
		Locking = new LockingData(weaponProfileData.locking);
		Radiation = weaponProfileData.radiation;
		Range = weaponProfileData.range;
		ScoreModifier = new ScoreModifierData(weaponProfileData.scoreModifier);
		Spread = new SpreadData(weaponProfileData.spread, weaponProfileData.cooldown);
		Zoom = new ZoomData(weaponProfileData.zoom);
		VolleySize = ((weaponProfileData.volleySize <= 0) ? 1 : weaponProfileData.volleySize);
		VolleyDelay = weaponProfileData.volleyDelay;
		SpawnedEntity = new SpawnedEntityData(weaponProfileData.spawnedEntity);
		if (!string.IsNullOrEmpty(weaponProfileData.crosshair) && Enum<Match.CrosshairType>.TryParse(weaponProfileData.crosshair, out var value))
		{
			Crosshair = value;
		}
		HideShotPathBlockedIcon = weaponProfileData.hideShotPathBlockedIcon;
		Critical = new CriticalData(weaponProfileData.critical);
		DamageInterval = weaponProfileData.damageInterval;
		Reload = new ReloadData(weaponProfileData.reload, Cooldown, num);
		Knockback = new KnockbackData(weaponProfileData.knockback);
		Spinup = new SpinupData(weaponProfileData.spinup, num);
		Projectile = new ProjectileData(weaponProfileData.projectile);
		EquipmentOverrides = new EquipmentOverideData(weaponProfileData.equipmentOverrides);
	}

	public void SetAsSkin(WeaponSkinProfile weaponSkinProfile)
	{
		base.CatalogVersion = weaponSkinProfile.CatalogVersion;
		base.Id = weaponSkinProfile.Id;
		base.Tags = weaponSkinProfile.Tags;
		base.Description = weaponSkinProfile.Description;
		base.Name = weaponSkinProfile.Name;
		base.Icon = weaponSkinProfile.Icon;
		base.ReleaseVersion = weaponSkinProfile.ReleaseVersion;
		base.Cost = weaponSkinProfile.Cost;
		base.UsageCount = weaponSkinProfile.UsageCount;
		base.UsageLifespan = weaponSkinProfile.UsageLifespan;
		base.IsConsumable = weaponSkinProfile.IsConsumable;
		base.Rarity = weaponSkinProfile.Rarity;
		base.RarityColor = weaponSkinProfile.RarityColor;
	}

	public override string ToString()
	{
		return $"[WeaponProfile] Id={base.Id}, Explodes?={Explosion.Explodes}, Charged?={Charge.CanCharge}";
	}

	public bool ShouldSendGroundHitToServer(HitType hitType)
	{
		if (!SpawnedEntity.SpawnsEntity && !Explosion.Explodes)
		{
			if (hitType == HitType.Raycast)
			{
				return Spread.IsAoE;
			}
			return false;
		}
		return true;
	}

	public int CalculateAmmoUsageAtChargeTime(float chargeTime)
	{
		return Mathf.Min(Mathf.FloorToInt(chargeTime / Charge.AmmoIncrementTime), ClipSize);
	}

	public float CalculateRangeAtTime(float chargeTime)
	{
		float num = (Explosion.Explodes ? Explosion.Range : Range);
		if (!Charge.CanCharge || Charge.RangeAtMax <= 0f)
		{
			return num;
		}
		return Mathf.Lerp(num, Charge.RangeAtMax, chargeTime / Charge.Time);
	}

	public float CaculateProjectileSpeedAtTime(float chargeTime)
	{
		if (!Charge.CanCharge || Charge.ProjectileSpeedAtMax <= 0f)
		{
			return Projectile.Speed;
		}
		return Mathf.Lerp(Projectile.Speed, Charge.ProjectileSpeedAtMax, chargeTime / Charge.Time);
	}

	public float CaculateDamageAtTime(float chargeTime, float baseDamage)
	{
		if (!Charge.CanCharge || Charge.DamageAtMax <= 0f)
		{
			return baseDamage;
		}
		return Mathf.Lerp(baseDamage, Charge.DamageAtMax, chargeTime / Charge.Time);
	}

	public float CalculateDamage(HitInfo hitInfo, LoadoutController loadoutController, bool isSelf, float damageModifier, float meleeDamageModifier)
	{
		float num = Damage;
		int num2 = 0;
		float chargeTime = hitInfo.chargeTime;
		if (Charge.CanCharge && !Charge.SingleAmmoCharge)
		{
			num2 = CalculateAmmoUsageAtChargeTime(hitInfo.chargeTime);
			if (num2 > 0)
			{
				chargeTime = Mathf.Lerp(0f, Charge.Time, (float)(1 + num2) / (float)ClipSize);
			}
		}
		if (Charge.CanCharge && !Explosion.Explodes)
		{
			num = CaculateDamageAtTime(chargeTime, num);
		}
		else if (Explosion.Explodes)
		{
			float num3 = Explosion.MaxDamageRange;
			float num4 = Explosion.Range;
			float num5 = Explosion.MaxDamage;
			float num6 = Explosion.MinDamage;
			if (Charge.CanCharge)
			{
				float num7 = num3 / num4;
				float num8 = num6 / num5;
				if (Charge.RangeAtMax > 0f)
				{
					num4 = CalculateRangeAtTime(chargeTime);
					num3 = num4 * num7;
				}
				if (Charge.DamageAtMax > 0f)
				{
					num5 = CaculateDamageAtTime(chargeTime, num5);
					num6 = num5 * num8;
				}
			}
			if (hitInfo.directHit)
			{
				num = num5;
			}
			else if (!hitInfo.directHit)
			{
				float t = Mathf.InverseLerp(num3, num4, hitInfo.distance);
				num = Mathf.Lerp(num5, num6, t);
			}
			if (isSelf)
			{
				num *= _selfDamageModifier;
			}
		}
		else if (Spread.IsSpread && Spread.FalloffRange > 0f && hitInfo.distance > Range)
		{
			float t2 = (hitInfo.distance - Range) / (Spread.FalloffRange - Range);
			num = Mathf.Lerp(num, Spread.FalloffDamage, t2);
		}
		else if (IsMelee)
		{
			num = Melee.GetOptionByDirection(hitInfo.meleeDirection).Damage;
		}
		if (Critical.CanCritical && hitInfo.isHeadshot)
		{
			num *= 1f + Critical.ExtraDamage;
		}
		num = ((!IsMelee) ? TryGetModifiedDamageForEquipmentType(num, loadoutController, EquipmentType.WeaponDamage) : TryGetModifiedDamageForEquipmentType(num, loadoutController, EquipmentType.MeleeDamage));
		if (!isSelf)
		{
			num *= 1f + damageModifier;
			if (hitInfo.isMelee)
			{
				num *= 1f + meleeDamageModifier;
			}
		}
		return num;
	}

	private float TryGetModifiedDamageForEquipmentType(float damage, LoadoutController loadoutController, EquipmentType equipmentType)
	{
		float num = damage;
		if (loadoutController.MajorEquipmentSlotProfile != null && loadoutController.MajorEquipmentSlotProfile.Type == equipmentType)
		{
			num = ((!(EquipmentOverrides.WeaponDamageMajorEffectPercentage > 0f)) ? loadoutController.MajorEquipmentSlotProfile.GetMajorModifiedValue(num) : loadoutController.MajorEquipmentSlotProfile.GetModifiedValue(num, EquipmentOverrides.WeaponDamageMajorEffectPercentage));
		}
		if (loadoutController.MinorEquipmentSlotProfile != null && loadoutController.MinorEquipmentSlotProfile.Type == equipmentType)
		{
			num = ((!(EquipmentOverrides.WeaponDamageMinorEffectPercentage > 0f)) ? loadoutController.MinorEquipmentSlotProfile.GetMinorModifiedValue(num) : loadoutController.MinorEquipmentSlotProfile.GetModifiedValue(num, EquipmentOverrides.WeaponDamageMinorEffectPercentage));
		}
		return num;
	}

	public PrefabId GetSpawnedEntityId()
	{
		switch (base.Id)
		{
		case "cannonBull":
			return BoltPrefabs.CannonBullEntity;
		case "darkMatter":
			return BoltPrefabs.DarkMatterEntity;
		case "motherOfNature":
			return BoltPrefabs.MotherOfNatureEntity;
		case "chainsawShotgun":
			return BoltPrefabs.ChainsawShotgunEntity;
		case "daydreamer":
			return BoltPrefabs.DaydreamerEntity;
		case "tntLauncher":
		case "tntLauncherHandGun":
			return BoltPrefabs.TNTEntity;
		default:
			Debug.LogError("[WeaponProfile] Profile with Id " + base.Id + " is not setup to spawn a bolt entity");
			return default(PrefabId);
		}
	}
}
