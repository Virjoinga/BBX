using System;
using System.Collections.Generic;
using UnityEngine;

public class TDMClassSelector : MonoBehaviour
{
	[SerializeField]
	private List<ClassSelectionButton> _classSelectionButtons = new List<ClassSelectionButton>();

	private ClassSelectionButton _selectedClassButton;

	private event Action<HeroClass> _classSelected;

	public event Action<HeroClass> ClassSelected
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

	private void Awake()
	{
		foreach (ClassSelectionButton classSelectionButton in _classSelectionButtons)
		{
			classSelectionButton.ClassSelected += OnClassSelected;
		}
	}

	public void SetSelectedClass(HeroClass heroClass)
	{
		for (int i = 0; i < _classSelectionButtons.Count; i++)
		{
			if (_classSelectionButtons[i].HeroClass == heroClass)
			{
				_classSelectionButtons[i].SetHighlightActive(isActive: true);
				_selectedClassButton = _classSelectionButtons[i];
			}
			else
			{
				_classSelectionButtons[i].SetHighlightActive(isActive: false);
			}
		}
	}

	private void OnClassSelected(ClassSelectionButton selectedButton)
	{
		this._classSelected?.Invoke(selectedButton.HeroClass);
		if (_selectedClassButton != null)
		{
			_selectedClassButton.SetHighlightActive(isActive: false);
		}
		_selectedClassButton = selectedButton;
		_selectedClassButton.SetHighlightActive(isActive: true);
	}
}
