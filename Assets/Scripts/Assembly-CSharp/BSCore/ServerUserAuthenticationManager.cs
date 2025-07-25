using System;

namespace BSCore
{
	public class ServerUserAuthenticationManager
	{
		private IUserAuthenticationService _authService;

		public ServerUserAuthenticationManager(IUserAuthenticationService authService)
		{
			_authService = authService;
		}

		public void AuthenticateUser(string sessionTicket, Action<PlayerProfile> onSuccess, Action<FailureReasons> onFailure)
		{
			_authService.AuthenticateUser(sessionTicket, onSuccess, onFailure);
		}
	}
}
