using System.Collections.Generic;
using System.Linq;
using BSCore;
using TMPro;
using UnityEngine;

public class UITestLoadoutLoader : MonoBehaviour
{
	private const string NONE = "None";

	private const string OUTFIT_INDEX = "LoadoutTestOutfit";

	private const string PRIMARY_INDEX = "LoadoutTestPrimary";

	private const string SECONDARY_INDEX = "LoadoutTestSecondary";

	private const string MELEE_INDEX = "LoadoutTestMelee";

	private const string BACKPACK_INDEX = "LoadoutTestBackpack";

	private const string HAT_INDEX = "LoadoutTestHat";

	[SerializeField]
	private TestLoadoutLoader _loadoutLoader;

	[SerializeField]
	private TMP_Dropdown _outfitOptions;

	[SerializeField]
	private TMP_Dropdown _primaryOptions;

	[SerializeField]
	private TMP_Dropdown _secondaryOptions;

	[SerializeField]
	private TMP_Dropdown _meleeOptions;

	[SerializeField]
	private TMP_Dropdown _backpackOptions;

	[SerializeField]
	private TMP_Dropdown _hatOptions;

	private LoadoutData _loadout;

	private readonly Dictionary<ItemType, List<ProfileWithHeroClass>> _optionsByType = new Dictionary<ItemType, List<ProfileWithHeroClass>>();

	private readonly Dictionary<ItemType, List<ProfileWithHeroClass>> _limitedOptionsByType = new Dictionary<ItemType, List<ProfileWithHeroClass>>();

	private readonly List<ItemType> _equipmentTypes = new List<ItemType>
	{
		ItemType.outfit,
		ItemType.primaryWeapon,
		ItemType.secondaryWeapon,
		ItemType.meleeWeapon,
		ItemType.backpack,
		ItemType.hat
	};

	private bool _resetting = true;

	private void Start()
	{
		_outfitOptions.onValueChanged.AddListener(delegate(int i)
		{
			OnOutfitChanged(i, clearSaves: true);
		});
		_primaryOptions.onValueChanged.AddListener(OnPrimaryChanged);
		_secondaryOptions.onValueChanged.AddListener(OnSecondaryChanged);
		_meleeOptions.onValueChanged.AddListener(OnMeleeChanged);
		_backpackOptions.onValueChanged.AddListener(OnBackpackChanged);
		_hatOptions.onValueChanged.AddListener(OnHatChanged);
	}

	public void OnProfilesFetched(List<BaseProfile> profiles)
	{
		_loadout = new LoadoutData
		{
			Weapons = new string[4]
		};
		GenerateOptions(profiles);
		LoadSavedLoadout();
		_resetting = false;
	}

	private void LoadSavedLoadout()
	{
		int num = PlayerPrefs.GetInt("LoadoutTestOutfit", 0);
		_outfitOptions.value = num;
		OnOutfitChanged(num, clearSaves: false);
		int num2 = PlayerPrefs.GetInt("LoadoutTestPrimary", 0);
		_primaryOptions.value = num2;
		OnPrimaryChanged(num2);
		int num3 = PlayerPrefs.GetInt("LoadoutTestSecondary", 0);
		_secondaryOptions.value = num3;
		OnSecondaryChanged(num3);
		int num4 = PlayerPrefs.GetInt("LoadoutTestMelee", 0);
		_meleeOptions.value = num4;
		OnMeleeChanged(num4);
		int num5 = PlayerPrefs.GetInt("LoadoutTestBackpack", 0);
		_backpackOptions.value = num5;
		OnBackpackChanged(num5);
		int num6 = PlayerPrefs.GetInt("LoadoutTestHat", 0);
		_hatOptions.value = num6;
		OnHatChanged(num6);
	}

	private void GenerateOptions(List<BaseProfile> profiles)
	{
		foreach (ItemType equipmentType in _equipmentTypes)
		{
			_optionsByType.Add(equipmentType, new List<ProfileWithHeroClass>());
			_limitedOptionsByType.Add(equipmentType, new List<ProfileWithHeroClass>());
		}
		foreach (BaseProfile profile in profiles)
		{
			if (_equipmentTypes.Contains(profile.ItemType))
			{
				_optionsByType[profile.ItemType].Add(profile as ProfileWithHeroClass);
			}
		}
		foreach (ItemType equipmentType2 in _equipmentTypes)
		{
			_optionsByType[equipmentType2] = (from p in _optionsByType[equipmentType2]
				orderby p.HeroClass.ToString(), p.Name
				select p).ToList();
		}
		_outfitOptions.options = _optionsByType[ItemType.outfit].Select((ProfileWithHeroClass profile) => new TMP_Dropdown.OptionData($"{profile.HeroClass} - {profile.Name}")).ToList();
	}

