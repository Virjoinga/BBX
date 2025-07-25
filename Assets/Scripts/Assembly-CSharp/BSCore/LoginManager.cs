using System;
using Zenject;

namespace BSCore
{
	public class LoginManager
	{
		public class LoggedInSignal
		{
			public PlayerProfile player;

			public LoggedInSignal(PlayerProfile player)
			{
				this.player = player;
			}
		}

		private ILoginService _loginService;

		private SignalBus _signalBus;

		public bool IsLoggedIn => _loginService.IsLoggedIn;

		private event Action<PlayerProfile, bool> _loggedIn;

		public event Action<PlayerProfile, bool> LoggedIn
		{
			add
			{
				_loggedIn += value;
			}
			remove
			{
				_loggedIn -= value;
			}
		}

		[Inject]
		public LoginManager(ILoginService loginService, SignalBus signalBus)
		{
			_loginService = loginService;
			_signalBus = signalBus;
		}

		private Action<PlayerProfile, bool> OnLoginSuccessWrapper(Action<PlayerProfile, bool> onSuccess)
		{
			return delegate(PlayerProfile player, bool isNewlyCreated)
			{
				this._loggedIn?.Invoke(player, isNewlyCreated);
				onSuccess(player, isNewlyCreated);
				_signalBus.Fire(new LoggedInSignal(player));
			};
		}

		public void Login(string username, string password, Action<PlayerProfile, bool> onSuccess, Action<FailureReasons> onFailure)
		{
			_loginService.Login(username, password, OnLoginSuccessWrapper(onSuccess), onFailure);
		}

		public void LoginWithCustomId(string customId, bool createAccount, Action<PlayerProfile, bool> onSuccess, Action<FailureReasons> onFailure)
		{
			_loginService.LoginWithCustomId(customId, createAccount, OnLoginSuccessWrapper(onSuccess), onFailure);
		}

		public void LoginWithSteam(bool createAccount, Action<PlayerProfile, bool> onSuccess, Action<FailureReasons> onFailure)
		{
			_loginService.LoginWithSteam(createAccount, OnLoginSuccessWrapper(onSuccess), onFailure);
		}

		public void LoginWithEmail(string email, string password, Action<PlayerProfile, bool> onSuccess, Action<FailureReasons> onFailure)
		{
			_loginService.LoginWithEmail(email, password, OnLoginSuccessWrapper(onSuccess), onFailure);
		}

		public void AddUsernamePassword(string username, string password, string email, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_loginService.AddUsernamePassword(username, password, email, onSuccess, onFailure);
		}

		public void LinkSeamlessAccount(Action onSuccess, Action<FailureReasons> onFailure)
		{
		}

		public void LinkSteamAccount(string steamTicket, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_loginService.LinkSteamAccount(steamTicket, onSuccess, onFailure);
		}

		public void LinkCustomId(string customId, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_loginService.LinkCustomId(customId, onSuccess, onFailure);
		}

		public void SendAccountRecoveryEmail(string email, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_loginService.SendAccountRecoveryEmail(email, onSuccess, onFailure);
		}
	}
}
