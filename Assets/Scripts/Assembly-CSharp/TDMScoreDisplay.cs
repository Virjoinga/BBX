using TMPro;
using UnityEngine;
using Zenject;

public class TDMScoreDisplay : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private TextMeshProUGUI _allyTeamScore;

	[SerializeField]
	private TextMeshProUGUI _opponentTeamScore;

	private void Start()
	{
		_allyTeamScore.text = "0";
		_opponentTeamScore.text = "0";
		_signalBus.Subscribe<TDMScoresUpdatedSignal>(OnScoresUpdated);
		_signalBus.Subscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
		base.gameObject.SetActive(value: false);
		ClientTeamDeathMatchGameModeEntity tDMGameMode = ClientTeamDeathMatchGameModeEntity.TDMGameMode;
		if (tDMGameMode != null && (tDMGameMode.state.MatchState == 0 || tDMGameMode.state.MatchState == 1))
		{
			base.gameObject.SetActive(value: true);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<TDMScoresUpdatedSignal>(OnScoresUpdated);
		_signalBus.Unsubscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
	}

	private void OnScoresUpdated(TDMScoresUpdatedSignal tdmScoresUpdatedSignal)
	{
		_allyTeamScore.text = tdmScoresUpdatedSignal.AllyTeamScore.ToString();
		_opponentTeamScore.text = tdmScoresUpdatedSignal.OpponentTeamScore.ToString();
	}

	private void OnMatchStateUpdated(MatchStateUpdatedSignal signal)
	{
		if (signal.MatchState == MatchState.Active)
		{
			base.gameObject.SetActive(value: true);
		}
	}
}
