using System;
using System.Collections.Generic;
using BSCore;
using MatchMaking;
using PlayFab;
using PlayFab.AuthenticationModels;
using UnityEngine;
using Zenject;

public class TeamDeathMatchHelper : PlayFabService
{
	private bool _hasAuthenticated;

	[Inject]
	private MatchMakerApi _matchMakerApi;

	public void EndMatch(string matchId)
	{
		Debug.Log("[TeamDeathMatchHelper] Ending Match with Id: " + matchId);
		_matchMakerApi.EndMatch(matchId, delegate
		{
			Debug.Log("Match with " + matchId + " ended");
		}, delegate
		{
			Debug.Log("Failed to end match with " + matchId + " ended");
		});
	}

	public void GetMatchPlayers(string matchId, string queueName, Action<List<MatchedMember>> matchPlayersFetched)
	{
		Debug.Log("[TeamDeathMatchHelper] Fetching Match with Id: " + matchId);
		_matchMakerApi.GetMatch(matchId, delegate(Match match)
		{
			Debug.Log("[TeamDeathMatchHelper] Got Match: " + match.matchId);
			matchPlayersFetched(match.members);
		}, delegate(ErrorResponse error)
		{
			Debug.LogError("[TeamDeathMatchHelper] Failed to get match: " + error);
		});
	}

	private void GetTitleEntityToken(string matchId, string queueName, Action<List<MatchedMember>> matchPlayersFetched)
	{
		PlayFabAuthenticationAPI.GetEntityToken(new GetEntityTokenRequest(), errorCallback: OnFailureCallback(delegate
		{
			GetTitleEntityToken(matchId, queueName, matchPlayersFetched);
		}, delegate(FailureReasons reason)
		{
			Debug.LogError("[TeamDeathMatchHelper] Failed to get entity token: " + reason);
		}), resultCallback: delegate(GetEntityTokenResponse result)
		{
			Debug.Log("[TeamDeathMatchHelper] Got title entity token: " + result.EntityToken);
			_hasAuthenticated = true;
			GetMatchPlayers(matchId, queueName, matchPlayersFetched);
		});
	}
}
