using System.Collections.Generic;
using System.Linq;
using BSCore;
using UnityEngine;
using Zenject;

public class TDMScoreboard : MonoBehaviour
{
	private const float FADE_TIME = 0.1f;

	[Inject]
	private SignalBus _signalBus;

	[Inject]
	private ProfileManager _profileManager;

	[SerializeField]
	private FadeableUI _container;

	[SerializeField]
	private List<TDMScoreboardPlate> _myTeamPlates;

	[SerializeField]
	private List<TDMScoreboardPlate> _enemyTeamPlates;

	private readonly float _maxFadeIn = 0.77f;

	private bool _isShowing;

	private bool _matchHasEnded;

	private void Awake()
	{
		_container.FadeOut(0f);
		DisablePlayerPlates(_myTeamPlates);
		DisablePlayerPlates(_enemyTeamPlates);
		_signalBus.Subscribe<TDMPlayersUpdatedSignal>(OnPlayersUpdated);
		_signalBus.Subscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<TDMPlayersUpdatedSignal>(OnPlayersUpdated);
		_signalBus.Unsubscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
	}

	private void OnMatchStateUpdated(MatchStateUpdatedSignal matchStateUpdatedSignal)
	{
		if (matchStateUpdatedSignal.MatchState == MatchState.Complete)
		{
			_matchHasEnded = true;
		}
	}

	private void DisablePlayerPlates(List<TDMScoreboardPlate> playerPlates)
	{
		foreach (TDMScoreboardPlate playerPlate in playerPlates)
		{
			playerPlate.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (_matchHasEnded || ActiveUI.Manager.IsActiveUIShown)
		{
			if (_isShowing)
			{
				_container.FadeOut(0.1f);
				_isShowing = false;
			}
			return;
		}
		if (BSCoreInput.GetButtonDown(Option.Scoreboard))
		{
			_container.FadeTo(_maxFadeIn, 0.1f, isInteractable: false, null);
			_isShowing = true;
		}
		if (BSCoreInput.GetButtonUp(Option.Scoreboard))
		{
			_container.FadeOut(0.1f);
			_isShowing = false;
		}
	}

	private void OnPlayersUpdated(TDMPlayersUpdatedSignal signal)
	{
		if (PlayerController.HasLocalPlayer && PlayerController.LocalPlayer.HasTeamSet)
		{
			int localPlayerTeam = PlayerController.LocalPlayer.Team;
			List<TDMPlayerState> players = signal.Players;
			List<TDMPlayerState> players2 = players.Where((TDMPlayerState p) => p.Team == localPlayerTeam).ToList();
			List<TDMPlayerState> players3 = players.Where((TDMPlayerState p) => p.Team != localPlayerTeam).ToList();
			TryPopulateOrUpdateStats(players2, _myTeamPlates);
			TryPopulateOrUpdateStats(players3, _enemyTeamPlates);
		}
	}

	private void TryPopulateOrUpdateStats(List<TDMPlayerState> players, List<TDMScoreboardPlate> plates)
	{
		foreach (TDMPlayerState player in players)
		{
			if (string.IsNullOrEmpty(player.DisplayName) || string.IsNullOrEmpty(player.Loadout))
			{
				continue;
			}
			TDMScoreboardPlate tDMScoreboardPlate = plates.FirstOrDefault((TDMScoreboardPlate x) => x.HasPlayer && x.Player.EntityId == player.EntityId);
			if (tDMScoreboardPlate == null)
			{
				LoadoutData loadoutData = LoadoutExtensions.Deserialize(player.Loadout);
				OutfitProfile byId = _profileManager.GetById<OutfitProfile>(loadoutData.Outfit);
				plates.FirstOrDefault((TDMScoreboardPlate x) => !x.HasPlayer).Populate(player, byId.HeroClass);
			}
			else
			{
				LoadoutData loadoutData2 = LoadoutExtensions.Deserialize(player.Loadout);
				OutfitProfile byId2 = _profileManager.GetById<OutfitProfile>(loadoutData2.Outfit);
				tDMScoreboardPlate.UpdatePlate(player, byId2.HeroClass);
			}
		}
		plates.Sort((TDMScoreboardPlate a, TDMScoreboardPlate b) => b.KDA.CompareTo(a.KDA));
		for (int num = 0; num < plates.Count; num++)
		{
			plates[num].transform.SetSiblingIndex(num);
		}
	}
}
