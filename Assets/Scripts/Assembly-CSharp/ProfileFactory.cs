using System.Collections.Generic;
using System.Linq;
using BSCore;

public class ProfileFactory : BaseProfileFactory
{
	public ProfileFactory(ConfigManager configManager)
		: base(configManager)
	{
	}

	protected override BaseProfile GenerateProfile(GameItem gameItem, Dictionary<string, BaseProfile> profiles)
	{
		switch (gameItem.ItemType)
		{
		case ItemType.heroClass:
			return new HeroClassProfile(gameItem, profiles);
		case ItemType.outfit:
			return new OutfitProfile(gameItem, profiles);
		case ItemType.meleeWeapon:
		case ItemType.primaryWeapon:
		case ItemType.secondaryWeapon:
			return new WeaponProfile(gameItem, _configManager);
		case ItemType.backpack:
			return new BackpackProfile(gameItem);
		case ItemType.hat:
			return new HatProfile(gameItem);
		case ItemType.inAppPurchase:
			return new IAPProfile(gameItem);
		case ItemType.weaponSkin:
			return new WeaponSkinProfile(gameItem, _configManager, profiles).GetWeaponProfileForSkin();
		case ItemType.equipment:
			return new EquipmentProfile(gameItem);
		default:
			return new BaseProfile(gameItem);
		}
	}

	protected override List<GameItem> SortGameItemsByLoadOrder(List<GameItem> gameItems)
	{
		return (from x in gameItems
			orderby x.ItemType != ItemType.primaryWeapon, x.ItemType != ItemType.secondaryWeapon, x.ItemType != ItemType.meleeWeapon, x.ItemType != ItemType.weaponSkin, x.ItemType != ItemType.backpack, x.ItemType != ItemType.heroClass, x.ItemType != ItemType.outfit, x.ItemType != ItemType.equipment, x.ItemType != ItemType.inAppPurchase
			select x).ToList();
	}
}
