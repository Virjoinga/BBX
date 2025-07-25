public static class SurvivalGameModeStateExtensions
{
	public static bool TryGetPlayerByEntity(this ISurvivalGameModeState state, BoltEntity entity, out SurvivalPlayer player)
	{
		player = null;
		foreach (SurvivalPlayer player2 in state.Players)
		{
			if (player2.EntityId == entity)
			{
				player = player2;
				return true;
			}
		}
		return false;
	}
}
