using BSCore;

public static class LoadoutExtensions
{
	public static void Deserialize(this Loadout loadout, string jsonString)
	{
		loadout.FromLoadoutData(LoadoutData.FromJson(jsonString));
	}

	public static void FromLoadoutData(this Loadout loadout, LoadoutData newLoadout)
	{
		if (loadout.Outfit != newLoadout.Outfit)
		{
			loadout.Outfit = newLoadout.Outfit;
		}
		if (loadout.MeleeWeapon.Id != newLoadout.MeleeWeapon)
		{
			loadout.MeleeWeapon.Id = newLoadout.MeleeWeapon;
		}
		for (int i = 0; i < loadout.Weapons.Length; i++)
		{
			if (loadout.Weapons[i].Id != newLoadout.Weapons[i])
			{
				loadout.Weapons[i].Id = newLoadout.Weapons[i];
			}
		}
		if (loadout.Backpack != newLoadout.Backpack)
		{
			loadout.Backpack = newLoadout.Backpack;
		}
		if (loadout.Hat != newLoadout.Hat)
		{
			loadout.Hat = newLoadout.Hat;
		}
		if (loadout.Emote != newLoadout.Emote)
		{
			loadout.Emote = newLoadout.Emote;
		}
		if (loadout.MajorEquipmentSlot != newLoadout.MajorEquipmentSlot)
		{
			loadout.MajorEquipmentSlot = newLoadout.MajorEquipmentSlot;
		}
		if (loadout.MinorEquipmentSlot != newLoadout.MinorEquipmentSlot)
		{
			loadout.MinorEquipmentSlot = newLoadout.MinorEquipmentSlot;
		}
	}

	public static string Serialize(this Loadout loadout)
	{
		return LoadoutData.FromLoadout(loadout).ToJson();
	}

	public static LoadoutData Deserialize(string jsonString)
	{
		return LoadoutData.FromJson(jsonString);
	}

	public static string Serialize(LoadoutData loadout)
	{
		return loadout.ToJson();
	}

	public static bool IsValid(this LoadoutData loadout, ProfileManager profileManager)
	{
		HeroClass heroClass = profileManager.GetById<OutfitProfile>(loadout.Outfit).HeroClass;
		for (int i = 0; i < loadout.Weapons.Length; i++)
		{
			WeaponProfile byId = profileManager.GetById<WeaponProfile>(loadout.Weapons[i]);
			if (byId != null && byId.HeroClass != heroClass && byId.HeroClass != HeroClass.all)
			{
				return false;
			}
		}
		WeaponProfile byId2 = profileManager.GetById<WeaponProfile>(loadout.MeleeWeapon);
		if (byId2 != null && byId2.HeroClass != heroClass && byId2.HeroClass != HeroClass.all)
		{
			return false;
		}
		BackpackProfile byId3 = profileManager.GetById<BackpackProfile>(loadout.Backpack);
		if (byId3 != null && byId3.HeroClass != heroClass && byId3.HeroClass != HeroClass.all)
		{
			return false;
		}
		return true;
	}

	public static float TryGetModifiedHealth(this Loadout loadout, ProfileManager profileManager)
	{
		float num = profileManager.GetById<OutfitProfile>(loadout.Outfit).HeroClassProfile.Health;
		EquipmentProfile byId = profileManager.GetById<EquipmentProfile>(loadout.MajorEquipmentSlot);
		if (byId != null && byId.Type == EquipmentType.Health)
		{
			num = byId.GetMajorModifiedValue(num);
		}
		EquipmentProfile byId2 = profileManager.GetById<EquipmentProfile>(loadout.MinorEquipmentSlot);
		if (byId2 != null && byId2.Type == EquipmentType.Health)
		{
			num = byId2.GetMinorModifiedValue(num);
		}
		return num;
	}
}
