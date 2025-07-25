using System;
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Networking;

namespace BSCore
{
	public class PlayFabRemoteFileService : PlayFabService, IRemoteFileService
	{
		public void FetchFile(string fileId, Action<byte[]> onSuccess, Action<FailureReasons> onFailure)
		{
			GetContentDownloadUrlRequest request = new GetContentDownloadUrlRequest
			{
				Key = fileId
			};
			Action<GetContentDownloadUrlResult> resultCallback = delegate(GetContentDownloadUrlResult result)
			{
				DelayedAction.RunCoroutine(DownloadFile(result.URL, onSuccess, onFailure));
			};
			Action<PlayFabError> errorCallback = OnFailureCallback(delegate
			{
				FetchFile(fileId, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			});
			PlayFabClientAPI.GetContentDownloadUrl(request, resultCallback, errorCallback);
		}

		private IEnumerator DownloadFile(string url, Action<byte[]> onSuccess, Action<FailureReasons> onFailure)
		{
			using (UnityWebRequest uwr = UnityWebRequest.Get(url))
			{
				yield return uwr.SendWebRequest();
				if (uwr.isNetworkError || uwr.isHttpError)
				{
					Debug.LogWarningFormat("[RemoteFileService] Failed to download file. Error: {0}", uwr.error);
					onFailure(FailureReasons.WebTimeout);
				}
				else
				{
					onSuccess(uwr.downloadHandler.data);
				}
			}
		}
	}
}
