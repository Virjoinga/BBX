using TMPro;
using UnityEngine;
using Zenject;

public class TimeRemainingDisplay : MonoBehaviour
{
	private const string TIME_FORMAT = "{0:00}:{1:00}";

	private const int SECONDS_IN_MINUTE = 60;

	private const float WARNING_TIME = 30f;

	[Inject]
	protected SignalBus _signalBus;

	[SerializeField]
	private TextMeshProUGUI _timerText;

	private float _matchEndTime;

	private void OnEnable()
	{
		_signalBus.Subscribe<MatchTimesUpdatedSignal>(OnMatchEndTimeUpdated);
	}

	private void OnDisable()
	{
		_signalBus.Unsubscribe<MatchTimesUpdatedSignal>(OnMatchEndTimeUpdated);
	}

	private void OnMatchEndTimeUpdated(MatchTimesUpdatedSignal matchEndTimeUpdatedSignal)
	{
		_matchEndTime = matchEndTimeUpdatedSignal.MatchEndTime;
	}

	private void FixedUpdate()
	{
		if (ClientTeamDeathMatchGameModeEntity.HasTDMGameMode)
		{
			float num = ClientTeamDeathMatchGameModeEntity.TDMGameMode.MatchEndTime - BoltNetwork.ServerTime;
			int num2 = (int)num / 60;
			int num3 = (int)num % 60;
			if (num <= 30f)
			{
				_timerText.color = Color.red;
			}
			else
			{
				_timerText.color = Color.white;
			}
			_timerText.text = $"{num2:00}:{num3:00}";
		}
	}
}
