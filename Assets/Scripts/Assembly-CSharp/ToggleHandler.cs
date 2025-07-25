using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class ToggleHandler : MonoBehaviour
{
	[SerializeField]
	protected Toggle _toggle;

	private bool _wasOn;

	public bool IsOn => _toggle.isOn;

	private event Action<ToggleHandler> _toggledOn;

	public event Action<ToggleHandler> ToggledOn
	{
		add
		{
			_toggledOn += value;
		}
		remove
		{
			_toggledOff -= value;
		}
	}

	private event Action<ToggleHandler> _toggledOff;

	public event Action<ToggleHandler> ToggledOff
	{
		add
		{
			_toggledOff += value;
		}
		remove
		{
			_toggledOff -= value;
		}
	}

	protected void RaiseToggledOn()
	{
		this._toggledOn?.Invoke(this);
	}

	protected void RaiseToggledOff()
	{
		this._toggledOff?.Invoke(this);
	}

	protected virtual void Reset()
	{
		_toggle = GetComponentInChildren<Toggle>();
	}

	protected virtual void Start()
	{
		_wasOn = _toggle.isOn;
		_toggle.onValueChanged.AddListener(OnToggle);
	}

	private void OnToggle(bool isOn)
	{
		if (!_wasOn && isOn)
		{
			HandleToggledOn();
		}
		else if (_wasOn && !isOn)
		{
			HandleToggledOff();
		}
		_wasOn = isOn;
	}

	protected virtual void HandleToggledOn()
	{
		RaiseToggledOn();
	}

	protected virtual void HandleToggledOff()
	{
		RaiseToggledOff();
	}
}
