using System;
using System.Collections.Generic;

namespace NodeClient
{
	public static class SocketClientRequestTracker
	{
		private struct RequestData
		{
			public ISocketClientRequest Request;

			public Action<ISocketClientRequest, ISocketClientResponse> OnResponse;

			public Action<string> OnFailure;

			public RequestData(ISocketClientRequest request, Action<ISocketClientRequest, ISocketClientResponse> onResponse, Action<string> onFailure)
			{
				Request = request;
				OnResponse = onResponse;
				OnFailure = onFailure;
			}
		}

		private static ulong _idTracker = 0uL;

		private static readonly Dictionary<ulong, RequestData> RequestsById = new Dictionary<ulong, RequestData>();

		public static void RegisterRequest(ISocketClientRequest request, Action<ISocketClientRequest, ISocketClientResponse> onResponse, Action<string> onFailure)
		{
			request.id = _idTracker++;
			RequestsById.Add(request.id, new RequestData(request, onResponse, onFailure));
		}

		public static void HandleResponse(ISocketClientResponse response)
		{
			if (RequestsById.TryGetValue(response.Id, out var value))
			{
				if (response.Success)
				{
					value.OnResponse?.Invoke(value.Request, response);
				}
				else
				{
					value.OnFailure?.Invoke(response.ErrorMsg);
				}
				RequestsById.Remove(response.Id);
			}
		}
	}
}
