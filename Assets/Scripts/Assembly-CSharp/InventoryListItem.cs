using BSCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryListItem : ToggleHandler
{
	[SerializeField]
	private Image _icon;

	[SerializeField]
	private GameObject _activeHighlight;

	[SerializeField]
	private TextMeshProUGUI _titleText;

	private bool _allowUnEquip;

	public BaseProfile Profile { get; private set; }

	public void Init(BaseProfile profile, bool isSelected, ToggleGroup toggleGroup, bool allowUnEquip = false)
	{
		Profile = profile;
		_toggle.group = toggleGroup;
		_toggle.isOn = isSelected;
		_allowUnEquip = allowUnEquip;
		if (_allowUnEquip)
		{
			_toggle.interactable = true;
		}
		_activeHighlight.SetActive(isSelected);
		_icon.overrideSprite = profile.Icon;
		_titleText.text = profile.Name;
	}

	public void UpdateEquippedState(bool isEquipped)
	{
		_toggle.isOn = isEquipped;
	}

	protected override void Start()
	{
		base.Start();
		if (!_allowUnEquip)
		{
			_toggle.interactable = !_toggle.isOn;
		}
	}

	protected override void HandleToggledOn()
	{
		base.HandleToggledOn();
		if (!_allowUnEquip)
		{
			_toggle.interactable = false;
		}
		_activeHighlight.SetActive(value: true);
	}

	protected override void HandleToggledOff()
	{
		base.HandleToggledOff();
		if (!_allowUnEquip)
		{
			_toggle.interactable = true;
		}
		_activeHighlight.SetActive(value: false);
	}
}
