using System;
using System.Collections.Generic;
using BSCore;
using UnityEngine;

public class WeaponSkinProfile : BaseProfile
{
	[Serializable]
	public class WeaponSkinData : BaseProfileData
	{
		public string WeaponId;
	}

	private WeaponProfile _baseWeaponProfile;

	private ConfigManager _configManager;

	public string WeaponId { get; protected set; }

	public WeaponSkinProfile(GameItem gameItem, ConfigManager configManager, Dictionary<string, BaseProfile> profiles)
		: base(gameItem)
	{
		_configManager = configManager;
		if (profiles.TryGetValue(WeaponId, out var value))
		{
			_baseWeaponProfile = value as WeaponProfile;
		}
		else
		{
			Debug.LogError("[MeleeWeaponSkinProfile] Failed to find heroClass profile " + WeaponId);
		}
	}

	public WeaponProfile GetWeaponProfileForSkin()
	{
		WeaponProfile weaponProfile = new WeaponProfile(_baseWeaponProfile.GameItem, _configManager);
		weaponProfile.SetAsSkin(this);
		return weaponProfile;
	}

	protected override void DeserializeData(string json)
	{
		_profileData = BaseProfileData.FromJson<WeaponSkinData>(json);
	}

	protected override void ParseCustomData()
	{
		base.ParseCustomData();
		WeaponSkinData weaponSkinData = _profileData as WeaponSkinData;
		WeaponId = weaponSkinData.WeaponId;
	}
}
