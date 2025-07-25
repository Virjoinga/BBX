using TMPro;
using UnityEngine;
using Zenject;

public class BRPlayersRemainingDisplay : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private TextMeshProUGUI _remaining1stLife;

	[SerializeField]
	private TextMeshProUGUI _remaining2ndLife;

	[SerializeField]
	private GameObject _remaining2ndLifeContainer;

	[Inject]
	private void PostConstruct()
	{
		_signalBus.Subscribe<BattleRoyaleRemainingPlayersUpdatedSignal>(OnRemainingPlayersUpdated);
	}

	private void Awake()
	{
		OnRemainingPlayersUpdated();
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<BattleRoyaleRemainingPlayersUpdatedSignal>(OnRemainingPlayersUpdated);
	}

	private void OnRemainingPlayersUpdated()
	{
		_remaining1stLife.text = ClientBaseGameModeEntity<IBattleRoyaleGameModeState>.RemainingFirstLifePlayers.ToString();
		int num = ClientBaseGameModeEntity<IBattleRoyaleGameModeState>.RemainingPlayers - ClientBaseGameModeEntity<IBattleRoyaleGameModeState>.RemainingFirstLifePlayers;
		_remaining2ndLifeContainer.gameObject.SetActive(num > 0);
		_remaining2ndLife.text = num.ToString();
	}
}
