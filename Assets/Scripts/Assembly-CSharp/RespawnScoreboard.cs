using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RespawnScoreboard : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[Inject]
	private ProfileManager _profileManager;

	[SerializeField]
	private List<TDMScoreboardPlate> _myTeamPlates;

	[SerializeField]
	private List<TDMScoreboardPlate> _enemyTeamPlates;

	[SerializeField]
	private RectTransform _layoutRoot;

	private void Awake()
	{
		DisablePlayerPlates(_myTeamPlates);
		DisablePlayerPlates(_enemyTeamPlates);
		_signalBus.Subscribe<TDMPlayersUpdatedSignal>(OnPlayersUpdated);
	}

	private void OnEnable()
	{
		StartCoroutine(RebuildAfterFrame());
	}

	private IEnumerator RebuildAfterFrame()
	{
		yield return new WaitForEndOfFrame();
		LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutRoot);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<TDMPlayersUpdatedSignal>(OnPlayersUpdated);
	}

	private void DisablePlayerPlates(List<TDMScoreboardPlate> playerPlates)
	{
		foreach (TDMScoreboardPlate playerPlate in playerPlates)
		{
			playerPlate.gameObject.SetActive(value: false);
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
