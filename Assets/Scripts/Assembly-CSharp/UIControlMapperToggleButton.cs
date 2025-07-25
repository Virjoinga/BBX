using BSCore;
using Rewired.UI.ControlMapper;
using UnityEngine;

public class UIControlMapperToggleButton : UIBaseButtonClickHandler
{
	private ControlMapper _controlMapper;

	private void Awake()
	{
		_controlMapper = Object.FindObjectOfType<ControlMapper>();
	}

	protected override void OnClick()
	{
		_controlMapper.Open();
	}
}
