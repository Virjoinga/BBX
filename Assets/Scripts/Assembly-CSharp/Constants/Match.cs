using System.Collections.Generic;
using RSG;
using UnityEngine;

namespace Constants
{
	public static class Match
	{
		public enum EffectType
		{
			None = 0,
			HealthChangeOverTime = 1,
			SpeedChange = 2,
			Stun = 3,
			Blind = 4,
			Hugged = 5,
			ForcedMelee = 6,
			DamageMultiplier = 7,
			DamageShield = 8,
			ForcedMovement = 9,
			MeleeDamageMultiplier = 10,
			SizeChange = 11,
			PreventJump = 12
		}

		public enum StatusType
		{
			None = 0,
			Poisoned = 1,
			Burning = 2,
			FeathersInFace = 4,
			Inked = 8,
			Chilled = 0x10,
			Gooed = 0x20,
			Stunned = 0x40,
			Hugged = 0x80,
			DarkMatter = 0x100,
			SpeedBoost = 0x200,
			DamageBoost = 0x400,
			Shielded = 0x800,
			PhantomHugged = 0x1000,
			Tripping = 0x2000,
			ToughLove = 0x4000
		}

		public enum CrosshairType
		{
			Single = 0,
			Automatic = 1,
			AOE = 2,
			Beam = 3,
			HoldRelease = 4,
			Simple = 5
		}

		public static class BattleRoyale
		{
			public const float STOP_DROP_DISABLE_DELAY = 2f;

			public const float SPAWN_PITCH = 85f;
		}

		public enum ServerMessageIds
		{
			NotEnoughPlayers = 0,
			TeamsRebalanced = 1,
			HackingDetected = 2,
			KickedForHacking = 3
		}

		public static readonly Color FriendlyTeamColor = new Color(0f, 255f, 0f);

		public static readonly Color EnemyTeamColor = new Color(255f, 0f, 0f);

		public static readonly float RESPAWNUI_DELAY = 1.5f;

		public const float GLOBAL_EFFECT_TICK_INTERVAL = 0.25f;

		public const float MATCH_START_DELAY_AFTER_PLAYERS_LOADED = 1.5f;

		public const string SURVIVAL_TAG = "s";

		public const int LAG_COMPENSATION_MAX_HISTORY = 600;

		public const float POST_GROUNDED_GRACE = 0.1f;

		public const float MIN_HEIGHT_FOR_DOWNWARD_MELEE_WINDUP = 5f;

		public const int FIRE_HELD_GRACE_FRAMES_MAX = 3;

		public static readonly Dictionary<string, string> MapNameToFriendlyName = new Dictionary<string, string>
		{
			{ "TDM_AbusementPark", "Abusement Park" },
			{ "TDM_FacingTemples", "Facing Temples" },
			{ "TDM_NoBearsLand", "No Bears Land" },
			{ "TDM_SpaceOddity", "Space Oddity" },
			{ "TDM_Bearrens", "The Bearrens" },
			{ "TDM_HIPLab", "H.I.P. Test Lab" },
			{ "TDM_MarecraftCarrier", "Marecraft Carrier" },
			{ "TDM_Forest", "Mystic Forest" },
			{ "TDM_GoldDigger", "Gold Digger" }
		};

		public static readonly List<Tuple<float, float>> BULLET_SPREAD_PATTERN = new List<Tuple<float, float>>
		{
			new Tuple<float, float>(0.1688656f, -0.1071653f),
			new Tuple<float, float>(-0.7176788f, -0.4554527f),
			new Tuple<float, float>(0.1784787f, 0.2157437f),
			new Tuple<float, float>(0.7711501f, -0.4239429f),
			new Tuple<float, float>(0.0363466f, 0.2877133f),
			new Tuple<float, float>(0.04332549f, -0.6886385f),
			new Tuple<float, float>(-0.1941954f, -0.1232402f),
			new Tuple<float, float>(-0.04583704f, 0.7285595f),
			new Tuple<float, float>(0.1840623f, 0.4648882f),
			new Tuple<float, float>(-0.1730496f, -0.5325916f),
			new Tuple<float, float>(-0.1077862f, -0.8532187f),
			new Tuple<float, float>(0.2770081f, 0.2601279f),
			new Tuple<float, float>(-0.7691029f, -0.3619124f),
			new Tuple<float, float>(0.2617417f, 0.1661063f),
			new Tuple<float, float>(0f, 0.9299999f),
			new Tuple<float, float>(-0.8232958f, 0.2113862f),
			new Tuple<float, float>(-0.174083f, -0.6780081f),
			new Tuple<float, float>(-0.5040599f, 0.163779f),
			new Tuple<float, float>(0.3929617f, -0.3250863f),
			new Tuple<float, float>(-0.5986724f, -0.4349613f),
			new Tuple<float, float>(-0.920154f, 0.2362552f),
			new Tuple<float, float>(-0.6366287f, -0.6779409f),
			new Tuple<float, float>(0.07870012f, 0.4125607f),
			new Tuple<float, float>(-0.3533969f, 0.7510064f),
			new Tuple<float, float>(-0.7884411f, -0.0496046f)
		};

		public static Vector3 GetSpreadForward(Vector3 forward, Vector3 up, Vector3 right, float spreadAmmount, int shotCount)
		{
			int index = shotCount % BULLET_SPREAD_PATTERN.Count;
			Tuple<float, float> tuple = BULLET_SPREAD_PATTERN[index];
			return Quaternion.AngleAxis(tuple.Item1 * spreadAmmount, up) * Quaternion.AngleAxis(tuple.Item2 * spreadAmmount, right) * forward;
		}
	}
}
