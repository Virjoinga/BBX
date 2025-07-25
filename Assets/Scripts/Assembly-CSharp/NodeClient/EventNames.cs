namespace NodeClient
{
	public static class EventNames
	{
		public static readonly string Reconnect = "reconnect";

		public static readonly string Reconnecting = "reconnecting";

		public static readonly string ReconnectAttempt = "reconnect_attempt";

		public static readonly string ReconnectFailed = "reconnect_failed";

		public static readonly string Login = "Login";

		public static readonly string LoggedIn = "LoggedIn";

		public static readonly string UpdatePlayerStatus = "S";

		public static readonly string PlayerAdded = "A";

		public static readonly string PlayerRemoved = "R";

		public static readonly string PlayerUpdated = "U";

		public static readonly string JoinChannel = "J";

		public static readonly string LeaveChannel = "L";

		public static readonly string JoinedChannel = "JoinedChannel";

		public static readonly string LeftChannel = "LeftChannel";

		public static readonly string PublicMessage = "M";

		public static readonly string PrivateMessage = "P";

		public static readonly string TimeoutPlayer = "TP";

		public static readonly string BlockPlayer = "BP";

		public static readonly string UnblockPlayer = "UP";

		public static readonly string RequestFriend = "FR";

		public static readonly string ConfirmFriend = "FC";

		public static readonly string RemoveFriend = "FX";

		public static readonly string FriendListUpdated = "FU";

		public static readonly string FriendStatusUpdated = "FS";

		public static readonly string Invite = "GI";

		public static readonly string RespondToInvite = "GRI";

		public static readonly string LeaveGroup = "GL";

		public static readonly string RelayMatchmakingTicketId = "GM";

		public static readonly string RelayMatchmakingCanceled = "GMX";

		public static readonly string Promote = "GP";

		public static readonly string Remove = "GX";

		public static readonly string InviteResponse = "GIR";

		public static readonly string RespondToInviteResponse = "GRIR";

		public static readonly string LeaveGroupResponse = "GLR";

		public static readonly string RelayMatchmakingTicketIdResponse = "GMR";

		public static readonly string PromoteResponse = "GPR";

		public static readonly string RemoveResponse = "GXR";

		public static readonly string Invited = "GID";

		public static readonly string InviteRejected = "GIX";

		public static readonly string GroupUpdated = "GU";

		public static readonly string MatchmakingJoined = "GMD";

		public static readonly string MatchmakingCanceled = "GMXD";
	}
}
