using System;

namespace BSCore
{
	public interface ILoginService
	{
		bool IsLoggedIn { get; }

		void Login(string username, string password, Action<PlayerProfile, bool> onSuccess, Action<FailureReasons> onFailure);

		void LoginWithCustomId(string customId, bool createAccount, Action<PlayerProfile, bool> onSuccess, Action<FailureReasons> onFailure);

		void LoginWithSteam(bool createAccount, Action<PlayerProfile, bool> onSuccess, Action<FailureReasons> onFailure);

		void LoginWithEmail(string email, string password, Action<PlayerProfile, bool> onSuccess, Action<FailureReasons> onFailure);

		void AddUsernamePassword(string username, string password, string email, Action onSuccess, Action<FailureReasons> onFailure);

		void LinkSteamAccount(string steamTicket, Action onSuccess, Action<FailureReasons> onFailure);

		void LinkCustomId(string customId, Action onSuccess, Action<FailureReasons> onFailure);

		void SendAccountRecoveryEmail(string email, Action onSuccess, Action<FailureReasons> onFailure);
	}
}
