using System;
using BSCore;
using UnityEngine;

public class EquipmentProfile : BaseProfile
{
	[Serializable]
	public class EquipmentProfileData : BaseProfileData
	{
		public string EquipmentType;

		public float MajorEffectPercentage;

		public float MinorEffectPercentage;
	}

	public EquipmentType Type { get; protected set; }

	public float MajorEffectPercentage { get; protected set; }

	public float MinorEffectPercentage { get; protected set; }

	public EquipmentProfile(GameItem gameItem)
		: base(gameItem)
	{
	}

	protected override void DeserializeData(string json)
	{
		_profileData = BaseProfileData.FromJson<EquipmentProfileData>(json);
	}

	protected override void ParseCustomData()
	{
		base.ParseCustomData();
		EquipmentProfileData equipmentProfileData = _profileData as EquipmentProfileData;
		if (Enum<EquipmentType>.TryParse(equipmentProfileData.EquipmentType, out var value))
		{
			Type = value;
		}
		else
		{
			Debug.LogError("[EquipmentProfile] Unable to parse Equipment type " + equipmentProfileData.EquipmentType);
		}
		MajorEffectPercentage = equipmentProfileData.MajorEffectPercentage;
		MinorEffectPercentage = equipmentProfileData.MinorEffectPercentage;
	}

	public float GetMajorModifiedValue(float initialValue)
	{
		return GetModifiedValue(initialValue, MajorEffectPercentage);
	}

	public float GetMinorModifiedValue(float initialValue)
	{
		return GetModifiedValue(initialValue, MinorEffectPercentage);
	}

	public float GetModifiedValue(float initialValue, float percentage)
	{
		float num = initialValue / 100f * percentage;
		Debug.Log($"[EquipmentProfile] Getting modified value for {Type}. Percentage: {percentage} | Initial: {initialValue} | Changed: {num} | Final: {initialValue + num}");
		return initialValue + num;
	}
}
