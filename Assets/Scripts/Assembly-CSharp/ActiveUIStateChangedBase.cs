using UnityEngine;

public abstract class ActiveUIStateChangedBase : MonoBehaviour
{
	[SerializeField]
	private BB2ActiveUI _activeUI;

	protected virtual void Awake()
	{
		_activeUI.LocalActiveUIToggled += OnActiveUIToggled;
	}

	private void OnActiveUIToggled(bool isActive)
	{
		if (isActive)
		{
			OnActiveUIShown();
		}
		else
		{
			OnActiveUIHidden();
		}
	}

	protected abstract void OnActiveUIShown();

	protected abstract void OnActiveUIHidden();

	protected virtual void OnDestroy()
	{
		_activeUI.LocalActiveUIToggled -= OnActiveUIToggled;
	}
}
