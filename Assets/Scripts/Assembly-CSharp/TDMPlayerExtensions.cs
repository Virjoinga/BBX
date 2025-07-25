public static class TDMPlayerExtensions
{
	public static float KDA(this TDMPlayerState player)
	{
		return (float)(player.Kills - player.Deaths) + (float)player.Assists / 3f;
	}
}
