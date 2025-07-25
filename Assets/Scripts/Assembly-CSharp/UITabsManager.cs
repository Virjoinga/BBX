using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UITabsManager : MonoBehaviour
{
	[SerializeField]
	private List<UITabHeader> _tabs;

	private UITabHeader _tabSelected;

	private void Reset()
	{
		_tabs = GetComponentsInChildren<UITabHeader>().ToList();
	}

	private void Start()
	{
		for (int i = 0; i < _tabs.Count; i++)
		{
			if (i == 0)
			{
				TabSelected(_tabs[i]);
			}
			else
			{
				_tabs[i].Hide();
			}
		}
	}

	public void TabSelected(UITabHeader selected)
	{
		if (_tabSelected != null)
		{
			_tabSelected.Hide();
		}
		_tabSelected = selected;
		_tabSelected.Show();
	}
}
