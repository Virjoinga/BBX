using System.Collections.Generic;
using System.Linq;
using BSCore;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InventoryList : MonoBehaviour
{
	[Inject]
	private MenuLoadoutManager _menuLoadoutManager;

	[SerializeField]
	private Transform _itemContainer;

	[SerializeField]
	private ToggleGroup _toggleGroup;

	[SerializeField]
	private InventoryListItem _inventoryItemPrefab;

	[SerializeField]
	private ItemType _itemType;

	[SerializeField]
	private bool _allowUnEquip;

	private readonly List<InventoryListItem> _listItems = new List<InventoryListItem>();

	public ItemType ListItemType => _itemType;

	private void OnEnable()
	{
		TrySwitchActiveWeaponType();
	}

	private void TrySwitchActiveWeaponType()
	{
		switch (_itemType)
		{
		case ItemType.primaryWeapon:
			_menuLoadoutManager.HideMeleeWeapon();
			_menuLoadoutManager.DeployWeapon(0);
			break;
		case ItemType.secondaryWeapon:
			_menuLoadoutManager.HideMeleeWeapon();
			_menuLoadoutManager.DeployWeapon(1);
			break;
		case ItemType.meleeWeapon:
			_menuLoadoutManager.StowWeapon();
			_menuLoadoutManager.DisplayMeleeWeapon();
			break;
		case ItemType.outfit:
			_menuLoadoutManager.PresentFront();
			_menuLoadoutManager.StowWeapon();
			_menuLoadoutManager.HideMeleeWeapon();
			break;
		case ItemType.backpack:
			_menuLoadoutManager.PresentBack();
			_menuLoadoutManager.StowWeapon();
			_menuLoadoutManager.HideMeleeWeapon();
			break;
		case ItemType.hat:
			_menuLoadoutManager.PresentFront();
			_menuLoadoutManager.StowWeapon();
			_menuLoadoutManager.HideMeleeWeapon();
			break;
		case ItemType.heroClass:
		case ItemType.special:
			break;
		}
	}

	public void PopulateList(List<InventoryItem> inventoryItems)
	{
		ClearList();
		inventoryItems = inventoryItems.OrderBy((InventoryItem i) => i.Name).ToList();
		foreach (InventoryItem inventoryItem in inventoryItems)
		{
			bool isSelected = _menuLoadoutManager.CurrentLoadout.IsEquipped(inventoryItem.Profile.Id);
			InventoryListItem inventoryListItem = Object.Instantiate(_inventoryItemPrefab, _itemContainer);
			inventoryListItem.Init(inventoryItem.Profile, isSelected, _toggleGroup, _allowUnEquip);
			inventoryListItem.ToggledOn += OnListItemToggledOn;
			inventoryListItem.ToggledOff += OnListItemToggledOff;
			_listItems.Add(inventoryListItem);
		}
	}

	public void ClearList()
	{
		_itemContainer.DestroyChildren();
		_listItems.Clear();
	}

	private void OnListItemToggledOn(ToggleHandler listItem)
	{
		if (_toggleGroup.gameObject.activeInHierarchy)
		{
			BaseProfile profile = (listItem as InventoryListItem).Profile;
			_menuLoadoutManager.EquipItem(_itemType, profile.Id);
		}
	}

	private void OnListItemToggledOff(ToggleHandler listItem)
	{
		if (_allowUnEquip && !_listItems.Any((InventoryListItem li) => li.IsOn))
		{
			_menuLoadoutManager.EquipItem(_itemType, string.Empty);
		}
	}
}
