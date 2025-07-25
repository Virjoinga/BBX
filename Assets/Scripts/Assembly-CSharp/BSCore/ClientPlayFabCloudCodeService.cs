using System;
using BSCore.Constants.CloudCode;
using BSCore.Utils;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace BSCore
{
	public class ClientPlayFabCloudCodeService : PlayFabService, ICloudCodeService
	{
		public void Run(FunctionName functionName, object parameters, Action<object> onComplete, Action<FailureReasons> onFailed)
		{
			CallAPI(functionName.ToString(), parameters, onComplete, onFailed);
		}

		public void Run<T>(FunctionName functionName, object parameters, Action<T> onComplete, Action<FailureReasons> onFailed)
		{
			CallAPI(functionName.ToString(), parameters, delegate(object result)
			{
				onComplete(JsonUtility.FromJson<T>(result.ToString()));
			}, onFailed);
		}

		private void CallAPI(string functionName, object parameters, Action<object> onComplete, Action<FailureReasons> onFailed)
		{
			ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
			{
				FunctionName = functionName,
				FunctionParameter = parameters,
				RevisionSelection = CloudScriptRevisionOption.Live
			};
			Action<PlayFabError> onFailure = OnFailureCallback(delegate
			{
				CallAPI(functionName, parameters, onComplete, onFailed);
			}, onFailed);
			PlayFabClientAPI.ExecuteCloudScript(request, delegate(ExecuteCloudScriptResult result)
			{
				if (result.Error != null)
				{
					Debug.LogError("[PlayFabCloudCodeService] Error running cloud script function: " + PlayFabUtils.ParseError(result.Error));
					onFailed(FailureReasons.CloudScriptExecutionError);
				}
				else
				{
					onComplete(result.FunctionResult);
				}
			}, delegate(PlayFabError error)
			{
				Debug.LogError("[PlayFabCloudCodeService] Error running cloud script function: " + PlayFabUtils.ParseError(error));
				onFailure(error);
			});
		}
	}
}
