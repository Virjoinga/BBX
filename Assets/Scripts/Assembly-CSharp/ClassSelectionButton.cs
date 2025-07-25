using System;
using BSCore;
using UnityEngine;

public class ClassSelectionButton : UIBaseButtonClickHandler
{
	[SerializeField]
	private HeroClass _heroClass;

	[SerializeField]
	private GameObject _activeHighlight;

	public HeroClass HeroClass => _heroClass;

	private event Action<ClassSelectionButton> _classSelected;

	public event Action<ClassSelectionButton> ClassSelected
	{
		add
		{
			_classSelected += value;
		}
		remove
		{
			_classSelected -= value;
		}
	}

	protected override void OnClick()
	{
		this._classSelected?.Invoke(this);
	}

	public void SetHighlightActive(bool isActive)
	{
		_activeHighlight.SetActive(isActive);
	}
}
