using System;
using PlayFab.ClientModels;

namespace PlayFab.Authentication.Strategies
{
	internal sealed class FacebookInstantGameAuthStrategy : IAuthenticationStrategy
	{
		public AuthTypes AuthType => AuthTypes.FacebookInstantGames;

		public void Authenticate(PlayFabAuthService authService, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, AuthKeys authKeys)
		{
			if (authKeys == null || string.IsNullOrEmpty(authKeys.AuthTicket))
			{
				authService.InvokeDisplayAuthentication();
				return;
			}
			PlayFabClientAPI.LoginWithFacebookInstantGamesId(new LoginWithFacebookInstantGamesIdRequest
			{
				FacebookInstantGamesSignature = authKeys.AuthTicket,
				InfoRequestParameters = authService.InfoRequestParams,
				CreateAccount = true
			}, resultCallback, errorCallback);
		}

		public void Link(PlayFabAuthService authService, AuthKeys authKeys)
		{
			PlayFabClientAPI.LinkFacebookInstantGamesId(new LinkFacebookInstantGamesIdRequest
			{
				FacebookInstantGamesSignature = authKeys.AuthTicket,
				AuthenticationContext = authService.AuthenticationContext,
				ForceLink = authService.ForceLink
			}, delegate
			{
				authService.InvokeLink(AuthTypes.FacebookInstantGames);
			}, delegate(PlayFabError errorCallback)
			{
				authService.InvokeLink(AuthTypes.FacebookInstantGames, errorCallback);
			});
		}

		public void Unlink(PlayFabAuthService authService, AuthKeys authKeys)
		{
			PlayFabClientAPI.UnlinkFacebookInstantGamesId(new UnlinkFacebookInstantGamesIdRequest
			{
				AuthenticationContext = authService.AuthenticationContext,
				FacebookInstantGamesId = authKeys.AuthTicket
			}, delegate
			{
				authService.InvokeUnlink(AuthTypes.FacebookInstantGames);
			}, delegate(PlayFabError errorCallback)
			{
				authService.InvokeUnlink(AuthTypes.FacebookInstantGames, errorCallback);
			});
		}
	}
}
