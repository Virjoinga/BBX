using System;
using System.Collections.Generic;

namespace BSCore
{
	public interface IUserAuthenticationService
	{
		void AuthenticateUser(string sessionTicket, Action<PlayerProfile> onSuccess, Action<FailureReasons> onFailure);

		void FetchCombinedUserData(PlayerProfile.Data playerData, List<string> keys, Action<PlayerProfile> onSuccess, Action<FailureReasons> onFailure);
	}
}
