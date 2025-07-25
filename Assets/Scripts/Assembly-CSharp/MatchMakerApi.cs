using System;
using BSCore;
using Duck.Http;
using Duck.Http.Service;
using MatchMaking;
using UnityEngine;
using Zenject;

public class MatchMakerApi : AbstractApi
{
	[Inject]
	protected GameConfigData _gameConfigData;

	private string baseUrl => _gameConfigData.GetMatchMakingServer();

	public void CreateMatchMakingTicket(TicketRequest request, Action<TicketCreatedResponse> onSuccess, Action<ErrorResponse> onError)
	{
		Debug.Log(JsonUtility.ToJson(request));
		Http.PostJson(baseUrl + "/ticket", JsonUtility.ToJson(request)).OnSuccess(delegate(HttpResponse res)
		{
			HandleSuccess(res, onSuccess, onError);
		}).OnError(delegate(HttpResponse res)
		{
			HandleFailure(res, onError);
		})
			.Send();
	}

	public void GetTicket(string ticketId, Action<TicketResponse> onSuccess, Action<ErrorResponse> onError)
	{
		Debug.Log("Get ticket " + ticketId);
		Http.Get(baseUrl + "/ticket-long-polling/" + ticketId).OnSuccess(delegate(HttpResponse res)
		{
			HandleSuccess(res, onSuccess, onError);
		}).OnError(delegate(HttpResponse res)
		{
			HandleFailure(res, onError);
		})
			.Send();
	}

	public void CancelTicket(string ticketId, Action<object> onSuccess, Action<ErrorResponse> onError)
	{
		Debug.Log("Canncel ticket with Id " + ticketId);
		Http.Delete(baseUrl + "/ticket/" + ticketId).OnSuccess(delegate(HttpResponse res)
		{
			HandleSuccess(res, onSuccess, onError);
		}).OnError(delegate(HttpResponse res)
		{
			HandleFailure(res, onError);
		})
			.Send();
	}

	public void GetMatch(string matchId, Action<Match> onSuccess, Action<ErrorResponse> onError)
	{
		Http.Get(baseUrl + "/match/" + matchId).OnSuccess(delegate(HttpResponse res)
		{
			HandleSuccess(res, onSuccess, onError);
		}).OnError(delegate(HttpResponse res)
		{
			HandleFailure(res, onError);
		})
			.Send();
	}

	public void EndMatch(string matchId, Action<object> onSuccess, Action<ErrorResponse> onError)
	{
		Http.Delete(baseUrl + "/match/" + matchId).OnSuccess(delegate(HttpResponse res)
		{
			HandleSuccess(res, onSuccess, onError);
		}).OnError(delegate(HttpResponse res)
		{
			HandleFailure(res, onError);
		})
			.Send();
	}
}
