using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class EquipmentSelectionController : MonoBehaviour
{
	[Inject]
	private MenuLoadoutManager _menuLoadoutManager;

	[SerializeField]
	private ToggleGroup _majorToggleGroup;

	[SerializeField]
	private ToggleGroup _minorToggleGroup;

	[SerializeField]
	private List<EquipmentToggle> _majorEquipmentOptions;

	[SerializeField]
	private List<EquipmentToggle> _minorEquipmentOptions;

	private EquipmentToggle _equippedMajorToggle;

	private EquipmentToggle _equippedMinorToggle;

	private void Awake()
	{
		_menuLoadoutManager.CurrentLoadoutChanged += SetEquippedOptions;
	}

	private void OnDestroy()
	{
		_menuLoadoutManager.CurrentLoadoutChanged -= SetEquippedOptions;
	}

	private void Start()
	{
		foreach (EquipmentToggle majorEquipmentOption in _majorEquipmentOptions)
		{
			majorEquipmentOption.ToggledOn += OnMajorEquipmentToggledOn;
			majorEquipmentOption.ToggledOff += OnMajorEquipmentToggledOff;
		}
		foreach (EquipmentToggle minorEquipmentOption in _minorEquipmentOptions)
		{
			minorEquipmentOption.ToggledOn += OnMinorEquipmentToggledOn;
			minorEquipmentOption.ToggledOff += OnMinorEquipmentToggledOff;
		}
	}

	private void OnEnable()
	{
		SetEquippedOptions();
	}

	private void SetEquippedOptions()
	{
		TryEnableEquippedMajorEquipment();
		TryEnableEquippedMinorEquipment();
	}

	public void TryEnableEquippedMajorEquipment()
	{
		_equippedMajorToggle = null;
		LoadoutData currentLoadout = _menuLoadoutManager.CurrentLoadout;
		foreach (EquipmentToggle majorEquipmentOption in _majorEquipmentOptions)
		{
			bool flag = currentLoadout.MajorEquipmentSlot == majorEquipmentOption.EquipmentId;
			majorEquipmentOption.SetIsEquipped(flag);
			if (flag)
			{
				_equippedMajorToggle = majorEquipmentOption;
			}
		}
	}

	public void TryEnableEquippedMinorEquipment()
	{
		_equippedMinorToggle = null;
		LoadoutData currentLoadout = _menuLoadoutManager.CurrentLoadout;
		foreach (EquipmentToggle minorEquipmentOption in _minorEquipmentOptions)
		{
			bool flag = currentLoadout.MinorEquipmentSlot == minorEquipmentOption.EquipmentId;
			minorEquipmentOption.SetIsEquipped(flag);
			if (flag)
			{
				_equippedMinorToggle = minorEquipmentOption;
			}
		}
	}

	private void OnMajorEquipmentToggledOn(ToggleHandler listItem)
	{
		if (_majorToggleGroup.gameObject.activeInHierarchy)
		{
			string equipmentId = (listItem as EquipmentToggle).EquipmentId;
			_menuLoadoutManager.EquipEquipmentInMajorSlot(equipmentId);
			_equippedMajorToggle = listItem as EquipmentToggle;
			if (_equippedMinorToggle != null && _equippedMinorToggle.EquipmentId == equipmentId)
			{
				_equippedMinorToggle.SetIsEquipped(isEquipped: false);
				_menuLoadoutManager.EquipEquipmentInMinorSlot(string.Empty);
				_equippedMinorToggle = null;
			}
		}
	}

	private void OnMajorEquipmentToggledOff(ToggleHandler listItem)
	{
		if (!_majorEquipmentOptions.Any((EquipmentToggle li) => li.IsOn))
		{
			_menuLoadoutManager.EquipEquipmentInMajorSlot(string.Empty);
			_equippedMajorToggle = null;
		}
	}

	private void OnMinorEquipmentToggledOn(ToggleHandler listItem)
	{
		if (_minorToggleGroup.gameObject.activeInHierarchy)
		{
			string equipmentId = (listItem as EquipmentToggle).EquipmentId;
			_menuLoadoutManager.EquipEquipmentInMinorSlot(equipmentId);
			_equippedMinorToggle = listItem as EquipmentToggle;
			if (_equippedMajorToggle != null && _equippedMajorToggle.EquipmentId == equipmentId)
			{
				_equippedMajorToggle.SetIsEquipped(isEquipped: false);
				_menuLoadoutManager.EquipEquipmentInMajorSlot(string.Empty);
				_equippedMajorToggle = null;
			}
		}
	}

	private void OnMinorEquipmentToggledOff(ToggleHandler listItem)
	{
		if (!_minorEquipmentOptions.Any((EquipmentToggle li) => li.IsOn))
		{
			_menuLoadoutManager.EquipEquipmentInMinorSlot(string.Empty);
			_equippedMinorToggle = null;
		}
	}
}
