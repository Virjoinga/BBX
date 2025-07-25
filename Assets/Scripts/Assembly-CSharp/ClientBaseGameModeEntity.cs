using Bolt;
using UnityEngine;
using Zenject;

public class ClientBaseGameModeEntity<TState> : EntityEventListener<TState>
{
	[Inject]
	protected SignalBus _signalBus;

	[SerializeField]
	protected GameModeEntityHelper _helper;

	public static int RemainingPlayers { get; protected set; }

	public static int RemainingFirstLifePlayers { get; protected set; }

	public static MatchState MatchState { get; protected set; }
}
