using System;
using System.Collections;
using BSCore;
using NodeClient;
using RSG;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class ClientBootloader : ClientBSCoreBootloader
{
	[Inject]
	protected SteamAbstractionLayer _steamAbstractionLayer;

	[Inject]
	private SocketClient _socketClient;

	[SerializeField]
	private bool _connectToChat = true;

	protected override void Start()
	{
		Screen.sleepTimeout = -1;
		Debug.Log("[ClientBootloader] Scene loaded...");
		BeforeLogin(InitilizeSteam);
		AfterUserDataFetched(CheckDisplayName);
		AfterDone(OnDone);
		base.Start();
	}

	private IPromise InitilizeSteam()
	{
		Promise promise = new Promise();
		_steamAbstractionLayer.Initilize(delegate
		{
			promise.Resolve();
		});
		return promise;
	}

	private IPromise CheckDisplayName()
	{
		Debug.Log("[ClientBootloader] Checking display name");
		Promise promise = new Promise();
		if (_userManager.CurrentUser.HasDisplayName)
		{
			promise.Resolve();
			return promise;
		}
		UIPrefabManager.Instantiate(UIPrefabIds.DisplayNamePopup, delegate(GameObject go)
		{
			OnDisplayNamePopupCreated(go, promise);
		});
		return promise;
	}

	private void OnDisplayNamePopupCreated(GameObject uiGameobject, Promise promise)
	{
		if (uiGameobject == null)
		{
			promise.Reject(new Exception("Failed to Spawn Display Name UI"));
		}
		else
		{
			uiGameobject.GetComponent<DisplayNamePopup>().SetPromise(promise);
		}
	}

	private IPromise ConnectToChat()
	{
		Promise promise = new Promise();
		StartCoroutine(WaitForChatServerConnection(promise));
		return promise;
	}

	private IEnumerator WaitForChatServerConnection(Promise promise)
	{
		Debug.Log("[ClientBootloader] Waiting for chat server connection");
		yield return new MaxWaitUntil(() => _socketClient.IsConnected, 5f);
		Debug.Log("[ClientBootloader] Chat server connected");
		_socketClient.Authenticate(_userManager.CurrentUser.Id, _userManager.CurrentUser.SessionTicket);
		yield return new MaxWaitUntil(() => _socketClient.IsAuthenticated, 5f);
		if (_socketClient.IsAuthenticated)
		{
			Debug.Log("[ClientBootloader] Chat server authenticated");
		}
		else
		{
			_socketClient.Disconnect();
			Debug.LogError("[ClientBootloader] Failed to connect to chat server within time limit");
		}
		promise.Resolve();
	}

	protected virtual void OnDone()
	{
		Debug.Log("[ClientBootloader] Bootloading complete...");
		UIPrefabManager.Instantiate(UIPrefabIds.LoadingOverlay, OnLoadingOverlayCreated);
	}

	private void OnLoadingOverlayCreated(GameObject uiGameobject)
	{
		SceneManager.LoadSceneAsync("MainMenu");
	}
}
