using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TDMScoreboardPlate : MonoBehaviour
{
	[SerializeField]
	private Image _classIcon;

	[SerializeField]
	private TextMeshProUGUI _playerNameText;

	[SerializeField]
	private TextMeshProUGUI _assistsText;

	[SerializeField]
	private TextMeshProUGUI _killsText;

	[SerializeField]
	private TextMeshProUGUI _deathsText;

	[SerializeField]
	private Color _iconGreyOutColor;

	[SerializeField]
	private Color _textGreyOutColor;

	[SerializeField]
	private GameObject _disconnectedIcon;

	public TDMPlayerState Player { get; private set; }

	public bool HasPlayer => Player != null;

	public float KDA { get; private set; }

	public void Populate(TDMPlayerState player, HeroClass heroClass)
	{
		_disconnectedIcon.SetActive(value: false);
		_playerNameText.text = player.DisplayName;
		_assistsText.text = player.Assists.ToString();
		_killsText.text = player.Kills.ToString();
		_deathsText.text = player.Deaths.ToString();
		Sprite sprite = Resources.Load<Sprite>($"{heroClass}_HeroClassIcon");
		_classIcon.sprite = sprite;
		if (player.Disconnected)
		{
			SetDisconnected();
		}
		Player = player;
		KDA = player.KDA();
		base.gameObject.SetActive(value: true);
	}

	public void UpdatePlate(TDMPlayerState player, HeroClass heroClass)
	{
		KDA = player.KDA();
		_assistsText.text = player.Assists.ToString();
		_killsText.text = player.Kills.ToString();
		_deathsText.text = player.Deaths.ToString();
		Sprite sprite = Resources.Load<Sprite>($"{heroClass}_HeroClassIcon");
		_classIcon.sprite = sprite;
		if (player.Disconnected)
		{
			SetDisconnected();
		}
		else
		{
			SetConnect();
		}
	}

	private void SetConnect()
	{
		_classIcon.color = Color.white;
		_playerNameText.color = Color.white;
		_playerNameText.fontStyle = FontStyles.Normal;
		_disconnectedIcon.SetActive(value: false);
	}

	private void SetDisconnected()
	{
		_classIcon.color = _iconGreyOutColor;
		_playerNameText.color = _textGreyOutColor;
		_playerNameText.fontStyle = FontStyles.Strikethrough;
		_disconnectedIcon.SetActive(value: true);
	}
}