	private void OnOutfitChanged(int index, bool clearSaves)
	{
		Debug.Log($"CHANGING TO {index}");
		_resetting = true;
		OutfitProfile outfitProfile = _optionsByType[ItemType.outfit][index] as OutfitProfile;
		_loadout.Outfit = outfitProfile.Id;
		OutfitProfile outfitProfile2 = _loadoutLoader.LoadoutController.OutfitProfile;
		if (outfitProfile2 == null || outfitProfile2.HeroClass != outfitProfile.HeroClass)
		{
			LimitByHeroClass(outfitProfile.HeroClass);
			_loadout.Weapons[0] = string.Empty;
			_loadout.Weapons[1] = string.Empty;
			_loadout.MeleeWeapon = _limitedOptionsByType[ItemType.meleeWeapon][0].Id;
			_loadout.Backpack = _limitedOptionsByType[ItemType.backpack][0].Id;
			_loadout.Hat = string.Empty;
		}
		_loadoutLoader.LoadLoadout(_loadout);
		PlayerPrefs.SetInt("LoadoutTestOutfit", index);
		if (clearSaves)
		{
			PlayerPrefs.SetInt("LoadoutTestPrimary", 0);
			PlayerPrefs.SetInt("LoadoutTestSecondary", 0);
			PlayerPrefs.SetInt("LoadoutTestMelee", 0);
			PlayerPrefs.SetInt("LoadoutTestBackpack", 0);
			PlayerPrefs.SetInt("LoadoutTestHat", 0);
		}
		_resetting = false;
	}

	private void OnPrimaryChanged(int index)
	{
		if (!_resetting)
		{
			_loadout.Weapons[0] = _limitedOptionsByType[ItemType.primaryWeapon][index]?.Id ?? string.Empty;
			_loadoutLoader.EquipWeapon(0, _loadout.Weapons[0]);
			PlayerPrefs.SetInt("LoadoutTestPrimary", index);
		}
	}

	private void OnSecondaryChanged(int index)
	{
		if (!_resetting)
		{
			_loadout.Weapons[1] = _limitedOptionsByType[ItemType.secondaryWeapon][index]?.Id ?? string.Empty;
			_loadoutLoader.EquipWeapon(1, _loadout.Weapons[1]);
			PlayerPrefs.SetInt("LoadoutTestSecondary", index);
		}
	}

	private void OnMeleeChanged(int index)
	{
		if (!_resetting)
		{
			_loadout.MeleeWeapon = _limitedOptionsByType[ItemType.meleeWeapon][index]?.Id ?? string.Empty;
			_loadoutLoader.EquipMeleeMelee(_loadout.MeleeWeapon);
			PlayerPrefs.SetInt("LoadoutTestMelee", index);
		}
	}

	private void OnBackpackChanged(int index)
	{
		if (!_resetting)
		{
			_loadout.Backpack = _limitedOptionsByType[ItemType.backpack][index]?.Id ?? string.Empty;
			_loadoutLoader.LoadBackpack(_loadout.Backpack);
			PlayerPrefs.SetInt("LoadoutTestBackpack", index);
		}
	}

	private void OnHatChanged(int index)
	{
		if (!_resetting)
		{
			_loadout.Hat = _limitedOptionsByType[ItemType.hat][index]?.Id ?? string.Empty;
			_loadoutLoader.LoadHat(_loadout.Hat);
			PlayerPrefs.SetInt("LoadoutTestHat", index);
		}
	}

	private void LimitByHeroClass(HeroClass heroClass)
	{
		LimitByHeroClass(_primaryOptions, heroClass, ItemType.primaryWeapon, allowNone: true);
		LimitByHeroClass(_secondaryOptions, heroClass, ItemType.secondaryWeapon, allowNone: true);
		LimitByHeroClass(_meleeOptions, heroClass, ItemType.meleeWeapon);
		LimitByHeroClass(_backpackOptions, heroClass, ItemType.backpack);
		LimitByHeroClass(_hatOptions, heroClass, ItemType.hat, allowNone: true);
	}

	private void LimitByHeroClass(TMP_Dropdown dropdown, HeroClass heroClass, ItemType itemType, bool allowNone = false)
	{
		_limitedOptionsByType[itemType].Clear();
		List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
		if (allowNone)
		{
			_limitedOptionsByType[itemType].Add(null);
			list.Add(new TMP_Dropdown.OptionData("None"));
		}
		foreach (ProfileWithHeroClass item in _optionsByType[itemType])
		{
			if (item.HeroClass == heroClass)
			{
				_limitedOptionsByType[itemType].Add(item);
				list.Add(new TMP_Dropdown.OptionData(item.Name));
			}
		}
		dropdown.ClearOptions();
		dropdown.AddOptions(list);
	}
}
