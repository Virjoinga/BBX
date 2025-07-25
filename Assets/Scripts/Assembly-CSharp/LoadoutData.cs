using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct LoadoutData : IEquatable<LoadoutData>
{
	public string Outfit;

	public string MeleeWeapon;

	public string[] Weapons;

	public string Backpack;

	public string Hat;

	public string Emote;

	public string MajorEquipmentSlot;

	public string MinorEquipmentSlot;

	public static LoadoutData FromJson(string json)
	{
		return JsonUtility.FromJson<LoadoutData>(json);
	}

	public static LoadoutData FromLoadout(Loadout loadout)
	{
		return new LoadoutData
		{
			Outfit = loadout.Outfit,
			MeleeWeapon = loadout.MeleeWeapon.Id,
			Weapons = loadout.Weapons.Select((Weapon w) => w?.Id).ToArray(),
			Backpack = loadout.Backpack,
			Hat = loadout.Hat,
			Emote = loadout.Emote,
			MajorEquipmentSlot = loadout.MajorEquipmentSlot,
			MinorEquipmentSlot = loadout.MinorEquipmentSlot
		};
	}

	public bool IsEquipped(string itemId)
	{
		if (ToJson().Contains($"\"{itemId}\""))
		{
			return true;
		}
		return false;
	}

	public string ToJson()
	{
		return JsonUtility.ToJson(this);
	}

	public static LoadoutData Default()
	{
		return new LoadoutData
		{
			Outfit = "heavyOutfitCamoSteelGreen",
			Weapons = new string[4],
			MeleeWeapon = "",
			Backpack = "heavyBackpackBBRblack",
			Hat = "",
			Emote = "",
			MajorEquipmentSlot = "",
			MinorEquipmentSlot = ""
		};
	}

	public bool IsValid()
	{
		if (!string.IsNullOrEmpty(Outfit) && !string.IsNullOrEmpty(Backpack))
		{
			return !string.IsNullOrEmpty(MeleeWeapon);
		}
		return false;
	}

	public bool Equals(LoadoutData other)
	{
		if (other.Outfit == Outfit && other.Weapons.Except(Weapons).Count() == 0 && Weapons.Except(other.Weapons).Count() == 0 && other.MeleeWeapon == MeleeWeapon && other.Backpack == Backpack && other.Hat == Hat && other.Emote == Emote && other.MajorEquipmentSlot == MajorEquipmentSlot)
		{
			return other.MinorEquipmentSlot == MinorEquipmentSlot;
		}
		return false;
	}
}
