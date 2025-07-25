using BSCore;
using Rewired;
using Rewired.ComponentControls;
using UnityEngine;

public class FiringAimTouchPad : MonoBehaviour
{
	[SerializeField]
	private TouchPad _touchPad;

	[SerializeField]
	private TouchButton _leftFireButton;

	[SerializeField]
	private RectTransform _fireButtonRect;

	private bool _firingPressed;

	private bool _firingWasPressed;

	private Vector2 _originalFireButtonPosition;

	private Rewired.CustomController _customController;

	private void Awake()
	{
		_customController = BSCoreInput.Player.controllers.GetController<Rewired.CustomController>(0);
		_originalFireButtonPosition = _fireButtonRect.anchoredPosition;
		_touchPad.PressDownEvent += OnTouchPadPressed;
		_touchPad.PressUpEvent += OnTouchPadRelease;
	}

	private void Update()
	{
		if (_firingPressed)
		{
			_fireButtonRect.position = _touchPad.touchPosition;
			_leftFireButton.SetRawValue(1f);
		}
		else if (_firingWasPressed)
		{
			_leftFireButton.SetRawValue(0f);
		}
		_firingWasPressed = _firingPressed;
	}

	private void OnTouchPadPressed()
	{
		Vector2 touchStartPosition = _touchPad.touchStartPosition;
		if (RectTransformUtility.RectangleContainsScreenPoint(_fireButtonRect, touchStartPosition))
		{
			_firingPressed = true;
		}
	}

	private void OnTouchPadRelease()
	{
		if (_firingPressed)
		{
			_firingPressed = false;
			_fireButtonRect.anchoredPosition = _originalFireButtonPosition;
		}
	}
}
