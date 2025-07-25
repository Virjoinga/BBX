using System;
using BSCore;
using UnityEngine;
using UnityEngine.UI;

public class PickupItemButton : UIBaseButtonClickHandler
{
	[SerializeField]
	private int _slotItemType = 1;

	[SerializeField]
	private Image _iconImage;

	private event Action<int> _slotSelected;

	public event Action<int> SlotSelected
	{
		add
		{
			_slotSelected += value;
		}
		remove
		{
			_slotSelected -= value;
		}
	}

	protected override void OnClick()
	{
		this._slotSelected?.Invoke(_slotItemType);
	}

	public void SetIcon(Sprite icon)
	{
		_iconImage.sprite = icon;
	}
}
