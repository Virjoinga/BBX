using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using Constants;
using DG.Tweening;
using MatchMaking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TDMMatchLoadingScreen : MonoBehaviour
{
	[Serializable]
	private struct MapNameToScreenshot
	{
		public string Map;

		public Sprite Screenshot;
	}

	[Inject]
	private SignalBus _signalBus;

	[Inject]
	private UserManager _userManager;

	[Inject]
	private ProfileManager _profileManager;

	[SerializeField]
	private TextMeshProUGUI _mapNameText;

	[SerializeField]
	private Image _mapImage;

	[SerializeField]
	private List<TDMLoadingPlate> _myTeamsPlates;

	[SerializeField]
	private List<TDMLoadingPlate> _enemyTeamsPlates;

	[SerializeField]
	private List<MapNameToScreenshot> _mapNameToScreenshots;

	private List<TDMLoadingPlate> _allPlates;

	private void Awake()
	{
		DisablePlates(_myTeamsPlates);
		DisablePlates(_enemyTeamsPlates);
		_allPlates = new List<TDMLoadingPlate>();
		_allPlates.AddRange(_myTeamsPlates);
		_allPlates.AddRange(_enemyTeamsPlates);
	}

	private IEnumerator Start()
	{
		yield return new WaitUntil(() => ClientTeamDeathMatchGameModeEntity.HasTDMGameMode && ClientTeamDeathMatchGameModeEntity.TDMGameMode.Map != null);
		PopulateMap(ClientTeamDeathMatchGameModeEntity.TDMGameMode.Map);
	}

	private void PopulateMap(string mapName)
	{
		string text = mapName;
		if (Constants.Match.MapNameToFriendlyName.ContainsKey(mapName))
		{
			text = Constants.Match.MapNameToFriendlyName[mapName];
		}
		_mapNameText.text = text;
		Sprite screenshot = _mapNameToScreenshots.FirstOrDefault((MapNameToScreenshot x) => x.Map == mapName).Screenshot;
		if (screenshot != null)
		{
			_mapImage.overrideSprite = screenshot;
			_mapImage.DOFade(1f, 1f);
		}
	}

	private void OnDestroy()
	{
		_signalBus.TryUnsubscribe<ClientPlayerLoadedSignal>(OnPlayerLoaded);
	}

	private void DisablePlates(List<TDMLoadingPlate> plates)
	{
		foreach (TDMLoadingPlate plate in plates)
		{
			plate.gameObject.SetActive(value: false);
		}
	}

	public void Populate(List<MatchedMember> matchmakingPlayers)
	{
		MatchedMember localPlayerMM = matchmakingPlayers.FirstOrDefault((MatchedMember p) => p.playerAttributes.entityId == _userManager.CurrentUser.Entity.Id);
		if (localPlayerMM == null)
		{
			Debug.LogError("[TDMMatchLoadingScreen] Unable to populate loading screen! Local Player is null");
			return;
		}
		List<MatchedMember> matchmakingPlayers2 = matchmakingPlayers.Where((MatchedMember x) => x.teamId == localPlayerMM?.teamId).ToList();
		List<MatchedMember> matchmakingPlayers3 = matchmakingPlayers.Where((MatchedMember x) => x.teamId != localPlayerMM?.teamId).ToList();
		PopulatePlayerList(matchmakingPlayers2, _myTeamsPlates);
		PopulatePlayerList(matchmakingPlayers3, _enemyTeamsPlates);
		_signalBus.Subscribe<ClientPlayerLoadedSignal>(OnPlayerLoaded);
	}

	private void PopulatePlayerList(List<MatchedMember> matchmakingPlayers, List<TDMLoadingPlate> plates)
	{
		int num = 0;
		foreach (MatchedMember matchmakingPlayer in matchmakingPlayers)
		{
			BaseProfile byId = _profileManager.GetById(matchmakingPlayer.playerAttributes.outfit);
			Sprite outfitSprite = null;
			if (byId != null)
			{
				outfitSprite = byId.Icon;
			}
			else
			{
				Debug.LogError("[TDMMatchLoadingScreen] Unable to get outfitProfile for player " + matchmakingPlayer.playerAttributes.displayName + " with Id " + matchmakingPlayer.playerAttributes.outfit);
			}
			if (plates[num] == null)
			{
				Debug.LogError("[TDMMatchLoadingScreen] Ran out of UI plates! Player setup is incorrect");
				break;
			}
			plates[num].Populate(matchmakingPlayer.playerAttributes.displayName, outfitSprite);
			num++;
		}
	}

	private void OnPlayerLoaded(ClientPlayerLoadedSignal signal)
	{
		Debug.Log("[TDMMatchLoadingScreen] Player Loaded! " + signal.DisplayName);
		TDMLoadingPlate tDMLoadingPlate = _allPlates.FirstOrDefault((TDMLoadingPlate p) => p.DisplayName == signal.DisplayName);
		if (tDMLoadingPlate != null)
		{
			tDMLoadingPlate.ShowLoaded();
		}
		else
		{
			Debug.LogError("[TDMMatchLoadingScreen] Unable to find plate for player with name " + signal.DisplayName);
		}
	}
}
