using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPlate : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _rankText;

	[SerializeField]
	private Image _rankImage;

	[SerializeField]
	private TextMeshProUGUI _playerNameText;

	[SerializeField]
	private TextMeshProUGUI _statText;

	[SerializeField]
	private Sprite _rank1Sprite;

	[SerializeField]
	private Sprite _rank2Sprite;

	[SerializeField]
	private Sprite _rank3Sprite;

	[SerializeField]
	private GameObject _localPlayerOverlay;

	public void Populate(PlayerLeaderboardEntry leaderboardEntry, bool isLocalPlayer = false)
	{
		_playerNameText.text = leaderboardEntry.DisplayName;
		_statText.text = leaderboardEntry.StatValue.ToString();
		SetRankDisplay(leaderboardEntry.Position + 1);
		_localPlayerOverlay.SetActive(isLocalPlayer);
	}

	public void PopulateNoStat(string playerName, bool isLocalPlayer = false)
	{
		_rankText.text = string.Empty;
		_rankImage.gameObject.SetActive(value: false);
		_playerNameText.text = playerName;
		_statText.text = "0";
		_localPlayerOverlay.SetActive(isLocalPlayer);
	}

	private void SetRankDisplay(int rank)
	{
		bool flag = false;
		switch (rank)
		{
		case 1:
			flag = true;
			_rankImage.overrideSprite = _rank1Sprite;
			break;
		case 2:
			flag = true;
			_rankImage.overrideSprite = _rank2Sprite;
			break;
		case 3:
			flag = true;
			_rankImage.overrideSprite = _rank3Sprite;
			break;
		default:
			_rankText.text = rank.ToString();
			break;
		}
		_rankImage.gameObject.SetActive(flag);
		_rankText.gameObject.SetActive(!flag);
	}
}
