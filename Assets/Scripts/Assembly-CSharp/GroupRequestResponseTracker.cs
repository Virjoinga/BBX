using System.Collections.Generic;

public static class GroupRequestResponseTracker
{
	public static readonly Dictionary<uint, InviteRequest> InviteRequestsById = new Dictionary<uint, InviteRequest>();
}
