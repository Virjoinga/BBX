using System;
using BestHTTP;
using BestHTTP.Examples;
using BestHTTP.WebSocket;
using UnityEngine;

public class WebSocketSample : MonoBehaviour
{
	private string address = "wss://echo.websocket.org";

	private string msgToSend = "Hello World!";

	private string Text = string.Empty;

	private WebSocket webSocket;

	private Vector2 scrollPos;

	private void OnDestroy()
	{
		if (webSocket != null)
		{
			webSocket.Close();
		}
	}

	private void OnGUI()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, drawHeader: true, delegate
		{
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			GUILayout.Label(Text);
			GUILayout.EndScrollView();
			GUILayout.Space(5f);
			GUILayout.FlexibleSpace();
			address = GUILayout.TextField(address);
			if (webSocket == null && GUILayout.Button("Open Web Socket"))
			{
				webSocket = new WebSocket(new Uri(address));
				if (HTTPManager.Proxy != null)
				{
					webSocket.InternalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, isTransparent: false);
				}
				WebSocket obj = webSocket;
				obj.OnOpen = (OnWebSocketOpenDelegate)Delegate.Combine(obj.OnOpen, new OnWebSocketOpenDelegate(OnOpen));
				WebSocket obj2 = webSocket;
				obj2.OnMessage = (OnWebSocketMessageDelegate)Delegate.Combine(obj2.OnMessage, new OnWebSocketMessageDelegate(OnMessageReceived));
				WebSocket obj3 = webSocket;
				obj3.OnClosed = (OnWebSocketClosedDelegate)Delegate.Combine(obj3.OnClosed, new OnWebSocketClosedDelegate(OnClosed));
				WebSocket obj4 = webSocket;
				obj4.OnError = (OnWebSocketErrorDelegate)Delegate.Combine(obj4.OnError, new OnWebSocketErrorDelegate(OnError));
				webSocket.Open();
				Text += "Opening Web Socket...\n";
			}
			if (webSocket != null && webSocket.IsOpen)
			{
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal();
				msgToSend = GUILayout.TextField(msgToSend);
				if (GUILayout.Button("Send", GUILayout.MaxWidth(70f)))
				{
					Text += "Sending message...\n";
					webSocket.Send(msgToSend);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				if (GUILayout.Button("Close"))
				{
					webSocket.Close(1000, "Bye!");
				}
			}
		});
	}

	private void OnOpen(WebSocket ws)
	{
		Text += $"-WebSocket Open!\n";
	}

	private void OnMessageReceived(WebSocket ws, string message)
	{
		Text += $"-Message received: {message}\n";
	}

	private void OnClosed(WebSocket ws, ushort code, string message)
	{
		Text += $"-WebSocket closed! Code: {code} Message: {message}\n";
		webSocket = null;
	}

	private void OnError(WebSocket ws, Exception ex)
	{
		string text = string.Empty;
		if (ws.InternalRequest.Response != null)
		{
			text = $"Status Code from Server: {ws.InternalRequest.Response.StatusCode} and Message: {ws.InternalRequest.Response.Message}";
		}
		Text += string.Format("-An error occured: {0}\n", (ex != null) ? ex.Message : ("Unknown Error " + text));
		webSocket = null;
	}
}
