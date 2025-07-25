using System;
using System.IO;
using BestHTTP.ServerSentEvents;
using BestHTTP.WebSocket;

namespace BestHTTP
{
	internal static class HTTPProtocolFactory
	{
		public static HTTPResponse Get(SupportedProtocols protocol, HTTPRequest request, Stream stream, bool isStreamed, bool isFromCache)
		{
			switch (protocol)
			{
			case SupportedProtocols.WebSocket:
				return new WebSocketResponse(request, stream, isStreamed, isFromCache);
			case SupportedProtocols.ServerSentEvents:
				return new EventSourceResponse(request, stream, isStreamed, isFromCache);
			default:
				return new HTTPResponse(request, stream, isStreamed, isFromCache);
			}
		}

		public static SupportedProtocols GetProtocolFromUri(Uri uri)
		{
			if (uri == null || uri.Scheme == null)
			{
				throw new Exception("Malformed URI in GetProtocolFromUri");
			}
			string text = uri.Scheme.ToLowerInvariant();
			if (text == "ws" || text == "wss")
			{
				return SupportedProtocols.WebSocket;
			}
			return SupportedProtocols.HTTP;
		}

		public static bool IsSecureProtocol(Uri uri)
		{
			if (uri == null || uri.Scheme == null)
			{
				throw new Exception("Malformed URI in IsSecureProtocol");
			}
			string text = uri.Scheme.ToLowerInvariant();
			if (text == "https" || text == "wss")
			{
				return true;
			}
			return false;
		}
	}
}
