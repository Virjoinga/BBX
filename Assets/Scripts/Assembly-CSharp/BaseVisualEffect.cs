using UnityEngine;

public class BaseVisualEffect : MonoBehaviour
{
	public virtual void Setup(Outfit outfit)
	{
	}

	protected void OnEnable()
	{
		Show();
	}

	protected void OnDisable()
	{
		Hide();
	}

	protected virtual void Show()
	{
	}

	protected virtual void Hide()
	{
	}
}
