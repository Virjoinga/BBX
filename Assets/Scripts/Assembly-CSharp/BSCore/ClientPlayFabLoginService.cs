using System;
using BSCore.Utils;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Zenject;

namespace BSCore
{
	public class ClientPlayFabLoginService : PlayFabService, ILoginService
	{
		[Inject]
		private SteamAbstractionLayer _steamAbstractionLayer;

		private DiContainer _container;

		public bool IsLoggedIn { get; private set; }

		[Inject]
		public ClientPlayFabLoginService(DiContainer container)
		{
			_container = container;
		}

		public void Login(string username, string password, Action<PlayerProfile, bool> onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
			{
				Username = username,
				Password = password,
				TitleId = PlayFabSettings.TitleId,
				InfoRequestParameters = GetLoginParams()
			}, errorCallback: OnFailureCallback(delegate
			{
				Login(username, password, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: OnloginSuccessWrapper(onSuccess));
		}

		public void LoginWithCustomId(string customId, bool createAccount, Action<PlayerProfile, bool> onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
			{
				CustomId = customId,
				TitleId = PlayFabSettings.TitleId,
				CreateAccount = createAccount,
				InfoRequestParameters = GetLoginParams()
			}, errorCallback: OnFailureCallback(delegate
			{
				LoginWithCustomId(customId, createAccount, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: OnloginSuccessWrapper(onSuccess));
		}

		public void LoginWithSteam(bool createAccount, Action<PlayerProfile, bool> onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.LoginWithSteam(new LoginWithSteamRequest
			{
				TitleId = PlayFabSettings.TitleId,
				SteamTicket = _steamAbstractionLayer.GetAuthSessionTicket(),
				CreateAccount = createAccount,
				InfoRequestParameters = GetLoginParams()
			}, errorCallback: OnFailureCallback(delegate
			{
				LoginWithSteam(createAccount, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: OnloginSuccessWrapper(onSuccess));
		}

		public void LoginWithEmail(string email, string password, Action<PlayerProfile, bool> onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest
			{
				Email = email,
				Password = password,
				TitleId = PlayFabSettings.TitleId,
				InfoRequestParameters = GetLoginParams()
			}, errorCallback: OnFailureCallback(delegate
			{
				LoginWithEmail(email, password, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: OnloginSuccessWrapper(onSuccess));
		}

		public void AddUsernamePassword(string username, string password, string email, Action onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.AddUsernamePassword(new AddUsernamePasswordRequest
			{
				Username = username,
				Password = password,
				Email = email
			}, errorCallback: OnFailureCallback(delegate
			{
				AddUsernamePassword(username, password, email, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: delegate
			{
				onSuccess();
			});
			AddOrUpdateContactEmail(email);
		}

		private void AddOrUpdateContactEmail(string email)
		{
			PlayFabClientAPI.AddOrUpdateContactEmail(new AddOrUpdateContactEmailRequest
			{
				EmailAddress = email
			}, delegate
			{
				Debug.Log("Successfully Added Contact Email");
			}, delegate(PlayFabError error)
			{
				Debug.Log("Failed to add Contact Email - " + error.ErrorMessage);
			});
		}

		public void LinkSeamlessAccount(Action onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.LinkSteamAccount(new LinkSteamAccountRequest
			{
				SteamTicket = _steamAbstractionLayer.GetAuthSessionTicket(),
				ForceLink = false
			}, delegate
			{
				onSuccess();
			}, delegate(PlayFabError error)
			{
				onFailure(PlayFabUtils.ConvertToFailureReason(error.Error));
			});
		}

		public void LinkSteamAccount(string steamTicket, Action onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.LinkSteamAccount(new LinkSteamAccountRequest
			{
				SteamTicket = steamTicket,
				ForceLink = false
			}, errorCallback: OnFailureCallback(delegate
			{
				LinkSteamAccount(steamTicket, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: delegate
			{
				onSuccess();
			});
		}

		public void LinkCustomId(string customId, Action onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.LinkCustomID(new LinkCustomIDRequest
			{
				CustomId = customId,
				ForceLink = false
			}, errorCallback: OnFailureCallback(delegate
			{
				LinkCustomId(customId, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: delegate
			{
				onSuccess();
			});
		}

		public void SendAccountRecoveryEmail(string email, Action onSuccess, Action<FailureReasons> onFailure)
		{
			PlayFabClientAPI.SendAccountRecoveryEmail(new SendAccountRecoveryEmailRequest
			{
				Email = email,
				TitleId = PlayFabSettings.TitleId
			}, errorCallback: OnFailureCallback(delegate
			{
				SendAccountRecoveryEmail(email, onSuccess, onFailure);
			}, delegate(FailureReasons reason)
			{
				onFailure(reason);
			}), resultCallback: delegate
			{
				onSuccess();
			});
		}

		private GetPlayerCombinedInfoRequestParams GetLoginParams()
		{
			return new GetPlayerCombinedInfoRequestParams
			{
				GetPlayerProfile = true,
				GetUserAccountInfo = true
			};
		}

		private Action<LoginResult> OnloginSuccessWrapper(Action<PlayerProfile, bool> onSuccess)
		{
			return delegate(LoginResult result)
			{
				Debug.Log("[ClientPlayFabLoginService] Logged in as " + result.PlayFabId);
				IsLoggedIn = true;
				onSuccess(ConvertToPlayerProfile(result), result.NewlyCreated);
			};
		}

		private PlayerProfile ConvertToPlayerProfile(LoginResult result)
		{
			PlayerProfile.Data profileData = new PlayerProfile.Data
			{
				serviceId = result.PlayFabId,
				sessionTicket = result.SessionTicket
			};
			if (result.InfoResultPayload.PlayerProfile != null)
			{
				profileData.displayName = result.InfoResultPayload.PlayerProfile.DisplayName;
			}
			if (result.InfoResultPayload.AccountInfo != null)
			{
				profileData.hasUniversalAccount = !string.IsNullOrEmpty(result.InfoResultPayload.AccountInfo.Username);
			}
			if (result.EntityToken != null)
			{
				profileData.entity = result.EntityToken.Entity;
			}
			else
			{
				Debug.LogError("Player Profile has no Entity Token");
			}
			return new PlayerProfile(profileData, _container);
		}
	}
}
