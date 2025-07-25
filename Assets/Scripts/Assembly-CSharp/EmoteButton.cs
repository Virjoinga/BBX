using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmoteButton : MonoBehaviour
{
	[SerializeField]
	private EmoticonType _emoticon;

	[SerializeField]
	private Button _button;

	[SerializeField]
	private EventTrigger _eventTrigger;

	[SerializeField]
	private GameObject _highlight;

	private event Action<EmoticonType> _emoteRequested;

	public event Action<EmoticonType> EmoteRequested
	{
		add
		{
			_emoteRequested += value;
		}
		remove
		{
			_emoteRequested -= value;
		}
	}

	private event Action<EmoticonType> _emoteHoverEnter;

	public event Action<EmoticonType> EmoteHoverEnter
	{
		add
		{
			_emoteHoverEnter += value;
		}
		remove
		{
			_emoteHoverEnter -= value;
		}
	}

	private event Action _emoteHoverExit;

	public event Action EmoteHoverExit
	{
		add
		{
			_emoteHoverExit += value;
		}
		remove
		{
			_emoteHoverExit -= value;
		}
	}

	private void Awake()
	{
		_button.onClick.AddListener(delegate
		{
			this._emoteRequested?.Invoke(_emoticon);
		});
		_highlight.SetActive(value: false);
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter;
		entry.callback.AddListener(OnMouseHoverEnter);
		_eventTrigger.triggers.Add(entry);
		EventTrigger.Entry entry2 = new EventTrigger.Entry();
		entry2.eventID = EventTriggerType.PointerExit;
		entry2.callback.AddListener(OnMouseHoverExit);
		_eventTrigger.triggers.Add(entry2);
	}

	private void OnMouseHoverEnter(BaseEventData data)
	{
		_highlight.SetActive(value: true);
		this._emoteHoverEnter?.Invoke(_emoticon);
	}

	private void OnMouseHoverExit(BaseEventData data)
	{
		_highlight.SetActive(value: false);
		this._emoteHoverExit?.Invoke();
	}

	public void ClearHighlight()
	{
		_highlight.SetActive(value: false);
	}

	private void Reset()
	{
		_button = GetComponent<Button>();
		_eventTrigger = GetComponent<EventTrigger>();
	}
}
