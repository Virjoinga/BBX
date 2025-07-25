using System;
using System.Collections;
using BSCore;
using RootMotion.FinalIK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TDMPlayerPlate : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _nameText;

	[SerializeField]
	private TextMeshProUGUI _mvpNameText;

	[SerializeField]
	private TextMeshProUGUI _killsText;

	[SerializeField]
	private TextMeshProUGUI _deathsText;

	[SerializeField]
	private TextMeshProUGUI _assistsText;

	[SerializeField]
	private LoadoutController _loadoutController;

	[SerializeField]
	private GameObject _mvpContainer;

	[SerializeField]
	private GameObject _normalNameContainer;

	[SerializeField]
	private Shader _unlitShader;

	[SerializeField]
	private Image _platformIcon;

	[SerializeField]
	private Sprite _pcIconSprite;

	[SerializeField]
	private Sprite _mobileIconSprite;

	[SerializeField]
	private Sprite _consoleIconSprite;

	[SerializeField]
	private Button _addFriendButton;

	[SerializeField]
	private GameObject _requestSentText;

	[SerializeField]
	private Image _emoticonIcon;

	[SerializeField]
	private FadeableUI _emoticonFader;

	private bool _didWin;

	public string DisplayName { get; private set; }

	private event Action<string> _addFriendButtonClicked;

	public event Action<string> AddFriendButtonClicked
	{
		add
		{
			_addFriendButtonClicked += value;
		}
		remove
		{
			_addFriendButtonClicked -= value;
		}
	}

	public void Populate(TDMPlayerState player, bool didWin, bool mvp, bool isFriend)
	{
		DisplayName = player.DisplayName;
		_nameText.text = DisplayName;
		_mvpNameText.text = player.DisplayName;
		_assistsText.text = player.Assists.ToString();
		_killsText.text = player.Kills.ToString();
		_deathsText.text = player.Deaths.ToString();
		_mvpContainer.SetActive(mvp);
		_normalNameContainer.SetActive(!mvp);
		SetPlatformIcon((PlatformType)player.Platform);
		_requestSentText.SetActive(value: false);
		_addFriendButton.gameObject.SetActive(!isFriend);
		_addFriendButton.onClick.AddListener(AddFriendClicked);
		_didWin = didWin;
		base.gameObject.SetActive(value: true);
		LoadoutData loadoutData = LoadoutData.FromJson(player.Loadout);
		loadoutData.MeleeWeapon = string.Empty;
		loadoutData.Weapons = new string[4];
		_loadoutController.SetLoadout(loadoutData, OnLoadoutSetCompleted);
	}

	private void OnLoadoutSetCompleted(bool didSet)
	{
		UnlitifyRenderers();
		StartCoroutine(DisableAimTargetAfterDelay());
		_loadoutController.transform.SetLayerOnAll(LayerMask.NameToLayer("UI"));
		if (_didWin)
		{
			_loadoutController.Outfit.GetComponent<Animator>().SetTrigger(PlayerAnimationController.Parameter.Win.ToString());
		}
		else
		{
			_loadoutController.Outfit.GetComponent<Animator>().SetTrigger(PlayerAnimationController.Parameter.Lose.ToString());
		}
	}

	public void ShowEmoticon(Sprite emoticon)
	{
		_emoticonIcon.sprite = emoticon;
		StartCoroutine(ShowEmoticonRoutine());
	}

	private IEnumerator ShowEmoticonRoutine()
	{
		yield return _emoticonFader.FadeToRoutine(1f, 0.25f, null);
		yield return new WaitForSeconds(EmoteWheel.EMOTICON_SHOW_DURATION);
		yield return _emoticonFader.FadeToRoutine(0f, 0.25f, null);
	}

	private void AddFriendClicked()
	{
		this._addFriendButtonClicked(DisplayName);
		_addFriendButton.gameObject.SetActive(value: false);
		_requestSentText.SetActive(value: true);
	}

	private IEnumerator DisableAimTargetAfterDelay()
	{
		yield return new WaitForFixedUpdate();
		BipedIK componentInChildren = _loadoutController.GetComponentInChildren<BipedIK>();
		if (componentInChildren != null)
		{
			componentInChildren.solvers.lookAt.SetLookAtWeight(0f);
		}
	}

	private void UnlitifyRenderers()
	{
		Renderer[] componentsInChildren = _loadoutController.Outfit.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (renderer.GetComponent<ParticleSystem>() == null)
			{
				renderer.material.shader = _unlitShader;
			}
		}
	}

	private void SetPlatformIcon(PlatformType platform)
	{
		switch (platform)
		{
		case PlatformType.PC:
			_platformIcon.sprite = _pcIconSprite;
			break;
		case PlatformType.Mobile:
			_platformIcon.sprite = _mobileIconSprite;
			break;
		case PlatformType.Console:
			_platformIcon.sprite = _consoleIconSprite;
			break;
		default:
			_platformIcon.sprite = _pcIconSprite;
			break;
		}
	}
}
