using BSCore;
using UnityEngine;

public class ActiveUILazyLoadButton : UIBaseButtonClickHandler
{
	[SerializeField]
	private UIPrefabIds _prefabId;

	[SerializeField]
	private int _layer;

	[SerializeField]
	private bool _interactive = true;

	private BB2ActiveUI _activeUI;

	protected override void OnClick()
	{
		if (_activeUI == null)
		{
			UIPrefabManager.Instantiate(_prefabId, OnUICreated, _interactive, _layer);
		}
		else if (!_activeUI.IsActive)
		{
			_activeUI.Show();
		}
	}

	private void OnUICreated(GameObject uiGameobject)
	{
		if (!(uiGameobject == null))
		{
			_activeUI = uiGameobject.GetComponent<BB2ActiveUI>();
			if (!_activeUI.IsActive)
			{
				_activeUI.Show();
			}
		}
	}
}
