namespace NodeClient
{
	public class FriendStatusUpdatedCrumb
	{
		public string i;

		public int s;

		public string Id => i;

		public PlayerStatus Status => (PlayerStatus)s;

		public override string ToString()
		{
			return $"FriendStatusUpdatedCrumb(Id: {i}, Status: {Status})";
		}
	}
}
