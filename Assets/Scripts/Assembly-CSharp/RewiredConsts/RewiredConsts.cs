using Rewired.Dev;

namespace RewiredConsts
{
	public static class RewiredConsts
	{
		public static class Action
		{
			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Vertical")]
			public const int Vertical = 0;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Horizontal")]
			public const int Horizontal = 1;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Camera Vertical")]
			public const int CameraVertical = 3;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Camera Horizontal")]
			public const int CameraHorizontal = 2;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Jump")]
			public const int Jump = 4;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Fire")]
			public const int Fire = 5;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Emote")]
			public const int Emote = 6;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "ReleaseMouse")]
			public const int ReleaseMouse = 7;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Melee")]
			public const int Melee = 8;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Switch Weapon")]
			public const int SwitchWeapon = 9;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Reload")]
			public const int Reload = 10;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Settings")]
			public const int Settings = 11;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Zoom")]
			public const int Zoom = 12;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Use Special")]
			public const int UseSpecial = 13;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Clear")]
			public const int Clear = 14;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Scoreboard")]
			public const int Scoreboard = 15;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Weapon Slot 1")]
			public const int WeaponSlot1 = 16;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Weapon Slot 2")]
			public const int WeaponSlot2 = 17;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Weapon Slot 3")]
			public const int WeaponSlot3 = 18;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Weapon Slot 4")]
			public const int WeaponSlot4 = 19;

			[ActionIdFieldInfo(categoryName = "Default", friendlyName = "Weapon Slot 5")]
			public const int WeaponSlot5 = 20;
		}

		public static class LayoutManagerRuleSet
		{
		}

		public static class MapEnablerRuleSet
		{
		}
	}
}
