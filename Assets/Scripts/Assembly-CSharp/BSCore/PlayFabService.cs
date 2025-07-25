using System;
using System.Collections.Generic;
using System.Text;
using BSCore.Utils;
using PlayFab;
using UnityEngine;

namespace BSCore
{
	public class PlayFabService
	{
		protected Action<PlayFabError> OnFailureCallback(Action retry, Action<FailureReasons> callback)
		{
			BackoffTimeout timeout = new BackoffTimeout();
			return delegate(PlayFabError error)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"Playfab Error: {error.Error}");
				stringBuilder.AppendLine("Message: " + error.ErrorMessage);
				stringBuilder.AppendLine("API: " + error.ApiEndpoint);
				stringBuilder.AppendLine($"HttpCode: {error.HttpCode}");
				stringBuilder.AppendLine("HttpStatus: " + error.HttpStatus);
				string text = ((error.CustomData == null) ? "Null" : error.CustomData.ToString());
				stringBuilder.AppendLine("CustomData: " + text);
				if (error.ErrorDetails == null || error.ErrorDetails.Count <= 0)
				{
					stringBuilder.AppendLine("ErrorDetails: Null or Empty");
				}
				else
				{
					stringBuilder.AppendLine("Error Details:");
					foreach (KeyValuePair<string, List<string>> errorDetail in error.ErrorDetails)
					{
						stringBuilder.AppendLine($"Key: {errorDetail.Key} | Value: {errorDetail.Value}");
					}
				}
				Debug.Log(stringBuilder.ToString());
				FailureReasons failureReasons = PlayFabUtils.ParseFailureReason(error);
				if (failureReasons != FailureReasons.WebTimeout || !timeout.RunAfterBackoff(retry))
				{
					Debug.LogErrorFormat("[ServiceError] {0}: {1}", error.Error, error.ErrorMessage);
					callback(failureReasons);
				}
			};
		}
	}
}
