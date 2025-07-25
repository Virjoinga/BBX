using System;
using System.Collections.Generic;
using Zenject;

namespace BSCore
{
	public class ServerUserAuthenticationService : PlayFabService, IUserAuthenticationService
	{
		private DiContainer _container;

		public ServerUserAuthenticationService(DiContainer container)
		{
			_container = container;
		}

		public void AuthenticateUser(string sessionTicket, Action<PlayerProfile> onSuccess, Action<FailureReasons> onFailure)
		{
		}

		public void FetchCombinedUserData(PlayerProfile.Data playerData, List<string> keys, Action<PlayerProfile> onSuccess, Action<FailureReasons> onFailure)
		{
		}

		private void FetchUserData(PlayerProfile.Data player, List<string> keys, Action<Dictionary<string, string>> onSuccess, Action<FailureReasons> onFailure)
		{
		}

		private void FetchUserReadOnlyData(PlayerProfile.Data player, List<string> keys, Action<Dictionary<string, string>> onSuccess, Action<FailureReasons> onFailure)
		{
		}

		public void FetchUserProfile(PlayerProfile.Data player, Action<string> onSuccess, Action<FailureReasons> onFailure)
		{
		}
	}
}
