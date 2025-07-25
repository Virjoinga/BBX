using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Zenject;

namespace BSCore
{
	public class UserManager
	{
		private IUserService _userService;

		public static PlayerProfile LocalUser { get; private set; }

		public PlayerProfile CurrentUser { get; private set; }

		private event Action _userPropertiesFilled;

		public event Action UserPropertiesFilled
		{
			add
			{
				_userPropertiesFilled += value;
			}
			remove
			{
				_userPropertiesFilled -= value;
			}
		}

		public static bool IsLocalUser(string userId)
		{
			if (LocalUser != null)
			{
				return LocalUser.Id == userId;
			}
			return false;
		}

		[Inject]
		public UserManager(IUserService userService, LoginManager loginManager, SignalBus signalBus)
		{
			_userService = userService;
			signalBus.Subscribe(delegate(LoginManager.LoggedInSignal signal)
			{
				CurrentUser = signal.player;
			});
			loginManager.LoggedIn += OnUserLoggedIn;
		}

		private void OnUserLoggedIn(PlayerProfile player, bool isNewlyCreated)
		{
			CurrentUser = player;
			if (IsValidDisplayName(CurrentUser.DisplayName))
			{
				this._userPropertiesFilled?.Invoke();
			}
			LocalUser = CurrentUser;
		}

		public void FetchServiceIdByDisplayName(string displayName, Action<string> onSuccess, Action<FailureReasons> onFailure)
		{
			_userService.FetchServiceIdByDisplayName(displayName, onSuccess, onFailure);
		}

		public void FetchUserData(Action onSuccess, Action<FailureReasons> onFailure)
		{
			FetchUserData(CurrentUser, onSuccess, onFailure);
		}

		public void FetchUserData(List<string> keys, Action onSuccess, Action<FailureReasons> onFailure)
		{
			FetchUserData(CurrentUser, keys, onSuccess, onFailure);
		}

		public void FetchUserData(PlayerProfile player, Action onSuccess, Action<FailureReasons> onFailure)
		{
			FetchUserData(player, null, onSuccess, onFailure);
		}

		public void FetchUserData(PlayerProfile player, List<string> keys, Action onSuccess, Action<FailureReasons> onFailure)
		{
			Action<Dictionary<string, string>, Dictionary<string, string>> onSuccess2 = delegate(Dictionary<string, string> data, Dictionary<string, string> readOnlyData)
			{
				CurrentUser.OnDataFetched(data, readOnlyData);
				onSuccess();
			};
			_userService.FetchCombinedUserData(player, keys, onSuccess2, onFailure);
		}

		public void FetchUserProfile(string playFabId, Action<PlayerProfile> onSuccess, Action<FailureReasons> onFailure)
		{
			_userService.FetchUserProfile(playFabId, onSuccess, onFailure);
		}

		public void UpdateDisplayName(string desiredDisplayName, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_userService.UpdateDisplayName(desiredDisplayName, delegate
			{
				CurrentUser.DisplayName = desiredDisplayName;
				onSuccess();
				this._userPropertiesFilled?.Invoke();
			}, onFailure);
		}

		public void UpdateUserData(Dictionary<string, string> data, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_userService.UpdateUserData(data, onSuccess, onFailure);
		}

		public void UpdateUserData(Dictionary<string, string> data, bool isPublic, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_userService.UpdateUserData(data, isPublic, onSuccess, onFailure);
		}

		public void UpdateUserData(PlayerProfile player, Dictionary<string, string> data, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_userService.UpdateUserData(player, data, onSuccess, onFailure);
		}

		public void UpdateUserData(PlayerProfile player, Dictionary<string, string> data, bool isPublic, Action onSuccess, Action<FailureReasons> onFailure)
		{
			_userService.UpdateUserData(player, data, isPublic, onSuccess, onFailure);
		}

		public bool IsValidDisplayName(string displayName)
		{
			if (!string.IsNullOrEmpty(displayName))
			{
				return Regex.IsMatch(displayName, "^(?=.{3,20}$)[a-zA-Z]+(?:[a-zA-Z0-9]+)?$");
			}
			return false;
		}

		public string StripInvalidCharacters(string displayname)
		{
			if (string.IsNullOrEmpty(displayname))
			{
				return "";
			}
			return Regex.Replace(displayname, "[^a-zA-Z0-9]+", "");
		}
	}
}
