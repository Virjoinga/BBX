using System;
using System.Collections;
using System.Collections.Generic;
using BSCore;
using PlayFab;
using PlayFab.Internal;
using UnityEngine;
using Zenject;

public class ServerBootloader : ServerBSCoreBootloader
{
	[Inject]
	private ConfigManager _configManager;

	[Inject]
	private ProfileManager _profileManager;

	[SerializeField]
	private GameModeType _gameMode = GameModeType.BattleRoyale;

	private bool _configFetched;

	private bool _profilesFetched;

	public GameModeType GameMode => _gameMode;

	protected void Start()
	{
		Debug.Log("[ServerBootloader] Init server at " + DateTime.Now.ToString("HH:mm:ss tt"));
		Application.targetFrameRate = 60;
		PlayFabSettings.RequestType = WebRequestType.HttpWebRequest;
		PlayFabWebRequest.SkipCertificateValidation();
		if (CommandLineArgsManager.IsHeadlessMode())
		{
			PlayfabServerManagement.InitServer();
		}
		Debug.Log("[ServerBootloader] After PlayFab server startup before Playfab API calls");
		_configManager.Fetch(OnConfigFetched, OnFetchFailed);
		_profileManager.Fetch(_gameConfig.DefaultCatalog, OnProfilesFetched, OnFetchFailed);
	}

	protected void OnConfigFetched()
	{
		Debug.Log("Playfab Title Data Fetched");
		_configFetched = true;
		OnDataFetched();
	}

	protected void OnProfilesFetched(List<BaseProfile> profiles)
	{
		Debug.Log("Playfab Catalog Profiles Fetched");
		_profilesFetched = true;
		OnDataFetched();
	}

	private void OnDataFetched()
	{
		Debug.Log($"[ServerBootloader] Start of On Data Fetched. Profiles Fetched? {_profilesFetched} | Config Data Fetched? {_configFetched}");
		if (_profilesFetched && _configFetched)
		{
			Debug.Log("[ServerBootloader] Data fetched");
			ConnectionManager.InitialConnect -= OnInitialConnect;
			ConnectionManager.InitialConnect += OnInitialConnect;
			if (CommandLineArgsManager.IsHeadlessMode())
			{
				StartCoroutine(StartServerInternal());
			}
			else
			{
				ConnectionManager.StartServer();
			}
		}
	}

	private IEnumerator StartServerInternal()
	{
		while (!PlayfabServerManagement.IsInitializedAndReady)
		{
			Debug.Log("[ServerBootloader] Loading done, now wait until server state is active at " + DateTime.Now.ToString("HH:mm:ss tt"));
			PlayfabServerManagement.StandByUntilActive();
			yield return new WaitUntil(() => PlayfabServerManagement.IsInitializedAndReady);
		}
		Debug.Log("[ServerBootloader] state switch to active, boot server at " + DateTime.Now.ToString("HH:mm:ss tt"));
		ConnectionManager.StartServer();
	}

	private void OnFetchFailed(FailureReasons reason)
	{
		Debug.LogError("[ServerBootloader] Failed to fetch game data... This is a problem");
	}

	private void OnInitialConnect()
	{
		Debug.Log("[ServerBootloader] Bolt Connected");
		Debug.Log("[ServerBootloader] Connected bolt at " + DateTime.Now.ToString("HH:mm:ss tt"));
		ConnectionManager.InitialConnect -= OnInitialConnect;
		GameModeType gameModeType = ((!CommandLineArgsManager.IsHeadlessMode()) ? _gameMode : CommandLineArgsManager.GetArg("-gameMode", GameModeType.BattleRoyale));
		Debug.Log($"[ServerBootloader] Loading Scene: {gameModeType}GameMode");
		BoltNetwork.LoadScene($"{gameModeType}GameMode");
	}
}
