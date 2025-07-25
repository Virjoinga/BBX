using System;
using System.Collections.Generic;
using BSCore;
using UnityEngine;

public class UITabHeader : UIBaseButtonClickHandler
{
	[SerializeField]
	private GameObject _content;

	[SerializeField]
	private GameObject _highlight;

	[SerializeField]
	private List<GameObject> _enableOnShow = new List<GameObject>();

	private UITabsManager _tabsManager;

	protected override void Start()
	{
		base.Start();
		_tabsManager = GetComponentInParent<UITabsManager>();
		if (_tabsManager == null)
		{
			throw new Exception("A Tab header must be nested under a Tab Manager.");
		}
	}

	protected override void OnClick()
	{
		_tabsManager.TabSelected(this);
	}

	public void Show()
	{
		SetShowHideState(shown: true);
	}

	public void Hide()
	{
		SetShowHideState(shown: false);
	}

	private void SetShowHideState(bool shown)
	{
		if (_highlight != null)
		{
			_highlight.SetActive(shown);
		}
		_content.SetActive(shown);
		foreach (GameObject item in _enableOnShow)
		{
			item.SetActive(shown);
		}
	}
}
