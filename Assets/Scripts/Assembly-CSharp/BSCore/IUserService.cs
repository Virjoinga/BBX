using System;
using System.Collections.Generic;

namespace BSCore
{
	public interface IUserService
	{
		void FetchUserProfile(string playFabId, Action<PlayerProfile> onSuccess, Action<FailureReasons> onFailure);

		void FetchServiceIdByDisplayName(string displayName, Action<string> onSuccess, Action<FailureReasons> onFailure);

		void UpdateDisplayName(string desiredDisplayName, Action onSuccess, Action<FailureReasons> onFailure);

		void FetchCombinedUserData(PlayerProfile player, List<string> keys, Action<Dictionary<string, string>, Dictionary<string, string>> onSuccess, Action<FailureReasons> onFailure);

		void UpdateUserData(Dictionary<string, string> data, Action onSuccess, Action<FailureReasons> onFailure);

		void UpdateUserData(Dictionary<string, string> data, bool isPublic, Action onSuccess, Action<FailureReasons> onFailure);

		void UpdateUserData(PlayerProfile player, Dictionary<string, string> data, Action onSuccess, Action<FailureReasons> onFailure);

		void UpdateUserData(PlayerProfile player, Dictionary<string, string> data, bool isPublic, Action onSuccess, Action<FailureReasons> onFailure);
	}
}
