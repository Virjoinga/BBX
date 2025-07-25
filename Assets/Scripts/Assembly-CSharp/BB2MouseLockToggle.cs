using BSCore;

public class BB2MouseLockToggle : MouseLockToggle
{
	protected override bool CheckForMovementInput()
	{
		if (BSCoreInput.GetAxis(Option.Vertical) == 0f)
		{
			return BSCoreInput.GetAxis(Option.Horizontal) != 0f;
		}
		return true;
	}

	protected override bool CheckReleaseMousePressed()
	{
		return BSCoreInput.GetButtonDown(Option.ReleaseMouse);
	}
}
