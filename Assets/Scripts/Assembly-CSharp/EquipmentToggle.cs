using UnityEngine;

public class EquipmentToggle : ToggleHandler
{
	[SerializeField]
	private EquipmentType _equipmentType;

	[SerializeField]
	private GameObject _activeHighlight;

	public string EquipmentId => $"equipment{_equipmentType}";

	public void SetIsEquipped(bool isEquipped)
	{
		_toggle.isOn = isEquipped;
		_activeHighlight.SetActive(isEquipped);
	}

	protected override void HandleToggledOn()
	{
		base.HandleToggledOn();
		_activeHighlight.SetActive(value: true);
	}

	protected override void HandleToggledOff()
	{
		base.HandleToggledOff();
		_activeHighlight.SetActive(value: false);
	}
}
