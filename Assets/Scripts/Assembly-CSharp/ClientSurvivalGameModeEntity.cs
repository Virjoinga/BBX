using Bolt;

public class ClientSurvivalGameModeEntity : ClientBaseGameModeEntity<ISurvivalGameModeState>
{
	private const float FORCE_LEAVE_TIME = 30f;

	public override void Attached()
	{
		if (!base.entity.isOwner)
		{
			base.state.AddCallback("Map", OnMapUpdated);
			base.state.AddCallback("MatchState", OnMatchStateUpdated);
			base.state.AddCallback("Players[]", OnPlayersUpdated);
			base.state.AddCallback("Wave", OnStateUpdated);
			base.state.AddCallback("EnemiesToKill", OnStateUpdated);
			_signalBus.Subscribe<LeaderboardInstantiatedSignal>(OnLeaderboardInstantiated);
		}
	}

	public override void Detached()
	{
		if (!base.entity.isOwner)
		{
			_signalBus.Unsubscribe<LeaderboardInstantiatedSignal>(OnLeaderboardInstantiated);
		}
	}

	private void OnLeaderboardInstantiated()
	{
		if (!base.entity.isOwner && base.entity.isAttached)
		{
			for (int i = 0; i < base.state.Players.Length; i++)
			{
				_signalBus.Fire(new LeaderboardUpdatedSignal(base.state.Players[i], i));
			}
		}
		OnStateUpdated();
	}

	private void OnPlayersUpdated(IState iState, string propertyPath, ArrayIndices arrayIndices)
	{
		int index = arrayIndices[0];
		_signalBus.Fire(new LeaderboardUpdatedSignal(base.state.Players[index], index));
	}

	private void OnMapUpdated()
	{
		_helper.LoadMap(base.state.Map);
	}

	private void OnMatchStateUpdated()
	{
		ClientBaseGameModeEntity<ISurvivalGameModeState>.MatchState = (MatchState)base.state.MatchState;
		_signalBus.Fire(new MatchStateUpdatedSignal(ClientBaseGameModeEntity<ISurvivalGameModeState>.MatchState));
	}

	private void OnStateUpdated()
	{
		_signalBus.Fire(new SurvivalStateUpdatedSignal(base.state.EnemiesToKill, base.state.Wave));
	}
}
