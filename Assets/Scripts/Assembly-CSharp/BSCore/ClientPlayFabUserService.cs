using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Zenject;

namespace BSCore
{
	public class ClientPlayFabUserService : PlayFabService, IUserService
	{
		private DiContainer _container;

		[Inject]
		public ClientPlayFabUserService(DiContainer container)
		{
			_container = container;
		}

		public void FetchServiceIdByDisplayName(string displayName, Action<string> onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest
			{
				TitleDisplayName = displayName
			}, errorCallback: OnFailureCallback(delegate
			{
				FetchServiceIdByDisplayName(displayName, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: delegate(GetAccountInfoResult response)
			{
				onSuccess(response.AccountInfo.PlayFabId);
			});
		}

		public void FetchCombinedUserData(PlayerProfile player, List<string> keys, Action<Dictionary<string, string>, Dictionary<string, string>> onSuccess, Action<FailureReasons> onFailure)
		{
			Dictionary<string, string> data = null;
			Dictionary<string, string> readOnlyData = null;
			Action<Dictionary<string, string>> onSuccess2 = delegate(Dictionary<string, string> userData)
			{
				data = userData;
				if (readOnlyData != null)
				{
					onSuccess(data, readOnlyData);
				}
			};
			Action<Dictionary<string, string>> onSuccess3 = delegate(Dictionary<string, string> userData)
			{
				readOnlyData = userData;
				if (data != null)
				{
					onSuccess(data, readOnlyData);
				}
			};
			bool failed = false;
			Action<FailureReasons> onFailure2 = delegate(FailureReasons reason)
			{
				if (!failed)
				{
					failed = true;
					onFailure(reason);
				}
			};
			FetchUserData(player, keys, onSuccess2, onFailure2);
			FetchUserReadOnlyData(player, keys, onSuccess3, onFailure2);
		}

		private void FetchUserData(PlayerProfile player, List<string> keys, Action<Dictionary<string, string>> onSuccess, Action<FailureReasons> onFailure)
		{
			GetUserDataRequest getUserDataRequest = new GetUserDataRequest();
			getUserDataRequest.PlayFabId = player.Id;
			if (keys != null)
			{
				getUserDataRequest.Keys = keys;
			}
			Action<PlayFabError> errorCallback = OnFailureCallback(delegate
			{
				FetchUserData(player, keys, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			});
			PlayFabClientAPI.GetUserData(getUserDataRequest, delegate(GetUserDataResult result)
			{
				onSuccess(DataResultToDictionary(result));
			}, errorCallback);
		}

		private void FetchUserReadOnlyData(PlayerProfile player, List<string> keys, Action<Dictionary<string, string>> onSuccess, Action<FailureReasons> onFailure)
		{
			GetUserDataRequest getUserDataRequest = new GetUserDataRequest();
			getUserDataRequest.PlayFabId = player.Id;
			if (keys != null)
			{
				getUserDataRequest.Keys = keys;
			}
			Action<PlayFabError> errorCallback = OnFailureCallback(delegate
			{
				FetchUserReadOnlyData(player, keys, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			});
			PlayFabClientAPI.GetUserReadOnlyData(getUserDataRequest, delegate(GetUserDataResult result)
			{
				onSuccess(DataResultToDictionary(result));
			}, errorCallback);
		}

		public void FetchUserProfile(string playFabId, Action<PlayerProfile> onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest
			{
				PlayFabId = playFabId
			}, errorCallback: OnFailureCallback(delegate
			{
				FetchUserProfile(playFabId, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: delegate(GetPlayerProfileResult result)
			{
				onSuccess(ConvertToPlayerProfile(result));
			});
		}

		public void UpdateDisplayName(string desiredDisplayName, Action onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
			{
				DisplayName = desiredDisplayName
			}, errorCallback: OnFailureCallback(delegate
			{
				UpdateDisplayName(desiredDisplayName, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: delegate
			{
				onSuccess();
			});
		}

		public void UpdateUserData(Dictionary<string, string> data, Action onSuccess, Action<FailureReasons> onFailure)
		{
			UpdateUserData(data, isPublic: true, onSuccess, onFailure);
		}

		public void UpdateUserData(Dictionary<string, string> data, bool isPublic, Action onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
			{
				Data = data,
				Permission = (isPublic ? UserDataPermission.Public : UserDataPermission.Private)
			}, errorCallback: OnFailureCallback(delegate
			{
				UpdateUserData(data, isPublic, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: delegate
			{
				onSuccess();
			});
		}

		public void UpdateUserData(PlayerProfile player, Dictionary<string, string> data, Action onSuccess, Action<FailureReasons> onFailure)
		{
			throw new NotImplementedException();
		}

		public void UpdateUserData(PlayerProfile player, Dictionary<string, string> data, bool isPublic, Action onSuccess, Action<FailureReasons> onFailure)
		{
			throw new NotImplementedException();
		}

		private PlayerProfile ConvertToPlayerProfile(GetPlayerProfileResult result)
		{
			return new PlayerProfile(new PlayerProfile.Data
			{
				serviceId = result.PlayerProfile.PlayerId,
				displayName = result.PlayerProfile.DisplayName
			}, _container);
		}

		public static Dictionary<string, string> DataResultToDictionary(GetUserDataResult result)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (result != null && result.Data != null)
			{
				foreach (KeyValuePair<string, UserDataRecord> datum in result.Data)
				{
					dictionary.Add(datum.Key, datum.Value.Value);
				}
			}
			return dictionary;
		}
	}
}
