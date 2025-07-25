using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

namespace BSCore
{
	public class ClientPlayFabConfigService : PlayFabService, IConfigService
	{
		public void Fetch(List<string> keys, Action<Dictionary<string, string>> onSuccess, Action<FailureReasons> onFailure)
		{
			GetTitleDataRequest getTitleDataRequest = new GetTitleDataRequest();
			if (keys != null)
			{
				getTitleDataRequest.Keys = keys;
			}
			Action<GetTitleDataResult> resultCallback = delegate(GetTitleDataResult result)
			{
				onSuccess(result.Data);
			};
			Action<PlayFabError> errorCallback = OnFailureCallback(delegate
			{
				Fetch(keys, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			});
			PlayFabClientAPI.GetTitleData(getTitleDataRequest, resultCallback, errorCallback);
		}
	}
}
