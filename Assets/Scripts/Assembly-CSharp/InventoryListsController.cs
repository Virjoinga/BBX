using System.Collections.Generic;
using System.Linq;
using BSCore;
using UnityEngine;
using Zenject;

public class InventoryListsController : MonoBehaviour
{
	[Inject]
	private InventoryManager _inventoryManager;

	[Inject]
	private MenuLoadoutManager _menuLoadoutManager;

	[SerializeField]
	private BB2ActiveUI _activeUI;

	[SerializeField]
	private List<InventoryList> _inventoryLists;

	private Dictionary<ItemType, List<InventoryItem>> _itemTypeToInventoryItems = new Dictionary<ItemType, List<InventoryItem>>();

	private void Awake()
	{
		_inventoryManager.InventoryFetched += PopulateLists;
		_activeUI.LocalActiveUIToggled += OnLocalActiveUIToggled;
		_menuLoadoutManager.CurrentLoadoutChanged += PopulateLists;
	}

	private void OnDestroy()
	{
		_inventoryManager.InventoryFetched -= PopulateLists;
		_activeUI.LocalActiveUIToggled -= OnLocalActiveUIToggled;
		_menuLoadoutManager.CurrentLoadoutChanged -= PopulateLists;
	}

	private void OnLocalActiveUIToggled(bool isActive)
	{
		if (isActive)
		{
			PopulateLists();
			return;
		}
		_menuLoadoutManager.SaveLoadoutToServer();
		_menuLoadoutManager.HideMeleeWeapon();
		_menuLoadoutManager.DeployWeapon(0);
		_menuLoadoutManager.PresentFront();
	}

	private void PopulateLists()
	{
		_itemTypeToInventoryItems = new Dictionary<ItemType, List<InventoryItem>>();
		foreach (InventoryItem item in _inventoryManager.Inventory)
		{
			if (!_itemTypeToInventoryItems.ContainsKey(item.Type))
			{
				_itemTypeToInventoryItems.Add(item.Type, new List<InventoryItem>());
			}
			_itemTypeToInventoryItems[item.Type].Add(item);
		}
		foreach (InventoryList inventoryList in _inventoryLists)
		{
			if (_itemTypeToInventoryItems.ContainsKey(inventoryList.ListItemType))
			{
				HeroClass currentHeroClass = _menuLoadoutManager.CurrentHeroClass;
				List<InventoryItem> list = new List<InventoryItem>();
				foreach (InventoryItem item2 in _itemTypeToInventoryItems[inventoryList.ListItemType])
				{
					if (item2.Profile is ProfileWithHeroClass profileWithHeroClass && (profileWithHeroClass.HeroClass == currentHeroClass || profileWithHeroClass.HeroClass == HeroClass.all))
					{
						list.Add(item2);
					}
				}
				inventoryList.PopulateList(list);
			}
			else
			{
				inventoryList.ClearList();
			}
		}
	}

	private void Reset()
	{
		_activeUI = GetComponent<BB2ActiveUI>();
		_inventoryLists = GetComponentsInChildren<InventoryList>().ToList();
	}
}
