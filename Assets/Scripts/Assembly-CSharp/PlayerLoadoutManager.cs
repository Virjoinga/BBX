using System;
using System.Collections.Generic;
using BSCore;
using UnityEngine;
using Zenject;

public class PlayerLoadoutManager
{
	private const string EQUIPPED_HEROCLASS_KEY = "EquippedHeroClass";

	private const string HEROCLASS_LOADOUTS_KEY = "Loadouts";

	private readonly HeroClass DEFAULT_HEROCLASS = HeroClass.soldier;

	private bool _equippedHeroClassChanged;

	private Dictionary<HeroClass, HeroClassLoadouts> _heroClassLoadoutsByClass = new Dictionary<HeroClass, HeroClassLoadouts>();

	private ProfileManager _profileManager;

	private UserManager _userManager;

	public HeroClass EquippedHeroClass { get; private set; }

	private bool IsServer => _userManager == null;

	public PlayerLoadoutManager(DiContainer container)
	{
		_profileManager = container.Resolve<ProfileManager>();
		_userManager = container.Resolve<UserManager>();
		EquippedHeroClass = DEFAULT_HEROCLASS;
	}

	public void PopulateLoadouts(Dictionary<string, string> data)
	{
		if (data.TryGetValue("EquippedHeroClass", out var value))
		{
			if (Enum<HeroClass>.TryParse(value, out var value2))
			{
				EquippedHeroClass = value2;
			}
			else
			{
				_equippedHeroClassChanged = true;
			}
		}
		else
		{
			_equippedHeroClassChanged = true;
		}
		_heroClassLoadoutsByClass = new Dictionary<HeroClass, HeroClassLoadouts>();
		foreach (HeroClass value5 in Enum.GetValues(typeof(HeroClass)))
		{
			if (value5 != HeroClass.all && data.TryGetValue(string.Concat(value5, "Loadouts"), out var value3))
			{
				HeroClassLoadouts value4 = HeroClassLoadouts.FromJson(value3);
				_heroClassLoadoutsByClass.Add(value5, value4);
			}
		}
	}

	public void SaveLoadouts()
	{
		if (IsServer)
		{
			Debug.LogError("[PlayerLoadoutManager] Server is Unable to Save Loadouts to Playfab");
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (_equippedHeroClassChanged)
		{
			dictionary.Add("EquippedHeroClass", EquippedHeroClass.ToString());
			_equippedHeroClassChanged = false;
		}
		foreach (KeyValuePair<HeroClass, HeroClassLoadouts> item in _heroClassLoadoutsByClass)
		{
			if (item.Value.IsDirty)
			{
				dictionary.Add(string.Concat(item.Key, "Loadouts"), item.Value.ToJson());
				item.Value.IsDirty = false;
			}
		}
		if (dictionary.Count > 0)
		{
			_userManager.UpdateUserData(dictionary, delegate
			{
				Debug.Log("[PlayerLoadoutManager] Loadout Data successfully saved!");
			}, delegate(FailureReasons error)
			{
				Debug.Log($"[PlayerLoadoutManager] Failed to save loadout data to Playfab - {error}");
			});
		}
	}

	public LoadoutData GetEquippedLoadout()
	{
		return GetLoadoutForClass(EquippedHeroClass);
	}

	public LoadoutData GetLoadoutForClass(HeroClass heroClass, bool saveAsWell = true)
	{
		if (_heroClassLoadoutsByClass.ContainsKey(heroClass))
		{
			LoadoutData equippedLoadout = _heroClassLoadoutsByClass[heroClass].GetEquippedLoadout();
			if (equippedLoadout.IsValid())
			{
				return equippedLoadout;
			}
		}
		LoadoutData defaultLoadout = GetDefaultLoadout(heroClass);
		SetLoadout(heroClass, defaultLoadout);
		if (saveAsWell)
		{
			SaveLoadouts();
		}
		return defaultLoadout;
	}

	public LoadoutData GetLoadoutForClass(HeroClass heroClass, LoadoutSlot loadoutSlot)
	{
		if (_heroClassLoadoutsByClass.ContainsKey(heroClass))
		{
			LoadoutData loadoutForSlot = _heroClassLoadoutsByClass[heroClass].GetLoadoutForSlot(loadoutSlot);
			if (loadoutForSlot.IsValid())
			{
				return loadoutForSlot;
			}
		}
		LoadoutData defaultLoadout = GetDefaultLoadout(heroClass);
		SetLoadout(heroClass, defaultLoadout);
		SaveLoadouts();
		return defaultLoadout;
	}

	public LoadoutData GetDefaultLoadout(HeroClass heroClass)
	{
		Debug.Log($"[PlayerLoadoutManager] Unable to get loadout for class {heroClass}. Using Default");
		HeroClassProfile byId = _profileManager.GetById<HeroClassProfile>(heroClass.ToString());
		if (byId != null)
		{
			return byId.DefaultLoadout;
		}
		Debug.LogError($"[PlayerLoadoutManager] Unable to get profile for hero class {heroClass}");
		return LoadoutData.Default();
	}

	public void SetEquippedHeroClass(HeroClass heroClass)
	{
		if (heroClass != EquippedHeroClass)
		{
			EquippedHeroClass = heroClass;
			_equippedHeroClassChanged = true;
		}
	}

	public void SetLoadout(LoadoutData loadoutData)
	{
		SetLoadout(EquippedHeroClass, loadoutData);
	}

	public void SetLoadout(HeroClass heroClass, LoadoutData loadoutData)
	{
		if (!_heroClassLoadoutsByClass.ContainsKey(heroClass))
		{
			_heroClassLoadoutsByClass.Add(heroClass, new HeroClassLoadouts());
		}
		_heroClassLoadoutsByClass[heroClass].SetLoadout(loadoutData);
	}

	public void SetLoadout(HeroClass heroClass, LoadoutSlot loadoutSlot, LoadoutData loadoutData, bool setEquippedSlot = true)
	{
		if (!_heroClassLoadoutsByClass.ContainsKey(heroClass))
		{
			_heroClassLoadoutsByClass.Add(heroClass, new HeroClassLoadouts());
		}
		_heroClassLoadoutsByClass[heroClass].SetLoadout(loadoutSlot, loadoutData, setEquippedSlot);
	}

	public void SetEquippedSlot(LoadoutSlot loadoutSlot)
	{
		SetEquippedSlot(EquippedHeroClass, loadoutSlot);
	}

	public void SetEquippedSlot(HeroClass heroClass, LoadoutSlot loadoutSlot)
	{
		if (_heroClassLoadoutsByClass.ContainsKey(heroClass))
		{
			_heroClassLoadoutsByClass[heroClass].SetEquippedSlot(loadoutSlot);
		}
		else
		{
			Debug.LogError($"[PlayerLoadoutManager] Unable to set slot {loadoutSlot} for class {heroClass}");
		}
	}
}
