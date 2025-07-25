using System;
using BestHTTP.Examples;
using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using UnityEngine;

internal sealed class ConnectionStatusSample : MonoBehaviour
{
	private readonly Uri URI = new Uri("https://besthttpsignalr.azurewebsites.net/signalr");

	private Connection signalRConnection;

	private GUIMessageList messages = new GUIMessageList();

	private void Start()
	{
		signalRConnection = new Connection(URI, "StatusHub");
		signalRConnection.OnNonHubMessage += signalRConnection_OnNonHubMessage;
		signalRConnection.OnError += signalRConnection_OnError;
		signalRConnection.OnStateChanged += signalRConnection_OnStateChanged;
		signalRConnection["StatusHub"].OnMethodCall += statusHub_OnMethodCall;
		signalRConnection.Open();
	}

	private void OnDestroy()
	{
		signalRConnection.Close();
	}

	private void OnGUI()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, drawHeader: true, delegate
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("START") && signalRConnection.State != ConnectionStates.Connected)
			{
				signalRConnection.Open();
			}
			if (GUILayout.Button("STOP") && signalRConnection.State == ConnectionStates.Connected)
			{
				signalRConnection.Close();
				messages.Clear();
			}
			if (GUILayout.Button("PING") && signalRConnection.State == ConnectionStates.Connected)
			{
				signalRConnection["StatusHub"].Call("Ping");
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(20f);
			GUILayout.Label("Connection Status Messages");
			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			messages.Draw(Screen.width - 20, 0f);
			GUILayout.EndHorizontal();
		});
	}

	private void signalRConnection_OnNonHubMessage(Connection manager, object data)
	{
		messages.Add("[Server Message] " + data.ToString());
	}

	private void signalRConnection_OnStateChanged(Connection manager, ConnectionStates oldState, ConnectionStates newState)
	{
		messages.Add($"[State Change] {oldState} => {newState}");
	}

	private void signalRConnection_OnError(Connection manager, string error)
	{
		messages.Add("[Error] " + error);
	}

	private void statusHub_OnMethodCall(Hub hub, string method, params object[] args)
	{
		string arg = ((args.Length != 0) ? (args[0] as string) : string.Empty);
		string arg2 = ((args.Length > 1) ? args[1].ToString() : string.Empty);
		switch (method)
		{
		case "joined":
			messages.Add($"[{hub.Name}] {arg} joined at {arg2}");
			break;
		case "rejoined":
			messages.Add($"[{hub.Name}] {arg} reconnected at {arg2}");
			break;
		case "leave":
			messages.Add($"[{hub.Name}] {arg} leaved at {arg2}");
			break;
		default:
			messages.Add($"[{hub.Name}] {method}");
			break;
		}
	}
}
