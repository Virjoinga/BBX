using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class EmoteWheel : MonoBehaviour
{
	public static readonly float EMOTICON_SHOW_DURATION = 3f;

	[Inject]
	private SignalBus _signalBus;

	[Inject]
	private EmoticonData _emoticonData;

	[SerializeField]
	private ActiveUI _activeUI;

	[SerializeField]
	private List<EmoteButton> _emoteButtons;

	[SerializeField]
	private GameObject _previewContainer;

	[SerializeField]
	private Image _previewImage;

	[SerializeField]
	private TextMeshProUGUI _previewText;

	private void Awake()
	{
		_activeUI.LocalActiveUIToggled += OnActiveUIToggled;
		foreach (EmoteButton emoteButton in _emoteButtons)
		{
			emoteButton.EmoteRequested += OnEmoteRequested;
			emoteButton.EmoteHoverEnter += OnEmoteHoverEnter;
			emoteButton.EmoteHoverExit += OnEmoteHoverExit;
		}
	}

	private void OnActiveUIToggled(bool isEnabled)
	{
		if (isEnabled)
		{
			return;
		}
		_previewContainer.SetActive(value: false);
		foreach (EmoteButton emoteButton in _emoteButtons)
		{
			emoteButton.ClearHighlight();
		}
	}

	private void OnEmoteRequested(EmoticonType emoticon)
	{
		StartCoroutine(PreventOpeningForShowDuration());
		_activeUI.Hide();
		_signalBus.Fire(new RequestEmoticonSignal
		{
			EmoticonId = (int)emoticon
		});
	}

	private IEnumerator PreventOpeningForShowDuration()
	{
		_activeUI.PreventOpening = true;
		yield return new WaitForSeconds(EMOTICON_SHOW_DURATION);
		_activeUI.PreventOpening = false;
	}

	private void OnEmoteHoverEnter(EmoticonType type)
	{
		_previewContainer.SetActive(value: true);
		_previewText.text = type.ToString();
		Sprite spriteForEmoticon = _emoticonData.GetSpriteForEmoticon(type);
		if (spriteForEmoticon != null)
		{
			_previewImage.sprite = spriteForEmoticon;
		}
	}

	private void OnEmoteHoverExit()
	{
		_previewContainer.SetActive(value: false);
	}

	private void Reset()
	{
		_emoteButtons = GetComponentsInChildren<EmoteButton>().ToList();
	}
}
