using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TDMLoadingPlate : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _nameText;

	[SerializeField]
	private Image _outfitIcon;

	private float _fadeInTime = 1f;

	private bool _isLoaded;

	public string DisplayName { get; private set; }

	public void Populate(string displayName, Sprite outfitSprite)
	{
		Debug.Log("[TDMMatchLoadingScreen] Populating Player " + displayName);
		_outfitIcon.DOColor(Color.black, 0f);
		DisplayName = displayName;
		_nameText.text = displayName;
		if (outfitSprite != null)
		{
			_outfitIcon.overrideSprite = outfitSprite;
		}
		base.gameObject.SetActive(value: true);
	}

	public void ShowLoaded()
	{
		if (!_isLoaded)
		{
			_outfitIcon.DOColor(Color.white, _fadeInTime);
			_isLoaded = true;
		}
	}
}
