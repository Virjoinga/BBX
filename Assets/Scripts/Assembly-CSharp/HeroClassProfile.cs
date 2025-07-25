using System;
using System.Collections.Generic;
using BSCore;

public class HeroClassProfile : BaseProfile
{
	[Serializable]
	public class ProfileData : BaseProfileData
	{
		public ProfileAccelerationData acceleration;

		public ProfileBoosterData booster;

		public float health;

		public ProfileSpeedData speed;

		public LoadoutData defaultLoadout;

		public JumpProfileData jump;
	}

	[Serializable]
	public class ProfileAccelerationData
	{
		public float air;

		public float ground;
	}

	[Serializable]
	public class ProfileBoosterData
	{
		public float armor;

		public float speed;
	}

	[Serializable]
	public class ProfileSpeedData
	{
		public float forward;

		public float backward;

		public float side;

		public float noWeaponMultiplier;
	}

	[Serializable]
	public class JumpProfileData
	{
		public float height;

		public float doubleJumpCooldown;

		public float doubleJumpHeight;
	}

	public class AccelerationData
	{
		public float Air { get; private set; }

		public float Ground { get; private set; }

		public AccelerationData(float air, float ground)
		{
			Air = air;
			Ground = ground;
		}
	}

	public class BoosterData
	{
		public float Armor { get; private set; }

		public float Speed { get; private set; }

		public BoosterData(float armor, float speed)
		{
			Armor = armor;
			Speed = speed;
		}
	}

	public class SpeedData
	{
		public float Forward { get; private set; }

		public float Backward { get; private set; }

		public float Sideways { get; private set; }

		public float NoWeaponMultiplier { get; private set; }

		public SpeedData(ProfileSpeedData data)
		{
			Forward = data.forward;
			Backward = data.backward;
			Sideways = data.side;
			NoWeaponMultiplier = data.noWeaponMultiplier;
		}
	}

	public class JumpData
	{
		public float Height { get; private set; }

		public float DoubleJumpCooldown { get; private set; }

		public float DoubleJumpHeight { get; private set; }

		public JumpData(JumpProfileData data)
		{
			Height = data.height;
			DoubleJumpCooldown = data.doubleJumpCooldown;
			DoubleJumpHeight = data.doubleJumpHeight;
		}
	}

	public LoadoutData DefaultLoadout { get; private set; }

	public AccelerationData Acceleration { get; private set; } = new AccelerationData(0f, 0f);

	public BoosterData Booster { get; private set; } = new BoosterData(0f, 0f);

	public SpeedData Speed { get; private set; } = new SpeedData(new ProfileSpeedData());

	public JumpData Jump { get; private set; }

	public float Health { get; private set; } = 1f;

	public HeroClassProfile(GameItem gameItem, Dictionary<string, BaseProfile> profiles)
		: base(gameItem)
	{
	}

	protected override void DeserializeData(string json)
	{
		_profileData = BaseProfileData.FromJson<ProfileData>(json);
	}

	protected override void ParseCustomData()
	{
		base.ParseCustomData();
		ProfileData profileData = _profileData as ProfileData;
		Acceleration = new AccelerationData(profileData.acceleration.air, profileData.acceleration.ground);
		Booster = new BoosterData(profileData.booster.armor, profileData.booster.speed);
		Speed = new SpeedData(profileData.speed);
		Jump = new JumpData(profileData.jump);
		Health = profileData.health;
		DefaultLoadout = profileData.defaultLoadout;
	}
}
