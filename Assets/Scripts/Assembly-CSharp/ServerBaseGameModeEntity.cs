using System.Collections;
using BSCore;
using Bolt;
using UnityEngine;
using Zenject;

public abstract class ServerBaseGameModeEntity<TState> : EntityEventListener<TState>
{
	protected const float MATCHEND_SHUTDOWN_TIME = 60f;

	[Inject]
	protected SignalBus _signalBus;

	[Inject]
	protected ConfigManager _configManager;

	[SerializeField]
	protected GameModeEntityHelper _helper;

	protected int _expectedPlayerCount;

	protected float _playerLoadingTimeoutTimer;

	protected bool _playerLoadingStarted;

	public override void Attached()
	{
		if (base.entity.isOwner)
		{
			_signalBus.Subscribe<PlayerLoadedSignal>(OnPlayerLoaded);
			_signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
		}
	}

	public override void Detached()
	{
		if (base.entity.isOwner)
		{
			_signalBus.TryUnsubscribe<PlayerLoadedSignal>(OnPlayerLoaded);
			_signalBus.TryUnsubscribe<PlayerDiedSignal>(OnPlayerDied);
		}
	}

	protected abstract void OnPlayerLoaded(PlayerLoadedSignal playerLoadedSignal);

	protected abstract void OnPlayerDied(PlayerDiedSignal playerDiedSignal);

	protected abstract void UpdateMatchEndState();

	protected virtual IEnumerator EndMatch()
	{
		ServerReporter.TrySendServerReport();
		UpdateMatchEndState();
		yield return new WaitForSeconds(60f);
		Debug.Log("[ServerBaseGameModeEntity] Match has ended. Shutting down server");
		ConnectionManager.DisconnectedFromBolt += OnDisconnectFromBolt;
		ConnectionManager.Shutdown();
	}

	protected void OnDisconnectFromBolt()
	{
		ConnectionManager.DisconnectedFromBolt -= OnDisconnectFromBolt;
		if (PlayfabServerManagement.IsInitializedAndReady)
		{
			PlayfabServerManagement.ShutdownServer();
		}
		else
		{
			Application.Quit();
		}
	}

	protected void SetPlayerInputState(IPlayerState playerState, bool inputEnabled)
	{
		playerState.InputEnabled = inputEnabled;
	}
}
