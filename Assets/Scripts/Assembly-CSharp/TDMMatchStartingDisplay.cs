using TMPro;
using UnityEngine;
using Zenject;

public class TDMMatchStartingDisplay : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private GameObject _waitingForPlayersContainer;

	[SerializeField]
	private GameObject _matchStartingContainer;

	[SerializeField]
	private TextMeshProUGUI _connectedPlayersText;

	[SerializeField]
	private TextMeshProUGUI _matchForceStartTimerText;

	[SerializeField]
	private TextMeshProUGUI _matchStartingTimerText;

	private int _connectedPlayers;

	private int _expectedPlayers;

	private void Start()
	{
		if (ClientTeamDeathMatchGameModeEntity.HasTDMGameMode)
		{
			_expectedPlayers = ClientTeamDeathMatchGameModeEntity.TDMGameMode.ExpectedPlayerCount;
		}
		UpdatedConnectedPlayersText();
		_signalBus.Subscribe<MatchExpectedPlayerCountUpdatedSignal>(OnExpectedPlayerCountUpdated);
		_signalBus.Subscribe<TDMPlayersUpdatedSignal>(OnTDMPlayersUpdated);
		_signalBus.Subscribe<MatchPlayersLoadedUpdatedSignal>(OnMatchPlayersLoadedUpdated);
		_signalBus.Subscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
		if (ClientTeamDeathMatchGameModeEntity.TDMGameMode != null && ClientTeamDeathMatchGameModeEntity.TDMGameMode.state.MatchState == 1)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<MatchExpectedPlayerCountUpdatedSignal>(OnExpectedPlayerCountUpdated);
		_signalBus.Unsubscribe<TDMPlayersUpdatedSignal>(OnTDMPlayersUpdated);
		_signalBus.Unsubscribe<MatchPlayersLoadedUpdatedSignal>(OnMatchPlayersLoadedUpdated);
		_signalBus.Unsubscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
	}

	private void OnExpectedPlayerCountUpdated(MatchExpectedPlayerCountUpdatedSignal signal)
	{
		_expectedPlayers = signal.ExpectedPlayerCount;
		UpdatedConnectedPlayersText();
	}

	private void OnTDMPlayersUpdated(TDMPlayersUpdatedSignal signal)
	{
		_connectedPlayers = signal.Players.Count;
		UpdatedConnectedPlayersText();
	}

	private void UpdatedConnectedPlayersText()
	{
		_connectedPlayersText.text = $"Waiting For Players ({_connectedPlayers}/{_expectedPlayers})";
	}

	private void OnMatchPlayersLoadedUpdated(MatchPlayersLoadedUpdatedSignal signal)
	{
		if (signal.PlayersLoaded)
		{
			_waitingForPlayersContainer.SetActive(value: false);
			_matchStartingContainer.SetActive(value: true);
		}
	}

	private void FixedUpdate()
	{
		if (!ClientTeamDeathMatchGameModeEntity.HasTDMGameMode)
		{
			return;
		}
		float num = ClientTeamDeathMatchGameModeEntity.TDMGameMode.MatchStartTime - BoltNetwork.ServerTime;
		if (num > 0f)
		{
			if (num <= 3f)
			{
				_matchForceStartTimerText.color = Color.red;
			}
			else
			{
				_matchForceStartTimerText.color = Color.white;
			}
			_matchForceStartTimerText.text = $"{num:0}";
			_matchStartingTimerText.text = $"{num:0}";
		}
		else
		{
			_matchForceStartTimerText.text = string.Empty;
			_matchStartingTimerText.text = string.Empty;
		}
	}

	private void OnMatchStateUpdated(MatchStateUpdatedSignal signal)
	{
		if (signal.MatchState == MatchState.Active)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
