using BSCore;

namespace NodeClient
{
	public static class PlayerProfileExtensions
	{
		public static bool IsMe(this PlayerProfile player, PlayerCrumb playerCrumb)
		{
			return playerCrumb.Id == player.Id;
		}
	}
}
