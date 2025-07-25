using System;
using UnityEngine;

[Serializable]
public class HeroClassLoadouts
{
	[SerializeField]
	private LoadoutSlot EquippedLoadoutSlot;

	[SerializeField]
	private LoadoutData Slot1Loadout;

	[SerializeField]
	private LoadoutData Slot2Loadout;

	[SerializeField]
	private LoadoutData Slot3Loadout;

	public bool IsDirty { get; set; }

	public static HeroClassLoadouts FromJson(string json)
	{
		return JsonUtility.FromJson<HeroClassLoadouts>(json);
	}

	public string ToJson()
	{
		return JsonUtility.ToJson(this);
	}

	public LoadoutData GetEquippedLoadout()
	{
		return GetLoadoutForSlot(EquippedLoadoutSlot);
	}

	public LoadoutData GetLoadoutForSlot(LoadoutSlot loadoutSlot)
	{
		switch (loadoutSlot)
		{
		case LoadoutSlot.Slot1:
			return Slot1Loadout;
		case LoadoutSlot.Slot2:
			return Slot2Loadout;
		case LoadoutSlot.Slot3:
			return Slot3Loadout;
		default:
			Debug.LogError($"[HeroClassLoadouts] No Case for Loadout Slot {loadoutSlot}");
			return Slot1Loadout;
		}
	}

	public void SetEquippedSlot(LoadoutSlot loadoutSlot)
	{
		EquippedLoadoutSlot = loadoutSlot;
		IsDirty = true;
	}

	public void SetLoadout(LoadoutData loadoutData)
	{
		SetLoadout(EquippedLoadoutSlot, loadoutData, setEquippedSlot: false);
	}

	public void SetLoadout(LoadoutSlot loadoutSlot, LoadoutData loadoutData, bool setEquippedSlot = true)
	{
		IsDirty = true;
		switch (loadoutSlot)
		{
		case LoadoutSlot.Slot1:
			Slot1Loadout = loadoutData;
			break;
		case LoadoutSlot.Slot2:
			Slot2Loadout = loadoutData;
			break;
		case LoadoutSlot.Slot3:
			Slot3Loadout = loadoutData;
			break;
		default:
			Debug.LogError($"[HeroClassLoadouts] No Case for Loadout Slot {loadoutSlot}");
			break;
		}
		if (setEquippedSlot)
		{
			SetEquippedSlot(loadoutSlot);
		}
	}
}
