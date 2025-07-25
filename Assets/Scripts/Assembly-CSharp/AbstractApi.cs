using System;
using Duck.Http.Service;
using MatchMaking;
using UnityEngine;

public class AbstractApi
{
	public void HandleSuccess<T>(HttpResponse response, Action<T> successCallback, Action<ErrorResponse> errorCallback)
	{
		BaseResponse<T> baseResponse = JsonUtility.FromJson<BaseResponse<T>>(response.Text);
		if (!baseResponse.error)
		{
			successCallback(baseResponse.data);
			return;
		}
		errorCallback(new ErrorResponse
		{
			code = 0,
			message = baseResponse.message
		});
	}

	public void HandleFailure(HttpResponse response, Action<ErrorResponse> errorCallback)
	{
		errorCallback(new ErrorResponse
		{
			code = (int)response.StatusCode,
			message = response.Text
		});
	}
}
