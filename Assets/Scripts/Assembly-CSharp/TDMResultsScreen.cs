using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using BSCore.Constants.Config;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class TDMResultsScreen : MonoBehaviour
{
	private enum MatchResults
	{
		Won = 0,
		Lost = 1,
		Tied = 2
	}

	[Inject]
	private UserManager _userManager;

	[Inject]
	private ConfigManager _configManager;

	[Inject]
	private ExperienceManager _experienceManager;

	[Inject]
	private ClientFriendsManager _friendManager;

	[Inject]
	private SignalBus _signalBus;

	[Inject]
	private EmoticonData _emoticonData;

	[SerializeField]
	private Color _defeatTextColor;

	[SerializeField]
	private TextMeshProUGUI _resultsText;

	[SerializeField]
	private TextMeshProUGUI _resultsSubText;

	[SerializeField]
	private List<string> _defeatSubTextOptions;

	[SerializeField]
	private float _minScale = 0.7f;

	[SerializeField]
	private float _scaleTime = 0.5f;

	[SerializeField]
	private TextMeshProUGUI _myTeamScore;

	[SerializeField]
	private TextMeshProUGUI _enemyTeamScore;

	[SerializeField]
	private List<TDMPlayerPlate> _myTeamPlayerPlates;

	[SerializeField]
	private List<TDMPlayerPlate> _enemyTeamPlayerPlates;

	[SerializeField]
	private TextMeshProUGUI _experienceEarned;

	[SerializeField]
	private TextMeshProUGUI _expBarAmount;

	[SerializeField]
	private TextMeshProUGUI _expBarNextLevelAmount;

	[SerializeField]
	private TextMeshProUGUI _accountLevel;

	[SerializeField]
	private UIFillBar _expBar;

	[SerializeField]
	private TextMeshProUGUI _currencyReward;

	private float _fillTime = 2f;

	private TeamDeathMatchConfigData _tdmConfigData;

	private string _localEntityId;

	private MatchResults _localPlayerMatchResults;

	private float _bestKDA;

	private List<TDMPlayerPlate> _allPlayerPlates;

	private void Awake()
	{
		DisablePlayerPlates(_myTeamPlayerPlates);
		DisablePlayerPlates(_enemyTeamPlayerPlates);
		_allPlayerPlates = new List<TDMPlayerPlate>();
		_allPlayerPlates.AddRange(_myTeamPlayerPlates);
		_allPlayerPlates.AddRange(_enemyTeamPlayerPlates);
	}

	private void Start()
	{
		MonoBehaviourSingleton<MouseLockToggle>.Instance.ReleaseCursor();
		MonoBehaviourSingleton<MouseLockToggle>.Instance.MouseCanLock = false;
		_signalBus.Subscribe<ShowEmoticonSignal>(TryShowEmoticon);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<ShowEmoticonSignal>(TryShowEmoticon);
	}

	private void DisablePlayerPlates(List<TDMPlayerPlate> playerPlates)
	{
		foreach (TDMPlayerPlate playerPlate in playerPlates)
		{
			playerPlate.gameObject.SetActive(value: false);
		}
	}

	public void Populate(List<TDMPlayerState> players)
	{
		_tdmConfigData = _configManager.Get<TeamDeathMatchConfigData>(DataKeys.TeamDeathMatch);
		_localEntityId = _userManager.CurrentUser.Entity.Id;
		TDMPlayerState localTDMPlayer = players.First((TDMPlayerState p) => p.EntityId == _localEntityId);
		if (localTDMPlayer == null)
		{
			Debug.LogError("[TDMResultsScreen] Unable to populate results screen! Local Player is Null");
			return;
		}
		List<TDMPlayerState> list = players.Where((TDMPlayerState p) => !string.IsNullOrEmpty(p.EntityId)).ToList();
		List<TDMPlayerState> list2 = list.Where((TDMPlayerState p) => p.Team == localTDMPlayer.Team).ToList();
		List<TDMPlayerState> list3 = list.Where((TDMPlayerState p) => p.Team != localTDMPlayer.Team).ToList();
		GetBestKDA(list);
		PopulateTeamScores(list3.Sum((TDMPlayerState p) => p.Deaths), list2.Sum((TDMPlayerState p) => p.Deaths));
		PopulatePlayerPlates(list2, _myTeamPlayerPlates, _localPlayerMatchResults == MatchResults.Won || _localPlayerMatchResults == MatchResults.Tied);
		PopulatePlayerPlates(list3, _enemyTeamPlayerPlates, _localPlayerMatchResults == MatchResults.Lost || _localPlayerMatchResults == MatchResults.Tied);
		base.transform.SetLayerOnAll(LayerMask.NameToLayer("UI"));
		ShowExpReward(localTDMPlayer);
		ShowCurrencyReward();
	}

	private void GetBestKDA(List<TDMPlayerState> players)
	{
		_bestKDA = 0f;
		foreach (TDMPlayerState player in players)
		{
			float num = player.KDA();
			if (num > _bestKDA)
			{
				_bestKDA = num;
			}
		}
	}

	private void PopulateTeamScores(int myTeamScore, int enemyTeamScore)
	{
		_myTeamScore.text = myTeamScore.ToString();
		_enemyTeamScore.text = enemyTeamScore.ToString();
		if (myTeamScore > enemyTeamScore)
		{
			StartVictoryTextPulse();
			_resultsText.text = "Victory!";
			_resultsSubText.text = string.Empty;
			_localPlayerMatchResults = MatchResults.Won;
		}
		else if (myTeamScore < enemyTeamScore)
		{
			_resultsText.color = _defeatTextColor;
			_resultsText.text = "Defeat";
			_resultsSubText.text = _defeatSubTextOptions.Random();
			_localPlayerMatchResults = MatchResults.Lost;
		}
		else
		{
			_resultsText.text = "Tie";
			_resultsSubText.text = string.Empty;
			_localPlayerMatchResults = MatchResults.Tied;
		}
	}

	private void StartVictoryTextPulse()
	{
		_resultsText.rectTransform.DOScale(0f, 0f);
		_resultsText.rectTransform.DOScale(1f, _scaleTime).OnComplete(delegate
		{
			_resultsText.rectTransform.DOScale(_minScale, _scaleTime).SetLoops(-1, LoopType.Yoyo);
		});
	}

	private void PopulatePlayerPlates(List<TDMPlayerState> players, List<TDMPlayerPlate> playerPlates, bool didWin)
	{
		for (int i = 0; i < players.Count; i++)
		{
			if (playerPlates[i] != null)
			{
				bool num = players[i].EntityId == _localEntityId;
				bool mvp = players[i].KDA() >= _bestKDA;
				bool isFriend = num || _friendManager.IsFriendByDisplayName(players[i].DisplayName);
				playerPlates[i].Populate(players[i], didWin, mvp, isFriend);
				playerPlates[i].AddFriendButtonClicked += SendAddFriendRequest;
			}
			else
			{
				Debug.LogError("[TDMResultsScreen] Not enough player plates setup!");
			}
		}
	}

	private void SendAddFriendRequest(string displayName)
	{
		_friendManager.SendFriendRequestByName(displayName, delegate
		{
			Debug.Log("[TDMResultsScreen] Friend Request Sent");
		}, delegate(FailureReasons e)
		{
			Debug.LogError($"[TDMResultsScreen] Failed to send friend request - {e}");
		});
	}

	private void ShowExpReward(TDMPlayerState localPlayer)
	{
		_accountLevel.text = _experienceManager.PlayerCurrentLevel.ToString();
		int num = CalculateEarnedExp(localPlayer);
		int startingExperienceForCurrentLevel = _experienceManager.StartingExperienceForCurrentLevel;
		int num2 = _experienceManager.ExperienceNeededForNextLevel - startingExperienceForCurrentLevel;
		int num3 = _experienceManager.PlayerExperience - startingExperienceForCurrentLevel;
		int num4 = Mathf.Clamp(_experienceManager.PlayerExperience + num - startingExperienceForCurrentLevel, 0, num2);
		_expBarNextLevelAmount.text = num2.ToString();
		float start = (float)num3 / (float)num2;
		float end = (float)num4 / (float)num2;
		_expBar.InterpolateBar(start, end, _fillTime);
		StartCoroutine(CountUp(_expBarAmount, num3, num4));
		StartCoroutine(CountUp(_experienceEarned, 0, _experienceManager.PlayerExperience + num - _experienceManager.PlayerExperience));
	}

	private void ShowCurrencyReward()
	{
		int num = _tdmConfigData.Rewards.BaseCurrency;
		if (_localPlayerMatchResults == MatchResults.Won)
		{
			num += _tdmConfigData.Rewards.WinCurrency;
		}
		StartCoroutine(CountUp(_currencyReward, 0, num));
	}

	private int CalculateEarnedExp(TDMPlayerState localPlayer)
	{
		int baseEXP = _tdmConfigData.Rewards.BaseEXP;
		int winEXP = _tdmConfigData.Rewards.WinEXP;
		int killEXP = _tdmConfigData.Rewards.KillEXP;
		int num = baseEXP;
		if (_localPlayerMatchResults == MatchResults.Won)
		{
			num += winEXP;
		}
		return num + localPlayer.Kills * killEXP;
	}

	private IEnumerator CountUp(TextMeshProUGUI textLabel, int start, int target)
	{
		for (float timer = 0f; timer < _fillTime; timer += Time.deltaTime)
		{
			float t = timer / _fillTime;
			textLabel.text = ((int)Mathf.Lerp(start, target, t)).ToString();
			yield return null;
		}
		int num = target;
		textLabel.text = num.ToString();
	}

	private void TryShowEmoticon(ShowEmoticonSignal signal)
	{
		TDMPlayerPlate tDMPlayerPlate = _allPlayerPlates.FirstOrDefault((TDMPlayerPlate x) => x.DisplayName == signal.Name);
		if (tDMPlayerPlate != null)
		{
			Sprite spriteForEmoticon = _emoticonData.GetSpriteForEmoticon((EmoticonType)signal.EmoticonId);
			if (spriteForEmoticon != null)
			{
				tDMPlayerPlate.ShowEmoticon(spriteForEmoticon);
			}
		}
	}
}
