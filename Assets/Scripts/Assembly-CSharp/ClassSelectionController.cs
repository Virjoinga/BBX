using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ClassSelectionController : MonoBehaviour
{
	[Inject]
	private MenuLoadoutManager _menuLoadoutManager;

	[SerializeField]
	private List<ClassSelectionButton> _classSelectionButtons = new List<ClassSelectionButton>();

	private ClassSelectionButton _selectedClass;

	private void Awake()
	{
		foreach (ClassSelectionButton classSelectionButton in _classSelectionButtons)
		{
			classSelectionButton.ClassSelected += OnClassSelected;
		}
	}

	private void Start()
	{
		for (int i = 0; i < _classSelectionButtons.Count; i++)
		{
			if (_classSelectionButtons[i].HeroClass == _menuLoadoutManager.CurrentHeroClass)
			{
				_classSelectionButtons[i].SetHighlightActive(isActive: true);
				_selectedClass = _classSelectionButtons[i];
			}
			else
			{
				_classSelectionButtons[i].SetHighlightActive(isActive: false);
			}
		}
	}

	private void OnClassSelected(ClassSelectionButton selectedButton)
	{
		Debug.Log("Class Selected: " + selectedButton.HeroClass);
		_menuLoadoutManager.SetHeroClass(selectedButton.HeroClass);
		if (_selectedClass != null)
		{
			_selectedClass.SetHighlightActive(isActive: false);
		}
		_selectedClass = selectedButton;
		_selectedClass.SetHighlightActive(isActive: true);
	}
}
