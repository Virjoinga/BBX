using System;
using BSCore;
using UnityEngine;

public class TestCharacterSelection : UIBaseButtonClickHandler
{
	[SerializeField]
	private string _character = "";

	public string Character => _character;

	public bool Interactable
	{
		get
		{
			return _button.interactable;
		}
		set
		{
			_button.interactable = value;
		}
	}

	private event Action<TestCharacterSelection> _clicked;

	public event Action<TestCharacterSelection> Clicked
	{
		add
		{
			_clicked += value;
		}
		remove
		{
			_clicked -= value;
		}
	}

	protected override void OnClick()
	{
		this._clicked?.Invoke(this);
	}
}
