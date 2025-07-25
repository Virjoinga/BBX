using System;
using PlayFab.AuthenticationModels;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.DataModels;
using PlayFab.EventsModels;
using PlayFab.GroupsModels;
using PlayFab.Internal;
using PlayFab.LocalizationModels;
using PlayFab.MultiplayerModels;
using PlayFab.ProfilesModels;
using PlayFab.SharedModels;

namespace PlayFab.Events
{
	public class PlayFabEvents
	{
		public delegate void PlayFabErrorEvent(PlayFabRequestCommon request, PlayFabError error);

		public delegate void PlayFabResultEvent<in TResult>(TResult result) where TResult : PlayFabResultCommon;

		public delegate void PlayFabRequestEvent<in TRequest>(TRequest request) where TRequest : PlayFabRequestCommon;

		private static PlayFabEvents _instance;

		public event PlayFabRequestEvent<GetEntityTokenRequest> OnAuthenticationGetEntityTokenRequestEvent;

		public event PlayFabResultEvent<GetEntityTokenResponse> OnAuthenticationGetEntityTokenResultEvent;

		public event PlayFabResultEvent<LoginResult> OnLoginResultEvent;

		public event PlayFabRequestEvent<AcceptTradeRequest> OnAcceptTradeRequestEvent;

		public event PlayFabResultEvent<AcceptTradeResponse> OnAcceptTradeResultEvent;

		public event PlayFabRequestEvent<AddFriendRequest> OnAddFriendRequestEvent;

		public event PlayFabResultEvent<AddFriendResult> OnAddFriendResultEvent;

		public event PlayFabRequestEvent<AddGenericIDRequest> OnAddGenericIDRequestEvent;

		public event PlayFabResultEvent<AddGenericIDResult> OnAddGenericIDResultEvent;

		public event PlayFabRequestEvent<AddOrUpdateContactEmailRequest> OnAddOrUpdateContactEmailRequestEvent;

		public event PlayFabResultEvent<AddOrUpdateContactEmailResult> OnAddOrUpdateContactEmailResultEvent;

		public event PlayFabRequestEvent<AddSharedGroupMembersRequest> OnAddSharedGroupMembersRequestEvent;

		public event PlayFabResultEvent<AddSharedGroupMembersResult> OnAddSharedGroupMembersResultEvent;

		public event PlayFabRequestEvent<AddUsernamePasswordRequest> OnAddUsernamePasswordRequestEvent;

		public event PlayFabResultEvent<AddUsernamePasswordResult> OnAddUsernamePasswordResultEvent;

		public event PlayFabRequestEvent<AddUserVirtualCurrencyRequest> OnAddUserVirtualCurrencyRequestEvent;

		public event PlayFabResultEvent<ModifyUserVirtualCurrencyResult> OnAddUserVirtualCurrencyResultEvent;

		public event PlayFabRequestEvent<AndroidDevicePushNotificationRegistrationRequest> OnAndroidDevicePushNotificationRegistrationRequestEvent;

		public event PlayFabResultEvent<AndroidDevicePushNotificationRegistrationResult> OnAndroidDevicePushNotificationRegistrationResultEvent;

		public event PlayFabRequestEvent<AttributeInstallRequest> OnAttributeInstallRequestEvent;

		public event PlayFabResultEvent<AttributeInstallResult> OnAttributeInstallResultEvent;

		public event PlayFabRequestEvent<CancelTradeRequest> OnCancelTradeRequestEvent;

		public event PlayFabResultEvent<CancelTradeResponse> OnCancelTradeResultEvent;

		public event PlayFabRequestEvent<ConfirmPurchaseRequest> OnConfirmPurchaseRequestEvent;

		public event PlayFabResultEvent<ConfirmPurchaseResult> OnConfirmPurchaseResultEvent;

		public event PlayFabRequestEvent<ConsumeItemRequest> OnConsumeItemRequestEvent;

		public event PlayFabResultEvent<ConsumeItemResult> OnConsumeItemResultEvent;

		public event PlayFabRequestEvent<ConsumePSNEntitlementsRequest> OnConsumePSNEntitlementsRequestEvent;

		public event PlayFabResultEvent<ConsumePSNEntitlementsResult> OnConsumePSNEntitlementsResultEvent;

		public event PlayFabRequestEvent<ConsumeXboxEntitlementsRequest> OnConsumeXboxEntitlementsRequestEvent;

		public event PlayFabResultEvent<ConsumeXboxEntitlementsResult> OnConsumeXboxEntitlementsResultEvent;

		public event PlayFabRequestEvent<CreateSharedGroupRequest> OnCreateSharedGroupRequestEvent;

		public event PlayFabResultEvent<CreateSharedGroupResult> OnCreateSharedGroupResultEvent;

		public event PlayFabRequestEvent<ExecuteCloudScriptRequest> OnExecuteCloudScriptRequestEvent;

		public event PlayFabResultEvent<PlayFab.ClientModels.ExecuteCloudScriptResult> OnExecuteCloudScriptResultEvent;

		public event PlayFabRequestEvent<GetAccountInfoRequest> OnGetAccountInfoRequestEvent;

		public event PlayFabResultEvent<GetAccountInfoResult> OnGetAccountInfoResultEvent;

		public event PlayFabRequestEvent<ListUsersCharactersRequest> OnGetAllUsersCharactersRequestEvent;

		public event PlayFabResultEvent<ListUsersCharactersResult> OnGetAllUsersCharactersResultEvent;

		public event PlayFabRequestEvent<GetCatalogItemsRequest> OnGetCatalogItemsRequestEvent;

		public event PlayFabResultEvent<GetCatalogItemsResult> OnGetCatalogItemsResultEvent;

		public event PlayFabRequestEvent<GetCharacterDataRequest> OnGetCharacterDataRequestEvent;

		public event PlayFabResultEvent<GetCharacterDataResult> OnGetCharacterDataResultEvent;

		public event PlayFabRequestEvent<GetCharacterInventoryRequest> OnGetCharacterInventoryRequestEvent;

		public event PlayFabResultEvent<GetCharacterInventoryResult> OnGetCharacterInventoryResultEvent;

		public event PlayFabRequestEvent<GetCharacterLeaderboardRequest> OnGetCharacterLeaderboardRequestEvent;

		public event PlayFabResultEvent<GetCharacterLeaderboardResult> OnGetCharacterLeaderboardResultEvent;

		public event PlayFabRequestEvent<GetCharacterDataRequest> OnGetCharacterReadOnlyDataRequestEvent;

		public event PlayFabResultEvent<GetCharacterDataResult> OnGetCharacterReadOnlyDataResultEvent;

		public event PlayFabRequestEvent<GetCharacterStatisticsRequest> OnGetCharacterStatisticsRequestEvent;

		public event PlayFabResultEvent<GetCharacterStatisticsResult> OnGetCharacterStatisticsResultEvent;

		public event PlayFabRequestEvent<GetContentDownloadUrlRequest> OnGetContentDownloadUrlRequestEvent;

		public event PlayFabResultEvent<GetContentDownloadUrlResult> OnGetContentDownloadUrlResultEvent;

		public event PlayFabRequestEvent<CurrentGamesRequest> OnGetCurrentGamesRequestEvent;

		public event PlayFabResultEvent<CurrentGamesResult> OnGetCurrentGamesResultEvent;

		public event PlayFabRequestEvent<GetFriendLeaderboardRequest> OnGetFriendLeaderboardRequestEvent;

		public event PlayFabResultEvent<GetLeaderboardResult> OnGetFriendLeaderboardResultEvent;

		public event PlayFabRequestEvent<GetFriendLeaderboardAroundPlayerRequest> OnGetFriendLeaderboardAroundPlayerRequestEvent;

		public event PlayFabResultEvent<GetFriendLeaderboardAroundPlayerResult> OnGetFriendLeaderboardAroundPlayerResultEvent;

		public event PlayFabRequestEvent<GetFriendsListRequest> OnGetFriendsListRequestEvent;

		public event PlayFabResultEvent<GetFriendsListResult> OnGetFriendsListResultEvent;

		public event PlayFabRequestEvent<GameServerRegionsRequest> OnGetGameServerRegionsRequestEvent;

		public event PlayFabResultEvent<GameServerRegionsResult> OnGetGameServerRegionsResultEvent;

		public event PlayFabRequestEvent<GetLeaderboardRequest> OnGetLeaderboardRequestEvent;

		public event PlayFabResultEvent<GetLeaderboardResult> OnGetLeaderboardResultEvent;

		public event PlayFabRequestEvent<GetLeaderboardAroundCharacterRequest> OnGetLeaderboardAroundCharacterRequestEvent;

		public event PlayFabResultEvent<GetLeaderboardAroundCharacterResult> OnGetLeaderboardAroundCharacterResultEvent;

		public event PlayFabRequestEvent<GetLeaderboardAroundPlayerRequest> OnGetLeaderboardAroundPlayerRequestEvent;

		public event PlayFabResultEvent<GetLeaderboardAroundPlayerResult> OnGetLeaderboardAroundPlayerResultEvent;

		public event PlayFabRequestEvent<GetLeaderboardForUsersCharactersRequest> OnGetLeaderboardForUserCharactersRequestEvent;

		public event PlayFabResultEvent<GetLeaderboardForUsersCharactersResult> OnGetLeaderboardForUserCharactersResultEvent;

		public event PlayFabRequestEvent<GetPaymentTokenRequest> OnGetPaymentTokenRequestEvent;

		public event PlayFabResultEvent<GetPaymentTokenResult> OnGetPaymentTokenResultEvent;

		public event PlayFabRequestEvent<GetPhotonAuthenticationTokenRequest> OnGetPhotonAuthenticationTokenRequestEvent;

		public event PlayFabResultEvent<GetPhotonAuthenticationTokenResult> OnGetPhotonAuthenticationTokenResultEvent;

		public event PlayFabRequestEvent<GetPlayerCombinedInfoRequest> OnGetPlayerCombinedInfoRequestEvent;

		public event PlayFabResultEvent<GetPlayerCombinedInfoResult> OnGetPlayerCombinedInfoResultEvent;

		public event PlayFabRequestEvent<GetPlayerProfileRequest> OnGetPlayerProfileRequestEvent;

		public event PlayFabResultEvent<GetPlayerProfileResult> OnGetPlayerProfileResultEvent;

		public event PlayFabRequestEvent<GetPlayerSegmentsRequest> OnGetPlayerSegmentsRequestEvent;

		public event PlayFabResultEvent<GetPlayerSegmentsResult> OnGetPlayerSegmentsResultEvent;

		public event PlayFabRequestEvent<GetPlayerStatisticsRequest> OnGetPlayerStatisticsRequestEvent;

		public event PlayFabResultEvent<GetPlayerStatisticsResult> OnGetPlayerStatisticsResultEvent;

		public event PlayFabRequestEvent<GetPlayerStatisticVersionsRequest> OnGetPlayerStatisticVersionsRequestEvent;

		public event PlayFabResultEvent<GetPlayerStatisticVersionsResult> OnGetPlayerStatisticVersionsResultEvent;

		public event PlayFabRequestEvent<GetPlayerTagsRequest> OnGetPlayerTagsRequestEvent;

		public event PlayFabResultEvent<GetPlayerTagsResult> OnGetPlayerTagsResultEvent;

		public event PlayFabRequestEvent<GetPlayerTradesRequest> OnGetPlayerTradesRequestEvent;

		public event PlayFabResultEvent<GetPlayerTradesResponse> OnGetPlayerTradesResultEvent;

		public event PlayFabRequestEvent<GetPlayFabIDsFromFacebookIDsRequest> OnGetPlayFabIDsFromFacebookIDsRequestEvent;

		public event PlayFabResultEvent<GetPlayFabIDsFromFacebookIDsResult> OnGetPlayFabIDsFromFacebookIDsResultEvent;

		public event PlayFabRequestEvent<GetPlayFabIDsFromFacebookInstantGamesIdsRequest> OnGetPlayFabIDsFromFacebookInstantGamesIdsRequestEvent;

		public event PlayFabResultEvent<GetPlayFabIDsFromFacebookInstantGamesIdsResult> OnGetPlayFabIDsFromFacebookInstantGamesIdsResultEvent;

		public event PlayFabRequestEvent<GetPlayFabIDsFromGameCenterIDsRequest> OnGetPlayFabIDsFromGameCenterIDsRequestEvent;

		public event PlayFabResultEvent<GetPlayFabIDsFromGameCenterIDsResult> OnGetPlayFabIDsFromGameCenterIDsResultEvent;

		public event PlayFabRequestEvent<GetPlayFabIDsFromGenericIDsRequest> OnGetPlayFabIDsFromGenericIDsRequestEvent;

		public event PlayFabResultEvent<GetPlayFabIDsFromGenericIDsResult> OnGetPlayFabIDsFromGenericIDsResultEvent;

		public event PlayFabRequestEvent<GetPlayFabIDsFromGoogleIDsRequest> OnGetPlayFabIDsFromGoogleIDsRequestEvent;

		public event PlayFabResultEvent<GetPlayFabIDsFromGoogleIDsResult> OnGetPlayFabIDsFromGoogleIDsResultEvent;

		public event PlayFabRequestEvent<GetPlayFabIDsFromKongregateIDsRequest> OnGetPlayFabIDsFromKongregateIDsRequestEvent;

		public event PlayFabResultEvent<GetPlayFabIDsFromKongregateIDsResult> OnGetPlayFabIDsFromKongregateIDsResultEvent;

		public event PlayFabRequestEvent<GetPlayFabIDsFromNintendoSwitchDeviceIdsRequest> OnGetPlayFabIDsFromNintendoSwitchDeviceIdsRequestEvent;

		public event PlayFabResultEvent<GetPlayFabIDsFromNintendoSwitchDeviceIdsResult> OnGetPlayFabIDsFromNintendoSwitchDeviceIdsResultEvent;

		public event PlayFabRequestEvent<GetPlayFabIDsFromPSNAccountIDsRequest> OnGetPlayFabIDsFromPSNAccountIDsRequestEvent;

		public event PlayFabResultEvent<GetPlayFabIDsFromPSNAccountIDsResult> OnGetPlayFabIDsFromPSNAccountIDsResultEvent;

		public event PlayFabRequestEvent<GetPlayFabIDsFromSteamIDsRequest> OnGetPlayFabIDsFromSteamIDsRequestEvent;

		public event PlayFabResultEvent<GetPlayFabIDsFromSteamIDsResult> OnGetPlayFabIDsFromSteamIDsResultEvent;

		public event PlayFabRequestEvent<GetPlayFabIDsFromTwitchIDsRequest> OnGetPlayFabIDsFromTwitchIDsRequestEvent;

		public event PlayFabResultEvent<GetPlayFabIDsFromTwitchIDsResult> OnGetPlayFabIDsFromTwitchIDsResultEvent;

		public event PlayFabRequestEvent<GetPlayFabIDsFromXboxLiveIDsRequest> OnGetPlayFabIDsFromXboxLiveIDsRequestEvent;

		public event PlayFabResultEvent<GetPlayFabIDsFromXboxLiveIDsResult> OnGetPlayFabIDsFromXboxLiveIDsResultEvent;

		public event PlayFabRequestEvent<GetPublisherDataRequest> OnGetPublisherDataRequestEvent;

		public event PlayFabResultEvent<GetPublisherDataResult> OnGetPublisherDataResultEvent;

		public event PlayFabRequestEvent<GetPurchaseRequest> OnGetPurchaseRequestEvent;

		public event PlayFabResultEvent<GetPurchaseResult> OnGetPurchaseResultEvent;

		public event PlayFabRequestEvent<GetSharedGroupDataRequest> OnGetSharedGroupDataRequestEvent;

		public event PlayFabResultEvent<GetSharedGroupDataResult> OnGetSharedGroupDataResultEvent;

		public event PlayFabRequestEvent<GetStoreItemsRequest> OnGetStoreItemsRequestEvent;

		public event PlayFabResultEvent<GetStoreItemsResult> OnGetStoreItemsResultEvent;

		public event PlayFabRequestEvent<GetTimeRequest> OnGetTimeRequestEvent;

		public event PlayFabResultEvent<GetTimeResult> OnGetTimeResultEvent;

		public event PlayFabRequestEvent<GetTitleDataRequest> OnGetTitleDataRequestEvent;

		public event PlayFabResultEvent<GetTitleDataResult> OnGetTitleDataResultEvent;

		public event PlayFabRequestEvent<GetTitleNewsRequest> OnGetTitleNewsRequestEvent;

		public event PlayFabResultEvent<GetTitleNewsResult> OnGetTitleNewsResultEvent;

		public event PlayFabRequestEvent<GetTitlePublicKeyRequest> OnGetTitlePublicKeyRequestEvent;

		public event PlayFabResultEvent<GetTitlePublicKeyResult> OnGetTitlePublicKeyResultEvent;

		public event PlayFabRequestEvent<GetTradeStatusRequest> OnGetTradeStatusRequestEvent;

		public event PlayFabResultEvent<GetTradeStatusResponse> OnGetTradeStatusResultEvent;

		public event PlayFabRequestEvent<GetUserDataRequest> OnGetUserDataRequestEvent;

		public event PlayFabResultEvent<GetUserDataResult> OnGetUserDataResultEvent;

		public event PlayFabRequestEvent<GetUserInventoryRequest> OnGetUserInventoryRequestEvent;

		public event PlayFabResultEvent<GetUserInventoryResult> OnGetUserInventoryResultEvent;

		public event PlayFabRequestEvent<GetUserDataRequest> OnGetUserPublisherDataRequestEvent;

		public event PlayFabResultEvent<GetUserDataResult> OnGetUserPublisherDataResultEvent;

		public event PlayFabRequestEvent<GetUserDataRequest> OnGetUserPublisherReadOnlyDataRequestEvent;

		public event PlayFabResultEvent<GetUserDataResult> OnGetUserPublisherReadOnlyDataResultEvent;

		public event PlayFabRequestEvent<GetUserDataRequest> OnGetUserReadOnlyDataRequestEvent;

		public event PlayFabResultEvent<GetUserDataResult> OnGetUserReadOnlyDataResultEvent;

		public event PlayFabRequestEvent<GetWindowsHelloChallengeRequest> OnGetWindowsHelloChallengeRequestEvent;

		public event PlayFabResultEvent<GetWindowsHelloChallengeResponse> OnGetWindowsHelloChallengeResultEvent;

		public event PlayFabRequestEvent<GrantCharacterToUserRequest> OnGrantCharacterToUserRequestEvent;

		public event PlayFabResultEvent<GrantCharacterToUserResult> OnGrantCharacterToUserResultEvent;

		public event PlayFabRequestEvent<LinkAndroidDeviceIDRequest> OnLinkAndroidDeviceIDRequestEvent;

		public event PlayFabResultEvent<LinkAndroidDeviceIDResult> OnLinkAndroidDeviceIDResultEvent;

		public event PlayFabRequestEvent<LinkCustomIDRequest> OnLinkCustomIDRequestEvent;

		public event PlayFabResultEvent<LinkCustomIDResult> OnLinkCustomIDResultEvent;

		public event PlayFabRequestEvent<LinkFacebookAccountRequest> OnLinkFacebookAccountRequestEvent;

		public event PlayFabResultEvent<LinkFacebookAccountResult> OnLinkFacebookAccountResultEvent;

		public event PlayFabRequestEvent<LinkFacebookInstantGamesIdRequest> OnLinkFacebookInstantGamesIdRequestEvent;

		public event PlayFabResultEvent<LinkFacebookInstantGamesIdResult> OnLinkFacebookInstantGamesIdResultEvent;

		public event PlayFabRequestEvent<LinkGameCenterAccountRequest> OnLinkGameCenterAccountRequestEvent;

		public event PlayFabResultEvent<LinkGameCenterAccountResult> OnLinkGameCenterAccountResultEvent;

		public event PlayFabRequestEvent<LinkGoogleAccountRequest> OnLinkGoogleAccountRequestEvent;

		public event PlayFabResultEvent<LinkGoogleAccountResult> OnLinkGoogleAccountResultEvent;

		public event PlayFabRequestEvent<LinkIOSDeviceIDRequest> OnLinkIOSDeviceIDRequestEvent;

		public event PlayFabResultEvent<LinkIOSDeviceIDResult> OnLinkIOSDeviceIDResultEvent;

		public event PlayFabRequestEvent<LinkKongregateAccountRequest> OnLinkKongregateRequestEvent;

		public event PlayFabResultEvent<LinkKongregateAccountResult> OnLinkKongregateResultEvent;

		public event PlayFabRequestEvent<LinkNintendoSwitchDeviceIdRequest> OnLinkNintendoSwitchDeviceIdRequestEvent;

		public event PlayFabResultEvent<LinkNintendoSwitchDeviceIdResult> OnLinkNintendoSwitchDeviceIdResultEvent;

		public event PlayFabRequestEvent<LinkOpenIdConnectRequest> OnLinkOpenIdConnectRequestEvent;

		public event PlayFabResultEvent<EmptyResult> OnLinkOpenIdConnectResultEvent;

		public event PlayFabRequestEvent<LinkPSNAccountRequest> OnLinkPSNAccountRequestEvent;

		public event PlayFabResultEvent<LinkPSNAccountResult> OnLinkPSNAccountResultEvent;

		public event PlayFabRequestEvent<LinkSteamAccountRequest> OnLinkSteamAccountRequestEvent;

		public event PlayFabResultEvent<LinkSteamAccountResult> OnLinkSteamAccountResultEvent;

		public event PlayFabRequestEvent<LinkTwitchAccountRequest> OnLinkTwitchRequestEvent;

		public event PlayFabResultEvent<LinkTwitchAccountResult> OnLinkTwitchResultEvent;

		public event PlayFabRequestEvent<LinkWindowsHelloAccountRequest> OnLinkWindowsHelloRequestEvent;

		public event PlayFabResultEvent<LinkWindowsHelloAccountResponse> OnLinkWindowsHelloResultEvent;

		public event PlayFabRequestEvent<LinkXboxAccountRequest> OnLinkXboxAccountRequestEvent;

		public event PlayFabResultEvent<LinkXboxAccountResult> OnLinkXboxAccountResultEvent;

		public event PlayFabRequestEvent<LoginWithAndroidDeviceIDRequest> OnLoginWithAndroidDeviceIDRequestEvent;

		public event PlayFabRequestEvent<LoginWithCustomIDRequest> OnLoginWithCustomIDRequestEvent;

		public event PlayFabRequestEvent<LoginWithEmailAddressRequest> OnLoginWithEmailAddressRequestEvent;

		public event PlayFabRequestEvent<LoginWithFacebookRequest> OnLoginWithFacebookRequestEvent;

		public event PlayFabRequestEvent<LoginWithFacebookInstantGamesIdRequest> OnLoginWithFacebookInstantGamesIdRequestEvent;

		public event PlayFabRequestEvent<LoginWithGameCenterRequest> OnLoginWithGameCenterRequestEvent;

		public event PlayFabRequestEvent<LoginWithGoogleAccountRequest> OnLoginWithGoogleAccountRequestEvent;

		public event PlayFabRequestEvent<LoginWithIOSDeviceIDRequest> OnLoginWithIOSDeviceIDRequestEvent;

		public event PlayFabRequestEvent<LoginWithKongregateRequest> OnLoginWithKongregateRequestEvent;

		public event PlayFabRequestEvent<LoginWithNintendoSwitchDeviceIdRequest> OnLoginWithNintendoSwitchDeviceIdRequestEvent;

		public event PlayFabRequestEvent<LoginWithOpenIdConnectRequest> OnLoginWithOpenIdConnectRequestEvent;

		public event PlayFabRequestEvent<LoginWithPlayFabRequest> OnLoginWithPlayFabRequestEvent;

		public event PlayFabRequestEvent<LoginWithPSNRequest> OnLoginWithPSNRequestEvent;

		public event PlayFabRequestEvent<LoginWithSteamRequest> OnLoginWithSteamRequestEvent;

		public event PlayFabRequestEvent<LoginWithTwitchRequest> OnLoginWithTwitchRequestEvent;

		public event PlayFabRequestEvent<LoginWithWindowsHelloRequest> OnLoginWithWindowsHelloRequestEvent;

		public event PlayFabRequestEvent<LoginWithXboxRequest> OnLoginWithXboxRequestEvent;

		public event PlayFabRequestEvent<MatchmakeRequest> OnMatchmakeRequestEvent;

		public event PlayFabResultEvent<MatchmakeResult> OnMatchmakeResultEvent;

		public event PlayFabRequestEvent<OpenTradeRequest> OnOpenTradeRequestEvent;

		public event PlayFabResultEvent<OpenTradeResponse> OnOpenTradeResultEvent;

		public event PlayFabRequestEvent<PayForPurchaseRequest> OnPayForPurchaseRequestEvent;

		public event PlayFabResultEvent<PayForPurchaseResult> OnPayForPurchaseResultEvent;

		public event PlayFabRequestEvent<PurchaseItemRequest> OnPurchaseItemRequestEvent;

		public event PlayFabResultEvent<PurchaseItemResult> OnPurchaseItemResultEvent;

		public event PlayFabRequestEvent<RedeemCouponRequest> OnRedeemCouponRequestEvent;

		public event PlayFabResultEvent<RedeemCouponResult> OnRedeemCouponResultEvent;

		public event PlayFabRequestEvent<RefreshPSNAuthTokenRequest> OnRefreshPSNAuthTokenRequestEvent;

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> OnRefreshPSNAuthTokenResultEvent;

		public event PlayFabRequestEvent<RegisterForIOSPushNotificationRequest> OnRegisterForIOSPushNotificationRequestEvent;

		public event PlayFabResultEvent<RegisterForIOSPushNotificationResult> OnRegisterForIOSPushNotificationResultEvent;

		public event PlayFabRequestEvent<RegisterPlayFabUserRequest> OnRegisterPlayFabUserRequestEvent;

		public event PlayFabResultEvent<RegisterPlayFabUserResult> OnRegisterPlayFabUserResultEvent;

		public event PlayFabRequestEvent<RegisterWithWindowsHelloRequest> OnRegisterWithWindowsHelloRequestEvent;

		public event PlayFabRequestEvent<RemoveContactEmailRequest> OnRemoveContactEmailRequestEvent;

		public event PlayFabResultEvent<RemoveContactEmailResult> OnRemoveContactEmailResultEvent;

		public event PlayFabRequestEvent<RemoveFriendRequest> OnRemoveFriendRequestEvent;

		public event PlayFabResultEvent<RemoveFriendResult> OnRemoveFriendResultEvent;

		public event PlayFabRequestEvent<RemoveGenericIDRequest> OnRemoveGenericIDRequestEvent;

		public event PlayFabResultEvent<RemoveGenericIDResult> OnRemoveGenericIDResultEvent;

		public event PlayFabRequestEvent<RemoveSharedGroupMembersRequest> OnRemoveSharedGroupMembersRequestEvent;

		public event PlayFabResultEvent<RemoveSharedGroupMembersResult> OnRemoveSharedGroupMembersResultEvent;

		public event PlayFabRequestEvent<DeviceInfoRequest> OnReportDeviceInfoRequestEvent;

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> OnReportDeviceInfoResultEvent;

		public event PlayFabRequestEvent<ReportPlayerClientRequest> OnReportPlayerRequestEvent;

		public event PlayFabResultEvent<ReportPlayerClientResult> OnReportPlayerResultEvent;

		public event PlayFabRequestEvent<RestoreIOSPurchasesRequest> OnRestoreIOSPurchasesRequestEvent;

		public event PlayFabResultEvent<RestoreIOSPurchasesResult> OnRestoreIOSPurchasesResultEvent;

		public event PlayFabRequestEvent<SendAccountRecoveryEmailRequest> OnSendAccountRecoveryEmailRequestEvent;

		public event PlayFabResultEvent<SendAccountRecoveryEmailResult> OnSendAccountRecoveryEmailResultEvent;

		public event PlayFabRequestEvent<SetFriendTagsRequest> OnSetFriendTagsRequestEvent;

		public event PlayFabResultEvent<SetFriendTagsResult> OnSetFriendTagsResultEvent;

		public event PlayFabRequestEvent<SetPlayerSecretRequest> OnSetPlayerSecretRequestEvent;

		public event PlayFabResultEvent<SetPlayerSecretResult> OnSetPlayerSecretResultEvent;

		public event PlayFabRequestEvent<StartGameRequest> OnStartGameRequestEvent;

		public event PlayFabResultEvent<StartGameResult> OnStartGameResultEvent;

		public event PlayFabRequestEvent<StartPurchaseRequest> OnStartPurchaseRequestEvent;

		public event PlayFabResultEvent<StartPurchaseResult> OnStartPurchaseResultEvent;

		public event PlayFabRequestEvent<SubtractUserVirtualCurrencyRequest> OnSubtractUserVirtualCurrencyRequestEvent;

		public event PlayFabResultEvent<ModifyUserVirtualCurrencyResult> OnSubtractUserVirtualCurrencyResultEvent;

		public event PlayFabRequestEvent<UnlinkAndroidDeviceIDRequest> OnUnlinkAndroidDeviceIDRequestEvent;

		public event PlayFabResultEvent<UnlinkAndroidDeviceIDResult> OnUnlinkAndroidDeviceIDResultEvent;

		public event PlayFabRequestEvent<UnlinkCustomIDRequest> OnUnlinkCustomIDRequestEvent;

		public event PlayFabResultEvent<UnlinkCustomIDResult> OnUnlinkCustomIDResultEvent;

		public event PlayFabRequestEvent<UnlinkFacebookAccountRequest> OnUnlinkFacebookAccountRequestEvent;

		public event PlayFabResultEvent<UnlinkFacebookAccountResult> OnUnlinkFacebookAccountResultEvent;

		public event PlayFabRequestEvent<UnlinkFacebookInstantGamesIdRequest> OnUnlinkFacebookInstantGamesIdRequestEvent;

		public event PlayFabResultEvent<UnlinkFacebookInstantGamesIdResult> OnUnlinkFacebookInstantGamesIdResultEvent;

		public event PlayFabRequestEvent<UnlinkGameCenterAccountRequest> OnUnlinkGameCenterAccountRequestEvent;

		public event PlayFabResultEvent<UnlinkGameCenterAccountResult> OnUnlinkGameCenterAccountResultEvent;

		public event PlayFabRequestEvent<UnlinkGoogleAccountRequest> OnUnlinkGoogleAccountRequestEvent;

		public event PlayFabResultEvent<UnlinkGoogleAccountResult> OnUnlinkGoogleAccountResultEvent;

		public event PlayFabRequestEvent<UnlinkIOSDeviceIDRequest> OnUnlinkIOSDeviceIDRequestEvent;

		public event PlayFabResultEvent<UnlinkIOSDeviceIDResult> OnUnlinkIOSDeviceIDResultEvent;

		public event PlayFabRequestEvent<UnlinkKongregateAccountRequest> OnUnlinkKongregateRequestEvent;

		public event PlayFabResultEvent<UnlinkKongregateAccountResult> OnUnlinkKongregateResultEvent;

		public event PlayFabRequestEvent<UnlinkNintendoSwitchDeviceIdRequest> OnUnlinkNintendoSwitchDeviceIdRequestEvent;

		public event PlayFabResultEvent<UnlinkNintendoSwitchDeviceIdResult> OnUnlinkNintendoSwitchDeviceIdResultEvent;

		public event PlayFabRequestEvent<UninkOpenIdConnectRequest> OnUnlinkOpenIdConnectRequestEvent;

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> OnUnlinkOpenIdConnectResultEvent;

		public event PlayFabRequestEvent<UnlinkPSNAccountRequest> OnUnlinkPSNAccountRequestEvent;

		public event PlayFabResultEvent<UnlinkPSNAccountResult> OnUnlinkPSNAccountResultEvent;

		public event PlayFabRequestEvent<UnlinkSteamAccountRequest> OnUnlinkSteamAccountRequestEvent;

		public event PlayFabResultEvent<UnlinkSteamAccountResult> OnUnlinkSteamAccountResultEvent;

		public event PlayFabRequestEvent<UnlinkTwitchAccountRequest> OnUnlinkTwitchRequestEvent;

		public event PlayFabResultEvent<UnlinkTwitchAccountResult> OnUnlinkTwitchResultEvent;

		public event PlayFabRequestEvent<UnlinkWindowsHelloAccountRequest> OnUnlinkWindowsHelloRequestEvent;

		public event PlayFabResultEvent<UnlinkWindowsHelloAccountResponse> OnUnlinkWindowsHelloResultEvent;

		public event PlayFabRequestEvent<UnlinkXboxAccountRequest> OnUnlinkXboxAccountRequestEvent;

		public event PlayFabResultEvent<UnlinkXboxAccountResult> OnUnlinkXboxAccountResultEvent;

		public event PlayFabRequestEvent<UnlockContainerInstanceRequest> OnUnlockContainerInstanceRequestEvent;

		public event PlayFabResultEvent<UnlockContainerItemResult> OnUnlockContainerInstanceResultEvent;

		public event PlayFabRequestEvent<UnlockContainerItemRequest> OnUnlockContainerItemRequestEvent;

		public event PlayFabResultEvent<UnlockContainerItemResult> OnUnlockContainerItemResultEvent;

		public event PlayFabRequestEvent<UpdateAvatarUrlRequest> OnUpdateAvatarUrlRequestEvent;

		public event PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse> OnUpdateAvatarUrlResultEvent;

		public event PlayFabRequestEvent<UpdateCharacterDataRequest> OnUpdateCharacterDataRequestEvent;

		public event PlayFabResultEvent<UpdateCharacterDataResult> OnUpdateCharacterDataResultEvent;

		public event PlayFabRequestEvent<UpdateCharacterStatisticsRequest> OnUpdateCharacterStatisticsRequestEvent;

		public event PlayFabResultEvent<UpdateCharacterStatisticsResult> OnUpdateCharacterStatisticsResultEvent;

		public event PlayFabRequestEvent<UpdatePlayerStatisticsRequest> OnUpdatePlayerStatisticsRequestEvent;

		public event PlayFabResultEvent<UpdatePlayerStatisticsResult> OnUpdatePlayerStatisticsResultEvent;

		public event PlayFabRequestEvent<UpdateSharedGroupDataRequest> OnUpdateSharedGroupDataRequestEvent;

		public event PlayFabResultEvent<UpdateSharedGroupDataResult> OnUpdateSharedGroupDataResultEvent;

		public event PlayFabRequestEvent<UpdateUserDataRequest> OnUpdateUserDataRequestEvent;

		public event PlayFabResultEvent<UpdateUserDataResult> OnUpdateUserDataResultEvent;

		public event PlayFabRequestEvent<UpdateUserDataRequest> OnUpdateUserPublisherDataRequestEvent;

		public event PlayFabResultEvent<UpdateUserDataResult> OnUpdateUserPublisherDataResultEvent;

		public event PlayFabRequestEvent<UpdateUserTitleDisplayNameRequest> OnUpdateUserTitleDisplayNameRequestEvent;

		public event PlayFabResultEvent<UpdateUserTitleDisplayNameResult> OnUpdateUserTitleDisplayNameResultEvent;

		public event PlayFabRequestEvent<ValidateAmazonReceiptRequest> OnValidateAmazonIAPReceiptRequestEvent;

		public event PlayFabResultEvent<ValidateAmazonReceiptResult> OnValidateAmazonIAPReceiptResultEvent;

		public event PlayFabRequestEvent<ValidateGooglePlayPurchaseRequest> OnValidateGooglePlayPurchaseRequestEvent;

		public event PlayFabResultEvent<ValidateGooglePlayPurchaseResult> OnValidateGooglePlayPurchaseResultEvent;

		public event PlayFabRequestEvent<ValidateIOSReceiptRequest> OnValidateIOSReceiptRequestEvent;

		public event PlayFabResultEvent<ValidateIOSReceiptResult> OnValidateIOSReceiptResultEvent;

		public event PlayFabRequestEvent<ValidateWindowsReceiptRequest> OnValidateWindowsStoreReceiptRequestEvent;

		public event PlayFabResultEvent<ValidateWindowsReceiptResult> OnValidateWindowsStoreReceiptResultEvent;

		public event PlayFabRequestEvent<WriteClientCharacterEventRequest> OnWriteCharacterEventRequestEvent;

		public event PlayFabResultEvent<WriteEventResponse> OnWriteCharacterEventResultEvent;

		public event PlayFabRequestEvent<WriteClientPlayerEventRequest> OnWritePlayerEventRequestEvent;

		public event PlayFabResultEvent<WriteEventResponse> OnWritePlayerEventResultEvent;

		public event PlayFabRequestEvent<WriteTitleEventRequest> OnWriteTitleEventRequestEvent;

		public event PlayFabResultEvent<WriteEventResponse> OnWriteTitleEventResultEvent;

		public event PlayFabRequestEvent<ExecuteEntityCloudScriptRequest> OnCloudScriptExecuteEntityCloudScriptRequestEvent;

		public event PlayFabResultEvent<PlayFab.CloudScriptModels.ExecuteCloudScriptResult> OnCloudScriptExecuteEntityCloudScriptResultEvent;

		public event PlayFabRequestEvent<AbortFileUploadsRequest> OnDataAbortFileUploadsRequestEvent;

		public event PlayFabResultEvent<AbortFileUploadsResponse> OnDataAbortFileUploadsResultEvent;

		public event PlayFabRequestEvent<DeleteFilesRequest> OnDataDeleteFilesRequestEvent;

		public event PlayFabResultEvent<DeleteFilesResponse> OnDataDeleteFilesResultEvent;

		public event PlayFabRequestEvent<FinalizeFileUploadsRequest> OnDataFinalizeFileUploadsRequestEvent;

		public event PlayFabResultEvent<FinalizeFileUploadsResponse> OnDataFinalizeFileUploadsResultEvent;

		public event PlayFabRequestEvent<GetFilesRequest> OnDataGetFilesRequestEvent;

		public event PlayFabResultEvent<GetFilesResponse> OnDataGetFilesResultEvent;

		public event PlayFabRequestEvent<GetObjectsRequest> OnDataGetObjectsRequestEvent;

		public event PlayFabResultEvent<GetObjectsResponse> OnDataGetObjectsResultEvent;

		public event PlayFabRequestEvent<InitiateFileUploadsRequest> OnDataInitiateFileUploadsRequestEvent;

		public event PlayFabResultEvent<InitiateFileUploadsResponse> OnDataInitiateFileUploadsResultEvent;

		public event PlayFabRequestEvent<SetObjectsRequest> OnDataSetObjectsRequestEvent;

		public event PlayFabResultEvent<SetObjectsResponse> OnDataSetObjectsResultEvent;

		public event PlayFabRequestEvent<WriteEventsRequest> OnEventsWriteEventsRequestEvent;

		public event PlayFabResultEvent<WriteEventsResponse> OnEventsWriteEventsResultEvent;

		public event PlayFabRequestEvent<AcceptGroupApplicationRequest> OnGroupsAcceptGroupApplicationRequestEvent;

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsAcceptGroupApplicationResultEvent;

		public event PlayFabRequestEvent<AcceptGroupInvitationRequest> OnGroupsAcceptGroupInvitationRequestEvent;

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsAcceptGroupInvitationResultEvent;

		public event PlayFabRequestEvent<AddMembersRequest> OnGroupsAddMembersRequestEvent;

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsAddMembersResultEvent;

		public event PlayFabRequestEvent<ApplyToGroupRequest> OnGroupsApplyToGroupRequestEvent;

		public event PlayFabResultEvent<ApplyToGroupResponse> OnGroupsApplyToGroupResultEvent;

		public event PlayFabRequestEvent<BlockEntityRequest> OnGroupsBlockEntityRequestEvent;

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsBlockEntityResultEvent;

		public event PlayFabRequestEvent<ChangeMemberRoleRequest> OnGroupsChangeMemberRoleRequestEvent;

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsChangeMemberRoleResultEvent;

		public event PlayFabRequestEvent<CreateGroupRequest> OnGroupsCreateGroupRequestEvent;

		public event PlayFabResultEvent<CreateGroupResponse> OnGroupsCreateGroupResultEvent;

		public event PlayFabRequestEvent<CreateGroupRoleRequest> OnGroupsCreateRoleRequestEvent;

		public event PlayFabResultEvent<CreateGroupRoleResponse> OnGroupsCreateRoleResultEvent;

		public event PlayFabRequestEvent<DeleteGroupRequest> OnGroupsDeleteGroupRequestEvent;

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsDeleteGroupResultEvent;

		public event PlayFabRequestEvent<DeleteRoleRequest> OnGroupsDeleteRoleRequestEvent;

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsDeleteRoleResultEvent;

		public event PlayFabRequestEvent<GetGroupRequest> OnGroupsGetGroupRequestEvent;

		public event PlayFabResultEvent<GetGroupResponse> OnGroupsGetGroupResultEvent;

		public event PlayFabRequestEvent<InviteToGroupRequest> OnGroupsInviteToGroupRequestEvent;

		public event PlayFabResultEvent<InviteToGroupResponse> OnGroupsInviteToGroupResultEvent;

		public event PlayFabRequestEvent<IsMemberRequest> OnGroupsIsMemberRequestEvent;

		public event PlayFabResultEvent<IsMemberResponse> OnGroupsIsMemberResultEvent;

		public event PlayFabRequestEvent<ListGroupApplicationsRequest> OnGroupsListGroupApplicationsRequestEvent;

		public event PlayFabResultEvent<ListGroupApplicationsResponse> OnGroupsListGroupApplicationsResultEvent;

		public event PlayFabRequestEvent<ListGroupBlocksRequest> OnGroupsListGroupBlocksRequestEvent;

		public event PlayFabResultEvent<ListGroupBlocksResponse> OnGroupsListGroupBlocksResultEvent;

		public event PlayFabRequestEvent<ListGroupInvitationsRequest> OnGroupsListGroupInvitationsRequestEvent;

		public event PlayFabResultEvent<ListGroupInvitationsResponse> OnGroupsListGroupInvitationsResultEvent;

		public event PlayFabRequestEvent<ListGroupMembersRequest> OnGroupsListGroupMembersRequestEvent;

		public event PlayFabResultEvent<ListGroupMembersResponse> OnGroupsListGroupMembersResultEvent;

		public event PlayFabRequestEvent<ListMembershipRequest> OnGroupsListMembershipRequestEvent;

		public event PlayFabResultEvent<ListMembershipResponse> OnGroupsListMembershipResultEvent;

		public event PlayFabRequestEvent<ListMembershipOpportunitiesRequest> OnGroupsListMembershipOpportunitiesRequestEvent;

		public event PlayFabResultEvent<ListMembershipOpportunitiesResponse> OnGroupsListMembershipOpportunitiesResultEvent;

		public event PlayFabRequestEvent<RemoveGroupApplicationRequest> OnGroupsRemoveGroupApplicationRequestEvent;

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsRemoveGroupApplicationResultEvent;

		public event PlayFabRequestEvent<RemoveGroupInvitationRequest> OnGroupsRemoveGroupInvitationRequestEvent;

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsRemoveGroupInvitationResultEvent;

		public event PlayFabRequestEvent<RemoveMembersRequest> OnGroupsRemoveMembersRequestEvent;

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsRemoveMembersResultEvent;

		public event PlayFabRequestEvent<UnblockEntityRequest> OnGroupsUnblockEntityRequestEvent;

		public event PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse> OnGroupsUnblockEntityResultEvent;

		public event PlayFabRequestEvent<UpdateGroupRequest> OnGroupsUpdateGroupRequestEvent;

		public event PlayFabResultEvent<UpdateGroupResponse> OnGroupsUpdateGroupResultEvent;

		public event PlayFabRequestEvent<UpdateGroupRoleRequest> OnGroupsUpdateRoleRequestEvent;

		public event PlayFabResultEvent<UpdateGroupRoleResponse> OnGroupsUpdateRoleResultEvent;

		public event PlayFabRequestEvent<GetLanguageListRequest> OnLocalizationGetLanguageListRequestEvent;

		public event PlayFabResultEvent<GetLanguageListResponse> OnLocalizationGetLanguageListResultEvent;

		public event PlayFabRequestEvent<CancelAllMatchmakingTicketsForPlayerRequest> OnMultiplayerCancelAllMatchmakingTicketsForPlayerRequestEvent;

		public event PlayFabResultEvent<CancelAllMatchmakingTicketsForPlayerResult> OnMultiplayerCancelAllMatchmakingTicketsForPlayerResultEvent;

		public event PlayFabRequestEvent<CancelMatchmakingTicketRequest> OnMultiplayerCancelMatchmakingTicketRequestEvent;

		public event PlayFabResultEvent<CancelMatchmakingTicketResult> OnMultiplayerCancelMatchmakingTicketResultEvent;

		public event PlayFabRequestEvent<CreateBuildWithCustomContainerRequest> OnMultiplayerCreateBuildWithCustomContainerRequestEvent;

		public event PlayFabResultEvent<CreateBuildWithCustomContainerResponse> OnMultiplayerCreateBuildWithCustomContainerResultEvent;

		public event PlayFabRequestEvent<CreateBuildWithManagedContainerRequest> OnMultiplayerCreateBuildWithManagedContainerRequestEvent;

		public event PlayFabResultEvent<CreateBuildWithManagedContainerResponse> OnMultiplayerCreateBuildWithManagedContainerResultEvent;

		public event PlayFabRequestEvent<CreateMatchmakingTicketRequest> OnMultiplayerCreateMatchmakingTicketRequestEvent;

		public event PlayFabResultEvent<CreateMatchmakingTicketResult> OnMultiplayerCreateMatchmakingTicketResultEvent;

		public event PlayFabRequestEvent<CreateRemoteUserRequest> OnMultiplayerCreateRemoteUserRequestEvent;

		public event PlayFabResultEvent<CreateRemoteUserResponse> OnMultiplayerCreateRemoteUserResultEvent;

		public event PlayFabRequestEvent<CreateServerMatchmakingTicketRequest> OnMultiplayerCreateServerMatchmakingTicketRequestEvent;

		public event PlayFabResultEvent<CreateMatchmakingTicketResult> OnMultiplayerCreateServerMatchmakingTicketResultEvent;

		public event PlayFabRequestEvent<DeleteAssetRequest> OnMultiplayerDeleteAssetRequestEvent;

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerDeleteAssetResultEvent;

		public event PlayFabRequestEvent<DeleteBuildRequest> OnMultiplayerDeleteBuildRequestEvent;

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerDeleteBuildResultEvent;

		public event PlayFabRequestEvent<DeleteCertificateRequest> OnMultiplayerDeleteCertificateRequestEvent;

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerDeleteCertificateResultEvent;

		public event PlayFabRequestEvent<DeleteRemoteUserRequest> OnMultiplayerDeleteRemoteUserRequestEvent;

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerDeleteRemoteUserResultEvent;

		public event PlayFabRequestEvent<EnableMultiplayerServersForTitleRequest> OnMultiplayerEnableMultiplayerServersForTitleRequestEvent;

		public event PlayFabResultEvent<EnableMultiplayerServersForTitleResponse> OnMultiplayerEnableMultiplayerServersForTitleResultEvent;

		public event PlayFabRequestEvent<GetAssetUploadUrlRequest> OnMultiplayerGetAssetUploadUrlRequestEvent;

		public event PlayFabResultEvent<GetAssetUploadUrlResponse> OnMultiplayerGetAssetUploadUrlResultEvent;

		public event PlayFabRequestEvent<GetBuildRequest> OnMultiplayerGetBuildRequestEvent;

		public event PlayFabResultEvent<GetBuildResponse> OnMultiplayerGetBuildResultEvent;

		public event PlayFabRequestEvent<GetContainerRegistryCredentialsRequest> OnMultiplayerGetContainerRegistryCredentialsRequestEvent;

		public event PlayFabResultEvent<GetContainerRegistryCredentialsResponse> OnMultiplayerGetContainerRegistryCredentialsResultEvent;

		public event PlayFabRequestEvent<GetMatchRequest> OnMultiplayerGetMatchRequestEvent;

		public event PlayFabResultEvent<GetMatchResult> OnMultiplayerGetMatchResultEvent;

		public event PlayFabRequestEvent<GetMatchmakingQueueRequest> OnMultiplayerGetMatchmakingQueueRequestEvent;

		public event PlayFabResultEvent<GetMatchmakingQueueResult> OnMultiplayerGetMatchmakingQueueResultEvent;

		public event PlayFabRequestEvent<GetMatchmakingTicketRequest> OnMultiplayerGetMatchmakingTicketRequestEvent;

		public event PlayFabResultEvent<GetMatchmakingTicketResult> OnMultiplayerGetMatchmakingTicketResultEvent;

		public event PlayFabRequestEvent<GetMultiplayerServerDetailsRequest> OnMultiplayerGetMultiplayerServerDetailsRequestEvent;

		public event PlayFabResultEvent<GetMultiplayerServerDetailsResponse> OnMultiplayerGetMultiplayerServerDetailsResultEvent;

		public event PlayFabRequestEvent<GetQueueStatisticsRequest> OnMultiplayerGetQueueStatisticsRequestEvent;

		public event PlayFabResultEvent<GetQueueStatisticsResult> OnMultiplayerGetQueueStatisticsResultEvent;

		public event PlayFabRequestEvent<GetRemoteLoginEndpointRequest> OnMultiplayerGetRemoteLoginEndpointRequestEvent;

		public event PlayFabResultEvent<GetRemoteLoginEndpointResponse> OnMultiplayerGetRemoteLoginEndpointResultEvent;

		public event PlayFabRequestEvent<GetTitleEnabledForMultiplayerServersStatusRequest> OnMultiplayerGetTitleEnabledForMultiplayerServersStatusRequestEvent;

		public event PlayFabResultEvent<GetTitleEnabledForMultiplayerServersStatusResponse> OnMultiplayerGetTitleEnabledForMultiplayerServersStatusResultEvent;

		public event PlayFabRequestEvent<JoinMatchmakingTicketRequest> OnMultiplayerJoinMatchmakingTicketRequestEvent;

		public event PlayFabResultEvent<JoinMatchmakingTicketResult> OnMultiplayerJoinMatchmakingTicketResultEvent;

		public event PlayFabRequestEvent<ListMultiplayerServersRequest> OnMultiplayerListArchivedMultiplayerServersRequestEvent;

		public event PlayFabResultEvent<ListMultiplayerServersResponse> OnMultiplayerListArchivedMultiplayerServersResultEvent;

		public event PlayFabRequestEvent<ListAssetSummariesRequest> OnMultiplayerListAssetSummariesRequestEvent;

		public event PlayFabResultEvent<ListAssetSummariesResponse> OnMultiplayerListAssetSummariesResultEvent;

		public event PlayFabRequestEvent<ListBuildSummariesRequest> OnMultiplayerListBuildSummariesRequestEvent;

		public event PlayFabResultEvent<ListBuildSummariesResponse> OnMultiplayerListBuildSummariesResultEvent;

		public event PlayFabRequestEvent<ListCertificateSummariesRequest> OnMultiplayerListCertificateSummariesRequestEvent;

		public event PlayFabResultEvent<ListCertificateSummariesResponse> OnMultiplayerListCertificateSummariesResultEvent;

		public event PlayFabRequestEvent<ListContainerImagesRequest> OnMultiplayerListContainerImagesRequestEvent;

		public event PlayFabResultEvent<ListContainerImagesResponse> OnMultiplayerListContainerImagesResultEvent;

		public event PlayFabRequestEvent<ListContainerImageTagsRequest> OnMultiplayerListContainerImageTagsRequestEvent;

		public event PlayFabResultEvent<ListContainerImageTagsResponse> OnMultiplayerListContainerImageTagsResultEvent;

		public event PlayFabRequestEvent<ListMatchmakingQueuesRequest> OnMultiplayerListMatchmakingQueuesRequestEvent;

		public event PlayFabResultEvent<ListMatchmakingQueuesResult> OnMultiplayerListMatchmakingQueuesResultEvent;

		public event PlayFabRequestEvent<ListMatchmakingTicketsForPlayerRequest> OnMultiplayerListMatchmakingTicketsForPlayerRequestEvent;

		public event PlayFabResultEvent<ListMatchmakingTicketsForPlayerResult> OnMultiplayerListMatchmakingTicketsForPlayerResultEvent;

		public event PlayFabRequestEvent<ListMultiplayerServersRequest> OnMultiplayerListMultiplayerServersRequestEvent;

		public event PlayFabResultEvent<ListMultiplayerServersResponse> OnMultiplayerListMultiplayerServersResultEvent;

		public event PlayFabRequestEvent<ListQosServersRequest> OnMultiplayerListQosServersRequestEvent;

		public event PlayFabResultEvent<ListQosServersResponse> OnMultiplayerListQosServersResultEvent;

		public event PlayFabRequestEvent<ListVirtualMachineSummariesRequest> OnMultiplayerListVirtualMachineSummariesRequestEvent;

		public event PlayFabResultEvent<ListVirtualMachineSummariesResponse> OnMultiplayerListVirtualMachineSummariesResultEvent;

		public event PlayFabRequestEvent<RemoveMatchmakingQueueRequest> OnMultiplayerRemoveMatchmakingQueueRequestEvent;

		public event PlayFabResultEvent<RemoveMatchmakingQueueResult> OnMultiplayerRemoveMatchmakingQueueResultEvent;

		public event PlayFabRequestEvent<RequestMultiplayerServerRequest> OnMultiplayerRequestMultiplayerServerRequestEvent;

		public event PlayFabResultEvent<RequestMultiplayerServerResponse> OnMultiplayerRequestMultiplayerServerResultEvent;

		public event PlayFabRequestEvent<RolloverContainerRegistryCredentialsRequest> OnMultiplayerRolloverContainerRegistryCredentialsRequestEvent;

		public event PlayFabResultEvent<RolloverContainerRegistryCredentialsResponse> OnMultiplayerRolloverContainerRegistryCredentialsResultEvent;

		public event PlayFabRequestEvent<SetMatchmakingQueueRequest> OnMultiplayerSetMatchmakingQueueRequestEvent;

		public event PlayFabResultEvent<SetMatchmakingQueueResult> OnMultiplayerSetMatchmakingQueueResultEvent;

		public event PlayFabRequestEvent<ShutdownMultiplayerServerRequest> OnMultiplayerShutdownMultiplayerServerRequestEvent;

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerShutdownMultiplayerServerResultEvent;

		public event PlayFabRequestEvent<UpdateBuildRegionsRequest> OnMultiplayerUpdateBuildRegionsRequestEvent;

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerUpdateBuildRegionsResultEvent;

		public event PlayFabRequestEvent<UploadCertificateRequest> OnMultiplayerUploadCertificateRequestEvent;

		public event PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse> OnMultiplayerUploadCertificateResultEvent;

		public event PlayFabRequestEvent<GetGlobalPolicyRequest> OnProfilesGetGlobalPolicyRequestEvent;

		public event PlayFabResultEvent<GetGlobalPolicyResponse> OnProfilesGetGlobalPolicyResultEvent;

		public event PlayFabRequestEvent<GetEntityProfileRequest> OnProfilesGetProfileRequestEvent;

		public event PlayFabResultEvent<GetEntityProfileResponse> OnProfilesGetProfileResultEvent;

		public event PlayFabRequestEvent<GetEntityProfilesRequest> OnProfilesGetProfilesRequestEvent;

		public event PlayFabResultEvent<GetEntityProfilesResponse> OnProfilesGetProfilesResultEvent;

		public event PlayFabRequestEvent<SetGlobalPolicyRequest> OnProfilesSetGlobalPolicyRequestEvent;

		public event PlayFabResultEvent<SetGlobalPolicyResponse> OnProfilesSetGlobalPolicyResultEvent;

		public event PlayFabRequestEvent<SetProfileLanguageRequest> OnProfilesSetProfileLanguageRequestEvent;

		public event PlayFabResultEvent<SetProfileLanguageResponse> OnProfilesSetProfileLanguageResultEvent;

		public event PlayFabRequestEvent<SetEntityProfilePolicyRequest> OnProfilesSetProfilePolicyRequestEvent;

		public event PlayFabResultEvent<SetEntityProfilePolicyResponse> OnProfilesSetProfilePolicyResultEvent;

		public event PlayFabErrorEvent OnGlobalErrorEvent;

		private PlayFabEvents()
		{
		}

		public static PlayFabEvents Init()
		{
			if (_instance == null)
			{
				_instance = new PlayFabEvents();
			}
			PlayFabHttp.ApiProcessingEventHandler += _instance.OnProcessingEvent;
			PlayFabHttp.ApiProcessingErrorEventHandler += _instance.OnProcessingErrorEvent;
			return _instance;
		}

		public void UnregisterInstance(object instance)
		{
			Delegate[] invocationList;
			if (this.OnLoginResultEvent != null)
			{
				invocationList = this.OnLoginResultEvent.GetInvocationList();
				foreach (Delegate obj in invocationList)
				{
					if (obj.Target == instance)
					{
						OnLoginResultEvent -= (PlayFabResultEvent<LoginResult>)obj;
					}
				}
			}
			if (this.OnAcceptTradeRequestEvent != null)
			{
				invocationList = this.OnAcceptTradeRequestEvent.GetInvocationList();
				foreach (Delegate obj2 in invocationList)
				{
					if (obj2.Target == instance)
					{
						OnAcceptTradeRequestEvent -= (PlayFabRequestEvent<AcceptTradeRequest>)obj2;
					}
				}
			}
			if (this.OnAcceptTradeResultEvent != null)
			{
				invocationList = this.OnAcceptTradeResultEvent.GetInvocationList();
				foreach (Delegate obj3 in invocationList)
				{
					if (obj3.Target == instance)
					{
						OnAcceptTradeResultEvent -= (PlayFabResultEvent<AcceptTradeResponse>)obj3;
					}
				}
			}
			if (this.OnAddFriendRequestEvent != null)
			{
				invocationList = this.OnAddFriendRequestEvent.GetInvocationList();
				foreach (Delegate obj4 in invocationList)
				{
					if (obj4.Target == instance)
					{
						OnAddFriendRequestEvent -= (PlayFabRequestEvent<AddFriendRequest>)obj4;
					}
				}
			}
			if (this.OnAddFriendResultEvent != null)
			{
				invocationList = this.OnAddFriendResultEvent.GetInvocationList();
				foreach (Delegate obj5 in invocationList)
				{
					if (obj5.Target == instance)
					{
						OnAddFriendResultEvent -= (PlayFabResultEvent<AddFriendResult>)obj5;
					}
				}
			}
			if (this.OnAddGenericIDRequestEvent != null)
			{
				invocationList = this.OnAddGenericIDRequestEvent.GetInvocationList();
				foreach (Delegate obj6 in invocationList)
				{
					if (obj6.Target == instance)
					{
						OnAddGenericIDRequestEvent -= (PlayFabRequestEvent<AddGenericIDRequest>)obj6;
					}
				}
			}
			if (this.OnAddGenericIDResultEvent != null)
			{
				invocationList = this.OnAddGenericIDResultEvent.GetInvocationList();
				foreach (Delegate obj7 in invocationList)
				{
					if (obj7.Target == instance)
					{
						OnAddGenericIDResultEvent -= (PlayFabResultEvent<AddGenericIDResult>)obj7;
					}
				}
			}
			if (this.OnAddOrUpdateContactEmailRequestEvent != null)
			{
				invocationList = this.OnAddOrUpdateContactEmailRequestEvent.GetInvocationList();
				foreach (Delegate obj8 in invocationList)
				{
					if (obj8.Target == instance)
					{
						OnAddOrUpdateContactEmailRequestEvent -= (PlayFabRequestEvent<AddOrUpdateContactEmailRequest>)obj8;
					}
				}
			}
			if (this.OnAddOrUpdateContactEmailResultEvent != null)
			{
				invocationList = this.OnAddOrUpdateContactEmailResultEvent.GetInvocationList();
				foreach (Delegate obj9 in invocationList)
				{
					if (obj9.Target == instance)
					{
						OnAddOrUpdateContactEmailResultEvent -= (PlayFabResultEvent<AddOrUpdateContactEmailResult>)obj9;
					}
				}
			}
			if (this.OnAddSharedGroupMembersRequestEvent != null)
			{
				invocationList = this.OnAddSharedGroupMembersRequestEvent.GetInvocationList();
				foreach (Delegate obj10 in invocationList)
				{
					if (obj10.Target == instance)
					{
						OnAddSharedGroupMembersRequestEvent -= (PlayFabRequestEvent<AddSharedGroupMembersRequest>)obj10;
					}
				}
			}
			if (this.OnAddSharedGroupMembersResultEvent != null)
			{
				invocationList = this.OnAddSharedGroupMembersResultEvent.GetInvocationList();
				foreach (Delegate obj11 in invocationList)
				{
					if (obj11.Target == instance)
					{
						OnAddSharedGroupMembersResultEvent -= (PlayFabResultEvent<AddSharedGroupMembersResult>)obj11;
					}
				}
			}
			if (this.OnAddUsernamePasswordRequestEvent != null)
			{
				invocationList = this.OnAddUsernamePasswordRequestEvent.GetInvocationList();
				foreach (Delegate obj12 in invocationList)
				{
					if (obj12.Target == instance)
					{
						OnAddUsernamePasswordRequestEvent -= (PlayFabRequestEvent<AddUsernamePasswordRequest>)obj12;
					}
				}
			}
			if (this.OnAddUsernamePasswordResultEvent != null)
			{
				invocationList = this.OnAddUsernamePasswordResultEvent.GetInvocationList();
				foreach (Delegate obj13 in invocationList)
				{
					if (obj13.Target == instance)
					{
						OnAddUsernamePasswordResultEvent -= (PlayFabResultEvent<AddUsernamePasswordResult>)obj13;
					}
				}
			}
			if (this.OnAddUserVirtualCurrencyRequestEvent != null)
			{
				invocationList = this.OnAddUserVirtualCurrencyRequestEvent.GetInvocationList();
				foreach (Delegate obj14 in invocationList)
				{
					if (obj14.Target == instance)
					{
						OnAddUserVirtualCurrencyRequestEvent -= (PlayFabRequestEvent<AddUserVirtualCurrencyRequest>)obj14;
					}
				}
			}
			if (this.OnAddUserVirtualCurrencyResultEvent != null)
			{
				invocationList = this.OnAddUserVirtualCurrencyResultEvent.GetInvocationList();
				foreach (Delegate obj15 in invocationList)
				{
					if (obj15.Target == instance)
					{
						OnAddUserVirtualCurrencyResultEvent -= (PlayFabResultEvent<ModifyUserVirtualCurrencyResult>)obj15;
					}
				}
			}
			if (this.OnAndroidDevicePushNotificationRegistrationRequestEvent != null)
			{
				invocationList = this.OnAndroidDevicePushNotificationRegistrationRequestEvent.GetInvocationList();
				foreach (Delegate obj16 in invocationList)
				{
					if (obj16.Target == instance)
					{
						OnAndroidDevicePushNotificationRegistrationRequestEvent -= (PlayFabRequestEvent<AndroidDevicePushNotificationRegistrationRequest>)obj16;
					}
				}
			}
			if (this.OnAndroidDevicePushNotificationRegistrationResultEvent != null)
			{
				invocationList = this.OnAndroidDevicePushNotificationRegistrationResultEvent.GetInvocationList();
				foreach (Delegate obj17 in invocationList)
				{
					if (obj17.Target == instance)
					{
						OnAndroidDevicePushNotificationRegistrationResultEvent -= (PlayFabResultEvent<AndroidDevicePushNotificationRegistrationResult>)obj17;
					}
				}
			}
			if (this.OnAttributeInstallRequestEvent != null)
			{
				invocationList = this.OnAttributeInstallRequestEvent.GetInvocationList();
				foreach (Delegate obj18 in invocationList)
				{
					if (obj18.Target == instance)
					{
						OnAttributeInstallRequestEvent -= (PlayFabRequestEvent<AttributeInstallRequest>)obj18;
					}
				}
			}
			if (this.OnAttributeInstallResultEvent != null)
			{
				invocationList = this.OnAttributeInstallResultEvent.GetInvocationList();
				foreach (Delegate obj19 in invocationList)
				{
					if (obj19.Target == instance)
					{
						OnAttributeInstallResultEvent -= (PlayFabResultEvent<AttributeInstallResult>)obj19;
					}
				}
			}
			if (this.OnCancelTradeRequestEvent != null)
			{
				invocationList = this.OnCancelTradeRequestEvent.GetInvocationList();
				foreach (Delegate obj20 in invocationList)
				{
					if (obj20.Target == instance)
					{
						OnCancelTradeRequestEvent -= (PlayFabRequestEvent<CancelTradeRequest>)obj20;
					}
				}
			}
			if (this.OnCancelTradeResultEvent != null)
			{
				invocationList = this.OnCancelTradeResultEvent.GetInvocationList();
				foreach (Delegate obj21 in invocationList)
				{
					if (obj21.Target == instance)
					{
						OnCancelTradeResultEvent -= (PlayFabResultEvent<CancelTradeResponse>)obj21;
					}
				}
			}
			if (this.OnConfirmPurchaseRequestEvent != null)
			{
				invocationList = this.OnConfirmPurchaseRequestEvent.GetInvocationList();
				foreach (Delegate obj22 in invocationList)
				{
					if (obj22.Target == instance)
					{
						OnConfirmPurchaseRequestEvent -= (PlayFabRequestEvent<ConfirmPurchaseRequest>)obj22;
					}
				}
			}
			if (this.OnConfirmPurchaseResultEvent != null)
			{
				invocationList = this.OnConfirmPurchaseResultEvent.GetInvocationList();
				foreach (Delegate obj23 in invocationList)
				{
					if (obj23.Target == instance)
					{
						OnConfirmPurchaseResultEvent -= (PlayFabResultEvent<ConfirmPurchaseResult>)obj23;
					}
				}
			}
			if (this.OnConsumeItemRequestEvent != null)
			{
				invocationList = this.OnConsumeItemRequestEvent.GetInvocationList();
				foreach (Delegate obj24 in invocationList)
				{
					if (obj24.Target == instance)
					{
						OnConsumeItemRequestEvent -= (PlayFabRequestEvent<ConsumeItemRequest>)obj24;
					}
				}
			}
			if (this.OnConsumeItemResultEvent != null)
			{
				invocationList = this.OnConsumeItemResultEvent.GetInvocationList();
				foreach (Delegate obj25 in invocationList)
				{
					if (obj25.Target == instance)
					{
						OnConsumeItemResultEvent -= (PlayFabResultEvent<ConsumeItemResult>)obj25;
					}
				}
			}
			if (this.OnConsumePSNEntitlementsRequestEvent != null)
			{
				invocationList = this.OnConsumePSNEntitlementsRequestEvent.GetInvocationList();
				foreach (Delegate obj26 in invocationList)
				{
					if (obj26.Target == instance)
					{
						OnConsumePSNEntitlementsRequestEvent -= (PlayFabRequestEvent<ConsumePSNEntitlementsRequest>)obj26;
					}
				}
			}
			if (this.OnConsumePSNEntitlementsResultEvent != null)
			{
				invocationList = this.OnConsumePSNEntitlementsResultEvent.GetInvocationList();
				foreach (Delegate obj27 in invocationList)
				{
					if (obj27.Target == instance)
					{
						OnConsumePSNEntitlementsResultEvent -= (PlayFabResultEvent<ConsumePSNEntitlementsResult>)obj27;
					}
				}
			}
			if (this.OnConsumeXboxEntitlementsRequestEvent != null)
			{
				invocationList = this.OnConsumeXboxEntitlementsRequestEvent.GetInvocationList();
				foreach (Delegate obj28 in invocationList)
				{
					if (obj28.Target == instance)
					{
						OnConsumeXboxEntitlementsRequestEvent -= (PlayFabRequestEvent<ConsumeXboxEntitlementsRequest>)obj28;
					}
				}
			}
			if (this.OnConsumeXboxEntitlementsResultEvent != null)
			{
				invocationList = this.OnConsumeXboxEntitlementsResultEvent.GetInvocationList();
				foreach (Delegate obj29 in invocationList)
				{
					if (obj29.Target == instance)
					{
						OnConsumeXboxEntitlementsResultEvent -= (PlayFabResultEvent<ConsumeXboxEntitlementsResult>)obj29;
					}
				}
			}
			if (this.OnCreateSharedGroupRequestEvent != null)
			{
				invocationList = this.OnCreateSharedGroupRequestEvent.GetInvocationList();
				foreach (Delegate obj30 in invocationList)
				{
					if (obj30.Target == instance)
					{
						OnCreateSharedGroupRequestEvent -= (PlayFabRequestEvent<CreateSharedGroupRequest>)obj30;
					}
				}
			}
			if (this.OnCreateSharedGroupResultEvent != null)
			{
				invocationList = this.OnCreateSharedGroupResultEvent.GetInvocationList();
				foreach (Delegate obj31 in invocationList)
				{
					if (obj31.Target == instance)
					{
						OnCreateSharedGroupResultEvent -= (PlayFabResultEvent<CreateSharedGroupResult>)obj31;
					}
				}
			}
			if (this.OnExecuteCloudScriptRequestEvent != null)
			{
				invocationList = this.OnExecuteCloudScriptRequestEvent.GetInvocationList();
				foreach (Delegate obj32 in invocationList)
				{
					if (obj32.Target == instance)
					{
						OnExecuteCloudScriptRequestEvent -= (PlayFabRequestEvent<ExecuteCloudScriptRequest>)obj32;
					}
				}
			}
			if (this.OnExecuteCloudScriptResultEvent != null)
			{
				invocationList = this.OnExecuteCloudScriptResultEvent.GetInvocationList();
				foreach (Delegate obj33 in invocationList)
				{
					if (obj33.Target == instance)
					{
						OnExecuteCloudScriptResultEvent -= (PlayFabResultEvent<PlayFab.ClientModels.ExecuteCloudScriptResult>)obj33;
					}
				}
			}
			if (this.OnGetAccountInfoRequestEvent != null)
			{
				invocationList = this.OnGetAccountInfoRequestEvent.GetInvocationList();
				foreach (Delegate obj34 in invocationList)
				{
					if (obj34.Target == instance)
					{
						OnGetAccountInfoRequestEvent -= (PlayFabRequestEvent<GetAccountInfoRequest>)obj34;
					}
				}
			}
			if (this.OnGetAccountInfoResultEvent != null)
			{
				invocationList = this.OnGetAccountInfoResultEvent.GetInvocationList();
				foreach (Delegate obj35 in invocationList)
				{
					if (obj35.Target == instance)
					{
						OnGetAccountInfoResultEvent -= (PlayFabResultEvent<GetAccountInfoResult>)obj35;
					}
				}
			}
			if (this.OnGetAllUsersCharactersRequestEvent != null)
			{
				invocationList = this.OnGetAllUsersCharactersRequestEvent.GetInvocationList();
				foreach (Delegate obj36 in invocationList)
				{
					if (obj36.Target == instance)
					{
						OnGetAllUsersCharactersRequestEvent -= (PlayFabRequestEvent<ListUsersCharactersRequest>)obj36;
					}
				}
			}
			if (this.OnGetAllUsersCharactersResultEvent != null)
			{
				invocationList = this.OnGetAllUsersCharactersResultEvent.GetInvocationList();
				foreach (Delegate obj37 in invocationList)
				{
					if (obj37.Target == instance)
					{
						OnGetAllUsersCharactersResultEvent -= (PlayFabResultEvent<ListUsersCharactersResult>)obj37;
					}
				}
			}
			if (this.OnGetCatalogItemsRequestEvent != null)
			{
				invocationList = this.OnGetCatalogItemsRequestEvent.GetInvocationList();
				foreach (Delegate obj38 in invocationList)
				{
					if (obj38.Target == instance)
					{
						OnGetCatalogItemsRequestEvent -= (PlayFabRequestEvent<GetCatalogItemsRequest>)obj38;
					}
				}
			}
			if (this.OnGetCatalogItemsResultEvent != null)
			{
				invocationList = this.OnGetCatalogItemsResultEvent.GetInvocationList();
				foreach (Delegate obj39 in invocationList)
				{
					if (obj39.Target == instance)
					{
						OnGetCatalogItemsResultEvent -= (PlayFabResultEvent<GetCatalogItemsResult>)obj39;
					}
				}
			}
			if (this.OnGetCharacterDataRequestEvent != null)
			{
				invocationList = this.OnGetCharacterDataRequestEvent.GetInvocationList();
				foreach (Delegate obj40 in invocationList)
				{
					if (obj40.Target == instance)
					{
						OnGetCharacterDataRequestEvent -= (PlayFabRequestEvent<GetCharacterDataRequest>)obj40;
					}
				}
			}
			if (this.OnGetCharacterDataResultEvent != null)
			{
				invocationList = this.OnGetCharacterDataResultEvent.GetInvocationList();
				foreach (Delegate obj41 in invocationList)
				{
					if (obj41.Target == instance)
					{
						OnGetCharacterDataResultEvent -= (PlayFabResultEvent<GetCharacterDataResult>)obj41;
					}
				}
			}
			if (this.OnGetCharacterInventoryRequestEvent != null)
			{
				invocationList = this.OnGetCharacterInventoryRequestEvent.GetInvocationList();
				foreach (Delegate obj42 in invocationList)
				{
					if (obj42.Target == instance)
					{
						OnGetCharacterInventoryRequestEvent -= (PlayFabRequestEvent<GetCharacterInventoryRequest>)obj42;
					}
				}
			}
			if (this.OnGetCharacterInventoryResultEvent != null)
			{
				invocationList = this.OnGetCharacterInventoryResultEvent.GetInvocationList();
				foreach (Delegate obj43 in invocationList)
				{
					if (obj43.Target == instance)
					{
						OnGetCharacterInventoryResultEvent -= (PlayFabResultEvent<GetCharacterInventoryResult>)obj43;
					}
				}
			}
			if (this.OnGetCharacterLeaderboardRequestEvent != null)
			{
				invocationList = this.OnGetCharacterLeaderboardRequestEvent.GetInvocationList();
				foreach (Delegate obj44 in invocationList)
				{
					if (obj44.Target == instance)
					{
						OnGetCharacterLeaderboardRequestEvent -= (PlayFabRequestEvent<GetCharacterLeaderboardRequest>)obj44;
					}
				}
			}
			if (this.OnGetCharacterLeaderboardResultEvent != null)
			{
				invocationList = this.OnGetCharacterLeaderboardResultEvent.GetInvocationList();
				foreach (Delegate obj45 in invocationList)
				{
					if (obj45.Target == instance)
					{
						OnGetCharacterLeaderboardResultEvent -= (PlayFabResultEvent<GetCharacterLeaderboardResult>)obj45;
					}
				}
			}
			if (this.OnGetCharacterReadOnlyDataRequestEvent != null)
			{
				invocationList = this.OnGetCharacterReadOnlyDataRequestEvent.GetInvocationList();
				foreach (Delegate obj46 in invocationList)
				{
					if (obj46.Target == instance)
					{
						OnGetCharacterReadOnlyDataRequestEvent -= (PlayFabRequestEvent<GetCharacterDataRequest>)obj46;
					}
				}
			}
			if (this.OnGetCharacterReadOnlyDataResultEvent != null)
			{
				invocationList = this.OnGetCharacterReadOnlyDataResultEvent.GetInvocationList();
				foreach (Delegate obj47 in invocationList)
				{
					if (obj47.Target == instance)
					{
						OnGetCharacterReadOnlyDataResultEvent -= (PlayFabResultEvent<GetCharacterDataResult>)obj47;
					}
				}
			}
			if (this.OnGetCharacterStatisticsRequestEvent != null)
			{
				invocationList = this.OnGetCharacterStatisticsRequestEvent.GetInvocationList();
				foreach (Delegate obj48 in invocationList)
				{
					if (obj48.Target == instance)
					{
						OnGetCharacterStatisticsRequestEvent -= (PlayFabRequestEvent<GetCharacterStatisticsRequest>)obj48;
					}
				}
			}
			if (this.OnGetCharacterStatisticsResultEvent != null)
			{
				invocationList = this.OnGetCharacterStatisticsResultEvent.GetInvocationList();
				foreach (Delegate obj49 in invocationList)
				{
					if (obj49.Target == instance)
					{
						OnGetCharacterStatisticsResultEvent -= (PlayFabResultEvent<GetCharacterStatisticsResult>)obj49;
					}
				}
			}
			if (this.OnGetContentDownloadUrlRequestEvent != null)
			{
				invocationList = this.OnGetContentDownloadUrlRequestEvent.GetInvocationList();
				foreach (Delegate obj50 in invocationList)
				{
					if (obj50.Target == instance)
					{
						OnGetContentDownloadUrlRequestEvent -= (PlayFabRequestEvent<GetContentDownloadUrlRequest>)obj50;
					}
				}
			}
			if (this.OnGetContentDownloadUrlResultEvent != null)
			{
				invocationList = this.OnGetContentDownloadUrlResultEvent.GetInvocationList();
				foreach (Delegate obj51 in invocationList)
				{
					if (obj51.Target == instance)
					{
						OnGetContentDownloadUrlResultEvent -= (PlayFabResultEvent<GetContentDownloadUrlResult>)obj51;
					}
				}
			}
			if (this.OnGetCurrentGamesRequestEvent != null)
			{
				invocationList = this.OnGetCurrentGamesRequestEvent.GetInvocationList();
				foreach (Delegate obj52 in invocationList)
				{
					if (obj52.Target == instance)
					{
						OnGetCurrentGamesRequestEvent -= (PlayFabRequestEvent<CurrentGamesRequest>)obj52;
					}
				}
			}
			if (this.OnGetCurrentGamesResultEvent != null)
			{
				invocationList = this.OnGetCurrentGamesResultEvent.GetInvocationList();
				foreach (Delegate obj53 in invocationList)
				{
					if (obj53.Target == instance)
					{
						OnGetCurrentGamesResultEvent -= (PlayFabResultEvent<CurrentGamesResult>)obj53;
					}
				}
			}
			if (this.OnGetFriendLeaderboardRequestEvent != null)
			{
				invocationList = this.OnGetFriendLeaderboardRequestEvent.GetInvocationList();
				foreach (Delegate obj54 in invocationList)
				{
					if (obj54.Target == instance)
					{
						OnGetFriendLeaderboardRequestEvent -= (PlayFabRequestEvent<GetFriendLeaderboardRequest>)obj54;
					}
				}
			}
			if (this.OnGetFriendLeaderboardResultEvent != null)
			{
				invocationList = this.OnGetFriendLeaderboardResultEvent.GetInvocationList();
				foreach (Delegate obj55 in invocationList)
				{
					if (obj55.Target == instance)
					{
						OnGetFriendLeaderboardResultEvent -= (PlayFabResultEvent<GetLeaderboardResult>)obj55;
					}
				}
			}
			if (this.OnGetFriendLeaderboardAroundPlayerRequestEvent != null)
			{
				invocationList = this.OnGetFriendLeaderboardAroundPlayerRequestEvent.GetInvocationList();
				foreach (Delegate obj56 in invocationList)
				{
					if (obj56.Target == instance)
					{
						OnGetFriendLeaderboardAroundPlayerRequestEvent -= (PlayFabRequestEvent<GetFriendLeaderboardAroundPlayerRequest>)obj56;
					}
				}
			}
			if (this.OnGetFriendLeaderboardAroundPlayerResultEvent != null)
			{
				invocationList = this.OnGetFriendLeaderboardAroundPlayerResultEvent.GetInvocationList();
				foreach (Delegate obj57 in invocationList)
				{
					if (obj57.Target == instance)
					{
						OnGetFriendLeaderboardAroundPlayerResultEvent -= (PlayFabResultEvent<GetFriendLeaderboardAroundPlayerResult>)obj57;
					}
				}
			}
			if (this.OnGetFriendsListRequestEvent != null)
			{
				invocationList = this.OnGetFriendsListRequestEvent.GetInvocationList();
				foreach (Delegate obj58 in invocationList)
				{
					if (obj58.Target == instance)
					{
						OnGetFriendsListRequestEvent -= (PlayFabRequestEvent<GetFriendsListRequest>)obj58;
					}
				}
			}
			if (this.OnGetFriendsListResultEvent != null)
			{
				invocationList = this.OnGetFriendsListResultEvent.GetInvocationList();
				foreach (Delegate obj59 in invocationList)
				{
					if (obj59.Target == instance)
					{
						OnGetFriendsListResultEvent -= (PlayFabResultEvent<GetFriendsListResult>)obj59;
					}
				}
			}
			if (this.OnGetGameServerRegionsRequestEvent != null)
			{
				invocationList = this.OnGetGameServerRegionsRequestEvent.GetInvocationList();
				foreach (Delegate obj60 in invocationList)
				{
					if (obj60.Target == instance)
					{
						OnGetGameServerRegionsRequestEvent -= (PlayFabRequestEvent<GameServerRegionsRequest>)obj60;
					}
				}
			}
			if (this.OnGetGameServerRegionsResultEvent != null)
			{
				invocationList = this.OnGetGameServerRegionsResultEvent.GetInvocationList();
				foreach (Delegate obj61 in invocationList)
				{
					if (obj61.Target == instance)
					{
						OnGetGameServerRegionsResultEvent -= (PlayFabResultEvent<GameServerRegionsResult>)obj61;
					}
				}
			}
			if (this.OnGetLeaderboardRequestEvent != null)
			{
				invocationList = this.OnGetLeaderboardRequestEvent.GetInvocationList();
				foreach (Delegate obj62 in invocationList)
				{
					if (obj62.Target == instance)
					{
						OnGetLeaderboardRequestEvent -= (PlayFabRequestEvent<GetLeaderboardRequest>)obj62;
					}
				}
			}
			if (this.OnGetLeaderboardResultEvent != null)
			{
				invocationList = this.OnGetLeaderboardResultEvent.GetInvocationList();
				foreach (Delegate obj63 in invocationList)
				{
					if (obj63.Target == instance)
					{
						OnGetLeaderboardResultEvent -= (PlayFabResultEvent<GetLeaderboardResult>)obj63;
					}
				}
			}
			if (this.OnGetLeaderboardAroundCharacterRequestEvent != null)
			{
				invocationList = this.OnGetLeaderboardAroundCharacterRequestEvent.GetInvocationList();
				foreach (Delegate obj64 in invocationList)
				{
					if (obj64.Target == instance)
					{
						OnGetLeaderboardAroundCharacterRequestEvent -= (PlayFabRequestEvent<GetLeaderboardAroundCharacterRequest>)obj64;
					}
				}
			}
			if (this.OnGetLeaderboardAroundCharacterResultEvent != null)
			{
				invocationList = this.OnGetLeaderboardAroundCharacterResultEvent.GetInvocationList();
				foreach (Delegate obj65 in invocationList)
				{
					if (obj65.Target == instance)
					{
						OnGetLeaderboardAroundCharacterResultEvent -= (PlayFabResultEvent<GetLeaderboardAroundCharacterResult>)obj65;
					}
				}
			}
			if (this.OnGetLeaderboardAroundPlayerRequestEvent != null)
			{
				invocationList = this.OnGetLeaderboardAroundPlayerRequestEvent.GetInvocationList();
				foreach (Delegate obj66 in invocationList)
				{
					if (obj66.Target == instance)
					{
						OnGetLeaderboardAroundPlayerRequestEvent -= (PlayFabRequestEvent<GetLeaderboardAroundPlayerRequest>)obj66;
					}
				}
			}
			if (this.OnGetLeaderboardAroundPlayerResultEvent != null)
			{
				invocationList = this.OnGetLeaderboardAroundPlayerResultEvent.GetInvocationList();
				foreach (Delegate obj67 in invocationList)
				{
					if (obj67.Target == instance)
					{
						OnGetLeaderboardAroundPlayerResultEvent -= (PlayFabResultEvent<GetLeaderboardAroundPlayerResult>)obj67;
					}
				}
			}
			if (this.OnGetLeaderboardForUserCharactersRequestEvent != null)
			{
				invocationList = this.OnGetLeaderboardForUserCharactersRequestEvent.GetInvocationList();
				foreach (Delegate obj68 in invocationList)
				{
					if (obj68.Target == instance)
					{
						OnGetLeaderboardForUserCharactersRequestEvent -= (PlayFabRequestEvent<GetLeaderboardForUsersCharactersRequest>)obj68;
					}
				}
			}
			if (this.OnGetLeaderboardForUserCharactersResultEvent != null)
			{
				invocationList = this.OnGetLeaderboardForUserCharactersResultEvent.GetInvocationList();
				foreach (Delegate obj69 in invocationList)
				{
					if (obj69.Target == instance)
					{
						OnGetLeaderboardForUserCharactersResultEvent -= (PlayFabResultEvent<GetLeaderboardForUsersCharactersResult>)obj69;
					}
				}
			}
			if (this.OnGetPaymentTokenRequestEvent != null)
			{
				invocationList = this.OnGetPaymentTokenRequestEvent.GetInvocationList();
				foreach (Delegate obj70 in invocationList)
				{
					if (obj70.Target == instance)
					{
						OnGetPaymentTokenRequestEvent -= (PlayFabRequestEvent<GetPaymentTokenRequest>)obj70;
					}
				}
			}
			if (this.OnGetPaymentTokenResultEvent != null)
			{
				invocationList = this.OnGetPaymentTokenResultEvent.GetInvocationList();
				foreach (Delegate obj71 in invocationList)
				{
					if (obj71.Target == instance)
					{
						OnGetPaymentTokenResultEvent -= (PlayFabResultEvent<GetPaymentTokenResult>)obj71;
					}
				}
			}
			if (this.OnGetPhotonAuthenticationTokenRequestEvent != null)
			{
				invocationList = this.OnGetPhotonAuthenticationTokenRequestEvent.GetInvocationList();
				foreach (Delegate obj72 in invocationList)
				{
					if (obj72.Target == instance)
					{
						OnGetPhotonAuthenticationTokenRequestEvent -= (PlayFabRequestEvent<GetPhotonAuthenticationTokenRequest>)obj72;
					}
				}
			}
			if (this.OnGetPhotonAuthenticationTokenResultEvent != null)
			{
				invocationList = this.OnGetPhotonAuthenticationTokenResultEvent.GetInvocationList();
				foreach (Delegate obj73 in invocationList)
				{
					if (obj73.Target == instance)
					{
						OnGetPhotonAuthenticationTokenResultEvent -= (PlayFabResultEvent<GetPhotonAuthenticationTokenResult>)obj73;
					}
				}
			}
			if (this.OnGetPlayerCombinedInfoRequestEvent != null)
			{
				invocationList = this.OnGetPlayerCombinedInfoRequestEvent.GetInvocationList();
				foreach (Delegate obj74 in invocationList)
				{
					if (obj74.Target == instance)
					{
						OnGetPlayerCombinedInfoRequestEvent -= (PlayFabRequestEvent<GetPlayerCombinedInfoRequest>)obj74;
					}
				}
			}
			if (this.OnGetPlayerCombinedInfoResultEvent != null)
			{
				invocationList = this.OnGetPlayerCombinedInfoResultEvent.GetInvocationList();
				foreach (Delegate obj75 in invocationList)
				{
					if (obj75.Target == instance)
					{
						OnGetPlayerCombinedInfoResultEvent -= (PlayFabResultEvent<GetPlayerCombinedInfoResult>)obj75;
					}
				}
			}
			if (this.OnGetPlayerProfileRequestEvent != null)
			{
				invocationList = this.OnGetPlayerProfileRequestEvent.GetInvocationList();
				foreach (Delegate obj76 in invocationList)
				{
					if (obj76.Target == instance)
					{
						OnGetPlayerProfileRequestEvent -= (PlayFabRequestEvent<GetPlayerProfileRequest>)obj76;
					}
				}
			}
			if (this.OnGetPlayerProfileResultEvent != null)
			{
				invocationList = this.OnGetPlayerProfileResultEvent.GetInvocationList();
				foreach (Delegate obj77 in invocationList)
				{
					if (obj77.Target == instance)
					{
						OnGetPlayerProfileResultEvent -= (PlayFabResultEvent<GetPlayerProfileResult>)obj77;
					}
				}
			}
			if (this.OnGetPlayerSegmentsRequestEvent != null)
			{
				invocationList = this.OnGetPlayerSegmentsRequestEvent.GetInvocationList();
				foreach (Delegate obj78 in invocationList)
				{
					if (obj78.Target == instance)
					{
						OnGetPlayerSegmentsRequestEvent -= (PlayFabRequestEvent<GetPlayerSegmentsRequest>)obj78;
					}
				}
			}
			if (this.OnGetPlayerSegmentsResultEvent != null)
			{
				invocationList = this.OnGetPlayerSegmentsResultEvent.GetInvocationList();
				foreach (Delegate obj79 in invocationList)
				{
					if (obj79.Target == instance)
					{
						OnGetPlayerSegmentsResultEvent -= (PlayFabResultEvent<GetPlayerSegmentsResult>)obj79;
					}
				}
			}
			if (this.OnGetPlayerStatisticsRequestEvent != null)
			{
				invocationList = this.OnGetPlayerStatisticsRequestEvent.GetInvocationList();
				foreach (Delegate obj80 in invocationList)
				{
					if (obj80.Target == instance)
					{
						OnGetPlayerStatisticsRequestEvent -= (PlayFabRequestEvent<GetPlayerStatisticsRequest>)obj80;
					}
				}
			}
			if (this.OnGetPlayerStatisticsResultEvent != null)
			{
				invocationList = this.OnGetPlayerStatisticsResultEvent.GetInvocationList();
				foreach (Delegate obj81 in invocationList)
				{
					if (obj81.Target == instance)
					{
						OnGetPlayerStatisticsResultEvent -= (PlayFabResultEvent<GetPlayerStatisticsResult>)obj81;
					}
				}
			}
			if (this.OnGetPlayerStatisticVersionsRequestEvent != null)
			{
				invocationList = this.OnGetPlayerStatisticVersionsRequestEvent.GetInvocationList();
				foreach (Delegate obj82 in invocationList)
				{
					if (obj82.Target == instance)
					{
						OnGetPlayerStatisticVersionsRequestEvent -= (PlayFabRequestEvent<GetPlayerStatisticVersionsRequest>)obj82;
					}
				}
			}
			if (this.OnGetPlayerStatisticVersionsResultEvent != null)
			{
				invocationList = this.OnGetPlayerStatisticVersionsResultEvent.GetInvocationList();
				foreach (Delegate obj83 in invocationList)
				{
					if (obj83.Target == instance)
					{
						OnGetPlayerStatisticVersionsResultEvent -= (PlayFabResultEvent<GetPlayerStatisticVersionsResult>)obj83;
					}
				}
			}
			if (this.OnGetPlayerTagsRequestEvent != null)
			{
				invocationList = this.OnGetPlayerTagsRequestEvent.GetInvocationList();
				foreach (Delegate obj84 in invocationList)
				{
					if (obj84.Target == instance)
					{
						OnGetPlayerTagsRequestEvent -= (PlayFabRequestEvent<GetPlayerTagsRequest>)obj84;
					}
				}
			}
			if (this.OnGetPlayerTagsResultEvent != null)
			{
				invocationList = this.OnGetPlayerTagsResultEvent.GetInvocationList();
				foreach (Delegate obj85 in invocationList)
				{
					if (obj85.Target == instance)
					{
						OnGetPlayerTagsResultEvent -= (PlayFabResultEvent<GetPlayerTagsResult>)obj85;
					}
				}
			}
			if (this.OnGetPlayerTradesRequestEvent != null)
			{
				invocationList = this.OnGetPlayerTradesRequestEvent.GetInvocationList();
				foreach (Delegate obj86 in invocationList)
				{
					if (obj86.Target == instance)
					{
						OnGetPlayerTradesRequestEvent -= (PlayFabRequestEvent<GetPlayerTradesRequest>)obj86;
					}
				}
			}
			if (this.OnGetPlayerTradesResultEvent != null)
			{
				invocationList = this.OnGetPlayerTradesResultEvent.GetInvocationList();
				foreach (Delegate obj87 in invocationList)
				{
					if (obj87.Target == instance)
					{
						OnGetPlayerTradesResultEvent -= (PlayFabResultEvent<GetPlayerTradesResponse>)obj87;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromFacebookIDsRequestEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromFacebookIDsRequestEvent.GetInvocationList();
				foreach (Delegate obj88 in invocationList)
				{
					if (obj88.Target == instance)
					{
						OnGetPlayFabIDsFromFacebookIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromFacebookIDsRequest>)obj88;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromFacebookIDsResultEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromFacebookIDsResultEvent.GetInvocationList();
				foreach (Delegate obj89 in invocationList)
				{
					if (obj89.Target == instance)
					{
						OnGetPlayFabIDsFromFacebookIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromFacebookIDsResult>)obj89;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromFacebookInstantGamesIdsRequestEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromFacebookInstantGamesIdsRequestEvent.GetInvocationList();
				foreach (Delegate obj90 in invocationList)
				{
					if (obj90.Target == instance)
					{
						OnGetPlayFabIDsFromFacebookInstantGamesIdsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromFacebookInstantGamesIdsRequest>)obj90;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromFacebookInstantGamesIdsResultEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromFacebookInstantGamesIdsResultEvent.GetInvocationList();
				foreach (Delegate obj91 in invocationList)
				{
					if (obj91.Target == instance)
					{
						OnGetPlayFabIDsFromFacebookInstantGamesIdsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromFacebookInstantGamesIdsResult>)obj91;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromGameCenterIDsRequestEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromGameCenterIDsRequestEvent.GetInvocationList();
				foreach (Delegate obj92 in invocationList)
				{
					if (obj92.Target == instance)
					{
						OnGetPlayFabIDsFromGameCenterIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromGameCenterIDsRequest>)obj92;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromGameCenterIDsResultEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromGameCenterIDsResultEvent.GetInvocationList();
				foreach (Delegate obj93 in invocationList)
				{
					if (obj93.Target == instance)
					{
						OnGetPlayFabIDsFromGameCenterIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromGameCenterIDsResult>)obj93;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromGenericIDsRequestEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromGenericIDsRequestEvent.GetInvocationList();
				foreach (Delegate obj94 in invocationList)
				{
					if (obj94.Target == instance)
					{
						OnGetPlayFabIDsFromGenericIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromGenericIDsRequest>)obj94;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromGenericIDsResultEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromGenericIDsResultEvent.GetInvocationList();
				foreach (Delegate obj95 in invocationList)
				{
					if (obj95.Target == instance)
					{
						OnGetPlayFabIDsFromGenericIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromGenericIDsResult>)obj95;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromGoogleIDsRequestEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromGoogleIDsRequestEvent.GetInvocationList();
				foreach (Delegate obj96 in invocationList)
				{
					if (obj96.Target == instance)
					{
						OnGetPlayFabIDsFromGoogleIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromGoogleIDsRequest>)obj96;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromGoogleIDsResultEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromGoogleIDsResultEvent.GetInvocationList();
				foreach (Delegate obj97 in invocationList)
				{
					if (obj97.Target == instance)
					{
						OnGetPlayFabIDsFromGoogleIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromGoogleIDsResult>)obj97;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromKongregateIDsRequestEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromKongregateIDsRequestEvent.GetInvocationList();
				foreach (Delegate obj98 in invocationList)
				{
					if (obj98.Target == instance)
					{
						OnGetPlayFabIDsFromKongregateIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromKongregateIDsRequest>)obj98;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromKongregateIDsResultEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromKongregateIDsResultEvent.GetInvocationList();
				foreach (Delegate obj99 in invocationList)
				{
					if (obj99.Target == instance)
					{
						OnGetPlayFabIDsFromKongregateIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromKongregateIDsResult>)obj99;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromNintendoSwitchDeviceIdsRequestEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromNintendoSwitchDeviceIdsRequestEvent.GetInvocationList();
				foreach (Delegate obj100 in invocationList)
				{
					if (obj100.Target == instance)
					{
						OnGetPlayFabIDsFromNintendoSwitchDeviceIdsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromNintendoSwitchDeviceIdsRequest>)obj100;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromNintendoSwitchDeviceIdsResultEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromNintendoSwitchDeviceIdsResultEvent.GetInvocationList();
				foreach (Delegate obj101 in invocationList)
				{
					if (obj101.Target == instance)
					{
						OnGetPlayFabIDsFromNintendoSwitchDeviceIdsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromNintendoSwitchDeviceIdsResult>)obj101;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromPSNAccountIDsRequestEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromPSNAccountIDsRequestEvent.GetInvocationList();
				foreach (Delegate obj102 in invocationList)
				{
					if (obj102.Target == instance)
					{
						OnGetPlayFabIDsFromPSNAccountIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromPSNAccountIDsRequest>)obj102;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromPSNAccountIDsResultEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromPSNAccountIDsResultEvent.GetInvocationList();
				foreach (Delegate obj103 in invocationList)
				{
					if (obj103.Target == instance)
					{
						OnGetPlayFabIDsFromPSNAccountIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromPSNAccountIDsResult>)obj103;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromSteamIDsRequestEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromSteamIDsRequestEvent.GetInvocationList();
				foreach (Delegate obj104 in invocationList)
				{
					if (obj104.Target == instance)
					{
						OnGetPlayFabIDsFromSteamIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromSteamIDsRequest>)obj104;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromSteamIDsResultEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromSteamIDsResultEvent.GetInvocationList();
				foreach (Delegate obj105 in invocationList)
				{
					if (obj105.Target == instance)
					{
						OnGetPlayFabIDsFromSteamIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromSteamIDsResult>)obj105;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromTwitchIDsRequestEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromTwitchIDsRequestEvent.GetInvocationList();
				foreach (Delegate obj106 in invocationList)
				{
					if (obj106.Target == instance)
					{
						OnGetPlayFabIDsFromTwitchIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromTwitchIDsRequest>)obj106;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromTwitchIDsResultEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromTwitchIDsResultEvent.GetInvocationList();
				foreach (Delegate obj107 in invocationList)
				{
					if (obj107.Target == instance)
					{
						OnGetPlayFabIDsFromTwitchIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromTwitchIDsResult>)obj107;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromXboxLiveIDsRequestEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromXboxLiveIDsRequestEvent.GetInvocationList();
				foreach (Delegate obj108 in invocationList)
				{
					if (obj108.Target == instance)
					{
						OnGetPlayFabIDsFromXboxLiveIDsRequestEvent -= (PlayFabRequestEvent<GetPlayFabIDsFromXboxLiveIDsRequest>)obj108;
					}
				}
			}
			if (this.OnGetPlayFabIDsFromXboxLiveIDsResultEvent != null)
			{
				invocationList = this.OnGetPlayFabIDsFromXboxLiveIDsResultEvent.GetInvocationList();
				foreach (Delegate obj109 in invocationList)
				{
					if (obj109.Target == instance)
					{
						OnGetPlayFabIDsFromXboxLiveIDsResultEvent -= (PlayFabResultEvent<GetPlayFabIDsFromXboxLiveIDsResult>)obj109;
					}
				}
			}
			if (this.OnGetPublisherDataRequestEvent != null)
			{
				invocationList = this.OnGetPublisherDataRequestEvent.GetInvocationList();
				foreach (Delegate obj110 in invocationList)
				{
					if (obj110.Target == instance)
					{
						OnGetPublisherDataRequestEvent -= (PlayFabRequestEvent<GetPublisherDataRequest>)obj110;
					}
				}
			}
			if (this.OnGetPublisherDataResultEvent != null)
			{
				invocationList = this.OnGetPublisherDataResultEvent.GetInvocationList();
				foreach (Delegate obj111 in invocationList)
				{
					if (obj111.Target == instance)
					{
						OnGetPublisherDataResultEvent -= (PlayFabResultEvent<GetPublisherDataResult>)obj111;
					}
				}
			}
			if (this.OnGetPurchaseRequestEvent != null)
			{
				invocationList = this.OnGetPurchaseRequestEvent.GetInvocationList();
				foreach (Delegate obj112 in invocationList)
				{
					if (obj112.Target == instance)
					{
						OnGetPurchaseRequestEvent -= (PlayFabRequestEvent<GetPurchaseRequest>)obj112;
					}
				}
			}
			if (this.OnGetPurchaseResultEvent != null)
			{
				invocationList = this.OnGetPurchaseResultEvent.GetInvocationList();
				foreach (Delegate obj113 in invocationList)
				{
					if (obj113.Target == instance)
					{
						OnGetPurchaseResultEvent -= (PlayFabResultEvent<GetPurchaseResult>)obj113;
					}
				}
			}
			if (this.OnGetSharedGroupDataRequestEvent != null)
			{
				invocationList = this.OnGetSharedGroupDataRequestEvent.GetInvocationList();
				foreach (Delegate obj114 in invocationList)
				{
					if (obj114.Target == instance)
					{
						OnGetSharedGroupDataRequestEvent -= (PlayFabRequestEvent<GetSharedGroupDataRequest>)obj114;
					}
				}
			}
			if (this.OnGetSharedGroupDataResultEvent != null)
			{
				invocationList = this.OnGetSharedGroupDataResultEvent.GetInvocationList();
				foreach (Delegate obj115 in invocationList)
				{
					if (obj115.Target == instance)
					{
						OnGetSharedGroupDataResultEvent -= (PlayFabResultEvent<GetSharedGroupDataResult>)obj115;
					}
				}
			}
			if (this.OnGetStoreItemsRequestEvent != null)
			{
				invocationList = this.OnGetStoreItemsRequestEvent.GetInvocationList();
				foreach (Delegate obj116 in invocationList)
				{
					if (obj116.Target == instance)
					{
						OnGetStoreItemsRequestEvent -= (PlayFabRequestEvent<GetStoreItemsRequest>)obj116;
					}
				}
			}
			if (this.OnGetStoreItemsResultEvent != null)
			{
				invocationList = this.OnGetStoreItemsResultEvent.GetInvocationList();
				foreach (Delegate obj117 in invocationList)
				{
					if (obj117.Target == instance)
					{
						OnGetStoreItemsResultEvent -= (PlayFabResultEvent<GetStoreItemsResult>)obj117;
					}
				}
			}
			if (this.OnGetTimeRequestEvent != null)
			{
				invocationList = this.OnGetTimeRequestEvent.GetInvocationList();
				foreach (Delegate obj118 in invocationList)
				{
					if (obj118.Target == instance)
					{
						OnGetTimeRequestEvent -= (PlayFabRequestEvent<GetTimeRequest>)obj118;
					}
				}
			}
			if (this.OnGetTimeResultEvent != null)
			{
				invocationList = this.OnGetTimeResultEvent.GetInvocationList();
				foreach (Delegate obj119 in invocationList)
				{
					if (obj119.Target == instance)
					{
						OnGetTimeResultEvent -= (PlayFabResultEvent<GetTimeResult>)obj119;
					}
				}
			}
			if (this.OnGetTitleDataRequestEvent != null)
			{
				invocationList = this.OnGetTitleDataRequestEvent.GetInvocationList();
				foreach (Delegate obj120 in invocationList)
				{
					if (obj120.Target == instance)
					{
						OnGetTitleDataRequestEvent -= (PlayFabRequestEvent<GetTitleDataRequest>)obj120;
					}
				}
			}
			if (this.OnGetTitleDataResultEvent != null)
			{
				invocationList = this.OnGetTitleDataResultEvent.GetInvocationList();
				foreach (Delegate obj121 in invocationList)
				{
					if (obj121.Target == instance)
					{
						OnGetTitleDataResultEvent -= (PlayFabResultEvent<GetTitleDataResult>)obj121;
					}
				}
			}
			if (this.OnGetTitleNewsRequestEvent != null)
			{
				invocationList = this.OnGetTitleNewsRequestEvent.GetInvocationList();
				foreach (Delegate obj122 in invocationList)
				{
					if (obj122.Target == instance)
					{
						OnGetTitleNewsRequestEvent -= (PlayFabRequestEvent<GetTitleNewsRequest>)obj122;
					}
				}
			}
			if (this.OnGetTitleNewsResultEvent != null)
			{
				invocationList = this.OnGetTitleNewsResultEvent.GetInvocationList();
				foreach (Delegate obj123 in invocationList)
				{
					if (obj123.Target == instance)
					{
						OnGetTitleNewsResultEvent -= (PlayFabResultEvent<GetTitleNewsResult>)obj123;
					}
				}
			}
			if (this.OnGetTitlePublicKeyRequestEvent != null)
			{
				invocationList = this.OnGetTitlePublicKeyRequestEvent.GetInvocationList();
				foreach (Delegate obj124 in invocationList)
				{
					if (obj124.Target == instance)
					{
						OnGetTitlePublicKeyRequestEvent -= (PlayFabRequestEvent<GetTitlePublicKeyRequest>)obj124;
					}
				}
			}
			if (this.OnGetTitlePublicKeyResultEvent != null)
			{
				invocationList = this.OnGetTitlePublicKeyResultEvent.GetInvocationList();
				foreach (Delegate obj125 in invocationList)
				{
					if (obj125.Target == instance)
					{
						OnGetTitlePublicKeyResultEvent -= (PlayFabResultEvent<GetTitlePublicKeyResult>)obj125;
					}
				}
			}
			if (this.OnGetTradeStatusRequestEvent != null)
			{
				invocationList = this.OnGetTradeStatusRequestEvent.GetInvocationList();
				foreach (Delegate obj126 in invocationList)
				{
					if (obj126.Target == instance)
					{
						OnGetTradeStatusRequestEvent -= (PlayFabRequestEvent<GetTradeStatusRequest>)obj126;
					}
				}
			}
			if (this.OnGetTradeStatusResultEvent != null)
			{
				invocationList = this.OnGetTradeStatusResultEvent.GetInvocationList();
				foreach (Delegate obj127 in invocationList)
				{
					if (obj127.Target == instance)
					{
						OnGetTradeStatusResultEvent -= (PlayFabResultEvent<GetTradeStatusResponse>)obj127;
					}
				}
			}
			if (this.OnGetUserDataRequestEvent != null)
			{
				invocationList = this.OnGetUserDataRequestEvent.GetInvocationList();
				foreach (Delegate obj128 in invocationList)
				{
					if (obj128.Target == instance)
					{
						OnGetUserDataRequestEvent -= (PlayFabRequestEvent<GetUserDataRequest>)obj128;
					}
				}
			}
			if (this.OnGetUserDataResultEvent != null)
			{
				invocationList = this.OnGetUserDataResultEvent.GetInvocationList();
				foreach (Delegate obj129 in invocationList)
				{
					if (obj129.Target == instance)
					{
						OnGetUserDataResultEvent -= (PlayFabResultEvent<GetUserDataResult>)obj129;
					}
				}
			}
			if (this.OnGetUserInventoryRequestEvent != null)
			{
				invocationList = this.OnGetUserInventoryRequestEvent.GetInvocationList();
				foreach (Delegate obj130 in invocationList)
				{
					if (obj130.Target == instance)
					{
						OnGetUserInventoryRequestEvent -= (PlayFabRequestEvent<GetUserInventoryRequest>)obj130;
					}
				}
			}
			if (this.OnGetUserInventoryResultEvent != null)
			{
				invocationList = this.OnGetUserInventoryResultEvent.GetInvocationList();
				foreach (Delegate obj131 in invocationList)
				{
					if (obj131.Target == instance)
					{
						OnGetUserInventoryResultEvent -= (PlayFabResultEvent<GetUserInventoryResult>)obj131;
					}
				}
			}
			if (this.OnGetUserPublisherDataRequestEvent != null)
			{
				invocationList = this.OnGetUserPublisherDataRequestEvent.GetInvocationList();
				foreach (Delegate obj132 in invocationList)
				{
					if (obj132.Target == instance)
					{
						OnGetUserPublisherDataRequestEvent -= (PlayFabRequestEvent<GetUserDataRequest>)obj132;
					}
				}
			}
			if (this.OnGetUserPublisherDataResultEvent != null)
			{
				invocationList = this.OnGetUserPublisherDataResultEvent.GetInvocationList();
				foreach (Delegate obj133 in invocationList)
				{
					if (obj133.Target == instance)
					{
						OnGetUserPublisherDataResultEvent -= (PlayFabResultEvent<GetUserDataResult>)obj133;
					}
				}
			}
			if (this.OnGetUserPublisherReadOnlyDataRequestEvent != null)
			{
				invocationList = this.OnGetUserPublisherReadOnlyDataRequestEvent.GetInvocationList();
				foreach (Delegate obj134 in invocationList)
				{
					if (obj134.Target == instance)
					{
						OnGetUserPublisherReadOnlyDataRequestEvent -= (PlayFabRequestEvent<GetUserDataRequest>)obj134;
					}
				}
			}
			if (this.OnGetUserPublisherReadOnlyDataResultEvent != null)
			{
				invocationList = this.OnGetUserPublisherReadOnlyDataResultEvent.GetInvocationList();
				foreach (Delegate obj135 in invocationList)
				{
					if (obj135.Target == instance)
					{
						OnGetUserPublisherReadOnlyDataResultEvent -= (PlayFabResultEvent<GetUserDataResult>)obj135;
					}
				}
			}
			if (this.OnGetUserReadOnlyDataRequestEvent != null)
			{
				invocationList = this.OnGetUserReadOnlyDataRequestEvent.GetInvocationList();
				foreach (Delegate obj136 in invocationList)
				{
					if (obj136.Target == instance)
					{
						OnGetUserReadOnlyDataRequestEvent -= (PlayFabRequestEvent<GetUserDataRequest>)obj136;
					}
				}
			}
			if (this.OnGetUserReadOnlyDataResultEvent != null)
			{
				invocationList = this.OnGetUserReadOnlyDataResultEvent.GetInvocationList();
				foreach (Delegate obj137 in invocationList)
				{
					if (obj137.Target == instance)
					{
						OnGetUserReadOnlyDataResultEvent -= (PlayFabResultEvent<GetUserDataResult>)obj137;
					}
				}
			}
			if (this.OnGetWindowsHelloChallengeRequestEvent != null)
			{
				invocationList = this.OnGetWindowsHelloChallengeRequestEvent.GetInvocationList();
				foreach (Delegate obj138 in invocationList)
				{
					if (obj138.Target == instance)
					{
						OnGetWindowsHelloChallengeRequestEvent -= (PlayFabRequestEvent<GetWindowsHelloChallengeRequest>)obj138;
					}
				}
			}
			if (this.OnGetWindowsHelloChallengeResultEvent != null)
			{
				invocationList = this.OnGetWindowsHelloChallengeResultEvent.GetInvocationList();
				foreach (Delegate obj139 in invocationList)
				{
					if (obj139.Target == instance)
					{
						OnGetWindowsHelloChallengeResultEvent -= (PlayFabResultEvent<GetWindowsHelloChallengeResponse>)obj139;
					}
				}
			}
			if (this.OnGrantCharacterToUserRequestEvent != null)
			{
				invocationList = this.OnGrantCharacterToUserRequestEvent.GetInvocationList();
				foreach (Delegate obj140 in invocationList)
				{
					if (obj140.Target == instance)
					{
						OnGrantCharacterToUserRequestEvent -= (PlayFabRequestEvent<GrantCharacterToUserRequest>)obj140;
					}
				}
			}
			if (this.OnGrantCharacterToUserResultEvent != null)
			{
				invocationList = this.OnGrantCharacterToUserResultEvent.GetInvocationList();
				foreach (Delegate obj141 in invocationList)
				{
					if (obj141.Target == instance)
					{
						OnGrantCharacterToUserResultEvent -= (PlayFabResultEvent<GrantCharacterToUserResult>)obj141;
					}
				}
			}
			if (this.OnLinkAndroidDeviceIDRequestEvent != null)
			{
				invocationList = this.OnLinkAndroidDeviceIDRequestEvent.GetInvocationList();
				foreach (Delegate obj142 in invocationList)
				{
					if (obj142.Target == instance)
					{
						OnLinkAndroidDeviceIDRequestEvent -= (PlayFabRequestEvent<LinkAndroidDeviceIDRequest>)obj142;
					}
				}
			}
			if (this.OnLinkAndroidDeviceIDResultEvent != null)
			{
				invocationList = this.OnLinkAndroidDeviceIDResultEvent.GetInvocationList();
				foreach (Delegate obj143 in invocationList)
				{
					if (obj143.Target == instance)
					{
						OnLinkAndroidDeviceIDResultEvent -= (PlayFabResultEvent<LinkAndroidDeviceIDResult>)obj143;
					}
				}
			}
			if (this.OnLinkCustomIDRequestEvent != null)
			{
				invocationList = this.OnLinkCustomIDRequestEvent.GetInvocationList();
				foreach (Delegate obj144 in invocationList)
				{
					if (obj144.Target == instance)
					{
						OnLinkCustomIDRequestEvent -= (PlayFabRequestEvent<LinkCustomIDRequest>)obj144;
					}
				}
			}
			if (this.OnLinkCustomIDResultEvent != null)
			{
				invocationList = this.OnLinkCustomIDResultEvent.GetInvocationList();
				foreach (Delegate obj145 in invocationList)
				{
					if (obj145.Target == instance)
					{
						OnLinkCustomIDResultEvent -= (PlayFabResultEvent<LinkCustomIDResult>)obj145;
					}
				}
			}
			if (this.OnLinkFacebookAccountRequestEvent != null)
			{
				invocationList = this.OnLinkFacebookAccountRequestEvent.GetInvocationList();
				foreach (Delegate obj146 in invocationList)
				{
					if (obj146.Target == instance)
					{
						OnLinkFacebookAccountRequestEvent -= (PlayFabRequestEvent<LinkFacebookAccountRequest>)obj146;
					}
				}
			}
			if (this.OnLinkFacebookAccountResultEvent != null)
			{
				invocationList = this.OnLinkFacebookAccountResultEvent.GetInvocationList();
				foreach (Delegate obj147 in invocationList)
				{
					if (obj147.Target == instance)
					{
						OnLinkFacebookAccountResultEvent -= (PlayFabResultEvent<LinkFacebookAccountResult>)obj147;
					}
				}
			}
			if (this.OnLinkFacebookInstantGamesIdRequestEvent != null)
			{
				invocationList = this.OnLinkFacebookInstantGamesIdRequestEvent.GetInvocationList();
				foreach (Delegate obj148 in invocationList)
				{
					if (obj148.Target == instance)
					{
						OnLinkFacebookInstantGamesIdRequestEvent -= (PlayFabRequestEvent<LinkFacebookInstantGamesIdRequest>)obj148;
					}
				}
			}
			if (this.OnLinkFacebookInstantGamesIdResultEvent != null)
			{
				invocationList = this.OnLinkFacebookInstantGamesIdResultEvent.GetInvocationList();
				foreach (Delegate obj149 in invocationList)
				{
					if (obj149.Target == instance)
					{
						OnLinkFacebookInstantGamesIdResultEvent -= (PlayFabResultEvent<LinkFacebookInstantGamesIdResult>)obj149;
					}
				}
			}
			if (this.OnLinkGameCenterAccountRequestEvent != null)
			{
				invocationList = this.OnLinkGameCenterAccountRequestEvent.GetInvocationList();
				foreach (Delegate obj150 in invocationList)
				{
					if (obj150.Target == instance)
					{
						OnLinkGameCenterAccountRequestEvent -= (PlayFabRequestEvent<LinkGameCenterAccountRequest>)obj150;
					}
				}
			}
			if (this.OnLinkGameCenterAccountResultEvent != null)
			{
				invocationList = this.OnLinkGameCenterAccountResultEvent.GetInvocationList();
				foreach (Delegate obj151 in invocationList)
				{
					if (obj151.Target == instance)
					{
						OnLinkGameCenterAccountResultEvent -= (PlayFabResultEvent<LinkGameCenterAccountResult>)obj151;
					}
				}
			}
			if (this.OnLinkGoogleAccountRequestEvent != null)
			{
				invocationList = this.OnLinkGoogleAccountRequestEvent.GetInvocationList();
				foreach (Delegate obj152 in invocationList)
				{
					if (obj152.Target == instance)
					{
						OnLinkGoogleAccountRequestEvent -= (PlayFabRequestEvent<LinkGoogleAccountRequest>)obj152;
					}
				}
			}
			if (this.OnLinkGoogleAccountResultEvent != null)
			{
				invocationList = this.OnLinkGoogleAccountResultEvent.GetInvocationList();
				foreach (Delegate obj153 in invocationList)
				{
					if (obj153.Target == instance)
					{
						OnLinkGoogleAccountResultEvent -= (PlayFabResultEvent<LinkGoogleAccountResult>)obj153;
					}
				}
			}
			if (this.OnLinkIOSDeviceIDRequestEvent != null)
			{
				invocationList = this.OnLinkIOSDeviceIDRequestEvent.GetInvocationList();
				foreach (Delegate obj154 in invocationList)
				{
					if (obj154.Target == instance)
					{
						OnLinkIOSDeviceIDRequestEvent -= (PlayFabRequestEvent<LinkIOSDeviceIDRequest>)obj154;
					}
				}
			}
			if (this.OnLinkIOSDeviceIDResultEvent != null)
			{
				invocationList = this.OnLinkIOSDeviceIDResultEvent.GetInvocationList();
				foreach (Delegate obj155 in invocationList)
				{
					if (obj155.Target == instance)
					{
						OnLinkIOSDeviceIDResultEvent -= (PlayFabResultEvent<LinkIOSDeviceIDResult>)obj155;
					}
				}
			}
			if (this.OnLinkKongregateRequestEvent != null)
			{
				invocationList = this.OnLinkKongregateRequestEvent.GetInvocationList();
				foreach (Delegate obj156 in invocationList)
				{
					if (obj156.Target == instance)
					{
						OnLinkKongregateRequestEvent -= (PlayFabRequestEvent<LinkKongregateAccountRequest>)obj156;
					}
				}
			}
			if (this.OnLinkKongregateResultEvent != null)
			{
				invocationList = this.OnLinkKongregateResultEvent.GetInvocationList();
				foreach (Delegate obj157 in invocationList)
				{
					if (obj157.Target == instance)
					{
						OnLinkKongregateResultEvent -= (PlayFabResultEvent<LinkKongregateAccountResult>)obj157;
					}
				}
			}
			if (this.OnLinkNintendoSwitchDeviceIdRequestEvent != null)
			{
				invocationList = this.OnLinkNintendoSwitchDeviceIdRequestEvent.GetInvocationList();
				foreach (Delegate obj158 in invocationList)
				{
					if (obj158.Target == instance)
					{
						OnLinkNintendoSwitchDeviceIdRequestEvent -= (PlayFabRequestEvent<LinkNintendoSwitchDeviceIdRequest>)obj158;
					}
				}
			}
			if (this.OnLinkNintendoSwitchDeviceIdResultEvent != null)
			{
				invocationList = this.OnLinkNintendoSwitchDeviceIdResultEvent.GetInvocationList();
				foreach (Delegate obj159 in invocationList)
				{
					if (obj159.Target == instance)
					{
						OnLinkNintendoSwitchDeviceIdResultEvent -= (PlayFabResultEvent<LinkNintendoSwitchDeviceIdResult>)obj159;
					}
				}
			}
			if (this.OnLinkOpenIdConnectRequestEvent != null)
			{
				invocationList = this.OnLinkOpenIdConnectRequestEvent.GetInvocationList();
				foreach (Delegate obj160 in invocationList)
				{
					if (obj160.Target == instance)
					{
						OnLinkOpenIdConnectRequestEvent -= (PlayFabRequestEvent<LinkOpenIdConnectRequest>)obj160;
					}
				}
			}
			if (this.OnLinkOpenIdConnectResultEvent != null)
			{
				invocationList = this.OnLinkOpenIdConnectResultEvent.GetInvocationList();
				foreach (Delegate obj161 in invocationList)
				{
					if (obj161.Target == instance)
					{
						OnLinkOpenIdConnectResultEvent -= (PlayFabResultEvent<EmptyResult>)obj161;
					}
				}
			}
			if (this.OnLinkPSNAccountRequestEvent != null)
			{
				invocationList = this.OnLinkPSNAccountRequestEvent.GetInvocationList();
				foreach (Delegate obj162 in invocationList)
				{
					if (obj162.Target == instance)
					{
						OnLinkPSNAccountRequestEvent -= (PlayFabRequestEvent<LinkPSNAccountRequest>)obj162;
					}
				}
			}
			if (this.OnLinkPSNAccountResultEvent != null)
			{
				invocationList = this.OnLinkPSNAccountResultEvent.GetInvocationList();
				foreach (Delegate obj163 in invocationList)
				{
					if (obj163.Target == instance)
					{
						OnLinkPSNAccountResultEvent -= (PlayFabResultEvent<LinkPSNAccountResult>)obj163;
					}
				}
			}
			if (this.OnLinkSteamAccountRequestEvent != null)
			{
				invocationList = this.OnLinkSteamAccountRequestEvent.GetInvocationList();
				foreach (Delegate obj164 in invocationList)
				{
					if (obj164.Target == instance)
					{
						OnLinkSteamAccountRequestEvent -= (PlayFabRequestEvent<LinkSteamAccountRequest>)obj164;
					}
				}
			}
			if (this.OnLinkSteamAccountResultEvent != null)
			{
				invocationList = this.OnLinkSteamAccountResultEvent.GetInvocationList();
				foreach (Delegate obj165 in invocationList)
				{
					if (obj165.Target == instance)
					{
						OnLinkSteamAccountResultEvent -= (PlayFabResultEvent<LinkSteamAccountResult>)obj165;
					}
				}
			}
			if (this.OnLinkTwitchRequestEvent != null)
			{
				invocationList = this.OnLinkTwitchRequestEvent.GetInvocationList();
				foreach (Delegate obj166 in invocationList)
				{
					if (obj166.Target == instance)
					{
						OnLinkTwitchRequestEvent -= (PlayFabRequestEvent<LinkTwitchAccountRequest>)obj166;
					}
				}
			}
			if (this.OnLinkTwitchResultEvent != null)
			{
				invocationList = this.OnLinkTwitchResultEvent.GetInvocationList();
				foreach (Delegate obj167 in invocationList)
				{
					if (obj167.Target == instance)
					{
						OnLinkTwitchResultEvent -= (PlayFabResultEvent<LinkTwitchAccountResult>)obj167;
					}
				}
			}
			if (this.OnLinkWindowsHelloRequestEvent != null)
			{
				invocationList = this.OnLinkWindowsHelloRequestEvent.GetInvocationList();
				foreach (Delegate obj168 in invocationList)
				{
					if (obj168.Target == instance)
					{
						OnLinkWindowsHelloRequestEvent -= (PlayFabRequestEvent<LinkWindowsHelloAccountRequest>)obj168;
					}
				}
			}
			if (this.OnLinkWindowsHelloResultEvent != null)
			{
				invocationList = this.OnLinkWindowsHelloResultEvent.GetInvocationList();
				foreach (Delegate obj169 in invocationList)
				{
					if (obj169.Target == instance)
					{
						OnLinkWindowsHelloResultEvent -= (PlayFabResultEvent<LinkWindowsHelloAccountResponse>)obj169;
					}
				}
			}
			if (this.OnLinkXboxAccountRequestEvent != null)
			{
				invocationList = this.OnLinkXboxAccountRequestEvent.GetInvocationList();
				foreach (Delegate obj170 in invocationList)
				{
					if (obj170.Target == instance)
					{
						OnLinkXboxAccountRequestEvent -= (PlayFabRequestEvent<LinkXboxAccountRequest>)obj170;
					}
				}
			}
			if (this.OnLinkXboxAccountResultEvent != null)
			{
				invocationList = this.OnLinkXboxAccountResultEvent.GetInvocationList();
				foreach (Delegate obj171 in invocationList)
				{
					if (obj171.Target == instance)
					{
						OnLinkXboxAccountResultEvent -= (PlayFabResultEvent<LinkXboxAccountResult>)obj171;
					}
				}
			}
			if (this.OnLoginWithAndroidDeviceIDRequestEvent != null)
			{
				invocationList = this.OnLoginWithAndroidDeviceIDRequestEvent.GetInvocationList();
				foreach (Delegate obj172 in invocationList)
				{
					if (obj172.Target == instance)
					{
						OnLoginWithAndroidDeviceIDRequestEvent -= (PlayFabRequestEvent<LoginWithAndroidDeviceIDRequest>)obj172;
					}
				}
			}
			if (this.OnLoginWithCustomIDRequestEvent != null)
			{
				invocationList = this.OnLoginWithCustomIDRequestEvent.GetInvocationList();
				foreach (Delegate obj173 in invocationList)
				{
					if (obj173.Target == instance)
					{
						OnLoginWithCustomIDRequestEvent -= (PlayFabRequestEvent<LoginWithCustomIDRequest>)obj173;
					}
				}
			}
			if (this.OnLoginWithEmailAddressRequestEvent != null)
			{
				invocationList = this.OnLoginWithEmailAddressRequestEvent.GetInvocationList();
				foreach (Delegate obj174 in invocationList)
				{
					if (obj174.Target == instance)
					{
						OnLoginWithEmailAddressRequestEvent -= (PlayFabRequestEvent<LoginWithEmailAddressRequest>)obj174;
					}
				}
			}
			if (this.OnLoginWithFacebookRequestEvent != null)
			{
				invocationList = this.OnLoginWithFacebookRequestEvent.GetInvocationList();
				foreach (Delegate obj175 in invocationList)
				{
					if (obj175.Target == instance)
					{
						OnLoginWithFacebookRequestEvent -= (PlayFabRequestEvent<LoginWithFacebookRequest>)obj175;
					}
				}
			}
			if (this.OnLoginWithFacebookInstantGamesIdRequestEvent != null)
			{
				invocationList = this.OnLoginWithFacebookInstantGamesIdRequestEvent.GetInvocationList();
				foreach (Delegate obj176 in invocationList)
				{
					if (obj176.Target == instance)
					{
						OnLoginWithFacebookInstantGamesIdRequestEvent -= (PlayFabRequestEvent<LoginWithFacebookInstantGamesIdRequest>)obj176;
					}
				}
			}
			if (this.OnLoginWithGameCenterRequestEvent != null)
			{
				invocationList = this.OnLoginWithGameCenterRequestEvent.GetInvocationList();
				foreach (Delegate obj177 in invocationList)
				{
					if (obj177.Target == instance)
					{
						OnLoginWithGameCenterRequestEvent -= (PlayFabRequestEvent<LoginWithGameCenterRequest>)obj177;
					}
				}
			}
			if (this.OnLoginWithGoogleAccountRequestEvent != null)
			{
				invocationList = this.OnLoginWithGoogleAccountRequestEvent.GetInvocationList();
				foreach (Delegate obj178 in invocationList)
				{
					if (obj178.Target == instance)
					{
						OnLoginWithGoogleAccountRequestEvent -= (PlayFabRequestEvent<LoginWithGoogleAccountRequest>)obj178;
					}
				}
			}
			if (this.OnLoginWithIOSDeviceIDRequestEvent != null)
			{
				invocationList = this.OnLoginWithIOSDeviceIDRequestEvent.GetInvocationList();
				foreach (Delegate obj179 in invocationList)
				{
					if (obj179.Target == instance)
					{
						OnLoginWithIOSDeviceIDRequestEvent -= (PlayFabRequestEvent<LoginWithIOSDeviceIDRequest>)obj179;
					}
				}
			}
			if (this.OnLoginWithKongregateRequestEvent != null)
			{
				invocationList = this.OnLoginWithKongregateRequestEvent.GetInvocationList();
				foreach (Delegate obj180 in invocationList)
				{
					if (obj180.Target == instance)
					{
						OnLoginWithKongregateRequestEvent -= (PlayFabRequestEvent<LoginWithKongregateRequest>)obj180;
					}
				}
			}
			if (this.OnLoginWithNintendoSwitchDeviceIdRequestEvent != null)
			{
				invocationList = this.OnLoginWithNintendoSwitchDeviceIdRequestEvent.GetInvocationList();
				foreach (Delegate obj181 in invocationList)
				{
					if (obj181.Target == instance)
					{
						OnLoginWithNintendoSwitchDeviceIdRequestEvent -= (PlayFabRequestEvent<LoginWithNintendoSwitchDeviceIdRequest>)obj181;
					}
				}
			}
			if (this.OnLoginWithOpenIdConnectRequestEvent != null)
			{
				invocationList = this.OnLoginWithOpenIdConnectRequestEvent.GetInvocationList();
				foreach (Delegate obj182 in invocationList)
				{
					if (obj182.Target == instance)
					{
						OnLoginWithOpenIdConnectRequestEvent -= (PlayFabRequestEvent<LoginWithOpenIdConnectRequest>)obj182;
					}
				}
			}
			if (this.OnLoginWithPlayFabRequestEvent != null)
			{
				invocationList = this.OnLoginWithPlayFabRequestEvent.GetInvocationList();
				foreach (Delegate obj183 in invocationList)
				{
					if (obj183.Target == instance)
					{
						OnLoginWithPlayFabRequestEvent -= (PlayFabRequestEvent<LoginWithPlayFabRequest>)obj183;
					}
				}
			}
			if (this.OnLoginWithPSNRequestEvent != null)
			{
				invocationList = this.OnLoginWithPSNRequestEvent.GetInvocationList();
				foreach (Delegate obj184 in invocationList)
				{
					if (obj184.Target == instance)
					{
						OnLoginWithPSNRequestEvent -= (PlayFabRequestEvent<LoginWithPSNRequest>)obj184;
					}
				}
			}
			if (this.OnLoginWithSteamRequestEvent != null)
			{
				invocationList = this.OnLoginWithSteamRequestEvent.GetInvocationList();
				foreach (Delegate obj185 in invocationList)
				{
					if (obj185.Target == instance)
					{
						OnLoginWithSteamRequestEvent -= (PlayFabRequestEvent<LoginWithSteamRequest>)obj185;
					}
				}
			}
			if (this.OnLoginWithTwitchRequestEvent != null)
			{
				invocationList = this.OnLoginWithTwitchRequestEvent.GetInvocationList();
				foreach (Delegate obj186 in invocationList)
				{
					if (obj186.Target == instance)
					{
						OnLoginWithTwitchRequestEvent -= (PlayFabRequestEvent<LoginWithTwitchRequest>)obj186;
					}
				}
			}
			if (this.OnLoginWithWindowsHelloRequestEvent != null)
			{
				invocationList = this.OnLoginWithWindowsHelloRequestEvent.GetInvocationList();
				foreach (Delegate obj187 in invocationList)
				{
					if (obj187.Target == instance)
					{
						OnLoginWithWindowsHelloRequestEvent -= (PlayFabRequestEvent<LoginWithWindowsHelloRequest>)obj187;
					}
				}
			}
			if (this.OnLoginWithXboxRequestEvent != null)
			{
				invocationList = this.OnLoginWithXboxRequestEvent.GetInvocationList();
				foreach (Delegate obj188 in invocationList)
				{
					if (obj188.Target == instance)
					{
						OnLoginWithXboxRequestEvent -= (PlayFabRequestEvent<LoginWithXboxRequest>)obj188;
					}
				}
			}
			if (this.OnMatchmakeRequestEvent != null)
			{
				invocationList = this.OnMatchmakeRequestEvent.GetInvocationList();
				foreach (Delegate obj189 in invocationList)
				{
					if (obj189.Target == instance)
					{
						OnMatchmakeRequestEvent -= (PlayFabRequestEvent<MatchmakeRequest>)obj189;
					}
				}
			}
			if (this.OnMatchmakeResultEvent != null)
			{
				invocationList = this.OnMatchmakeResultEvent.GetInvocationList();
				foreach (Delegate obj190 in invocationList)
				{
					if (obj190.Target == instance)
					{
						OnMatchmakeResultEvent -= (PlayFabResultEvent<MatchmakeResult>)obj190;
					}
				}
			}
			if (this.OnOpenTradeRequestEvent != null)
			{
				invocationList = this.OnOpenTradeRequestEvent.GetInvocationList();
				foreach (Delegate obj191 in invocationList)
				{
					if (obj191.Target == instance)
					{
						OnOpenTradeRequestEvent -= (PlayFabRequestEvent<OpenTradeRequest>)obj191;
					}
				}
			}
			if (this.OnOpenTradeResultEvent != null)
			{
				invocationList = this.OnOpenTradeResultEvent.GetInvocationList();
				foreach (Delegate obj192 in invocationList)
				{
					if (obj192.Target == instance)
					{
						OnOpenTradeResultEvent -= (PlayFabResultEvent<OpenTradeResponse>)obj192;
					}
				}
			}
			if (this.OnPayForPurchaseRequestEvent != null)
			{
				invocationList = this.OnPayForPurchaseRequestEvent.GetInvocationList();
				foreach (Delegate obj193 in invocationList)
				{
					if (obj193.Target == instance)
					{
						OnPayForPurchaseRequestEvent -= (PlayFabRequestEvent<PayForPurchaseRequest>)obj193;
					}
				}
			}
			if (this.OnPayForPurchaseResultEvent != null)
			{
				invocationList = this.OnPayForPurchaseResultEvent.GetInvocationList();
				foreach (Delegate obj194 in invocationList)
				{
					if (obj194.Target == instance)
					{
						OnPayForPurchaseResultEvent -= (PlayFabResultEvent<PayForPurchaseResult>)obj194;
					}
				}
			}
			if (this.OnPurchaseItemRequestEvent != null)
			{
				invocationList = this.OnPurchaseItemRequestEvent.GetInvocationList();
				foreach (Delegate obj195 in invocationList)
				{
					if (obj195.Target == instance)
					{
						OnPurchaseItemRequestEvent -= (PlayFabRequestEvent<PurchaseItemRequest>)obj195;
					}
				}
			}
			if (this.OnPurchaseItemResultEvent != null)
			{
				invocationList = this.OnPurchaseItemResultEvent.GetInvocationList();
				foreach (Delegate obj196 in invocationList)
				{
					if (obj196.Target == instance)
					{
						OnPurchaseItemResultEvent -= (PlayFabResultEvent<PurchaseItemResult>)obj196;
					}
				}
			}
			if (this.OnRedeemCouponRequestEvent != null)
			{
				invocationList = this.OnRedeemCouponRequestEvent.GetInvocationList();
				foreach (Delegate obj197 in invocationList)
				{
					if (obj197.Target == instance)
					{
						OnRedeemCouponRequestEvent -= (PlayFabRequestEvent<RedeemCouponRequest>)obj197;
					}
				}
			}
			if (this.OnRedeemCouponResultEvent != null)
			{
				invocationList = this.OnRedeemCouponResultEvent.GetInvocationList();
				foreach (Delegate obj198 in invocationList)
				{
					if (obj198.Target == instance)
					{
						OnRedeemCouponResultEvent -= (PlayFabResultEvent<RedeemCouponResult>)obj198;
					}
				}
			}
			if (this.OnRefreshPSNAuthTokenRequestEvent != null)
			{
				invocationList = this.OnRefreshPSNAuthTokenRequestEvent.GetInvocationList();
				foreach (Delegate obj199 in invocationList)
				{
					if (obj199.Target == instance)
					{
						OnRefreshPSNAuthTokenRequestEvent -= (PlayFabRequestEvent<RefreshPSNAuthTokenRequest>)obj199;
					}
				}
			}
			if (this.OnRefreshPSNAuthTokenResultEvent != null)
			{
				invocationList = this.OnRefreshPSNAuthTokenResultEvent.GetInvocationList();
				foreach (Delegate obj200 in invocationList)
				{
					if (obj200.Target == instance)
					{
						OnRefreshPSNAuthTokenResultEvent -= (PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse>)obj200;
					}
				}
			}
			if (this.OnRegisterForIOSPushNotificationRequestEvent != null)
			{
				invocationList = this.OnRegisterForIOSPushNotificationRequestEvent.GetInvocationList();
				foreach (Delegate obj201 in invocationList)
				{
					if (obj201.Target == instance)
					{
						OnRegisterForIOSPushNotificationRequestEvent -= (PlayFabRequestEvent<RegisterForIOSPushNotificationRequest>)obj201;
					}
				}
			}
			if (this.OnRegisterForIOSPushNotificationResultEvent != null)
			{
				invocationList = this.OnRegisterForIOSPushNotificationResultEvent.GetInvocationList();
				foreach (Delegate obj202 in invocationList)
				{
					if (obj202.Target == instance)
					{
						OnRegisterForIOSPushNotificationResultEvent -= (PlayFabResultEvent<RegisterForIOSPushNotificationResult>)obj202;
					}
				}
			}
			if (this.OnRegisterPlayFabUserRequestEvent != null)
			{
				invocationList = this.OnRegisterPlayFabUserRequestEvent.GetInvocationList();
				foreach (Delegate obj203 in invocationList)
				{
					if (obj203.Target == instance)
					{
						OnRegisterPlayFabUserRequestEvent -= (PlayFabRequestEvent<RegisterPlayFabUserRequest>)obj203;
					}
				}
			}
			if (this.OnRegisterPlayFabUserResultEvent != null)
			{
				invocationList = this.OnRegisterPlayFabUserResultEvent.GetInvocationList();
				foreach (Delegate obj204 in invocationList)
				{
					if (obj204.Target == instance)
					{
						OnRegisterPlayFabUserResultEvent -= (PlayFabResultEvent<RegisterPlayFabUserResult>)obj204;
					}
				}
			}
			if (this.OnRegisterWithWindowsHelloRequestEvent != null)
			{
				invocationList = this.OnRegisterWithWindowsHelloRequestEvent.GetInvocationList();
				foreach (Delegate obj205 in invocationList)
				{
					if (obj205.Target == instance)
					{
						OnRegisterWithWindowsHelloRequestEvent -= (PlayFabRequestEvent<RegisterWithWindowsHelloRequest>)obj205;
					}
				}
			}
			if (this.OnRemoveContactEmailRequestEvent != null)
			{
				invocationList = this.OnRemoveContactEmailRequestEvent.GetInvocationList();
				foreach (Delegate obj206 in invocationList)
				{
					if (obj206.Target == instance)
					{
						OnRemoveContactEmailRequestEvent -= (PlayFabRequestEvent<RemoveContactEmailRequest>)obj206;
					}
				}
			}
			if (this.OnRemoveContactEmailResultEvent != null)
			{
				invocationList = this.OnRemoveContactEmailResultEvent.GetInvocationList();
				foreach (Delegate obj207 in invocationList)
				{
					if (obj207.Target == instance)
					{
						OnRemoveContactEmailResultEvent -= (PlayFabResultEvent<RemoveContactEmailResult>)obj207;
					}
				}
			}
			if (this.OnRemoveFriendRequestEvent != null)
			{
				invocationList = this.OnRemoveFriendRequestEvent.GetInvocationList();
				foreach (Delegate obj208 in invocationList)
				{
					if (obj208.Target == instance)
					{
						OnRemoveFriendRequestEvent -= (PlayFabRequestEvent<RemoveFriendRequest>)obj208;
					}
				}
			}
			if (this.OnRemoveFriendResultEvent != null)
			{
				invocationList = this.OnRemoveFriendResultEvent.GetInvocationList();
				foreach (Delegate obj209 in invocationList)
				{
					if (obj209.Target == instance)
					{
						OnRemoveFriendResultEvent -= (PlayFabResultEvent<RemoveFriendResult>)obj209;
					}
				}
			}
			if (this.OnRemoveGenericIDRequestEvent != null)
			{
				invocationList = this.OnRemoveGenericIDRequestEvent.GetInvocationList();
				foreach (Delegate obj210 in invocationList)
				{
					if (obj210.Target == instance)
					{
						OnRemoveGenericIDRequestEvent -= (PlayFabRequestEvent<RemoveGenericIDRequest>)obj210;
					}
				}
			}
			if (this.OnRemoveGenericIDResultEvent != null)
			{
				invocationList = this.OnRemoveGenericIDResultEvent.GetInvocationList();
				foreach (Delegate obj211 in invocationList)
				{
					if (obj211.Target == instance)
					{
						OnRemoveGenericIDResultEvent -= (PlayFabResultEvent<RemoveGenericIDResult>)obj211;
					}
				}
			}
			if (this.OnRemoveSharedGroupMembersRequestEvent != null)
			{
				invocationList = this.OnRemoveSharedGroupMembersRequestEvent.GetInvocationList();
				foreach (Delegate obj212 in invocationList)
				{
					if (obj212.Target == instance)
					{
						OnRemoveSharedGroupMembersRequestEvent -= (PlayFabRequestEvent<RemoveSharedGroupMembersRequest>)obj212;
					}
				}
			}
			if (this.OnRemoveSharedGroupMembersResultEvent != null)
			{
				invocationList = this.OnRemoveSharedGroupMembersResultEvent.GetInvocationList();
				foreach (Delegate obj213 in invocationList)
				{
					if (obj213.Target == instance)
					{
						OnRemoveSharedGroupMembersResultEvent -= (PlayFabResultEvent<RemoveSharedGroupMembersResult>)obj213;
					}
				}
			}
			if (this.OnReportDeviceInfoRequestEvent != null)
			{
				invocationList = this.OnReportDeviceInfoRequestEvent.GetInvocationList();
				foreach (Delegate obj214 in invocationList)
				{
					if (obj214.Target == instance)
					{
						OnReportDeviceInfoRequestEvent -= (PlayFabRequestEvent<DeviceInfoRequest>)obj214;
					}
				}
			}
			if (this.OnReportDeviceInfoResultEvent != null)
			{
				invocationList = this.OnReportDeviceInfoResultEvent.GetInvocationList();
				foreach (Delegate obj215 in invocationList)
				{
					if (obj215.Target == instance)
					{
						OnReportDeviceInfoResultEvent -= (PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse>)obj215;
					}
				}
			}
			if (this.OnReportPlayerRequestEvent != null)
			{
				invocationList = this.OnReportPlayerRequestEvent.GetInvocationList();
				foreach (Delegate obj216 in invocationList)
				{
					if (obj216.Target == instance)
					{
						OnReportPlayerRequestEvent -= (PlayFabRequestEvent<ReportPlayerClientRequest>)obj216;
					}
				}
			}
			if (this.OnReportPlayerResultEvent != null)
			{
				invocationList = this.OnReportPlayerResultEvent.GetInvocationList();
				foreach (Delegate obj217 in invocationList)
				{
					if (obj217.Target == instance)
					{
						OnReportPlayerResultEvent -= (PlayFabResultEvent<ReportPlayerClientResult>)obj217;
					}
				}
			}
			if (this.OnRestoreIOSPurchasesRequestEvent != null)
			{
				invocationList = this.OnRestoreIOSPurchasesRequestEvent.GetInvocationList();
				foreach (Delegate obj218 in invocationList)
				{
					if (obj218.Target == instance)
					{
						OnRestoreIOSPurchasesRequestEvent -= (PlayFabRequestEvent<RestoreIOSPurchasesRequest>)obj218;
					}
				}
			}
			if (this.OnRestoreIOSPurchasesResultEvent != null)
			{
				invocationList = this.OnRestoreIOSPurchasesResultEvent.GetInvocationList();
				foreach (Delegate obj219 in invocationList)
				{
					if (obj219.Target == instance)
					{
						OnRestoreIOSPurchasesResultEvent -= (PlayFabResultEvent<RestoreIOSPurchasesResult>)obj219;
					}
				}
			}
			if (this.OnSendAccountRecoveryEmailRequestEvent != null)
			{
				invocationList = this.OnSendAccountRecoveryEmailRequestEvent.GetInvocationList();
				foreach (Delegate obj220 in invocationList)
				{
					if (obj220.Target == instance)
					{
						OnSendAccountRecoveryEmailRequestEvent -= (PlayFabRequestEvent<SendAccountRecoveryEmailRequest>)obj220;
					}
				}
			}
			if (this.OnSendAccountRecoveryEmailResultEvent != null)
			{
				invocationList = this.OnSendAccountRecoveryEmailResultEvent.GetInvocationList();
				foreach (Delegate obj221 in invocationList)
				{
					if (obj221.Target == instance)
					{
						OnSendAccountRecoveryEmailResultEvent -= (PlayFabResultEvent<SendAccountRecoveryEmailResult>)obj221;
					}
				}
			}
			if (this.OnSetFriendTagsRequestEvent != null)
			{
				invocationList = this.OnSetFriendTagsRequestEvent.GetInvocationList();
				foreach (Delegate obj222 in invocationList)
				{
					if (obj222.Target == instance)
					{
						OnSetFriendTagsRequestEvent -= (PlayFabRequestEvent<SetFriendTagsRequest>)obj222;
					}
				}
			}
			if (this.OnSetFriendTagsResultEvent != null)
			{
				invocationList = this.OnSetFriendTagsResultEvent.GetInvocationList();
				foreach (Delegate obj223 in invocationList)
				{
					if (obj223.Target == instance)
					{
						OnSetFriendTagsResultEvent -= (PlayFabResultEvent<SetFriendTagsResult>)obj223;
					}
				}
			}
			if (this.OnSetPlayerSecretRequestEvent != null)
			{
				invocationList = this.OnSetPlayerSecretRequestEvent.GetInvocationList();
				foreach (Delegate obj224 in invocationList)
				{
					if (obj224.Target == instance)
					{
						OnSetPlayerSecretRequestEvent -= (PlayFabRequestEvent<SetPlayerSecretRequest>)obj224;
					}
				}
			}
			if (this.OnSetPlayerSecretResultEvent != null)
			{
				invocationList = this.OnSetPlayerSecretResultEvent.GetInvocationList();
				foreach (Delegate obj225 in invocationList)
				{
					if (obj225.Target == instance)
					{
						OnSetPlayerSecretResultEvent -= (PlayFabResultEvent<SetPlayerSecretResult>)obj225;
					}
				}
			}
			if (this.OnStartGameRequestEvent != null)
			{
				invocationList = this.OnStartGameRequestEvent.GetInvocationList();
				foreach (Delegate obj226 in invocationList)
				{
					if (obj226.Target == instance)
					{
						OnStartGameRequestEvent -= (PlayFabRequestEvent<StartGameRequest>)obj226;
					}
				}
			}
			if (this.OnStartGameResultEvent != null)
			{
				invocationList = this.OnStartGameResultEvent.GetInvocationList();
				foreach (Delegate obj227 in invocationList)
				{
					if (obj227.Target == instance)
					{
						OnStartGameResultEvent -= (PlayFabResultEvent<StartGameResult>)obj227;
					}
				}
			}
			if (this.OnStartPurchaseRequestEvent != null)
			{
				invocationList = this.OnStartPurchaseRequestEvent.GetInvocationList();
				foreach (Delegate obj228 in invocationList)
				{
					if (obj228.Target == instance)
					{
						OnStartPurchaseRequestEvent -= (PlayFabRequestEvent<StartPurchaseRequest>)obj228;
					}
				}
			}
			if (this.OnStartPurchaseResultEvent != null)
			{
				invocationList = this.OnStartPurchaseResultEvent.GetInvocationList();
				foreach (Delegate obj229 in invocationList)
				{
					if (obj229.Target == instance)
					{
						OnStartPurchaseResultEvent -= (PlayFabResultEvent<StartPurchaseResult>)obj229;
					}
				}
			}
			if (this.OnSubtractUserVirtualCurrencyRequestEvent != null)
			{
				invocationList = this.OnSubtractUserVirtualCurrencyRequestEvent.GetInvocationList();
				foreach (Delegate obj230 in invocationList)
				{
					if (obj230.Target == instance)
					{
						OnSubtractUserVirtualCurrencyRequestEvent -= (PlayFabRequestEvent<SubtractUserVirtualCurrencyRequest>)obj230;
					}
				}
			}
			if (this.OnSubtractUserVirtualCurrencyResultEvent != null)
			{
				invocationList = this.OnSubtractUserVirtualCurrencyResultEvent.GetInvocationList();
				foreach (Delegate obj231 in invocationList)
				{
					if (obj231.Target == instance)
					{
						OnSubtractUserVirtualCurrencyResultEvent -= (PlayFabResultEvent<ModifyUserVirtualCurrencyResult>)obj231;
					}
				}
			}
			if (this.OnUnlinkAndroidDeviceIDRequestEvent != null)
			{
				invocationList = this.OnUnlinkAndroidDeviceIDRequestEvent.GetInvocationList();
				foreach (Delegate obj232 in invocationList)
				{
					if (obj232.Target == instance)
					{
						OnUnlinkAndroidDeviceIDRequestEvent -= (PlayFabRequestEvent<UnlinkAndroidDeviceIDRequest>)obj232;
					}
				}
			}
			if (this.OnUnlinkAndroidDeviceIDResultEvent != null)
			{
				invocationList = this.OnUnlinkAndroidDeviceIDResultEvent.GetInvocationList();
				foreach (Delegate obj233 in invocationList)
				{
					if (obj233.Target == instance)
					{
						OnUnlinkAndroidDeviceIDResultEvent -= (PlayFabResultEvent<UnlinkAndroidDeviceIDResult>)obj233;
					}
				}
			}
			if (this.OnUnlinkCustomIDRequestEvent != null)
			{
				invocationList = this.OnUnlinkCustomIDRequestEvent.GetInvocationList();
				foreach (Delegate obj234 in invocationList)
				{
					if (obj234.Target == instance)
					{
						OnUnlinkCustomIDRequestEvent -= (PlayFabRequestEvent<UnlinkCustomIDRequest>)obj234;
					}
				}
			}
			if (this.OnUnlinkCustomIDResultEvent != null)
			{
				invocationList = this.OnUnlinkCustomIDResultEvent.GetInvocationList();
				foreach (Delegate obj235 in invocationList)
				{
					if (obj235.Target == instance)
					{
						OnUnlinkCustomIDResultEvent -= (PlayFabResultEvent<UnlinkCustomIDResult>)obj235;
					}
				}
			}
			if (this.OnUnlinkFacebookAccountRequestEvent != null)
			{
				invocationList = this.OnUnlinkFacebookAccountRequestEvent.GetInvocationList();
				foreach (Delegate obj236 in invocationList)
				{
					if (obj236.Target == instance)
					{
						OnUnlinkFacebookAccountRequestEvent -= (PlayFabRequestEvent<UnlinkFacebookAccountRequest>)obj236;
					}
				}
			}
			if (this.OnUnlinkFacebookAccountResultEvent != null)
			{
				invocationList = this.OnUnlinkFacebookAccountResultEvent.GetInvocationList();
				foreach (Delegate obj237 in invocationList)
				{
					if (obj237.Target == instance)
					{
						OnUnlinkFacebookAccountResultEvent -= (PlayFabResultEvent<UnlinkFacebookAccountResult>)obj237;
					}
				}
			}
			if (this.OnUnlinkFacebookInstantGamesIdRequestEvent != null)
			{
				invocationList = this.OnUnlinkFacebookInstantGamesIdRequestEvent.GetInvocationList();
				foreach (Delegate obj238 in invocationList)
				{
					if (obj238.Target == instance)
					{
						OnUnlinkFacebookInstantGamesIdRequestEvent -= (PlayFabRequestEvent<UnlinkFacebookInstantGamesIdRequest>)obj238;
					}
				}
			}
			if (this.OnUnlinkFacebookInstantGamesIdResultEvent != null)
			{
				invocationList = this.OnUnlinkFacebookInstantGamesIdResultEvent.GetInvocationList();
				foreach (Delegate obj239 in invocationList)
				{
					if (obj239.Target == instance)
					{
						OnUnlinkFacebookInstantGamesIdResultEvent -= (PlayFabResultEvent<UnlinkFacebookInstantGamesIdResult>)obj239;
					}
				}
			}
			if (this.OnUnlinkGameCenterAccountRequestEvent != null)
			{
				invocationList = this.OnUnlinkGameCenterAccountRequestEvent.GetInvocationList();
				foreach (Delegate obj240 in invocationList)
				{
					if (obj240.Target == instance)
					{
						OnUnlinkGameCenterAccountRequestEvent -= (PlayFabRequestEvent<UnlinkGameCenterAccountRequest>)obj240;
					}
				}
			}
			if (this.OnUnlinkGameCenterAccountResultEvent != null)
			{
				invocationList = this.OnUnlinkGameCenterAccountResultEvent.GetInvocationList();
				foreach (Delegate obj241 in invocationList)
				{
					if (obj241.Target == instance)
					{
						OnUnlinkGameCenterAccountResultEvent -= (PlayFabResultEvent<UnlinkGameCenterAccountResult>)obj241;
					}
				}
			}
			if (this.OnUnlinkGoogleAccountRequestEvent != null)
			{
				invocationList = this.OnUnlinkGoogleAccountRequestEvent.GetInvocationList();
				foreach (Delegate obj242 in invocationList)
				{
					if (obj242.Target == instance)
					{
						OnUnlinkGoogleAccountRequestEvent -= (PlayFabRequestEvent<UnlinkGoogleAccountRequest>)obj242;
					}
				}
			}
			if (this.OnUnlinkGoogleAccountResultEvent != null)
			{
				invocationList = this.OnUnlinkGoogleAccountResultEvent.GetInvocationList();
				foreach (Delegate obj243 in invocationList)
				{
					if (obj243.Target == instance)
					{
						OnUnlinkGoogleAccountResultEvent -= (PlayFabResultEvent<UnlinkGoogleAccountResult>)obj243;
					}
				}
			}
			if (this.OnUnlinkIOSDeviceIDRequestEvent != null)
			{
				invocationList = this.OnUnlinkIOSDeviceIDRequestEvent.GetInvocationList();
				foreach (Delegate obj244 in invocationList)
				{
					if (obj244.Target == instance)
					{
						OnUnlinkIOSDeviceIDRequestEvent -= (PlayFabRequestEvent<UnlinkIOSDeviceIDRequest>)obj244;
					}
				}
			}
			if (this.OnUnlinkIOSDeviceIDResultEvent != null)
			{
				invocationList = this.OnUnlinkIOSDeviceIDResultEvent.GetInvocationList();
				foreach (Delegate obj245 in invocationList)
				{
					if (obj245.Target == instance)
					{
						OnUnlinkIOSDeviceIDResultEvent -= (PlayFabResultEvent<UnlinkIOSDeviceIDResult>)obj245;
					}
				}
			}
			if (this.OnUnlinkKongregateRequestEvent != null)
			{
				invocationList = this.OnUnlinkKongregateRequestEvent.GetInvocationList();
				foreach (Delegate obj246 in invocationList)
				{
					if (obj246.Target == instance)
					{
						OnUnlinkKongregateRequestEvent -= (PlayFabRequestEvent<UnlinkKongregateAccountRequest>)obj246;
					}
				}
			}
			if (this.OnUnlinkKongregateResultEvent != null)
			{
				invocationList = this.OnUnlinkKongregateResultEvent.GetInvocationList();
				foreach (Delegate obj247 in invocationList)
				{
					if (obj247.Target == instance)
					{
						OnUnlinkKongregateResultEvent -= (PlayFabResultEvent<UnlinkKongregateAccountResult>)obj247;
					}
				}
			}
			if (this.OnUnlinkNintendoSwitchDeviceIdRequestEvent != null)
			{
				invocationList = this.OnUnlinkNintendoSwitchDeviceIdRequestEvent.GetInvocationList();
				foreach (Delegate obj248 in invocationList)
				{
					if (obj248.Target == instance)
					{
						OnUnlinkNintendoSwitchDeviceIdRequestEvent -= (PlayFabRequestEvent<UnlinkNintendoSwitchDeviceIdRequest>)obj248;
					}
				}
			}
			if (this.OnUnlinkNintendoSwitchDeviceIdResultEvent != null)
			{
				invocationList = this.OnUnlinkNintendoSwitchDeviceIdResultEvent.GetInvocationList();
				foreach (Delegate obj249 in invocationList)
				{
					if (obj249.Target == instance)
					{
						OnUnlinkNintendoSwitchDeviceIdResultEvent -= (PlayFabResultEvent<UnlinkNintendoSwitchDeviceIdResult>)obj249;
					}
				}
			}
			if (this.OnUnlinkOpenIdConnectRequestEvent != null)
			{
				invocationList = this.OnUnlinkOpenIdConnectRequestEvent.GetInvocationList();
				foreach (Delegate obj250 in invocationList)
				{
					if (obj250.Target == instance)
					{
						OnUnlinkOpenIdConnectRequestEvent -= (PlayFabRequestEvent<UninkOpenIdConnectRequest>)obj250;
					}
				}
			}
			if (this.OnUnlinkOpenIdConnectResultEvent != null)
			{
				invocationList = this.OnUnlinkOpenIdConnectResultEvent.GetInvocationList();
				foreach (Delegate obj251 in invocationList)
				{
					if (obj251.Target == instance)
					{
						OnUnlinkOpenIdConnectResultEvent -= (PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse>)obj251;
					}
				}
			}
			if (this.OnUnlinkPSNAccountRequestEvent != null)
			{
				invocationList = this.OnUnlinkPSNAccountRequestEvent.GetInvocationList();
				foreach (Delegate obj252 in invocationList)
				{
					if (obj252.Target == instance)
					{
						OnUnlinkPSNAccountRequestEvent -= (PlayFabRequestEvent<UnlinkPSNAccountRequest>)obj252;
					}
				}
			}
			if (this.OnUnlinkPSNAccountResultEvent != null)
			{
				invocationList = this.OnUnlinkPSNAccountResultEvent.GetInvocationList();
				foreach (Delegate obj253 in invocationList)
				{
					if (obj253.Target == instance)
					{
						OnUnlinkPSNAccountResultEvent -= (PlayFabResultEvent<UnlinkPSNAccountResult>)obj253;
					}
				}
			}
			if (this.OnUnlinkSteamAccountRequestEvent != null)
			{
				invocationList = this.OnUnlinkSteamAccountRequestEvent.GetInvocationList();
				foreach (Delegate obj254 in invocationList)
				{
					if (obj254.Target == instance)
					{
						OnUnlinkSteamAccountRequestEvent -= (PlayFabRequestEvent<UnlinkSteamAccountRequest>)obj254;
					}
				}
			}
			if (this.OnUnlinkSteamAccountResultEvent != null)
			{
				invocationList = this.OnUnlinkSteamAccountResultEvent.GetInvocationList();
				foreach (Delegate obj255 in invocationList)
				{
					if (obj255.Target == instance)
					{
						OnUnlinkSteamAccountResultEvent -= (PlayFabResultEvent<UnlinkSteamAccountResult>)obj255;
					}
				}
			}
			if (this.OnUnlinkTwitchRequestEvent != null)
			{
				invocationList = this.OnUnlinkTwitchRequestEvent.GetInvocationList();
				foreach (Delegate obj256 in invocationList)
				{
					if (obj256.Target == instance)
					{
						OnUnlinkTwitchRequestEvent -= (PlayFabRequestEvent<UnlinkTwitchAccountRequest>)obj256;
					}
				}
			}
			if (this.OnUnlinkTwitchResultEvent != null)
			{
				invocationList = this.OnUnlinkTwitchResultEvent.GetInvocationList();
				foreach (Delegate obj257 in invocationList)
				{
					if (obj257.Target == instance)
					{
						OnUnlinkTwitchResultEvent -= (PlayFabResultEvent<UnlinkTwitchAccountResult>)obj257;
					}
				}
			}
			if (this.OnUnlinkWindowsHelloRequestEvent != null)
			{
				invocationList = this.OnUnlinkWindowsHelloRequestEvent.GetInvocationList();
				foreach (Delegate obj258 in invocationList)
				{
					if (obj258.Target == instance)
					{
						OnUnlinkWindowsHelloRequestEvent -= (PlayFabRequestEvent<UnlinkWindowsHelloAccountRequest>)obj258;
					}
				}
			}
			if (this.OnUnlinkWindowsHelloResultEvent != null)
			{
				invocationList = this.OnUnlinkWindowsHelloResultEvent.GetInvocationList();
				foreach (Delegate obj259 in invocationList)
				{
					if (obj259.Target == instance)
					{
						OnUnlinkWindowsHelloResultEvent -= (PlayFabResultEvent<UnlinkWindowsHelloAccountResponse>)obj259;
					}
				}
			}
			if (this.OnUnlinkXboxAccountRequestEvent != null)
			{
				invocationList = this.OnUnlinkXboxAccountRequestEvent.GetInvocationList();
				foreach (Delegate obj260 in invocationList)
				{
					if (obj260.Target == instance)
					{
						OnUnlinkXboxAccountRequestEvent -= (PlayFabRequestEvent<UnlinkXboxAccountRequest>)obj260;
					}
				}
			}
			if (this.OnUnlinkXboxAccountResultEvent != null)
			{
				invocationList = this.OnUnlinkXboxAccountResultEvent.GetInvocationList();
				foreach (Delegate obj261 in invocationList)
				{
					if (obj261.Target == instance)
					{
						OnUnlinkXboxAccountResultEvent -= (PlayFabResultEvent<UnlinkXboxAccountResult>)obj261;
					}
				}
			}
			if (this.OnUnlockContainerInstanceRequestEvent != null)
			{
				invocationList = this.OnUnlockContainerInstanceRequestEvent.GetInvocationList();
				foreach (Delegate obj262 in invocationList)
				{
					if (obj262.Target == instance)
					{
						OnUnlockContainerInstanceRequestEvent -= (PlayFabRequestEvent<UnlockContainerInstanceRequest>)obj262;
					}
				}
			}
			if (this.OnUnlockContainerInstanceResultEvent != null)
			{
				invocationList = this.OnUnlockContainerInstanceResultEvent.GetInvocationList();
				foreach (Delegate obj263 in invocationList)
				{
					if (obj263.Target == instance)
					{
						OnUnlockContainerInstanceResultEvent -= (PlayFabResultEvent<UnlockContainerItemResult>)obj263;
					}
				}
			}
			if (this.OnUnlockContainerItemRequestEvent != null)
			{
				invocationList = this.OnUnlockContainerItemRequestEvent.GetInvocationList();
				foreach (Delegate obj264 in invocationList)
				{
					if (obj264.Target == instance)
					{
						OnUnlockContainerItemRequestEvent -= (PlayFabRequestEvent<UnlockContainerItemRequest>)obj264;
					}
				}
			}
			if (this.OnUnlockContainerItemResultEvent != null)
			{
				invocationList = this.OnUnlockContainerItemResultEvent.GetInvocationList();
				foreach (Delegate obj265 in invocationList)
				{
					if (obj265.Target == instance)
					{
						OnUnlockContainerItemResultEvent -= (PlayFabResultEvent<UnlockContainerItemResult>)obj265;
					}
				}
			}
			if (this.OnUpdateAvatarUrlRequestEvent != null)
			{
				invocationList = this.OnUpdateAvatarUrlRequestEvent.GetInvocationList();
				foreach (Delegate obj266 in invocationList)
				{
					if (obj266.Target == instance)
					{
						OnUpdateAvatarUrlRequestEvent -= (PlayFabRequestEvent<UpdateAvatarUrlRequest>)obj266;
					}
				}
			}
			if (this.OnUpdateAvatarUrlResultEvent != null)
			{
				invocationList = this.OnUpdateAvatarUrlResultEvent.GetInvocationList();
				foreach (Delegate obj267 in invocationList)
				{
					if (obj267.Target == instance)
					{
						OnUpdateAvatarUrlResultEvent -= (PlayFabResultEvent<PlayFab.ClientModels.EmptyResponse>)obj267;
					}
				}
			}
			if (this.OnUpdateCharacterDataRequestEvent != null)
			{
				invocationList = this.OnUpdateCharacterDataRequestEvent.GetInvocationList();
				foreach (Delegate obj268 in invocationList)
				{
					if (obj268.Target == instance)
					{
						OnUpdateCharacterDataRequestEvent -= (PlayFabRequestEvent<UpdateCharacterDataRequest>)obj268;
					}
				}
			}
			if (this.OnUpdateCharacterDataResultEvent != null)
			{
				invocationList = this.OnUpdateCharacterDataResultEvent.GetInvocationList();
				foreach (Delegate obj269 in invocationList)
				{
					if (obj269.Target == instance)
					{
						OnUpdateCharacterDataResultEvent -= (PlayFabResultEvent<UpdateCharacterDataResult>)obj269;
					}
				}
			}
			if (this.OnUpdateCharacterStatisticsRequestEvent != null)
			{
				invocationList = this.OnUpdateCharacterStatisticsRequestEvent.GetInvocationList();
				foreach (Delegate obj270 in invocationList)
				{
					if (obj270.Target == instance)
					{
						OnUpdateCharacterStatisticsRequestEvent -= (PlayFabRequestEvent<UpdateCharacterStatisticsRequest>)obj270;
					}
				}
			}
			if (this.OnUpdateCharacterStatisticsResultEvent != null)
			{
				invocationList = this.OnUpdateCharacterStatisticsResultEvent.GetInvocationList();
				foreach (Delegate obj271 in invocationList)
				{
					if (obj271.Target == instance)
					{
						OnUpdateCharacterStatisticsResultEvent -= (PlayFabResultEvent<UpdateCharacterStatisticsResult>)obj271;
					}
				}
			}
			if (this.OnUpdatePlayerStatisticsRequestEvent != null)
			{
				invocationList = this.OnUpdatePlayerStatisticsRequestEvent.GetInvocationList();
				foreach (Delegate obj272 in invocationList)
				{
					if (obj272.Target == instance)
					{
						OnUpdatePlayerStatisticsRequestEvent -= (PlayFabRequestEvent<UpdatePlayerStatisticsRequest>)obj272;
					}
				}
			}
			if (this.OnUpdatePlayerStatisticsResultEvent != null)
			{
				invocationList = this.OnUpdatePlayerStatisticsResultEvent.GetInvocationList();
				foreach (Delegate obj273 in invocationList)
				{
					if (obj273.Target == instance)
					{
						OnUpdatePlayerStatisticsResultEvent -= (PlayFabResultEvent<UpdatePlayerStatisticsResult>)obj273;
					}
				}
			}
			if (this.OnUpdateSharedGroupDataRequestEvent != null)
			{
				invocationList = this.OnUpdateSharedGroupDataRequestEvent.GetInvocationList();
				foreach (Delegate obj274 in invocationList)
				{
					if (obj274.Target == instance)
					{
						OnUpdateSharedGroupDataRequestEvent -= (PlayFabRequestEvent<UpdateSharedGroupDataRequest>)obj274;
					}
				}
			}
			if (this.OnUpdateSharedGroupDataResultEvent != null)
			{
				invocationList = this.OnUpdateSharedGroupDataResultEvent.GetInvocationList();
				foreach (Delegate obj275 in invocationList)
				{
					if (obj275.Target == instance)
					{
						OnUpdateSharedGroupDataResultEvent -= (PlayFabResultEvent<UpdateSharedGroupDataResult>)obj275;
					}
				}
			}
			if (this.OnUpdateUserDataRequestEvent != null)
			{
				invocationList = this.OnUpdateUserDataRequestEvent.GetInvocationList();
				foreach (Delegate obj276 in invocationList)
				{
					if (obj276.Target == instance)
					{
						OnUpdateUserDataRequestEvent -= (PlayFabRequestEvent<UpdateUserDataRequest>)obj276;
					}
				}
			}
			if (this.OnUpdateUserDataResultEvent != null)
			{
				invocationList = this.OnUpdateUserDataResultEvent.GetInvocationList();
				foreach (Delegate obj277 in invocationList)
				{
					if (obj277.Target == instance)
					{
						OnUpdateUserDataResultEvent -= (PlayFabResultEvent<UpdateUserDataResult>)obj277;
					}
				}
			}
			if (this.OnUpdateUserPublisherDataRequestEvent != null)
			{
				invocationList = this.OnUpdateUserPublisherDataRequestEvent.GetInvocationList();
				foreach (Delegate obj278 in invocationList)
				{
					if (obj278.Target == instance)
					{
						OnUpdateUserPublisherDataRequestEvent -= (PlayFabRequestEvent<UpdateUserDataRequest>)obj278;
					}
				}
			}
			if (this.OnUpdateUserPublisherDataResultEvent != null)
			{
				invocationList = this.OnUpdateUserPublisherDataResultEvent.GetInvocationList();
				foreach (Delegate obj279 in invocationList)
				{
					if (obj279.Target == instance)
					{
						OnUpdateUserPublisherDataResultEvent -= (PlayFabResultEvent<UpdateUserDataResult>)obj279;
					}
				}
			}
			if (this.OnUpdateUserTitleDisplayNameRequestEvent != null)
			{
				invocationList = this.OnUpdateUserTitleDisplayNameRequestEvent.GetInvocationList();
				foreach (Delegate obj280 in invocationList)
				{
					if (obj280.Target == instance)
					{
						OnUpdateUserTitleDisplayNameRequestEvent -= (PlayFabRequestEvent<UpdateUserTitleDisplayNameRequest>)obj280;
					}
				}
			}
			if (this.OnUpdateUserTitleDisplayNameResultEvent != null)
			{
				invocationList = this.OnUpdateUserTitleDisplayNameResultEvent.GetInvocationList();
				foreach (Delegate obj281 in invocationList)
				{
					if (obj281.Target == instance)
					{
						OnUpdateUserTitleDisplayNameResultEvent -= (PlayFabResultEvent<UpdateUserTitleDisplayNameResult>)obj281;
					}
				}
			}
			if (this.OnValidateAmazonIAPReceiptRequestEvent != null)
			{
				invocationList = this.OnValidateAmazonIAPReceiptRequestEvent.GetInvocationList();
				foreach (Delegate obj282 in invocationList)
				{
					if (obj282.Target == instance)
					{
						OnValidateAmazonIAPReceiptRequestEvent -= (PlayFabRequestEvent<ValidateAmazonReceiptRequest>)obj282;
					}
				}
			}
			if (this.OnValidateAmazonIAPReceiptResultEvent != null)
			{
				invocationList = this.OnValidateAmazonIAPReceiptResultEvent.GetInvocationList();
				foreach (Delegate obj283 in invocationList)
				{
					if (obj283.Target == instance)
					{
						OnValidateAmazonIAPReceiptResultEvent -= (PlayFabResultEvent<ValidateAmazonReceiptResult>)obj283;
					}
				}
			}
			if (this.OnValidateGooglePlayPurchaseRequestEvent != null)
			{
				invocationList = this.OnValidateGooglePlayPurchaseRequestEvent.GetInvocationList();
				foreach (Delegate obj284 in invocationList)
				{
					if (obj284.Target == instance)
					{
						OnValidateGooglePlayPurchaseRequestEvent -= (PlayFabRequestEvent<ValidateGooglePlayPurchaseRequest>)obj284;
					}
				}
			}
			if (this.OnValidateGooglePlayPurchaseResultEvent != null)
			{
				invocationList = this.OnValidateGooglePlayPurchaseResultEvent.GetInvocationList();
				foreach (Delegate obj285 in invocationList)
				{
					if (obj285.Target == instance)
					{
						OnValidateGooglePlayPurchaseResultEvent -= (PlayFabResultEvent<ValidateGooglePlayPurchaseResult>)obj285;
					}
				}
			}
			if (this.OnValidateIOSReceiptRequestEvent != null)
			{
				invocationList = this.OnValidateIOSReceiptRequestEvent.GetInvocationList();
				foreach (Delegate obj286 in invocationList)
				{
					if (obj286.Target == instance)
					{
						OnValidateIOSReceiptRequestEvent -= (PlayFabRequestEvent<ValidateIOSReceiptRequest>)obj286;
					}
				}
			}
			if (this.OnValidateIOSReceiptResultEvent != null)
			{
				invocationList = this.OnValidateIOSReceiptResultEvent.GetInvocationList();
				foreach (Delegate obj287 in invocationList)
				{
					if (obj287.Target == instance)
					{
						OnValidateIOSReceiptResultEvent -= (PlayFabResultEvent<ValidateIOSReceiptResult>)obj287;
					}
				}
			}
			if (this.OnValidateWindowsStoreReceiptRequestEvent != null)
			{
				invocationList = this.OnValidateWindowsStoreReceiptRequestEvent.GetInvocationList();
				foreach (Delegate obj288 in invocationList)
				{
					if (obj288.Target == instance)
					{
						OnValidateWindowsStoreReceiptRequestEvent -= (PlayFabRequestEvent<ValidateWindowsReceiptRequest>)obj288;
					}
				}
			}
			if (this.OnValidateWindowsStoreReceiptResultEvent != null)
			{
				invocationList = this.OnValidateWindowsStoreReceiptResultEvent.GetInvocationList();
				foreach (Delegate obj289 in invocationList)
				{
					if (obj289.Target == instance)
					{
						OnValidateWindowsStoreReceiptResultEvent -= (PlayFabResultEvent<ValidateWindowsReceiptResult>)obj289;
					}
				}
			}
			if (this.OnWriteCharacterEventRequestEvent != null)
			{
				invocationList = this.OnWriteCharacterEventRequestEvent.GetInvocationList();
				foreach (Delegate obj290 in invocationList)
				{
					if (obj290.Target == instance)
					{
						OnWriteCharacterEventRequestEvent -= (PlayFabRequestEvent<WriteClientCharacterEventRequest>)obj290;
					}
				}
			}
			if (this.OnWriteCharacterEventResultEvent != null)
			{
				invocationList = this.OnWriteCharacterEventResultEvent.GetInvocationList();
				foreach (Delegate obj291 in invocationList)
				{
					if (obj291.Target == instance)
					{
						OnWriteCharacterEventResultEvent -= (PlayFabResultEvent<WriteEventResponse>)obj291;
					}
				}
			}
			if (this.OnWritePlayerEventRequestEvent != null)
			{
				invocationList = this.OnWritePlayerEventRequestEvent.GetInvocationList();
				foreach (Delegate obj292 in invocationList)
				{
					if (obj292.Target == instance)
					{
						OnWritePlayerEventRequestEvent -= (PlayFabRequestEvent<WriteClientPlayerEventRequest>)obj292;
					}
				}
			}
			if (this.OnWritePlayerEventResultEvent != null)
			{
				invocationList = this.OnWritePlayerEventResultEvent.GetInvocationList();
				foreach (Delegate obj293 in invocationList)
				{
					if (obj293.Target == instance)
					{
						OnWritePlayerEventResultEvent -= (PlayFabResultEvent<WriteEventResponse>)obj293;
					}
				}
			}
			if (this.OnWriteTitleEventRequestEvent != null)
			{
				invocationList = this.OnWriteTitleEventRequestEvent.GetInvocationList();
				foreach (Delegate obj294 in invocationList)
				{
					if (obj294.Target == instance)
					{
						OnWriteTitleEventRequestEvent -= (PlayFabRequestEvent<WriteTitleEventRequest>)obj294;
					}
				}
			}
			if (this.OnWriteTitleEventResultEvent != null)
			{
				invocationList = this.OnWriteTitleEventResultEvent.GetInvocationList();
				foreach (Delegate obj295 in invocationList)
				{
					if (obj295.Target == instance)
					{
						OnWriteTitleEventResultEvent -= (PlayFabResultEvent<WriteEventResponse>)obj295;
					}
				}
			}
			if (this.OnAuthenticationGetEntityTokenRequestEvent != null)
			{
				invocationList = this.OnAuthenticationGetEntityTokenRequestEvent.GetInvocationList();
				foreach (Delegate obj296 in invocationList)
				{
					if (obj296.Target == instance)
					{
						OnAuthenticationGetEntityTokenRequestEvent -= (PlayFabRequestEvent<GetEntityTokenRequest>)obj296;
					}
				}
			}
			if (this.OnAuthenticationGetEntityTokenResultEvent != null)
			{
				invocationList = this.OnAuthenticationGetEntityTokenResultEvent.GetInvocationList();
				foreach (Delegate obj297 in invocationList)
				{
					if (obj297.Target == instance)
					{
						OnAuthenticationGetEntityTokenResultEvent -= (PlayFabResultEvent<GetEntityTokenResponse>)obj297;
					}
				}
			}
			if (this.OnCloudScriptExecuteEntityCloudScriptRequestEvent != null)
			{
				invocationList = this.OnCloudScriptExecuteEntityCloudScriptRequestEvent.GetInvocationList();
				foreach (Delegate obj298 in invocationList)
				{
					if (obj298.Target == instance)
					{
						OnCloudScriptExecuteEntityCloudScriptRequestEvent -= (PlayFabRequestEvent<ExecuteEntityCloudScriptRequest>)obj298;
					}
				}
			}
			if (this.OnCloudScriptExecuteEntityCloudScriptResultEvent != null)
			{
				invocationList = this.OnCloudScriptExecuteEntityCloudScriptResultEvent.GetInvocationList();
				foreach (Delegate obj299 in invocationList)
				{
					if (obj299.Target == instance)
					{
						OnCloudScriptExecuteEntityCloudScriptResultEvent -= (PlayFabResultEvent<PlayFab.CloudScriptModels.ExecuteCloudScriptResult>)obj299;
					}
				}
			}
			if (this.OnDataAbortFileUploadsRequestEvent != null)
			{
				invocationList = this.OnDataAbortFileUploadsRequestEvent.GetInvocationList();
				foreach (Delegate obj300 in invocationList)
				{
					if (obj300.Target == instance)
					{
						OnDataAbortFileUploadsRequestEvent -= (PlayFabRequestEvent<AbortFileUploadsRequest>)obj300;
					}
				}
			}
			if (this.OnDataAbortFileUploadsResultEvent != null)
			{
				invocationList = this.OnDataAbortFileUploadsResultEvent.GetInvocationList();
				foreach (Delegate obj301 in invocationList)
				{
					if (obj301.Target == instance)
					{
						OnDataAbortFileUploadsResultEvent -= (PlayFabResultEvent<AbortFileUploadsResponse>)obj301;
					}
				}
			}
			if (this.OnDataDeleteFilesRequestEvent != null)
			{
				invocationList = this.OnDataDeleteFilesRequestEvent.GetInvocationList();
				foreach (Delegate obj302 in invocationList)
				{
					if (obj302.Target == instance)
					{
						OnDataDeleteFilesRequestEvent -= (PlayFabRequestEvent<DeleteFilesRequest>)obj302;
					}
				}
			}
			if (this.OnDataDeleteFilesResultEvent != null)
			{
				invocationList = this.OnDataDeleteFilesResultEvent.GetInvocationList();
				foreach (Delegate obj303 in invocationList)
				{
					if (obj303.Target == instance)
					{
						OnDataDeleteFilesResultEvent -= (PlayFabResultEvent<DeleteFilesResponse>)obj303;
					}
				}
			}
			if (this.OnDataFinalizeFileUploadsRequestEvent != null)
			{
				invocationList = this.OnDataFinalizeFileUploadsRequestEvent.GetInvocationList();
				foreach (Delegate obj304 in invocationList)
				{
					if (obj304.Target == instance)
					{
						OnDataFinalizeFileUploadsRequestEvent -= (PlayFabRequestEvent<FinalizeFileUploadsRequest>)obj304;
					}
				}
			}
			if (this.OnDataFinalizeFileUploadsResultEvent != null)
			{
				invocationList = this.OnDataFinalizeFileUploadsResultEvent.GetInvocationList();
				foreach (Delegate obj305 in invocationList)
				{
					if (obj305.Target == instance)
					{
						OnDataFinalizeFileUploadsResultEvent -= (PlayFabResultEvent<FinalizeFileUploadsResponse>)obj305;
					}
				}
			}
			if (this.OnDataGetFilesRequestEvent != null)
			{
				invocationList = this.OnDataGetFilesRequestEvent.GetInvocationList();
				foreach (Delegate obj306 in invocationList)
				{
					if (obj306.Target == instance)
					{
						OnDataGetFilesRequestEvent -= (PlayFabRequestEvent<GetFilesRequest>)obj306;
					}
				}
			}
			if (this.OnDataGetFilesResultEvent != null)
			{
				invocationList = this.OnDataGetFilesResultEvent.GetInvocationList();
				foreach (Delegate obj307 in invocationList)
				{
					if (obj307.Target == instance)
					{
						OnDataGetFilesResultEvent -= (PlayFabResultEvent<GetFilesResponse>)obj307;
					}
				}
			}
			if (this.OnDataGetObjectsRequestEvent != null)
			{
				invocationList = this.OnDataGetObjectsRequestEvent.GetInvocationList();
				foreach (Delegate obj308 in invocationList)
				{
					if (obj308.Target == instance)
					{
						OnDataGetObjectsRequestEvent -= (PlayFabRequestEvent<GetObjectsRequest>)obj308;
					}
				}
			}
			if (this.OnDataGetObjectsResultEvent != null)
			{
				invocationList = this.OnDataGetObjectsResultEvent.GetInvocationList();
				foreach (Delegate obj309 in invocationList)
				{
					if (obj309.Target == instance)
					{
						OnDataGetObjectsResultEvent -= (PlayFabResultEvent<GetObjectsResponse>)obj309;
					}
				}
			}
			if (this.OnDataInitiateFileUploadsRequestEvent != null)
			{
				invocationList = this.OnDataInitiateFileUploadsRequestEvent.GetInvocationList();
				foreach (Delegate obj310 in invocationList)
				{
					if (obj310.Target == instance)
					{
						OnDataInitiateFileUploadsRequestEvent -= (PlayFabRequestEvent<InitiateFileUploadsRequest>)obj310;
					}
				}
			}
			if (this.OnDataInitiateFileUploadsResultEvent != null)
			{
				invocationList = this.OnDataInitiateFileUploadsResultEvent.GetInvocationList();
				foreach (Delegate obj311 in invocationList)
				{
					if (obj311.Target == instance)
					{
						OnDataInitiateFileUploadsResultEvent -= (PlayFabResultEvent<InitiateFileUploadsResponse>)obj311;
					}
				}
			}
			if (this.OnDataSetObjectsRequestEvent != null)
			{
				invocationList = this.OnDataSetObjectsRequestEvent.GetInvocationList();
				foreach (Delegate obj312 in invocationList)
				{
					if (obj312.Target == instance)
					{
						OnDataSetObjectsRequestEvent -= (PlayFabRequestEvent<SetObjectsRequest>)obj312;
					}
				}
			}
			if (this.OnDataSetObjectsResultEvent != null)
			{
				invocationList = this.OnDataSetObjectsResultEvent.GetInvocationList();
				foreach (Delegate obj313 in invocationList)
				{
					if (obj313.Target == instance)
					{
						OnDataSetObjectsResultEvent -= (PlayFabResultEvent<SetObjectsResponse>)obj313;
					}
				}
			}
			if (this.OnEventsWriteEventsRequestEvent != null)
			{
				invocationList = this.OnEventsWriteEventsRequestEvent.GetInvocationList();
				foreach (Delegate obj314 in invocationList)
				{
					if (obj314.Target == instance)
					{
						OnEventsWriteEventsRequestEvent -= (PlayFabRequestEvent<WriteEventsRequest>)obj314;
					}
				}
			}
			if (this.OnEventsWriteEventsResultEvent != null)
			{
				invocationList = this.OnEventsWriteEventsResultEvent.GetInvocationList();
				foreach (Delegate obj315 in invocationList)
				{
					if (obj315.Target == instance)
					{
						OnEventsWriteEventsResultEvent -= (PlayFabResultEvent<WriteEventsResponse>)obj315;
					}
				}
			}
			if (this.OnGroupsAcceptGroupApplicationRequestEvent != null)
			{
				invocationList = this.OnGroupsAcceptGroupApplicationRequestEvent.GetInvocationList();
				foreach (Delegate obj316 in invocationList)
				{
					if (obj316.Target == instance)
					{
						OnGroupsAcceptGroupApplicationRequestEvent -= (PlayFabRequestEvent<AcceptGroupApplicationRequest>)obj316;
					}
				}
			}
			if (this.OnGroupsAcceptGroupApplicationResultEvent != null)
			{
				invocationList = this.OnGroupsAcceptGroupApplicationResultEvent.GetInvocationList();
				foreach (Delegate obj317 in invocationList)
				{
					if (obj317.Target == instance)
					{
						OnGroupsAcceptGroupApplicationResultEvent -= (PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse>)obj317;
					}
				}
			}
			if (this.OnGroupsAcceptGroupInvitationRequestEvent != null)
			{
				invocationList = this.OnGroupsAcceptGroupInvitationRequestEvent.GetInvocationList();
				foreach (Delegate obj318 in invocationList)
				{
					if (obj318.Target == instance)
					{
						OnGroupsAcceptGroupInvitationRequestEvent -= (PlayFabRequestEvent<AcceptGroupInvitationRequest>)obj318;
					}
				}
			}
			if (this.OnGroupsAcceptGroupInvitationResultEvent != null)
			{
				invocationList = this.OnGroupsAcceptGroupInvitationResultEvent.GetInvocationList();
				foreach (Delegate obj319 in invocationList)
				{
					if (obj319.Target == instance)
					{
						OnGroupsAcceptGroupInvitationResultEvent -= (PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse>)obj319;
					}
				}
			}
			if (this.OnGroupsAddMembersRequestEvent != null)
			{
				invocationList = this.OnGroupsAddMembersRequestEvent.GetInvocationList();
				foreach (Delegate obj320 in invocationList)
				{
					if (obj320.Target == instance)
					{
						OnGroupsAddMembersRequestEvent -= (PlayFabRequestEvent<AddMembersRequest>)obj320;
					}
				}
			}
			if (this.OnGroupsAddMembersResultEvent != null)
			{
				invocationList = this.OnGroupsAddMembersResultEvent.GetInvocationList();
				foreach (Delegate obj321 in invocationList)
				{
					if (obj321.Target == instance)
					{
						OnGroupsAddMembersResultEvent -= (PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse>)obj321;
					}
				}
			}
			if (this.OnGroupsApplyToGroupRequestEvent != null)
			{
				invocationList = this.OnGroupsApplyToGroupRequestEvent.GetInvocationList();
				foreach (Delegate obj322 in invocationList)
				{
					if (obj322.Target == instance)
					{
						OnGroupsApplyToGroupRequestEvent -= (PlayFabRequestEvent<ApplyToGroupRequest>)obj322;
					}
				}
			}
			if (this.OnGroupsApplyToGroupResultEvent != null)
			{
				invocationList = this.OnGroupsApplyToGroupResultEvent.GetInvocationList();
				foreach (Delegate obj323 in invocationList)
				{
					if (obj323.Target == instance)
					{
						OnGroupsApplyToGroupResultEvent -= (PlayFabResultEvent<ApplyToGroupResponse>)obj323;
					}
				}
			}
			if (this.OnGroupsBlockEntityRequestEvent != null)
			{
				invocationList = this.OnGroupsBlockEntityRequestEvent.GetInvocationList();
				foreach (Delegate obj324 in invocationList)
				{
					if (obj324.Target == instance)
					{
						OnGroupsBlockEntityRequestEvent -= (PlayFabRequestEvent<BlockEntityRequest>)obj324;
					}
				}
			}
			if (this.OnGroupsBlockEntityResultEvent != null)
			{
				invocationList = this.OnGroupsBlockEntityResultEvent.GetInvocationList();
				foreach (Delegate obj325 in invocationList)
				{
					if (obj325.Target == instance)
					{
						OnGroupsBlockEntityResultEvent -= (PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse>)obj325;
					}
				}
			}
			if (this.OnGroupsChangeMemberRoleRequestEvent != null)
			{
				invocationList = this.OnGroupsChangeMemberRoleRequestEvent.GetInvocationList();
				foreach (Delegate obj326 in invocationList)
				{
					if (obj326.Target == instance)
					{
						OnGroupsChangeMemberRoleRequestEvent -= (PlayFabRequestEvent<ChangeMemberRoleRequest>)obj326;
					}
				}
			}
			if (this.OnGroupsChangeMemberRoleResultEvent != null)
			{
				invocationList = this.OnGroupsChangeMemberRoleResultEvent.GetInvocationList();
				foreach (Delegate obj327 in invocationList)
				{
					if (obj327.Target == instance)
					{
						OnGroupsChangeMemberRoleResultEvent -= (PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse>)obj327;
					}
				}
			}
			if (this.OnGroupsCreateGroupRequestEvent != null)
			{
				invocationList = this.OnGroupsCreateGroupRequestEvent.GetInvocationList();
				foreach (Delegate obj328 in invocationList)
				{
					if (obj328.Target == instance)
					{
						OnGroupsCreateGroupRequestEvent -= (PlayFabRequestEvent<CreateGroupRequest>)obj328;
					}
				}
			}
			if (this.OnGroupsCreateGroupResultEvent != null)
			{
				invocationList = this.OnGroupsCreateGroupResultEvent.GetInvocationList();
				foreach (Delegate obj329 in invocationList)
				{
					if (obj329.Target == instance)
					{
						OnGroupsCreateGroupResultEvent -= (PlayFabResultEvent<CreateGroupResponse>)obj329;
					}
				}
			}
			if (this.OnGroupsCreateRoleRequestEvent != null)
			{
				invocationList = this.OnGroupsCreateRoleRequestEvent.GetInvocationList();
				foreach (Delegate obj330 in invocationList)
				{
					if (obj330.Target == instance)
					{
						OnGroupsCreateRoleRequestEvent -= (PlayFabRequestEvent<CreateGroupRoleRequest>)obj330;
					}
				}
			}
			if (this.OnGroupsCreateRoleResultEvent != null)
			{
				invocationList = this.OnGroupsCreateRoleResultEvent.GetInvocationList();
				foreach (Delegate obj331 in invocationList)
				{
					if (obj331.Target == instance)
					{
						OnGroupsCreateRoleResultEvent -= (PlayFabResultEvent<CreateGroupRoleResponse>)obj331;
					}
				}
			}
			if (this.OnGroupsDeleteGroupRequestEvent != null)
			{
				invocationList = this.OnGroupsDeleteGroupRequestEvent.GetInvocationList();
				foreach (Delegate obj332 in invocationList)
				{
					if (obj332.Target == instance)
					{
						OnGroupsDeleteGroupRequestEvent -= (PlayFabRequestEvent<DeleteGroupRequest>)obj332;
					}
				}
			}
			if (this.OnGroupsDeleteGroupResultEvent != null)
			{
				invocationList = this.OnGroupsDeleteGroupResultEvent.GetInvocationList();
				foreach (Delegate obj333 in invocationList)
				{
					if (obj333.Target == instance)
					{
						OnGroupsDeleteGroupResultEvent -= (PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse>)obj333;
					}
				}
			}
			if (this.OnGroupsDeleteRoleRequestEvent != null)
			{
				invocationList = this.OnGroupsDeleteRoleRequestEvent.GetInvocationList();
				foreach (Delegate obj334 in invocationList)
				{
					if (obj334.Target == instance)
					{
						OnGroupsDeleteRoleRequestEvent -= (PlayFabRequestEvent<DeleteRoleRequest>)obj334;
					}
				}
			}
			if (this.OnGroupsDeleteRoleResultEvent != null)
			{
				invocationList = this.OnGroupsDeleteRoleResultEvent.GetInvocationList();
				foreach (Delegate obj335 in invocationList)
				{
					if (obj335.Target == instance)
					{
						OnGroupsDeleteRoleResultEvent -= (PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse>)obj335;
					}
				}
			}
			if (this.OnGroupsGetGroupRequestEvent != null)
			{
				invocationList = this.OnGroupsGetGroupRequestEvent.GetInvocationList();
				foreach (Delegate obj336 in invocationList)
				{
					if (obj336.Target == instance)
					{
						OnGroupsGetGroupRequestEvent -= (PlayFabRequestEvent<GetGroupRequest>)obj336;
					}
				}
			}
			if (this.OnGroupsGetGroupResultEvent != null)
			{
				invocationList = this.OnGroupsGetGroupResultEvent.GetInvocationList();
				foreach (Delegate obj337 in invocationList)
				{
					if (obj337.Target == instance)
					{
						OnGroupsGetGroupResultEvent -= (PlayFabResultEvent<GetGroupResponse>)obj337;
					}
				}
			}
			if (this.OnGroupsInviteToGroupRequestEvent != null)
			{
				invocationList = this.OnGroupsInviteToGroupRequestEvent.GetInvocationList();
				foreach (Delegate obj338 in invocationList)
				{
					if (obj338.Target == instance)
					{
						OnGroupsInviteToGroupRequestEvent -= (PlayFabRequestEvent<InviteToGroupRequest>)obj338;
					}
				}
			}
			if (this.OnGroupsInviteToGroupResultEvent != null)
			{
				invocationList = this.OnGroupsInviteToGroupResultEvent.GetInvocationList();
				foreach (Delegate obj339 in invocationList)
				{
					if (obj339.Target == instance)
					{
						OnGroupsInviteToGroupResultEvent -= (PlayFabResultEvent<InviteToGroupResponse>)obj339;
					}
				}
			}
			if (this.OnGroupsIsMemberRequestEvent != null)
			{
				invocationList = this.OnGroupsIsMemberRequestEvent.GetInvocationList();
				foreach (Delegate obj340 in invocationList)
				{
					if (obj340.Target == instance)
					{
						OnGroupsIsMemberRequestEvent -= (PlayFabRequestEvent<IsMemberRequest>)obj340;
					}
				}
			}
			if (this.OnGroupsIsMemberResultEvent != null)
			{
				invocationList = this.OnGroupsIsMemberResultEvent.GetInvocationList();
				foreach (Delegate obj341 in invocationList)
				{
					if (obj341.Target == instance)
					{
						OnGroupsIsMemberResultEvent -= (PlayFabResultEvent<IsMemberResponse>)obj341;
					}
				}
			}
			if (this.OnGroupsListGroupApplicationsRequestEvent != null)
			{
				invocationList = this.OnGroupsListGroupApplicationsRequestEvent.GetInvocationList();
				foreach (Delegate obj342 in invocationList)
				{
					if (obj342.Target == instance)
					{
						OnGroupsListGroupApplicationsRequestEvent -= (PlayFabRequestEvent<ListGroupApplicationsRequest>)obj342;
					}
				}
			}
			if (this.OnGroupsListGroupApplicationsResultEvent != null)
			{
				invocationList = this.OnGroupsListGroupApplicationsResultEvent.GetInvocationList();
				foreach (Delegate obj343 in invocationList)
				{
					if (obj343.Target == instance)
					{
						OnGroupsListGroupApplicationsResultEvent -= (PlayFabResultEvent<ListGroupApplicationsResponse>)obj343;
					}
				}
			}
			if (this.OnGroupsListGroupBlocksRequestEvent != null)
			{
				invocationList = this.OnGroupsListGroupBlocksRequestEvent.GetInvocationList();
				foreach (Delegate obj344 in invocationList)
				{
					if (obj344.Target == instance)
					{
						OnGroupsListGroupBlocksRequestEvent -= (PlayFabRequestEvent<ListGroupBlocksRequest>)obj344;
					}
				}
			}
			if (this.OnGroupsListGroupBlocksResultEvent != null)
			{
				invocationList = this.OnGroupsListGroupBlocksResultEvent.GetInvocationList();
				foreach (Delegate obj345 in invocationList)
				{
					if (obj345.Target == instance)
					{
						OnGroupsListGroupBlocksResultEvent -= (PlayFabResultEvent<ListGroupBlocksResponse>)obj345;
					}
				}
			}
			if (this.OnGroupsListGroupInvitationsRequestEvent != null)
			{
				invocationList = this.OnGroupsListGroupInvitationsRequestEvent.GetInvocationList();
				foreach (Delegate obj346 in invocationList)
				{
					if (obj346.Target == instance)
					{
						OnGroupsListGroupInvitationsRequestEvent -= (PlayFabRequestEvent<ListGroupInvitationsRequest>)obj346;
					}
				}
			}
			if (this.OnGroupsListGroupInvitationsResultEvent != null)
			{
				invocationList = this.OnGroupsListGroupInvitationsResultEvent.GetInvocationList();
				foreach (Delegate obj347 in invocationList)
				{
					if (obj347.Target == instance)
					{
						OnGroupsListGroupInvitationsResultEvent -= (PlayFabResultEvent<ListGroupInvitationsResponse>)obj347;
					}
				}
			}
			if (this.OnGroupsListGroupMembersRequestEvent != null)
			{
				invocationList = this.OnGroupsListGroupMembersRequestEvent.GetInvocationList();
				foreach (Delegate obj348 in invocationList)
				{
					if (obj348.Target == instance)
					{
						OnGroupsListGroupMembersRequestEvent -= (PlayFabRequestEvent<ListGroupMembersRequest>)obj348;
					}
				}
			}
			if (this.OnGroupsListGroupMembersResultEvent != null)
			{
				invocationList = this.OnGroupsListGroupMembersResultEvent.GetInvocationList();
				foreach (Delegate obj349 in invocationList)
				{
					if (obj349.Target == instance)
					{
						OnGroupsListGroupMembersResultEvent -= (PlayFabResultEvent<ListGroupMembersResponse>)obj349;
					}
				}
			}
			if (this.OnGroupsListMembershipRequestEvent != null)
			{
				invocationList = this.OnGroupsListMembershipRequestEvent.GetInvocationList();
				foreach (Delegate obj350 in invocationList)
				{
					if (obj350.Target == instance)
					{
						OnGroupsListMembershipRequestEvent -= (PlayFabRequestEvent<ListMembershipRequest>)obj350;
					}
				}
			}
			if (this.OnGroupsListMembershipResultEvent != null)
			{
				invocationList = this.OnGroupsListMembershipResultEvent.GetInvocationList();
				foreach (Delegate obj351 in invocationList)
				{
					if (obj351.Target == instance)
					{
						OnGroupsListMembershipResultEvent -= (PlayFabResultEvent<ListMembershipResponse>)obj351;
					}
				}
			}
			if (this.OnGroupsListMembershipOpportunitiesRequestEvent != null)
			{
				invocationList = this.OnGroupsListMembershipOpportunitiesRequestEvent.GetInvocationList();
				foreach (Delegate obj352 in invocationList)
				{
					if (obj352.Target == instance)
					{
						OnGroupsListMembershipOpportunitiesRequestEvent -= (PlayFabRequestEvent<ListMembershipOpportunitiesRequest>)obj352;
					}
				}
			}
			if (this.OnGroupsListMembershipOpportunitiesResultEvent != null)
			{
				invocationList = this.OnGroupsListMembershipOpportunitiesResultEvent.GetInvocationList();
				foreach (Delegate obj353 in invocationList)
				{
					if (obj353.Target == instance)
					{
						OnGroupsListMembershipOpportunitiesResultEvent -= (PlayFabResultEvent<ListMembershipOpportunitiesResponse>)obj353;
					}
				}
			}
			if (this.OnGroupsRemoveGroupApplicationRequestEvent != null)
			{
				invocationList = this.OnGroupsRemoveGroupApplicationRequestEvent.GetInvocationList();
				foreach (Delegate obj354 in invocationList)
				{
					if (obj354.Target == instance)
					{
						OnGroupsRemoveGroupApplicationRequestEvent -= (PlayFabRequestEvent<RemoveGroupApplicationRequest>)obj354;
					}
				}
			}
			if (this.OnGroupsRemoveGroupApplicationResultEvent != null)
			{
				invocationList = this.OnGroupsRemoveGroupApplicationResultEvent.GetInvocationList();
				foreach (Delegate obj355 in invocationList)
				{
					if (obj355.Target == instance)
					{
						OnGroupsRemoveGroupApplicationResultEvent -= (PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse>)obj355;
					}
				}
			}
			if (this.OnGroupsRemoveGroupInvitationRequestEvent != null)
			{
				invocationList = this.OnGroupsRemoveGroupInvitationRequestEvent.GetInvocationList();
				foreach (Delegate obj356 in invocationList)
				{
					if (obj356.Target == instance)
					{
						OnGroupsRemoveGroupInvitationRequestEvent -= (PlayFabRequestEvent<RemoveGroupInvitationRequest>)obj356;
					}
				}
			}
			if (this.OnGroupsRemoveGroupInvitationResultEvent != null)
			{
				invocationList = this.OnGroupsRemoveGroupInvitationResultEvent.GetInvocationList();
				foreach (Delegate obj357 in invocationList)
				{
					if (obj357.Target == instance)
					{
						OnGroupsRemoveGroupInvitationResultEvent -= (PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse>)obj357;
					}
				}
			}
			if (this.OnGroupsRemoveMembersRequestEvent != null)
			{
				invocationList = this.OnGroupsRemoveMembersRequestEvent.GetInvocationList();
				foreach (Delegate obj358 in invocationList)
				{
					if (obj358.Target == instance)
					{
						OnGroupsRemoveMembersRequestEvent -= (PlayFabRequestEvent<RemoveMembersRequest>)obj358;
					}
				}
			}
			if (this.OnGroupsRemoveMembersResultEvent != null)
			{
				invocationList = this.OnGroupsRemoveMembersResultEvent.GetInvocationList();
				foreach (Delegate obj359 in invocationList)
				{
					if (obj359.Target == instance)
					{
						OnGroupsRemoveMembersResultEvent -= (PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse>)obj359;
					}
				}
			}
			if (this.OnGroupsUnblockEntityRequestEvent != null)
			{
				invocationList = this.OnGroupsUnblockEntityRequestEvent.GetInvocationList();
				foreach (Delegate obj360 in invocationList)
				{
					if (obj360.Target == instance)
					{
						OnGroupsUnblockEntityRequestEvent -= (PlayFabRequestEvent<UnblockEntityRequest>)obj360;
					}
				}
			}
			if (this.OnGroupsUnblockEntityResultEvent != null)
			{
				invocationList = this.OnGroupsUnblockEntityResultEvent.GetInvocationList();
				foreach (Delegate obj361 in invocationList)
				{
					if (obj361.Target == instance)
					{
						OnGroupsUnblockEntityResultEvent -= (PlayFabResultEvent<PlayFab.GroupsModels.EmptyResponse>)obj361;
					}
				}
			}
			if (this.OnGroupsUpdateGroupRequestEvent != null)
			{
				invocationList = this.OnGroupsUpdateGroupRequestEvent.GetInvocationList();
				foreach (Delegate obj362 in invocationList)
				{
					if (obj362.Target == instance)
					{
						OnGroupsUpdateGroupRequestEvent -= (PlayFabRequestEvent<UpdateGroupRequest>)obj362;
					}
				}
			}
			if (this.OnGroupsUpdateGroupResultEvent != null)
			{
				invocationList = this.OnGroupsUpdateGroupResultEvent.GetInvocationList();
				foreach (Delegate obj363 in invocationList)
				{
					if (obj363.Target == instance)
					{
						OnGroupsUpdateGroupResultEvent -= (PlayFabResultEvent<UpdateGroupResponse>)obj363;
					}
				}
			}
			if (this.OnGroupsUpdateRoleRequestEvent != null)
			{
				invocationList = this.OnGroupsUpdateRoleRequestEvent.GetInvocationList();
				foreach (Delegate obj364 in invocationList)
				{
					if (obj364.Target == instance)
					{
						OnGroupsUpdateRoleRequestEvent -= (PlayFabRequestEvent<UpdateGroupRoleRequest>)obj364;
					}
				}
			}
			if (this.OnGroupsUpdateRoleResultEvent != null)
			{
				invocationList = this.OnGroupsUpdateRoleResultEvent.GetInvocationList();
				foreach (Delegate obj365 in invocationList)
				{
					if (obj365.Target == instance)
					{
						OnGroupsUpdateRoleResultEvent -= (PlayFabResultEvent<UpdateGroupRoleResponse>)obj365;
					}
				}
			}
			if (this.OnLocalizationGetLanguageListRequestEvent != null)
			{
				invocationList = this.OnLocalizationGetLanguageListRequestEvent.GetInvocationList();
				foreach (Delegate obj366 in invocationList)
				{
					if (obj366.Target == instance)
					{
						OnLocalizationGetLanguageListRequestEvent -= (PlayFabRequestEvent<GetLanguageListRequest>)obj366;
					}
				}
			}
			if (this.OnLocalizationGetLanguageListResultEvent != null)
			{
				invocationList = this.OnLocalizationGetLanguageListResultEvent.GetInvocationList();
				foreach (Delegate obj367 in invocationList)
				{
					if (obj367.Target == instance)
					{
						OnLocalizationGetLanguageListResultEvent -= (PlayFabResultEvent<GetLanguageListResponse>)obj367;
					}
				}
			}
			if (this.OnMultiplayerCancelAllMatchmakingTicketsForPlayerRequestEvent != null)
			{
				invocationList = this.OnMultiplayerCancelAllMatchmakingTicketsForPlayerRequestEvent.GetInvocationList();
				foreach (Delegate obj368 in invocationList)
				{
					if (obj368.Target == instance)
					{
						OnMultiplayerCancelAllMatchmakingTicketsForPlayerRequestEvent -= (PlayFabRequestEvent<CancelAllMatchmakingTicketsForPlayerRequest>)obj368;
					}
				}
			}
			if (this.OnMultiplayerCancelAllMatchmakingTicketsForPlayerResultEvent != null)
			{
				invocationList = this.OnMultiplayerCancelAllMatchmakingTicketsForPlayerResultEvent.GetInvocationList();
				foreach (Delegate obj369 in invocationList)
				{
					if (obj369.Target == instance)
					{
						OnMultiplayerCancelAllMatchmakingTicketsForPlayerResultEvent -= (PlayFabResultEvent<CancelAllMatchmakingTicketsForPlayerResult>)obj369;
					}
				}
			}
			if (this.OnMultiplayerCancelMatchmakingTicketRequestEvent != null)
			{
				invocationList = this.OnMultiplayerCancelMatchmakingTicketRequestEvent.GetInvocationList();
				foreach (Delegate obj370 in invocationList)
				{
					if (obj370.Target == instance)
					{
						OnMultiplayerCancelMatchmakingTicketRequestEvent -= (PlayFabRequestEvent<CancelMatchmakingTicketRequest>)obj370;
					}
				}
			}
			if (this.OnMultiplayerCancelMatchmakingTicketResultEvent != null)
			{
				invocationList = this.OnMultiplayerCancelMatchmakingTicketResultEvent.GetInvocationList();
				foreach (Delegate obj371 in invocationList)
				{
					if (obj371.Target == instance)
					{
						OnMultiplayerCancelMatchmakingTicketResultEvent -= (PlayFabResultEvent<CancelMatchmakingTicketResult>)obj371;
					}
				}
			}
			if (this.OnMultiplayerCreateBuildWithCustomContainerRequestEvent != null)
			{
				invocationList = this.OnMultiplayerCreateBuildWithCustomContainerRequestEvent.GetInvocationList();
				foreach (Delegate obj372 in invocationList)
				{
					if (obj372.Target == instance)
					{
						OnMultiplayerCreateBuildWithCustomContainerRequestEvent -= (PlayFabRequestEvent<CreateBuildWithCustomContainerRequest>)obj372;
					}
				}
			}
			if (this.OnMultiplayerCreateBuildWithCustomContainerResultEvent != null)
			{
				invocationList = this.OnMultiplayerCreateBuildWithCustomContainerResultEvent.GetInvocationList();
				foreach (Delegate obj373 in invocationList)
				{
					if (obj373.Target == instance)
					{
						OnMultiplayerCreateBuildWithCustomContainerResultEvent -= (PlayFabResultEvent<CreateBuildWithCustomContainerResponse>)obj373;
					}
				}
			}
			if (this.OnMultiplayerCreateBuildWithManagedContainerRequestEvent != null)
			{
				invocationList = this.OnMultiplayerCreateBuildWithManagedContainerRequestEvent.GetInvocationList();
				foreach (Delegate obj374 in invocationList)
				{
					if (obj374.Target == instance)
					{
						OnMultiplayerCreateBuildWithManagedContainerRequestEvent -= (PlayFabRequestEvent<CreateBuildWithManagedContainerRequest>)obj374;
					}
				}
			}
			if (this.OnMultiplayerCreateBuildWithManagedContainerResultEvent != null)
			{
				invocationList = this.OnMultiplayerCreateBuildWithManagedContainerResultEvent.GetInvocationList();
				foreach (Delegate obj375 in invocationList)
				{
					if (obj375.Target == instance)
					{
						OnMultiplayerCreateBuildWithManagedContainerResultEvent -= (PlayFabResultEvent<CreateBuildWithManagedContainerResponse>)obj375;
					}
				}
			}
			if (this.OnMultiplayerCreateMatchmakingTicketRequestEvent != null)
			{
				invocationList = this.OnMultiplayerCreateMatchmakingTicketRequestEvent.GetInvocationList();
				foreach (Delegate obj376 in invocationList)
				{
					if (obj376.Target == instance)
					{
						OnMultiplayerCreateMatchmakingTicketRequestEvent -= (PlayFabRequestEvent<CreateMatchmakingTicketRequest>)obj376;
					}
				}
			}
			if (this.OnMultiplayerCreateMatchmakingTicketResultEvent != null)
			{
				invocationList = this.OnMultiplayerCreateMatchmakingTicketResultEvent.GetInvocationList();
				foreach (Delegate obj377 in invocationList)
				{
					if (obj377.Target == instance)
					{
						OnMultiplayerCreateMatchmakingTicketResultEvent -= (PlayFabResultEvent<CreateMatchmakingTicketResult>)obj377;
					}
				}
			}
			if (this.OnMultiplayerCreateRemoteUserRequestEvent != null)
			{
				invocationList = this.OnMultiplayerCreateRemoteUserRequestEvent.GetInvocationList();
				foreach (Delegate obj378 in invocationList)
				{
					if (obj378.Target == instance)
					{
						OnMultiplayerCreateRemoteUserRequestEvent -= (PlayFabRequestEvent<CreateRemoteUserRequest>)obj378;
					}
				}
			}
			if (this.OnMultiplayerCreateRemoteUserResultEvent != null)
			{
				invocationList = this.OnMultiplayerCreateRemoteUserResultEvent.GetInvocationList();
				foreach (Delegate obj379 in invocationList)
				{
					if (obj379.Target == instance)
					{
						OnMultiplayerCreateRemoteUserResultEvent -= (PlayFabResultEvent<CreateRemoteUserResponse>)obj379;
					}
				}
			}
			if (this.OnMultiplayerCreateServerMatchmakingTicketRequestEvent != null)
			{
				invocationList = this.OnMultiplayerCreateServerMatchmakingTicketRequestEvent.GetInvocationList();
				foreach (Delegate obj380 in invocationList)
				{
					if (obj380.Target == instance)
					{
						OnMultiplayerCreateServerMatchmakingTicketRequestEvent -= (PlayFabRequestEvent<CreateServerMatchmakingTicketRequest>)obj380;
					}
				}
			}
			if (this.OnMultiplayerCreateServerMatchmakingTicketResultEvent != null)
			{
				invocationList = this.OnMultiplayerCreateServerMatchmakingTicketResultEvent.GetInvocationList();
				foreach (Delegate obj381 in invocationList)
				{
					if (obj381.Target == instance)
					{
						OnMultiplayerCreateServerMatchmakingTicketResultEvent -= (PlayFabResultEvent<CreateMatchmakingTicketResult>)obj381;
					}
				}
			}
			if (this.OnMultiplayerDeleteAssetRequestEvent != null)
			{
				invocationList = this.OnMultiplayerDeleteAssetRequestEvent.GetInvocationList();
				foreach (Delegate obj382 in invocationList)
				{
					if (obj382.Target == instance)
					{
						OnMultiplayerDeleteAssetRequestEvent -= (PlayFabRequestEvent<DeleteAssetRequest>)obj382;
					}
				}
			}
			if (this.OnMultiplayerDeleteAssetResultEvent != null)
			{
				invocationList = this.OnMultiplayerDeleteAssetResultEvent.GetInvocationList();
				foreach (Delegate obj383 in invocationList)
				{
					if (obj383.Target == instance)
					{
						OnMultiplayerDeleteAssetResultEvent -= (PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse>)obj383;
					}
				}
			}
			if (this.OnMultiplayerDeleteBuildRequestEvent != null)
			{
				invocationList = this.OnMultiplayerDeleteBuildRequestEvent.GetInvocationList();
				foreach (Delegate obj384 in invocationList)
				{
					if (obj384.Target == instance)
					{
						OnMultiplayerDeleteBuildRequestEvent -= (PlayFabRequestEvent<DeleteBuildRequest>)obj384;
					}
				}
			}
			if (this.OnMultiplayerDeleteBuildResultEvent != null)
			{
				invocationList = this.OnMultiplayerDeleteBuildResultEvent.GetInvocationList();
				foreach (Delegate obj385 in invocationList)
				{
					if (obj385.Target == instance)
					{
						OnMultiplayerDeleteBuildResultEvent -= (PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse>)obj385;
					}
				}
			}
			if (this.OnMultiplayerDeleteCertificateRequestEvent != null)
			{
				invocationList = this.OnMultiplayerDeleteCertificateRequestEvent.GetInvocationList();
				foreach (Delegate obj386 in invocationList)
				{
					if (obj386.Target == instance)
					{
						OnMultiplayerDeleteCertificateRequestEvent -= (PlayFabRequestEvent<DeleteCertificateRequest>)obj386;
					}
				}
			}
			if (this.OnMultiplayerDeleteCertificateResultEvent != null)
			{
				invocationList = this.OnMultiplayerDeleteCertificateResultEvent.GetInvocationList();
				foreach (Delegate obj387 in invocationList)
				{
					if (obj387.Target == instance)
					{
						OnMultiplayerDeleteCertificateResultEvent -= (PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse>)obj387;
					}
				}
			}
			if (this.OnMultiplayerDeleteRemoteUserRequestEvent != null)
			{
				invocationList = this.OnMultiplayerDeleteRemoteUserRequestEvent.GetInvocationList();
				foreach (Delegate obj388 in invocationList)
				{
					if (obj388.Target == instance)
					{
						OnMultiplayerDeleteRemoteUserRequestEvent -= (PlayFabRequestEvent<DeleteRemoteUserRequest>)obj388;
					}
				}
			}
			if (this.OnMultiplayerDeleteRemoteUserResultEvent != null)
			{
				invocationList = this.OnMultiplayerDeleteRemoteUserResultEvent.GetInvocationList();
				foreach (Delegate obj389 in invocationList)
				{
					if (obj389.Target == instance)
					{
						OnMultiplayerDeleteRemoteUserResultEvent -= (PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse>)obj389;
					}
				}
			}
			if (this.OnMultiplayerEnableMultiplayerServersForTitleRequestEvent != null)
			{
				invocationList = this.OnMultiplayerEnableMultiplayerServersForTitleRequestEvent.GetInvocationList();
				foreach (Delegate obj390 in invocationList)
				{
					if (obj390.Target == instance)
					{
						OnMultiplayerEnableMultiplayerServersForTitleRequestEvent -= (PlayFabRequestEvent<EnableMultiplayerServersForTitleRequest>)obj390;
					}
				}
			}
			if (this.OnMultiplayerEnableMultiplayerServersForTitleResultEvent != null)
			{
				invocationList = this.OnMultiplayerEnableMultiplayerServersForTitleResultEvent.GetInvocationList();
				foreach (Delegate obj391 in invocationList)
				{
					if (obj391.Target == instance)
					{
						OnMultiplayerEnableMultiplayerServersForTitleResultEvent -= (PlayFabResultEvent<EnableMultiplayerServersForTitleResponse>)obj391;
					}
				}
			}
			if (this.OnMultiplayerGetAssetUploadUrlRequestEvent != null)
			{
				invocationList = this.OnMultiplayerGetAssetUploadUrlRequestEvent.GetInvocationList();
				foreach (Delegate obj392 in invocationList)
				{
					if (obj392.Target == instance)
					{
						OnMultiplayerGetAssetUploadUrlRequestEvent -= (PlayFabRequestEvent<GetAssetUploadUrlRequest>)obj392;
					}
				}
			}
			if (this.OnMultiplayerGetAssetUploadUrlResultEvent != null)
			{
				invocationList = this.OnMultiplayerGetAssetUploadUrlResultEvent.GetInvocationList();
				foreach (Delegate obj393 in invocationList)
				{
					if (obj393.Target == instance)
					{
						OnMultiplayerGetAssetUploadUrlResultEvent -= (PlayFabResultEvent<GetAssetUploadUrlResponse>)obj393;
					}
				}
			}
			if (this.OnMultiplayerGetBuildRequestEvent != null)
			{
				invocationList = this.OnMultiplayerGetBuildRequestEvent.GetInvocationList();
				foreach (Delegate obj394 in invocationList)
				{
					if (obj394.Target == instance)
					{
						OnMultiplayerGetBuildRequestEvent -= (PlayFabRequestEvent<GetBuildRequest>)obj394;
					}
				}
			}
			if (this.OnMultiplayerGetBuildResultEvent != null)
			{
				invocationList = this.OnMultiplayerGetBuildResultEvent.GetInvocationList();
				foreach (Delegate obj395 in invocationList)
				{
					if (obj395.Target == instance)
					{
						OnMultiplayerGetBuildResultEvent -= (PlayFabResultEvent<GetBuildResponse>)obj395;
					}
				}
			}
			if (this.OnMultiplayerGetContainerRegistryCredentialsRequestEvent != null)
			{
				invocationList = this.OnMultiplayerGetContainerRegistryCredentialsRequestEvent.GetInvocationList();
				foreach (Delegate obj396 in invocationList)
				{
					if (obj396.Target == instance)
					{
						OnMultiplayerGetContainerRegistryCredentialsRequestEvent -= (PlayFabRequestEvent<GetContainerRegistryCredentialsRequest>)obj396;
					}
				}
			}
			if (this.OnMultiplayerGetContainerRegistryCredentialsResultEvent != null)
			{
				invocationList = this.OnMultiplayerGetContainerRegistryCredentialsResultEvent.GetInvocationList();
				foreach (Delegate obj397 in invocationList)
				{
					if (obj397.Target == instance)
					{
						OnMultiplayerGetContainerRegistryCredentialsResultEvent -= (PlayFabResultEvent<GetContainerRegistryCredentialsResponse>)obj397;
					}
				}
			}
			if (this.OnMultiplayerGetMatchRequestEvent != null)
			{
				invocationList = this.OnMultiplayerGetMatchRequestEvent.GetInvocationList();
				foreach (Delegate obj398 in invocationList)
				{
					if (obj398.Target == instance)
					{
						OnMultiplayerGetMatchRequestEvent -= (PlayFabRequestEvent<GetMatchRequest>)obj398;
					}
				}
			}
			if (this.OnMultiplayerGetMatchResultEvent != null)
			{
				invocationList = this.OnMultiplayerGetMatchResultEvent.GetInvocationList();
				foreach (Delegate obj399 in invocationList)
				{
					if (obj399.Target == instance)
					{
						OnMultiplayerGetMatchResultEvent -= (PlayFabResultEvent<GetMatchResult>)obj399;
					}
				}
			}
			if (this.OnMultiplayerGetMatchmakingQueueRequestEvent != null)
			{
				invocationList = this.OnMultiplayerGetMatchmakingQueueRequestEvent.GetInvocationList();
				foreach (Delegate obj400 in invocationList)
				{
					if (obj400.Target == instance)
					{
						OnMultiplayerGetMatchmakingQueueRequestEvent -= (PlayFabRequestEvent<GetMatchmakingQueueRequest>)obj400;
					}
				}
			}
			if (this.OnMultiplayerGetMatchmakingQueueResultEvent != null)
			{
				invocationList = this.OnMultiplayerGetMatchmakingQueueResultEvent.GetInvocationList();
				foreach (Delegate obj401 in invocationList)
				{
					if (obj401.Target == instance)
					{
						OnMultiplayerGetMatchmakingQueueResultEvent -= (PlayFabResultEvent<GetMatchmakingQueueResult>)obj401;
					}
				}
			}
			if (this.OnMultiplayerGetMatchmakingTicketRequestEvent != null)
			{
				invocationList = this.OnMultiplayerGetMatchmakingTicketRequestEvent.GetInvocationList();
				foreach (Delegate obj402 in invocationList)
				{
					if (obj402.Target == instance)
					{
						OnMultiplayerGetMatchmakingTicketRequestEvent -= (PlayFabRequestEvent<GetMatchmakingTicketRequest>)obj402;
					}
				}
			}
			if (this.OnMultiplayerGetMatchmakingTicketResultEvent != null)
			{
				invocationList = this.OnMultiplayerGetMatchmakingTicketResultEvent.GetInvocationList();
				foreach (Delegate obj403 in invocationList)
				{
					if (obj403.Target == instance)
					{
						OnMultiplayerGetMatchmakingTicketResultEvent -= (PlayFabResultEvent<GetMatchmakingTicketResult>)obj403;
					}
				}
			}
			if (this.OnMultiplayerGetMultiplayerServerDetailsRequestEvent != null)
			{
				invocationList = this.OnMultiplayerGetMultiplayerServerDetailsRequestEvent.GetInvocationList();
				foreach (Delegate obj404 in invocationList)
				{
					if (obj404.Target == instance)
					{
						OnMultiplayerGetMultiplayerServerDetailsRequestEvent -= (PlayFabRequestEvent<GetMultiplayerServerDetailsRequest>)obj404;
					}
				}
			}
			if (this.OnMultiplayerGetMultiplayerServerDetailsResultEvent != null)
			{
				invocationList = this.OnMultiplayerGetMultiplayerServerDetailsResultEvent.GetInvocationList();
				foreach (Delegate obj405 in invocationList)
				{
					if (obj405.Target == instance)
					{
						OnMultiplayerGetMultiplayerServerDetailsResultEvent -= (PlayFabResultEvent<GetMultiplayerServerDetailsResponse>)obj405;
					}
				}
			}
			if (this.OnMultiplayerGetQueueStatisticsRequestEvent != null)
			{
				invocationList = this.OnMultiplayerGetQueueStatisticsRequestEvent.GetInvocationList();
				foreach (Delegate obj406 in invocationList)
				{
					if (obj406.Target == instance)
					{
						OnMultiplayerGetQueueStatisticsRequestEvent -= (PlayFabRequestEvent<GetQueueStatisticsRequest>)obj406;
					}
				}
			}
			if (this.OnMultiplayerGetQueueStatisticsResultEvent != null)
			{
				invocationList = this.OnMultiplayerGetQueueStatisticsResultEvent.GetInvocationList();
				foreach (Delegate obj407 in invocationList)
				{
					if (obj407.Target == instance)
					{
						OnMultiplayerGetQueueStatisticsResultEvent -= (PlayFabResultEvent<GetQueueStatisticsResult>)obj407;
					}
				}
			}
			if (this.OnMultiplayerGetRemoteLoginEndpointRequestEvent != null)
			{
				invocationList = this.OnMultiplayerGetRemoteLoginEndpointRequestEvent.GetInvocationList();
				foreach (Delegate obj408 in invocationList)
				{
					if (obj408.Target == instance)
					{
						OnMultiplayerGetRemoteLoginEndpointRequestEvent -= (PlayFabRequestEvent<GetRemoteLoginEndpointRequest>)obj408;
					}
				}
			}
			if (this.OnMultiplayerGetRemoteLoginEndpointResultEvent != null)
			{
				invocationList = this.OnMultiplayerGetRemoteLoginEndpointResultEvent.GetInvocationList();
				foreach (Delegate obj409 in invocationList)
				{
					if (obj409.Target == instance)
					{
						OnMultiplayerGetRemoteLoginEndpointResultEvent -= (PlayFabResultEvent<GetRemoteLoginEndpointResponse>)obj409;
					}
				}
			}
			if (this.OnMultiplayerGetTitleEnabledForMultiplayerServersStatusRequestEvent != null)
			{
				invocationList = this.OnMultiplayerGetTitleEnabledForMultiplayerServersStatusRequestEvent.GetInvocationList();
				foreach (Delegate obj410 in invocationList)
				{
					if (obj410.Target == instance)
					{
						OnMultiplayerGetTitleEnabledForMultiplayerServersStatusRequestEvent -= (PlayFabRequestEvent<GetTitleEnabledForMultiplayerServersStatusRequest>)obj410;
					}
				}
			}
			if (this.OnMultiplayerGetTitleEnabledForMultiplayerServersStatusResultEvent != null)
			{
				invocationList = this.OnMultiplayerGetTitleEnabledForMultiplayerServersStatusResultEvent.GetInvocationList();
				foreach (Delegate obj411 in invocationList)
				{
					if (obj411.Target == instance)
					{
						OnMultiplayerGetTitleEnabledForMultiplayerServersStatusResultEvent -= (PlayFabResultEvent<GetTitleEnabledForMultiplayerServersStatusResponse>)obj411;
					}
				}
			}
			if (this.OnMultiplayerJoinMatchmakingTicketRequestEvent != null)
			{
				invocationList = this.OnMultiplayerJoinMatchmakingTicketRequestEvent.GetInvocationList();
				foreach (Delegate obj412 in invocationList)
				{
					if (obj412.Target == instance)
					{
						OnMultiplayerJoinMatchmakingTicketRequestEvent -= (PlayFabRequestEvent<JoinMatchmakingTicketRequest>)obj412;
					}
				}
			}
			if (this.OnMultiplayerJoinMatchmakingTicketResultEvent != null)
			{
				invocationList = this.OnMultiplayerJoinMatchmakingTicketResultEvent.GetInvocationList();
				foreach (Delegate obj413 in invocationList)
				{
					if (obj413.Target == instance)
					{
						OnMultiplayerJoinMatchmakingTicketResultEvent -= (PlayFabResultEvent<JoinMatchmakingTicketResult>)obj413;
					}
				}
			}
			if (this.OnMultiplayerListArchivedMultiplayerServersRequestEvent != null)
			{
				invocationList = this.OnMultiplayerListArchivedMultiplayerServersRequestEvent.GetInvocationList();
				foreach (Delegate obj414 in invocationList)
				{
					if (obj414.Target == instance)
					{
						OnMultiplayerListArchivedMultiplayerServersRequestEvent -= (PlayFabRequestEvent<ListMultiplayerServersRequest>)obj414;
					}
				}
			}
			if (this.OnMultiplayerListArchivedMultiplayerServersResultEvent != null)
			{
				invocationList = this.OnMultiplayerListArchivedMultiplayerServersResultEvent.GetInvocationList();
				foreach (Delegate obj415 in invocationList)
				{
					if (obj415.Target == instance)
					{
						OnMultiplayerListArchivedMultiplayerServersResultEvent -= (PlayFabResultEvent<ListMultiplayerServersResponse>)obj415;
					}
				}
			}
			if (this.OnMultiplayerListAssetSummariesRequestEvent != null)
			{
				invocationList = this.OnMultiplayerListAssetSummariesRequestEvent.GetInvocationList();
				foreach (Delegate obj416 in invocationList)
				{
					if (obj416.Target == instance)
					{
						OnMultiplayerListAssetSummariesRequestEvent -= (PlayFabRequestEvent<ListAssetSummariesRequest>)obj416;
					}
				}
			}
			if (this.OnMultiplayerListAssetSummariesResultEvent != null)
			{
				invocationList = this.OnMultiplayerListAssetSummariesResultEvent.GetInvocationList();
				foreach (Delegate obj417 in invocationList)
				{
					if (obj417.Target == instance)
					{
						OnMultiplayerListAssetSummariesResultEvent -= (PlayFabResultEvent<ListAssetSummariesResponse>)obj417;
					}
				}
			}
			if (this.OnMultiplayerListBuildSummariesRequestEvent != null)
			{
				invocationList = this.OnMultiplayerListBuildSummariesRequestEvent.GetInvocationList();
				foreach (Delegate obj418 in invocationList)
				{
					if (obj418.Target == instance)
					{
						OnMultiplayerListBuildSummariesRequestEvent -= (PlayFabRequestEvent<ListBuildSummariesRequest>)obj418;
					}
				}
			}
			if (this.OnMultiplayerListBuildSummariesResultEvent != null)
			{
				invocationList = this.OnMultiplayerListBuildSummariesResultEvent.GetInvocationList();
				foreach (Delegate obj419 in invocationList)
				{
					if (obj419.Target == instance)
					{
						OnMultiplayerListBuildSummariesResultEvent -= (PlayFabResultEvent<ListBuildSummariesResponse>)obj419;
					}
				}
			}
			if (this.OnMultiplayerListCertificateSummariesRequestEvent != null)
			{
				invocationList = this.OnMultiplayerListCertificateSummariesRequestEvent.GetInvocationList();
				foreach (Delegate obj420 in invocationList)
				{
					if (obj420.Target == instance)
					{
						OnMultiplayerListCertificateSummariesRequestEvent -= (PlayFabRequestEvent<ListCertificateSummariesRequest>)obj420;
					}
				}
			}
			if (this.OnMultiplayerListCertificateSummariesResultEvent != null)
			{
				invocationList = this.OnMultiplayerListCertificateSummariesResultEvent.GetInvocationList();
				foreach (Delegate obj421 in invocationList)
				{
					if (obj421.Target == instance)
					{
						OnMultiplayerListCertificateSummariesResultEvent -= (PlayFabResultEvent<ListCertificateSummariesResponse>)obj421;
					}
				}
			}
			if (this.OnMultiplayerListContainerImagesRequestEvent != null)
			{
				invocationList = this.OnMultiplayerListContainerImagesRequestEvent.GetInvocationList();
				foreach (Delegate obj422 in invocationList)
				{
					if (obj422.Target == instance)
					{
						OnMultiplayerListContainerImagesRequestEvent -= (PlayFabRequestEvent<ListContainerImagesRequest>)obj422;
					}
				}
			}
			if (this.OnMultiplayerListContainerImagesResultEvent != null)
			{
				invocationList = this.OnMultiplayerListContainerImagesResultEvent.GetInvocationList();
				foreach (Delegate obj423 in invocationList)
				{
					if (obj423.Target == instance)
					{
						OnMultiplayerListContainerImagesResultEvent -= (PlayFabResultEvent<ListContainerImagesResponse>)obj423;
					}
				}
			}
			if (this.OnMultiplayerListContainerImageTagsRequestEvent != null)
			{
				invocationList = this.OnMultiplayerListContainerImageTagsRequestEvent.GetInvocationList();
				foreach (Delegate obj424 in invocationList)
				{
					if (obj424.Target == instance)
					{
						OnMultiplayerListContainerImageTagsRequestEvent -= (PlayFabRequestEvent<ListContainerImageTagsRequest>)obj424;
					}
				}
			}
			if (this.OnMultiplayerListContainerImageTagsResultEvent != null)
			{
				invocationList = this.OnMultiplayerListContainerImageTagsResultEvent.GetInvocationList();
				foreach (Delegate obj425 in invocationList)
				{
					if (obj425.Target == instance)
					{
						OnMultiplayerListContainerImageTagsResultEvent -= (PlayFabResultEvent<ListContainerImageTagsResponse>)obj425;
					}
				}
			}
			if (this.OnMultiplayerListMatchmakingQueuesRequestEvent != null)
			{
				invocationList = this.OnMultiplayerListMatchmakingQueuesRequestEvent.GetInvocationList();
				foreach (Delegate obj426 in invocationList)
				{
					if (obj426.Target == instance)
					{
						OnMultiplayerListMatchmakingQueuesRequestEvent -= (PlayFabRequestEvent<ListMatchmakingQueuesRequest>)obj426;
					}
				}
			}
			if (this.OnMultiplayerListMatchmakingQueuesResultEvent != null)
			{
				invocationList = this.OnMultiplayerListMatchmakingQueuesResultEvent.GetInvocationList();
				foreach (Delegate obj427 in invocationList)
				{
					if (obj427.Target == instance)
					{
						OnMultiplayerListMatchmakingQueuesResultEvent -= (PlayFabResultEvent<ListMatchmakingQueuesResult>)obj427;
					}
				}
			}
			if (this.OnMultiplayerListMatchmakingTicketsForPlayerRequestEvent != null)
			{
				invocationList = this.OnMultiplayerListMatchmakingTicketsForPlayerRequestEvent.GetInvocationList();
				foreach (Delegate obj428 in invocationList)
				{
					if (obj428.Target == instance)
					{
						OnMultiplayerListMatchmakingTicketsForPlayerRequestEvent -= (PlayFabRequestEvent<ListMatchmakingTicketsForPlayerRequest>)obj428;
					}
				}
			}
			if (this.OnMultiplayerListMatchmakingTicketsForPlayerResultEvent != null)
			{
				invocationList = this.OnMultiplayerListMatchmakingTicketsForPlayerResultEvent.GetInvocationList();
				foreach (Delegate obj429 in invocationList)
				{
					if (obj429.Target == instance)
					{
						OnMultiplayerListMatchmakingTicketsForPlayerResultEvent -= (PlayFabResultEvent<ListMatchmakingTicketsForPlayerResult>)obj429;
					}
				}
			}
			if (this.OnMultiplayerListMultiplayerServersRequestEvent != null)
			{
				invocationList = this.OnMultiplayerListMultiplayerServersRequestEvent.GetInvocationList();
				foreach (Delegate obj430 in invocationList)
				{
					if (obj430.Target == instance)
					{
						OnMultiplayerListMultiplayerServersRequestEvent -= (PlayFabRequestEvent<ListMultiplayerServersRequest>)obj430;
					}
				}
			}
			if (this.OnMultiplayerListMultiplayerServersResultEvent != null)
			{
				invocationList = this.OnMultiplayerListMultiplayerServersResultEvent.GetInvocationList();
				foreach (Delegate obj431 in invocationList)
				{
					if (obj431.Target == instance)
					{
						OnMultiplayerListMultiplayerServersResultEvent -= (PlayFabResultEvent<ListMultiplayerServersResponse>)obj431;
					}
				}
			}
			if (this.OnMultiplayerListQosServersRequestEvent != null)
			{
				invocationList = this.OnMultiplayerListQosServersRequestEvent.GetInvocationList();
				foreach (Delegate obj432 in invocationList)
				{
					if (obj432.Target == instance)
					{
						OnMultiplayerListQosServersRequestEvent -= (PlayFabRequestEvent<ListQosServersRequest>)obj432;
					}
				}
			}
			if (this.OnMultiplayerListQosServersResultEvent != null)
			{
				invocationList = this.OnMultiplayerListQosServersResultEvent.GetInvocationList();
				foreach (Delegate obj433 in invocationList)
				{
					if (obj433.Target == instance)
					{
						OnMultiplayerListQosServersResultEvent -= (PlayFabResultEvent<ListQosServersResponse>)obj433;
					}
				}
			}
			if (this.OnMultiplayerListVirtualMachineSummariesRequestEvent != null)
			{
				invocationList = this.OnMultiplayerListVirtualMachineSummariesRequestEvent.GetInvocationList();
				foreach (Delegate obj434 in invocationList)
				{
					if (obj434.Target == instance)
					{
						OnMultiplayerListVirtualMachineSummariesRequestEvent -= (PlayFabRequestEvent<ListVirtualMachineSummariesRequest>)obj434;
					}
				}
			}
			if (this.OnMultiplayerListVirtualMachineSummariesResultEvent != null)
			{
				invocationList = this.OnMultiplayerListVirtualMachineSummariesResultEvent.GetInvocationList();
				foreach (Delegate obj435 in invocationList)
				{
					if (obj435.Target == instance)
					{
						OnMultiplayerListVirtualMachineSummariesResultEvent -= (PlayFabResultEvent<ListVirtualMachineSummariesResponse>)obj435;
					}
				}
			}
			if (this.OnMultiplayerRemoveMatchmakingQueueRequestEvent != null)
			{
				invocationList = this.OnMultiplayerRemoveMatchmakingQueueRequestEvent.GetInvocationList();
				foreach (Delegate obj436 in invocationList)
				{
					if (obj436.Target == instance)
					{
						OnMultiplayerRemoveMatchmakingQueueRequestEvent -= (PlayFabRequestEvent<RemoveMatchmakingQueueRequest>)obj436;
					}
				}
			}
			if (this.OnMultiplayerRemoveMatchmakingQueueResultEvent != null)
			{
				invocationList = this.OnMultiplayerRemoveMatchmakingQueueResultEvent.GetInvocationList();
				foreach (Delegate obj437 in invocationList)
				{
					if (obj437.Target == instance)
					{
						OnMultiplayerRemoveMatchmakingQueueResultEvent -= (PlayFabResultEvent<RemoveMatchmakingQueueResult>)obj437;
					}
				}
			}
			if (this.OnMultiplayerRequestMultiplayerServerRequestEvent != null)
			{
				invocationList = this.OnMultiplayerRequestMultiplayerServerRequestEvent.GetInvocationList();
				foreach (Delegate obj438 in invocationList)
				{
					if (obj438.Target == instance)
					{
						OnMultiplayerRequestMultiplayerServerRequestEvent -= (PlayFabRequestEvent<RequestMultiplayerServerRequest>)obj438;
					}
				}
			}
			if (this.OnMultiplayerRequestMultiplayerServerResultEvent != null)
			{
				invocationList = this.OnMultiplayerRequestMultiplayerServerResultEvent.GetInvocationList();
				foreach (Delegate obj439 in invocationList)
				{
					if (obj439.Target == instance)
					{
						OnMultiplayerRequestMultiplayerServerResultEvent -= (PlayFabResultEvent<RequestMultiplayerServerResponse>)obj439;
					}
				}
			}
			if (this.OnMultiplayerRolloverContainerRegistryCredentialsRequestEvent != null)
			{
				invocationList = this.OnMultiplayerRolloverContainerRegistryCredentialsRequestEvent.GetInvocationList();
				foreach (Delegate obj440 in invocationList)
				{
					if (obj440.Target == instance)
					{
						OnMultiplayerRolloverContainerRegistryCredentialsRequestEvent -= (PlayFabRequestEvent<RolloverContainerRegistryCredentialsRequest>)obj440;
					}
				}
			}
			if (this.OnMultiplayerRolloverContainerRegistryCredentialsResultEvent != null)
			{
				invocationList = this.OnMultiplayerRolloverContainerRegistryCredentialsResultEvent.GetInvocationList();
				foreach (Delegate obj441 in invocationList)
				{
					if (obj441.Target == instance)
					{
						OnMultiplayerRolloverContainerRegistryCredentialsResultEvent -= (PlayFabResultEvent<RolloverContainerRegistryCredentialsResponse>)obj441;
					}
				}
			}
			if (this.OnMultiplayerSetMatchmakingQueueRequestEvent != null)
			{
				invocationList = this.OnMultiplayerSetMatchmakingQueueRequestEvent.GetInvocationList();
				foreach (Delegate obj442 in invocationList)
				{
					if (obj442.Target == instance)
					{
						OnMultiplayerSetMatchmakingQueueRequestEvent -= (PlayFabRequestEvent<SetMatchmakingQueueRequest>)obj442;
					}
				}
			}
			if (this.OnMultiplayerSetMatchmakingQueueResultEvent != null)
			{
				invocationList = this.OnMultiplayerSetMatchmakingQueueResultEvent.GetInvocationList();
				foreach (Delegate obj443 in invocationList)
				{
					if (obj443.Target == instance)
					{
						OnMultiplayerSetMatchmakingQueueResultEvent -= (PlayFabResultEvent<SetMatchmakingQueueResult>)obj443;
					}
				}
			}
			if (this.OnMultiplayerShutdownMultiplayerServerRequestEvent != null)
			{
				invocationList = this.OnMultiplayerShutdownMultiplayerServerRequestEvent.GetInvocationList();
				foreach (Delegate obj444 in invocationList)
				{
					if (obj444.Target == instance)
					{
						OnMultiplayerShutdownMultiplayerServerRequestEvent -= (PlayFabRequestEvent<ShutdownMultiplayerServerRequest>)obj444;
					}
				}
			}
			if (this.OnMultiplayerShutdownMultiplayerServerResultEvent != null)
			{
				invocationList = this.OnMultiplayerShutdownMultiplayerServerResultEvent.GetInvocationList();
				foreach (Delegate obj445 in invocationList)
				{
					if (obj445.Target == instance)
					{
						OnMultiplayerShutdownMultiplayerServerResultEvent -= (PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse>)obj445;
					}
				}
			}
			if (this.OnMultiplayerUpdateBuildRegionsRequestEvent != null)
			{
				invocationList = this.OnMultiplayerUpdateBuildRegionsRequestEvent.GetInvocationList();
				foreach (Delegate obj446 in invocationList)
				{
					if (obj446.Target == instance)
					{
						OnMultiplayerUpdateBuildRegionsRequestEvent -= (PlayFabRequestEvent<UpdateBuildRegionsRequest>)obj446;
					}
				}
			}
			if (this.OnMultiplayerUpdateBuildRegionsResultEvent != null)
			{
				invocationList = this.OnMultiplayerUpdateBuildRegionsResultEvent.GetInvocationList();
				foreach (Delegate obj447 in invocationList)
				{
					if (obj447.Target == instance)
					{
						OnMultiplayerUpdateBuildRegionsResultEvent -= (PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse>)obj447;
					}
				}
			}
			if (this.OnMultiplayerUploadCertificateRequestEvent != null)
			{
				invocationList = this.OnMultiplayerUploadCertificateRequestEvent.GetInvocationList();
				foreach (Delegate obj448 in invocationList)
				{
					if (obj448.Target == instance)
					{
						OnMultiplayerUploadCertificateRequestEvent -= (PlayFabRequestEvent<UploadCertificateRequest>)obj448;
					}
				}
			}
			if (this.OnMultiplayerUploadCertificateResultEvent != null)
			{
				invocationList = this.OnMultiplayerUploadCertificateResultEvent.GetInvocationList();
				foreach (Delegate obj449 in invocationList)
				{
					if (obj449.Target == instance)
					{
						OnMultiplayerUploadCertificateResultEvent -= (PlayFabResultEvent<PlayFab.MultiplayerModels.EmptyResponse>)obj449;
					}
				}
			}
			if (this.OnProfilesGetGlobalPolicyRequestEvent != null)
			{
				invocationList = this.OnProfilesGetGlobalPolicyRequestEvent.GetInvocationList();
				foreach (Delegate obj450 in invocationList)
				{
					if (obj450.Target == instance)
					{
						OnProfilesGetGlobalPolicyRequestEvent -= (PlayFabRequestEvent<GetGlobalPolicyRequest>)obj450;
					}
				}
			}
			if (this.OnProfilesGetGlobalPolicyResultEvent != null)
			{
				invocationList = this.OnProfilesGetGlobalPolicyResultEvent.GetInvocationList();
				foreach (Delegate obj451 in invocationList)
				{
					if (obj451.Target == instance)
					{
						OnProfilesGetGlobalPolicyResultEvent -= (PlayFabResultEvent<GetGlobalPolicyResponse>)obj451;
					}
				}
			}
			if (this.OnProfilesGetProfileRequestEvent != null)
			{
				invocationList = this.OnProfilesGetProfileRequestEvent.GetInvocationList();
				foreach (Delegate obj452 in invocationList)
				{
					if (obj452.Target == instance)
					{
						OnProfilesGetProfileRequestEvent -= (PlayFabRequestEvent<GetEntityProfileRequest>)obj452;
					}
				}
			}
			if (this.OnProfilesGetProfileResultEvent != null)
			{
				invocationList = this.OnProfilesGetProfileResultEvent.GetInvocationList();
				foreach (Delegate obj453 in invocationList)
				{
					if (obj453.Target == instance)
					{
						OnProfilesGetProfileResultEvent -= (PlayFabResultEvent<GetEntityProfileResponse>)obj453;
					}
				}
			}
			if (this.OnProfilesGetProfilesRequestEvent != null)
			{
				invocationList = this.OnProfilesGetProfilesRequestEvent.GetInvocationList();
				foreach (Delegate obj454 in invocationList)
				{
					if (obj454.Target == instance)
					{
						OnProfilesGetProfilesRequestEvent -= (PlayFabRequestEvent<GetEntityProfilesRequest>)obj454;
					}
				}
			}
			if (this.OnProfilesGetProfilesResultEvent != null)
			{
				invocationList = this.OnProfilesGetProfilesResultEvent.GetInvocationList();
				foreach (Delegate obj455 in invocationList)
				{
					if (obj455.Target == instance)
					{
						OnProfilesGetProfilesResultEvent -= (PlayFabResultEvent<GetEntityProfilesResponse>)obj455;
					}
				}
			}
			if (this.OnProfilesSetGlobalPolicyRequestEvent != null)
			{
				invocationList = this.OnProfilesSetGlobalPolicyRequestEvent.GetInvocationList();
				foreach (Delegate obj456 in invocationList)
				{
					if (obj456.Target == instance)
					{
						OnProfilesSetGlobalPolicyRequestEvent -= (PlayFabRequestEvent<SetGlobalPolicyRequest>)obj456;
					}
				}
			}
			if (this.OnProfilesSetGlobalPolicyResultEvent != null)
			{
				invocationList = this.OnProfilesSetGlobalPolicyResultEvent.GetInvocationList();
				foreach (Delegate obj457 in invocationList)
				{
					if (obj457.Target == instance)
					{
						OnProfilesSetGlobalPolicyResultEvent -= (PlayFabResultEvent<SetGlobalPolicyResponse>)obj457;
					}
				}
			}
			if (this.OnProfilesSetProfileLanguageRequestEvent != null)
			{
				invocationList = this.OnProfilesSetProfileLanguageRequestEvent.GetInvocationList();
				foreach (Delegate obj458 in invocationList)
				{
					if (obj458.Target == instance)
					{
						OnProfilesSetProfileLanguageRequestEvent -= (PlayFabRequestEvent<SetProfileLanguageRequest>)obj458;
					}
				}
			}
			if (this.OnProfilesSetProfileLanguageResultEvent != null)
			{
				invocationList = this.OnProfilesSetProfileLanguageResultEvent.GetInvocationList();
				foreach (Delegate obj459 in invocationList)
				{
					if (obj459.Target == instance)
					{
						OnProfilesSetProfileLanguageResultEvent -= (PlayFabResultEvent<SetProfileLanguageResponse>)obj459;
					}
				}
			}
			if (this.OnProfilesSetProfilePolicyRequestEvent != null)
			{
				invocationList = this.OnProfilesSetProfilePolicyRequestEvent.GetInvocationList();
				foreach (Delegate obj460 in invocationList)
				{
					if (obj460.Target == instance)
					{
						OnProfilesSetProfilePolicyRequestEvent -= (PlayFabRequestEvent<SetEntityProfilePolicyRequest>)obj460;
					}
				}
			}
			if (this.OnProfilesSetProfilePolicyResultEvent == null)
			{
				return;
			}
			invocationList = this.OnProfilesSetProfilePolicyResultEvent.GetInvocationList();
			foreach (Delegate obj461 in invocationList)
			{
				if (obj461.Target == instance)
				{
					OnProfilesSetProfilePolicyResultEvent -= (PlayFabResultEvent<SetEntityProfilePolicyResponse>)obj461;
				}
			}
		}

		private void OnProcessingErrorEvent(PlayFabRequestCommon request, PlayFabError error)
		{
			if (_instance.OnGlobalErrorEvent != null)
			{
				_instance.OnGlobalErrorEvent(request, error);
			}
		}

		private void OnProcessingEvent(ApiProcessingEventArgs e)
		{
			if (e.EventType == ApiProcessingEventType.Pre)
			{
				Type type = e.Request.GetType();
				if (type == typeof(AcceptTradeRequest) && _instance.OnAcceptTradeRequestEvent != null)
				{
					_instance.OnAcceptTradeRequestEvent((AcceptTradeRequest)e.Request);
				}
				else if (type == typeof(AddFriendRequest) && _instance.OnAddFriendRequestEvent != null)
				{
					_instance.OnAddFriendRequestEvent((AddFriendRequest)e.Request);
				}
				else if (type == typeof(AddGenericIDRequest) && _instance.OnAddGenericIDRequestEvent != null)
				{
					_instance.OnAddGenericIDRequestEvent((AddGenericIDRequest)e.Request);
				}
				else if (type == typeof(AddOrUpdateContactEmailRequest) && _instance.OnAddOrUpdateContactEmailRequestEvent != null)
				{
					_instance.OnAddOrUpdateContactEmailRequestEvent((AddOrUpdateContactEmailRequest)e.Request);
				}
				else if (type == typeof(AddSharedGroupMembersRequest) && _instance.OnAddSharedGroupMembersRequestEvent != null)
				{
					_instance.OnAddSharedGroupMembersRequestEvent((AddSharedGroupMembersRequest)e.Request);
				}
				else if (type == typeof(AddUsernamePasswordRequest) && _instance.OnAddUsernamePasswordRequestEvent != null)
				{
					_instance.OnAddUsernamePasswordRequestEvent((AddUsernamePasswordRequest)e.Request);
				}
				else if (type == typeof(AddUserVirtualCurrencyRequest) && _instance.OnAddUserVirtualCurrencyRequestEvent != null)
				{
					_instance.OnAddUserVirtualCurrencyRequestEvent((AddUserVirtualCurrencyRequest)e.Request);
				}
				else if (type == typeof(AndroidDevicePushNotificationRegistrationRequest) && _instance.OnAndroidDevicePushNotificationRegistrationRequestEvent != null)
				{
					_instance.OnAndroidDevicePushNotificationRegistrationRequestEvent((AndroidDevicePushNotificationRegistrationRequest)e.Request);
				}
				else if (type == typeof(AttributeInstallRequest) && _instance.OnAttributeInstallRequestEvent != null)
				{
					_instance.OnAttributeInstallRequestEvent((AttributeInstallRequest)e.Request);
				}
				else if (type == typeof(CancelTradeRequest) && _instance.OnCancelTradeRequestEvent != null)
				{
					_instance.OnCancelTradeRequestEvent((CancelTradeRequest)e.Request);
				}
				else if (type == typeof(ConfirmPurchaseRequest) && _instance.OnConfirmPurchaseRequestEvent != null)
				{
					_instance.OnConfirmPurchaseRequestEvent((ConfirmPurchaseRequest)e.Request);
				}
				else if (type == typeof(ConsumeItemRequest) && _instance.OnConsumeItemRequestEvent != null)
				{
					_instance.OnConsumeItemRequestEvent((ConsumeItemRequest)e.Request);
				}
				else if (type == typeof(ConsumePSNEntitlementsRequest) && _instance.OnConsumePSNEntitlementsRequestEvent != null)
				{
					_instance.OnConsumePSNEntitlementsRequestEvent((ConsumePSNEntitlementsRequest)e.Request);
				}
				else if (type == typeof(ConsumeXboxEntitlementsRequest) && _instance.OnConsumeXboxEntitlementsRequestEvent != null)
				{
					_instance.OnConsumeXboxEntitlementsRequestEvent((ConsumeXboxEntitlementsRequest)e.Request);
				}
				else if (type == typeof(CreateSharedGroupRequest) && _instance.OnCreateSharedGroupRequestEvent != null)
				{
					_instance.OnCreateSharedGroupRequestEvent((CreateSharedGroupRequest)e.Request);
				}
				else if (type == typeof(ExecuteCloudScriptRequest) && _instance.OnExecuteCloudScriptRequestEvent != null)
				{
					_instance.OnExecuteCloudScriptRequestEvent((ExecuteCloudScriptRequest)e.Request);
				}
				else if (type == typeof(GetAccountInfoRequest) && _instance.OnGetAccountInfoRequestEvent != null)
				{
					_instance.OnGetAccountInfoRequestEvent((GetAccountInfoRequest)e.Request);
				}
				else if (type == typeof(ListUsersCharactersRequest) && _instance.OnGetAllUsersCharactersRequestEvent != null)
				{
					_instance.OnGetAllUsersCharactersRequestEvent((ListUsersCharactersRequest)e.Request);
				}
				else if (type == typeof(GetCatalogItemsRequest) && _instance.OnGetCatalogItemsRequestEvent != null)
				{
					_instance.OnGetCatalogItemsRequestEvent((GetCatalogItemsRequest)e.Request);
				}
				else if (type == typeof(GetCharacterDataRequest) && _instance.OnGetCharacterDataRequestEvent != null)
				{
					_instance.OnGetCharacterDataRequestEvent((GetCharacterDataRequest)e.Request);
				}
				else if (type == typeof(GetCharacterInventoryRequest) && _instance.OnGetCharacterInventoryRequestEvent != null)
				{
					_instance.OnGetCharacterInventoryRequestEvent((GetCharacterInventoryRequest)e.Request);
				}
				else if (type == typeof(GetCharacterLeaderboardRequest) && _instance.OnGetCharacterLeaderboardRequestEvent != null)
				{
					_instance.OnGetCharacterLeaderboardRequestEvent((GetCharacterLeaderboardRequest)e.Request);
				}
				else if (type == typeof(GetCharacterDataRequest) && _instance.OnGetCharacterReadOnlyDataRequestEvent != null)
				{
					_instance.OnGetCharacterReadOnlyDataRequestEvent((GetCharacterDataRequest)e.Request);
				}
				else if (type == typeof(GetCharacterStatisticsRequest) && _instance.OnGetCharacterStatisticsRequestEvent != null)
				{
					_instance.OnGetCharacterStatisticsRequestEvent((GetCharacterStatisticsRequest)e.Request);
				}
				else if (type == typeof(GetContentDownloadUrlRequest) && _instance.OnGetContentDownloadUrlRequestEvent != null)
				{
					_instance.OnGetContentDownloadUrlRequestEvent((GetContentDownloadUrlRequest)e.Request);
				}
				else if (type == typeof(CurrentGamesRequest) && _instance.OnGetCurrentGamesRequestEvent != null)
				{
					_instance.OnGetCurrentGamesRequestEvent((CurrentGamesRequest)e.Request);
				}
				else if (type == typeof(GetFriendLeaderboardRequest) && _instance.OnGetFriendLeaderboardRequestEvent != null)
				{
					_instance.OnGetFriendLeaderboardRequestEvent((GetFriendLeaderboardRequest)e.Request);
				}
				else if (type == typeof(GetFriendLeaderboardAroundPlayerRequest) && _instance.OnGetFriendLeaderboardAroundPlayerRequestEvent != null)
				{
					_instance.OnGetFriendLeaderboardAroundPlayerRequestEvent((GetFriendLeaderboardAroundPlayerRequest)e.Request);
				}
				else if (type == typeof(GetFriendsListRequest) && _instance.OnGetFriendsListRequestEvent != null)
				{
					_instance.OnGetFriendsListRequestEvent((GetFriendsListRequest)e.Request);
				}
				else if (type == typeof(GameServerRegionsRequest) && _instance.OnGetGameServerRegionsRequestEvent != null)
				{
					_instance.OnGetGameServerRegionsRequestEvent((GameServerRegionsRequest)e.Request);
				}
				else if (type == typeof(GetLeaderboardRequest) && _instance.OnGetLeaderboardRequestEvent != null)
				{
					_instance.OnGetLeaderboardRequestEvent((GetLeaderboardRequest)e.Request);
				}
				else if (type == typeof(GetLeaderboardAroundCharacterRequest) && _instance.OnGetLeaderboardAroundCharacterRequestEvent != null)
				{
					_instance.OnGetLeaderboardAroundCharacterRequestEvent((GetLeaderboardAroundCharacterRequest)e.Request);
				}
				else if (type == typeof(GetLeaderboardAroundPlayerRequest) && _instance.OnGetLeaderboardAroundPlayerRequestEvent != null)
				{
					_instance.OnGetLeaderboardAroundPlayerRequestEvent((GetLeaderboardAroundPlayerRequest)e.Request);
				}
				else if (type == typeof(GetLeaderboardForUsersCharactersRequest) && _instance.OnGetLeaderboardForUserCharactersRequestEvent != null)
				{
					_instance.OnGetLeaderboardForUserCharactersRequestEvent((GetLeaderboardForUsersCharactersRequest)e.Request);
				}
				else if (type == typeof(GetPaymentTokenRequest) && _instance.OnGetPaymentTokenRequestEvent != null)
				{
					_instance.OnGetPaymentTokenRequestEvent((GetPaymentTokenRequest)e.Request);
				}
				else if (type == typeof(GetPhotonAuthenticationTokenRequest) && _instance.OnGetPhotonAuthenticationTokenRequestEvent != null)
				{
					_instance.OnGetPhotonAuthenticationTokenRequestEvent((GetPhotonAuthenticationTokenRequest)e.Request);
				}
				else if (type == typeof(GetPlayerCombinedInfoRequest) && _instance.OnGetPlayerCombinedInfoRequestEvent != null)
				{
					_instance.OnGetPlayerCombinedInfoRequestEvent((GetPlayerCombinedInfoRequest)e.Request);
				}
				else if (type == typeof(GetPlayerProfileRequest) && _instance.OnGetPlayerProfileRequestEvent != null)
				{
					_instance.OnGetPlayerProfileRequestEvent((GetPlayerProfileRequest)e.Request);
				}
				else if (type == typeof(GetPlayerSegmentsRequest) && _instance.OnGetPlayerSegmentsRequestEvent != null)
				{
					_instance.OnGetPlayerSegmentsRequestEvent((GetPlayerSegmentsRequest)e.Request);
				}
				else if (type == typeof(GetPlayerStatisticsRequest) && _instance.OnGetPlayerStatisticsRequestEvent != null)
				{
					_instance.OnGetPlayerStatisticsRequestEvent((GetPlayerStatisticsRequest)e.Request);
				}
				else if (type == typeof(GetPlayerStatisticVersionsRequest) && _instance.OnGetPlayerStatisticVersionsRequestEvent != null)
				{
					_instance.OnGetPlayerStatisticVersionsRequestEvent((GetPlayerStatisticVersionsRequest)e.Request);
				}
				else if (type == typeof(GetPlayerTagsRequest) && _instance.OnGetPlayerTagsRequestEvent != null)
				{
					_instance.OnGetPlayerTagsRequestEvent((GetPlayerTagsRequest)e.Request);
				}
				else if (type == typeof(GetPlayerTradesRequest) && _instance.OnGetPlayerTradesRequestEvent != null)
				{
					_instance.OnGetPlayerTradesRequestEvent((GetPlayerTradesRequest)e.Request);
				}
				else if (type == typeof(GetPlayFabIDsFromFacebookIDsRequest) && _instance.OnGetPlayFabIDsFromFacebookIDsRequestEvent != null)
				{
					_instance.OnGetPlayFabIDsFromFacebookIDsRequestEvent((GetPlayFabIDsFromFacebookIDsRequest)e.Request);
				}
				else if (type == typeof(GetPlayFabIDsFromFacebookInstantGamesIdsRequest) && _instance.OnGetPlayFabIDsFromFacebookInstantGamesIdsRequestEvent != null)
				{
					_instance.OnGetPlayFabIDsFromFacebookInstantGamesIdsRequestEvent((GetPlayFabIDsFromFacebookInstantGamesIdsRequest)e.Request);
				}
				else if (type == typeof(GetPlayFabIDsFromGameCenterIDsRequest) && _instance.OnGetPlayFabIDsFromGameCenterIDsRequestEvent != null)
				{
					_instance.OnGetPlayFabIDsFromGameCenterIDsRequestEvent((GetPlayFabIDsFromGameCenterIDsRequest)e.Request);
				}
				else if (type == typeof(GetPlayFabIDsFromGenericIDsRequest) && _instance.OnGetPlayFabIDsFromGenericIDsRequestEvent != null)
				{
					_instance.OnGetPlayFabIDsFromGenericIDsRequestEvent((GetPlayFabIDsFromGenericIDsRequest)e.Request);
				}
				else if (type == typeof(GetPlayFabIDsFromGoogleIDsRequest) && _instance.OnGetPlayFabIDsFromGoogleIDsRequestEvent != null)
				{
					_instance.OnGetPlayFabIDsFromGoogleIDsRequestEvent((GetPlayFabIDsFromGoogleIDsRequest)e.Request);
				}
				else if (type == typeof(GetPlayFabIDsFromKongregateIDsRequest) && _instance.OnGetPlayFabIDsFromKongregateIDsRequestEvent != null)
				{
					_instance.OnGetPlayFabIDsFromKongregateIDsRequestEvent((GetPlayFabIDsFromKongregateIDsRequest)e.Request);
				}
				else if (type == typeof(GetPlayFabIDsFromNintendoSwitchDeviceIdsRequest) && _instance.OnGetPlayFabIDsFromNintendoSwitchDeviceIdsRequestEvent != null)
				{
					_instance.OnGetPlayFabIDsFromNintendoSwitchDeviceIdsRequestEvent((GetPlayFabIDsFromNintendoSwitchDeviceIdsRequest)e.Request);
				}
				else if (type == typeof(GetPlayFabIDsFromPSNAccountIDsRequest) && _instance.OnGetPlayFabIDsFromPSNAccountIDsRequestEvent != null)
				{
					_instance.OnGetPlayFabIDsFromPSNAccountIDsRequestEvent((GetPlayFabIDsFromPSNAccountIDsRequest)e.Request);
				}
				else if (type == typeof(GetPlayFabIDsFromSteamIDsRequest) && _instance.OnGetPlayFabIDsFromSteamIDsRequestEvent != null)
				{
					_instance.OnGetPlayFabIDsFromSteamIDsRequestEvent((GetPlayFabIDsFromSteamIDsRequest)e.Request);
				}
				else if (type == typeof(GetPlayFabIDsFromTwitchIDsRequest) && _instance.OnGetPlayFabIDsFromTwitchIDsRequestEvent != null)
				{
					_instance.OnGetPlayFabIDsFromTwitchIDsRequestEvent((GetPlayFabIDsFromTwitchIDsRequest)e.Request);
				}
				else if (type == typeof(GetPlayFabIDsFromXboxLiveIDsRequest) && _instance.OnGetPlayFabIDsFromXboxLiveIDsRequestEvent != null)
				{
					_instance.OnGetPlayFabIDsFromXboxLiveIDsRequestEvent((GetPlayFabIDsFromXboxLiveIDsRequest)e.Request);
				}
				else if (type == typeof(GetPublisherDataRequest) && _instance.OnGetPublisherDataRequestEvent != null)
				{
					_instance.OnGetPublisherDataRequestEvent((GetPublisherDataRequest)e.Request);
				}
				else if (type == typeof(GetPurchaseRequest) && _instance.OnGetPurchaseRequestEvent != null)
				{
					_instance.OnGetPurchaseRequestEvent((GetPurchaseRequest)e.Request);
				}
				else if (type == typeof(GetSharedGroupDataRequest) && _instance.OnGetSharedGroupDataRequestEvent != null)
				{
					_instance.OnGetSharedGroupDataRequestEvent((GetSharedGroupDataRequest)e.Request);
				}
				else if (type == typeof(GetStoreItemsRequest) && _instance.OnGetStoreItemsRequestEvent != null)
				{
					_instance.OnGetStoreItemsRequestEvent((GetStoreItemsRequest)e.Request);
				}
				else if (type == typeof(GetTimeRequest) && _instance.OnGetTimeRequestEvent != null)
				{
					_instance.OnGetTimeRequestEvent((GetTimeRequest)e.Request);
				}
				else if (type == typeof(GetTitleDataRequest) && _instance.OnGetTitleDataRequestEvent != null)
				{
					_instance.OnGetTitleDataRequestEvent((GetTitleDataRequest)e.Request);
				}
				else if (type == typeof(GetTitleNewsRequest) && _instance.OnGetTitleNewsRequestEvent != null)
				{
					_instance.OnGetTitleNewsRequestEvent((GetTitleNewsRequest)e.Request);
				}
				else if (type == typeof(GetTitlePublicKeyRequest) && _instance.OnGetTitlePublicKeyRequestEvent != null)
				{
					_instance.OnGetTitlePublicKeyRequestEvent((GetTitlePublicKeyRequest)e.Request);
				}
				else if (type == typeof(GetTradeStatusRequest) && _instance.OnGetTradeStatusRequestEvent != null)
				{
					_instance.OnGetTradeStatusRequestEvent((GetTradeStatusRequest)e.Request);
				}
				else if (type == typeof(GetUserDataRequest) && _instance.OnGetUserDataRequestEvent != null)
				{
					_instance.OnGetUserDataRequestEvent((GetUserDataRequest)e.Request);
				}
				else if (type == typeof(GetUserInventoryRequest) && _instance.OnGetUserInventoryRequestEvent != null)
				{
					_instance.OnGetUserInventoryRequestEvent((GetUserInventoryRequest)e.Request);
				}
				else if (type == typeof(GetUserDataRequest) && _instance.OnGetUserPublisherDataRequestEvent != null)
				{
					_instance.OnGetUserPublisherDataRequestEvent((GetUserDataRequest)e.Request);
				}
				else if (type == typeof(GetUserDataRequest) && _instance.OnGetUserPublisherReadOnlyDataRequestEvent != null)
				{
					_instance.OnGetUserPublisherReadOnlyDataRequestEvent((GetUserDataRequest)e.Request);
				}
				else if (type == typeof(GetUserDataRequest) && _instance.OnGetUserReadOnlyDataRequestEvent != null)
				{
					_instance.OnGetUserReadOnlyDataRequestEvent((GetUserDataRequest)e.Request);
				}
				else if (type == typeof(GetWindowsHelloChallengeRequest) && _instance.OnGetWindowsHelloChallengeRequestEvent != null)
				{
					_instance.OnGetWindowsHelloChallengeRequestEvent((GetWindowsHelloChallengeRequest)e.Request);
				}
				else if (type == typeof(GrantCharacterToUserRequest) && _instance.OnGrantCharacterToUserRequestEvent != null)
				{
					_instance.OnGrantCharacterToUserRequestEvent((GrantCharacterToUserRequest)e.Request);
				}
				else if (type == typeof(LinkAndroidDeviceIDRequest) && _instance.OnLinkAndroidDeviceIDRequestEvent != null)
				{
					_instance.OnLinkAndroidDeviceIDRequestEvent((LinkAndroidDeviceIDRequest)e.Request);
				}
				else if (type == typeof(LinkCustomIDRequest) && _instance.OnLinkCustomIDRequestEvent != null)
				{
					_instance.OnLinkCustomIDRequestEvent((LinkCustomIDRequest)e.Request);
				}
				else if (type == typeof(LinkFacebookAccountRequest) && _instance.OnLinkFacebookAccountRequestEvent != null)
				{
					_instance.OnLinkFacebookAccountRequestEvent((LinkFacebookAccountRequest)e.Request);
				}
				else if (type == typeof(LinkFacebookInstantGamesIdRequest) && _instance.OnLinkFacebookInstantGamesIdRequestEvent != null)
				{
					_instance.OnLinkFacebookInstantGamesIdRequestEvent((LinkFacebookInstantGamesIdRequest)e.Request);
				}
				else if (type == typeof(LinkGameCenterAccountRequest) && _instance.OnLinkGameCenterAccountRequestEvent != null)
				{
					_instance.OnLinkGameCenterAccountRequestEvent((LinkGameCenterAccountRequest)e.Request);
				}
				else if (type == typeof(LinkGoogleAccountRequest) && _instance.OnLinkGoogleAccountRequestEvent != null)
				{
					_instance.OnLinkGoogleAccountRequestEvent((LinkGoogleAccountRequest)e.Request);
				}
				else if (type == typeof(LinkIOSDeviceIDRequest) && _instance.OnLinkIOSDeviceIDRequestEvent != null)
				{
					_instance.OnLinkIOSDeviceIDRequestEvent((LinkIOSDeviceIDRequest)e.Request);
				}
				else if (type == typeof(LinkKongregateAccountRequest) && _instance.OnLinkKongregateRequestEvent != null)
				{
					_instance.OnLinkKongregateRequestEvent((LinkKongregateAccountRequest)e.Request);
				}
				else if (type == typeof(LinkNintendoSwitchDeviceIdRequest) && _instance.OnLinkNintendoSwitchDeviceIdRequestEvent != null)
				{
					_instance.OnLinkNintendoSwitchDeviceIdRequestEvent((LinkNintendoSwitchDeviceIdRequest)e.Request);
				}
				else if (type == typeof(LinkOpenIdConnectRequest) && _instance.OnLinkOpenIdConnectRequestEvent != null)
				{
					_instance.OnLinkOpenIdConnectRequestEvent((LinkOpenIdConnectRequest)e.Request);
				}
				else if (type == typeof(LinkPSNAccountRequest) && _instance.OnLinkPSNAccountRequestEvent != null)
				{
					_instance.OnLinkPSNAccountRequestEvent((LinkPSNAccountRequest)e.Request);
				}
				else if (type == typeof(LinkSteamAccountRequest) && _instance.OnLinkSteamAccountRequestEvent != null)
				{
					_instance.OnLinkSteamAccountRequestEvent((LinkSteamAccountRequest)e.Request);
				}
				else if (type == typeof(LinkTwitchAccountRequest) && _instance.OnLinkTwitchRequestEvent != null)
				{
					_instance.OnLinkTwitchRequestEvent((LinkTwitchAccountRequest)e.Request);
				}
				else if (type == typeof(LinkWindowsHelloAccountRequest) && _instance.OnLinkWindowsHelloRequestEvent != null)
				{
					_instance.OnLinkWindowsHelloRequestEvent((LinkWindowsHelloAccountRequest)e.Request);
				}
				else if (type == typeof(LinkXboxAccountRequest) && _instance.OnLinkXboxAccountRequestEvent != null)
				{
					_instance.OnLinkXboxAccountRequestEvent((LinkXboxAccountRequest)e.Request);
				}
				else if (type == typeof(LoginWithAndroidDeviceIDRequest) && _instance.OnLoginWithAndroidDeviceIDRequestEvent != null)
				{
					_instance.OnLoginWithAndroidDeviceIDRequestEvent((LoginWithAndroidDeviceIDRequest)e.Request);
				}
				else if (type == typeof(LoginWithCustomIDRequest) && _instance.OnLoginWithCustomIDRequestEvent != null)
				{
					_instance.OnLoginWithCustomIDRequestEvent((LoginWithCustomIDRequest)e.Request);
				}
				else if (type == typeof(LoginWithEmailAddressRequest) && _instance.OnLoginWithEmailAddressRequestEvent != null)
				{
					_instance.OnLoginWithEmailAddressRequestEvent((LoginWithEmailAddressRequest)e.Request);
				}
				else if (type == typeof(LoginWithFacebookRequest) && _instance.OnLoginWithFacebookRequestEvent != null)
				{
					_instance.OnLoginWithFacebookRequestEvent((LoginWithFacebookRequest)e.Request);
				}
				else if (type == typeof(LoginWithFacebookInstantGamesIdRequest) && _instance.OnLoginWithFacebookInstantGamesIdRequestEvent != null)
				{
					_instance.OnLoginWithFacebookInstantGamesIdRequestEvent((LoginWithFacebookInstantGamesIdRequest)e.Request);
				}
				else if (type == typeof(LoginWithGameCenterRequest) && _instance.OnLoginWithGameCenterRequestEvent != null)
				{
					_instance.OnLoginWithGameCenterRequestEvent((LoginWithGameCenterRequest)e.Request);
				}
				else if (type == typeof(LoginWithGoogleAccountRequest) && _instance.OnLoginWithGoogleAccountRequestEvent != null)
				{
					_instance.OnLoginWithGoogleAccountRequestEvent((LoginWithGoogleAccountRequest)e.Request);
				}
				else if (type == typeof(LoginWithIOSDeviceIDRequest) && _instance.OnLoginWithIOSDeviceIDRequestEvent != null)
				{
					_instance.OnLoginWithIOSDeviceIDRequestEvent((LoginWithIOSDeviceIDRequest)e.Request);
				}
				else if (type == typeof(LoginWithKongregateRequest) && _instance.OnLoginWithKongregateRequestEvent != null)
				{
					_instance.OnLoginWithKongregateRequestEvent((LoginWithKongregateRequest)e.Request);
				}
				else if (type == typeof(LoginWithNintendoSwitchDeviceIdRequest) && _instance.OnLoginWithNintendoSwitchDeviceIdRequestEvent != null)
				{
					_instance.OnLoginWithNintendoSwitchDeviceIdRequestEvent((LoginWithNintendoSwitchDeviceIdRequest)e.Request);
				}
				else if (type == typeof(LoginWithOpenIdConnectRequest) && _instance.OnLoginWithOpenIdConnectRequestEvent != null)
				{
					_instance.OnLoginWithOpenIdConnectRequestEvent((LoginWithOpenIdConnectRequest)e.Request);
				}
				else if (type == typeof(LoginWithPlayFabRequest) && _instance.OnLoginWithPlayFabRequestEvent != null)
				{
					_instance.OnLoginWithPlayFabRequestEvent((LoginWithPlayFabRequest)e.Request);
				}
				else if (type == typeof(LoginWithPSNRequest) && _instance.OnLoginWithPSNRequestEvent != null)
				{
					_instance.OnLoginWithPSNRequestEvent((LoginWithPSNRequest)e.Request);
				}
				else if (type == typeof(LoginWithSteamRequest) && _instance.OnLoginWithSteamRequestEvent != null)
				{
					_instance.OnLoginWithSteamRequestEvent((LoginWithSteamRequest)e.Request);
				}
				else if (type == typeof(LoginWithTwitchRequest) && _instance.OnLoginWithTwitchRequestEvent != null)
				{
					_instance.OnLoginWithTwitchRequestEvent((LoginWithTwitchRequest)e.Request);
				}
				else if (type == typeof(LoginWithWindowsHelloRequest) && _instance.OnLoginWithWindowsHelloRequestEvent != null)
				{
					_instance.OnLoginWithWindowsHelloRequestEvent((LoginWithWindowsHelloRequest)e.Request);
				}
				else if (type == typeof(LoginWithXboxRequest) && _instance.OnLoginWithXboxRequestEvent != null)
				{
					_instance.OnLoginWithXboxRequestEvent((LoginWithXboxRequest)e.Request);
				}
				else if (type == typeof(MatchmakeRequest) && _instance.OnMatchmakeRequestEvent != null)
				{
					_instance.OnMatchmakeRequestEvent((MatchmakeRequest)e.Request);
				}
				else if (type == typeof(OpenTradeRequest) && _instance.OnOpenTradeRequestEvent != null)
				{
					_instance.OnOpenTradeRequestEvent((OpenTradeRequest)e.Request);
				}
				else if (type == typeof(PayForPurchaseRequest) && _instance.OnPayForPurchaseRequestEvent != null)
				{
					_instance.OnPayForPurchaseRequestEvent((PayForPurchaseRequest)e.Request);
				}
				else if (type == typeof(PurchaseItemRequest) && _instance.OnPurchaseItemRequestEvent != null)
				{
					_instance.OnPurchaseItemRequestEvent((PurchaseItemRequest)e.Request);
				}
				else if (type == typeof(RedeemCouponRequest) && _instance.OnRedeemCouponRequestEvent != null)
				{
					_instance.OnRedeemCouponRequestEvent((RedeemCouponRequest)e.Request);
				}
				else if (type == typeof(RefreshPSNAuthTokenRequest) && _instance.OnRefreshPSNAuthTokenRequestEvent != null)
				{
					_instance.OnRefreshPSNAuthTokenRequestEvent((RefreshPSNAuthTokenRequest)e.Request);
				}
				else if (type == typeof(RegisterForIOSPushNotificationRequest) && _instance.OnRegisterForIOSPushNotificationRequestEvent != null)
				{
					_instance.OnRegisterForIOSPushNotificationRequestEvent((RegisterForIOSPushNotificationRequest)e.Request);
				}
				else if (type == typeof(RegisterPlayFabUserRequest) && _instance.OnRegisterPlayFabUserRequestEvent != null)
				{
					_instance.OnRegisterPlayFabUserRequestEvent((RegisterPlayFabUserRequest)e.Request);
				}
				else if (type == typeof(RegisterWithWindowsHelloRequest) && _instance.OnRegisterWithWindowsHelloRequestEvent != null)
				{
					_instance.OnRegisterWithWindowsHelloRequestEvent((RegisterWithWindowsHelloRequest)e.Request);
				}
				else if (type == typeof(RemoveContactEmailRequest) && _instance.OnRemoveContactEmailRequestEvent != null)
				{
					_instance.OnRemoveContactEmailRequestEvent((RemoveContactEmailRequest)e.Request);
				}
				else if (type == typeof(RemoveFriendRequest) && _instance.OnRemoveFriendRequestEvent != null)
				{
					_instance.OnRemoveFriendRequestEvent((RemoveFriendRequest)e.Request);
				}
				else if (type == typeof(RemoveGenericIDRequest) && _instance.OnRemoveGenericIDRequestEvent != null)
				{
					_instance.OnRemoveGenericIDRequestEvent((RemoveGenericIDRequest)e.Request);
				}
				else if (type == typeof(RemoveSharedGroupMembersRequest) && _instance.OnRemoveSharedGroupMembersRequestEvent != null)
				{
					_instance.OnRemoveSharedGroupMembersRequestEvent((RemoveSharedGroupMembersRequest)e.Request);
				}
				else if (type == typeof(DeviceInfoRequest) && _instance.OnReportDeviceInfoRequestEvent != null)
				{
					_instance.OnReportDeviceInfoRequestEvent((DeviceInfoRequest)e.Request);
				}
				else if (type == typeof(ReportPlayerClientRequest) && _instance.OnReportPlayerRequestEvent != null)
				{
					_instance.OnReportPlayerRequestEvent((ReportPlayerClientRequest)e.Request);
				}
				else if (type == typeof(RestoreIOSPurchasesRequest) && _instance.OnRestoreIOSPurchasesRequestEvent != null)
				{
					_instance.OnRestoreIOSPurchasesRequestEvent((RestoreIOSPurchasesRequest)e.Request);
				}
				else if (type == typeof(SendAccountRecoveryEmailRequest) && _instance.OnSendAccountRecoveryEmailRequestEvent != null)
				{
					_instance.OnSendAccountRecoveryEmailRequestEvent((SendAccountRecoveryEmailRequest)e.Request);
				}
				else if (type == typeof(SetFriendTagsRequest) && _instance.OnSetFriendTagsRequestEvent != null)
				{
					_instance.OnSetFriendTagsRequestEvent((SetFriendTagsRequest)e.Request);
				}
				else if (type == typeof(SetPlayerSecretRequest) && _instance.OnSetPlayerSecretRequestEvent != null)
				{
					_instance.OnSetPlayerSecretRequestEvent((SetPlayerSecretRequest)e.Request);
				}
				else if (type == typeof(StartGameRequest) && _instance.OnStartGameRequestEvent != null)
				{
					_instance.OnStartGameRequestEvent((StartGameRequest)e.Request);
				}
				else if (type == typeof(StartPurchaseRequest) && _instance.OnStartPurchaseRequestEvent != null)
				{
					_instance.OnStartPurchaseRequestEvent((StartPurchaseRequest)e.Request);
				}
				else if (type == typeof(SubtractUserVirtualCurrencyRequest) && _instance.OnSubtractUserVirtualCurrencyRequestEvent != null)
				{
					_instance.OnSubtractUserVirtualCurrencyRequestEvent((SubtractUserVirtualCurrencyRequest)e.Request);
				}
				else if (type == typeof(UnlinkAndroidDeviceIDRequest) && _instance.OnUnlinkAndroidDeviceIDRequestEvent != null)
				{
					_instance.OnUnlinkAndroidDeviceIDRequestEvent((UnlinkAndroidDeviceIDRequest)e.Request);
				}
				else if (type == typeof(UnlinkCustomIDRequest) && _instance.OnUnlinkCustomIDRequestEvent != null)
				{
					_instance.OnUnlinkCustomIDRequestEvent((UnlinkCustomIDRequest)e.Request);
				}
				else if (type == typeof(UnlinkFacebookAccountRequest) && _instance.OnUnlinkFacebookAccountRequestEvent != null)
				{
					_instance.OnUnlinkFacebookAccountRequestEvent((UnlinkFacebookAccountRequest)e.Request);
				}
				else if (type == typeof(UnlinkFacebookInstantGamesIdRequest) && _instance.OnUnlinkFacebookInstantGamesIdRequestEvent != null)
				{
					_instance.OnUnlinkFacebookInstantGamesIdRequestEvent((UnlinkFacebookInstantGamesIdRequest)e.Request);
				}
				else if (type == typeof(UnlinkGameCenterAccountRequest) && _instance.OnUnlinkGameCenterAccountRequestEvent != null)
				{
					_instance.OnUnlinkGameCenterAccountRequestEvent((UnlinkGameCenterAccountRequest)e.Request);
				}
				else if (type == typeof(UnlinkGoogleAccountRequest) && _instance.OnUnlinkGoogleAccountRequestEvent != null)
				{
					_instance.OnUnlinkGoogleAccountRequestEvent((UnlinkGoogleAccountRequest)e.Request);
				}
				else if (type == typeof(UnlinkIOSDeviceIDRequest) && _instance.OnUnlinkIOSDeviceIDRequestEvent != null)
				{
					_instance.OnUnlinkIOSDeviceIDRequestEvent((UnlinkIOSDeviceIDRequest)e.Request);
				}
				else if (type == typeof(UnlinkKongregateAccountRequest) && _instance.OnUnlinkKongregateRequestEvent != null)
				{
					_instance.OnUnlinkKongregateRequestEvent((UnlinkKongregateAccountRequest)e.Request);
				}
				else if (type == typeof(UnlinkNintendoSwitchDeviceIdRequest) && _instance.OnUnlinkNintendoSwitchDeviceIdRequestEvent != null)
				{
					_instance.OnUnlinkNintendoSwitchDeviceIdRequestEvent((UnlinkNintendoSwitchDeviceIdRequest)e.Request);
				}
				else if (type == typeof(UninkOpenIdConnectRequest) && _instance.OnUnlinkOpenIdConnectRequestEvent != null)
				{
					_instance.OnUnlinkOpenIdConnectRequestEvent((UninkOpenIdConnectRequest)e.Request);
				}
				else if (type == typeof(UnlinkPSNAccountRequest) && _instance.OnUnlinkPSNAccountRequestEvent != null)
				{
					_instance.OnUnlinkPSNAccountRequestEvent((UnlinkPSNAccountRequest)e.Request);
				}
				else if (type == typeof(UnlinkSteamAccountRequest) && _instance.OnUnlinkSteamAccountRequestEvent != null)
				{
					_instance.OnUnlinkSteamAccountRequestEvent((UnlinkSteamAccountRequest)e.Request);
				}
				else if (type == typeof(UnlinkTwitchAccountRequest) && _instance.OnUnlinkTwitchRequestEvent != null)
				{
					_instance.OnUnlinkTwitchRequestEvent((UnlinkTwitchAccountRequest)e.Request);
				}
				else if (type == typeof(UnlinkWindowsHelloAccountRequest) && _instance.OnUnlinkWindowsHelloRequestEvent != null)
				{
					_instance.OnUnlinkWindowsHelloRequestEvent((UnlinkWindowsHelloAccountRequest)e.Request);
				}
				else if (type == typeof(UnlinkXboxAccountRequest) && _instance.OnUnlinkXboxAccountRequestEvent != null)
				{
					_instance.OnUnlinkXboxAccountRequestEvent((UnlinkXboxAccountRequest)e.Request);
				}
				else if (type == typeof(UnlockContainerInstanceRequest) && _instance.OnUnlockContainerInstanceRequestEvent != null)
				{
					_instance.OnUnlockContainerInstanceRequestEvent((UnlockContainerInstanceRequest)e.Request);
				}
				else if (type == typeof(UnlockContainerItemRequest) && _instance.OnUnlockContainerItemRequestEvent != null)
				{
					_instance.OnUnlockContainerItemRequestEvent((UnlockContainerItemRequest)e.Request);
				}
				else if (type == typeof(UpdateAvatarUrlRequest) && _instance.OnUpdateAvatarUrlRequestEvent != null)
				{
					_instance.OnUpdateAvatarUrlRequestEvent((UpdateAvatarUrlRequest)e.Request);
				}
				else if (type == typeof(UpdateCharacterDataRequest) && _instance.OnUpdateCharacterDataRequestEvent != null)
				{
					_instance.OnUpdateCharacterDataRequestEvent((UpdateCharacterDataRequest)e.Request);
				}
				else if (type == typeof(UpdateCharacterStatisticsRequest) && _instance.OnUpdateCharacterStatisticsRequestEvent != null)
				{
					_instance.OnUpdateCharacterStatisticsRequestEvent((UpdateCharacterStatisticsRequest)e.Request);
				}
				else if (type == typeof(UpdatePlayerStatisticsRequest) && _instance.OnUpdatePlayerStatisticsRequestEvent != null)
				{
					_instance.OnUpdatePlayerStatisticsRequestEvent((UpdatePlayerStatisticsRequest)e.Request);
				}
				else if (type == typeof(UpdateSharedGroupDataRequest) && _instance.OnUpdateSharedGroupDataRequestEvent != null)
				{
					_instance.OnUpdateSharedGroupDataRequestEvent((UpdateSharedGroupDataRequest)e.Request);
				}
				else if (type == typeof(UpdateUserDataRequest) && _instance.OnUpdateUserDataRequestEvent != null)
				{
					_instance.OnUpdateUserDataRequestEvent((UpdateUserDataRequest)e.Request);
				}
				else if (type == typeof(UpdateUserDataRequest) && _instance.OnUpdateUserPublisherDataRequestEvent != null)
				{
					_instance.OnUpdateUserPublisherDataRequestEvent((UpdateUserDataRequest)e.Request);
				}
				else if (type == typeof(UpdateUserTitleDisplayNameRequest) && _instance.OnUpdateUserTitleDisplayNameRequestEvent != null)
				{
					_instance.OnUpdateUserTitleDisplayNameRequestEvent((UpdateUserTitleDisplayNameRequest)e.Request);
				}
				else if (type == typeof(ValidateAmazonReceiptRequest) && _instance.OnValidateAmazonIAPReceiptRequestEvent != null)
				{
					_instance.OnValidateAmazonIAPReceiptRequestEvent((ValidateAmazonReceiptRequest)e.Request);
				}
				else if (type == typeof(ValidateGooglePlayPurchaseRequest) && _instance.OnValidateGooglePlayPurchaseRequestEvent != null)
				{
					_instance.OnValidateGooglePlayPurchaseRequestEvent((ValidateGooglePlayPurchaseRequest)e.Request);
				}
				else if (type == typeof(ValidateIOSReceiptRequest) && _instance.OnValidateIOSReceiptRequestEvent != null)
				{
					_instance.OnValidateIOSReceiptRequestEvent((ValidateIOSReceiptRequest)e.Request);
				}
				else if (type == typeof(ValidateWindowsReceiptRequest) && _instance.OnValidateWindowsStoreReceiptRequestEvent != null)
				{
					_instance.OnValidateWindowsStoreReceiptRequestEvent((ValidateWindowsReceiptRequest)e.Request);
				}
				else if (type == typeof(WriteClientCharacterEventRequest) && _instance.OnWriteCharacterEventRequestEvent != null)
				{
					_instance.OnWriteCharacterEventRequestEvent((WriteClientCharacterEventRequest)e.Request);
				}
				else if (type == typeof(WriteClientPlayerEventRequest) && _instance.OnWritePlayerEventRequestEvent != null)
				{
					_instance.OnWritePlayerEventRequestEvent((WriteClientPlayerEventRequest)e.Request);
				}
				else if (type == typeof(WriteTitleEventRequest) && _instance.OnWriteTitleEventRequestEvent != null)
				{
					_instance.OnWriteTitleEventRequestEvent((WriteTitleEventRequest)e.Request);
				}
				else if (type == typeof(GetEntityTokenRequest) && _instance.OnAuthenticationGetEntityTokenRequestEvent != null)
				{
					_instance.OnAuthenticationGetEntityTokenRequestEvent((GetEntityTokenRequest)e.Request);
				}
				else if (type == typeof(ExecuteEntityCloudScriptRequest) && _instance.OnCloudScriptExecuteEntityCloudScriptRequestEvent != null)
				{
					_instance.OnCloudScriptExecuteEntityCloudScriptRequestEvent((ExecuteEntityCloudScriptRequest)e.Request);
				}
				else if (type == typeof(AbortFileUploadsRequest) && _instance.OnDataAbortFileUploadsRequestEvent != null)
				{
					_instance.OnDataAbortFileUploadsRequestEvent((AbortFileUploadsRequest)e.Request);
				}
				else if (type == typeof(DeleteFilesRequest) && _instance.OnDataDeleteFilesRequestEvent != null)
				{
					_instance.OnDataDeleteFilesRequestEvent((DeleteFilesRequest)e.Request);
				}
				else if (type == typeof(FinalizeFileUploadsRequest) && _instance.OnDataFinalizeFileUploadsRequestEvent != null)
				{
					_instance.OnDataFinalizeFileUploadsRequestEvent((FinalizeFileUploadsRequest)e.Request);
				}
				else if (type == typeof(GetFilesRequest) && _instance.OnDataGetFilesRequestEvent != null)
				{
					_instance.OnDataGetFilesRequestEvent((GetFilesRequest)e.Request);
				}
				else if (type == typeof(GetObjectsRequest) && _instance.OnDataGetObjectsRequestEvent != null)
				{
					_instance.OnDataGetObjectsRequestEvent((GetObjectsRequest)e.Request);
				}
				else if (type == typeof(InitiateFileUploadsRequest) && _instance.OnDataInitiateFileUploadsRequestEvent != null)
				{
					_instance.OnDataInitiateFileUploadsRequestEvent((InitiateFileUploadsRequest)e.Request);
				}
				else if (type == typeof(SetObjectsRequest) && _instance.OnDataSetObjectsRequestEvent != null)
				{
					_instance.OnDataSetObjectsRequestEvent((SetObjectsRequest)e.Request);
				}
				else if (type == typeof(WriteEventsRequest) && _instance.OnEventsWriteEventsRequestEvent != null)
				{
					_instance.OnEventsWriteEventsRequestEvent((WriteEventsRequest)e.Request);
				}
				else if (type == typeof(AcceptGroupApplicationRequest) && _instance.OnGroupsAcceptGroupApplicationRequestEvent != null)
				{
					_instance.OnGroupsAcceptGroupApplicationRequestEvent((AcceptGroupApplicationRequest)e.Request);
				}
				else if (type == typeof(AcceptGroupInvitationRequest) && _instance.OnGroupsAcceptGroupInvitationRequestEvent != null)
				{
					_instance.OnGroupsAcceptGroupInvitationRequestEvent((AcceptGroupInvitationRequest)e.Request);
				}
				else if (type == typeof(AddMembersRequest) && _instance.OnGroupsAddMembersRequestEvent != null)
				{
					_instance.OnGroupsAddMembersRequestEvent((AddMembersRequest)e.Request);
				}
				else if (type == typeof(ApplyToGroupRequest) && _instance.OnGroupsApplyToGroupRequestEvent != null)
				{
					_instance.OnGroupsApplyToGroupRequestEvent((ApplyToGroupRequest)e.Request);
				}
				else if (type == typeof(BlockEntityRequest) && _instance.OnGroupsBlockEntityRequestEvent != null)
				{
					_instance.OnGroupsBlockEntityRequestEvent((BlockEntityRequest)e.Request);
				}
				else if (type == typeof(ChangeMemberRoleRequest) && _instance.OnGroupsChangeMemberRoleRequestEvent != null)
				{
					_instance.OnGroupsChangeMemberRoleRequestEvent((ChangeMemberRoleRequest)e.Request);
				}
				else if (type == typeof(CreateGroupRequest) && _instance.OnGroupsCreateGroupRequestEvent != null)
				{
					_instance.OnGroupsCreateGroupRequestEvent((CreateGroupRequest)e.Request);
				}
				else if (type == typeof(CreateGroupRoleRequest) && _instance.OnGroupsCreateRoleRequestEvent != null)
				{
					_instance.OnGroupsCreateRoleRequestEvent((CreateGroupRoleRequest)e.Request);
				}
				else if (type == typeof(DeleteGroupRequest) && _instance.OnGroupsDeleteGroupRequestEvent != null)
				{
					_instance.OnGroupsDeleteGroupRequestEvent((DeleteGroupRequest)e.Request);
				}
				else if (type == typeof(DeleteRoleRequest) && _instance.OnGroupsDeleteRoleRequestEvent != null)
				{
					_instance.OnGroupsDeleteRoleRequestEvent((DeleteRoleRequest)e.Request);
				}
				else if (type == typeof(GetGroupRequest) && _instance.OnGroupsGetGroupRequestEvent != null)
				{
					_instance.OnGroupsGetGroupRequestEvent((GetGroupRequest)e.Request);
				}
				else if (type == typeof(InviteToGroupRequest) && _instance.OnGroupsInviteToGroupRequestEvent != null)
				{
					_instance.OnGroupsInviteToGroupRequestEvent((InviteToGroupRequest)e.Request);
				}
				else if (type == typeof(IsMemberRequest) && _instance.OnGroupsIsMemberRequestEvent != null)
				{
					_instance.OnGroupsIsMemberRequestEvent((IsMemberRequest)e.Request);
				}
				else if (type == typeof(ListGroupApplicationsRequest) && _instance.OnGroupsListGroupApplicationsRequestEvent != null)
				{
					_instance.OnGroupsListGroupApplicationsRequestEvent((ListGroupApplicationsRequest)e.Request);
				}
				else if (type == typeof(ListGroupBlocksRequest) && _instance.OnGroupsListGroupBlocksRequestEvent != null)
				{
					_instance.OnGroupsListGroupBlocksRequestEvent((ListGroupBlocksRequest)e.Request);
				}
				else if (type == typeof(ListGroupInvitationsRequest) && _instance.OnGroupsListGroupInvitationsRequestEvent != null)
				{
					_instance.OnGroupsListGroupInvitationsRequestEvent((ListGroupInvitationsRequest)e.Request);
				}
				else if (type == typeof(ListGroupMembersRequest) && _instance.OnGroupsListGroupMembersRequestEvent != null)
				{
					_instance.OnGroupsListGroupMembersRequestEvent((ListGroupMembersRequest)e.Request);
				}
				else if (type == typeof(ListMembershipRequest) && _instance.OnGroupsListMembershipRequestEvent != null)
				{
					_instance.OnGroupsListMembershipRequestEvent((ListMembershipRequest)e.Request);
				}
				else if (type == typeof(ListMembershipOpportunitiesRequest) && _instance.OnGroupsListMembershipOpportunitiesRequestEvent != null)
				{
					_instance.OnGroupsListMembershipOpportunitiesRequestEvent((ListMembershipOpportunitiesRequest)e.Request);
				}
				else if (type == typeof(RemoveGroupApplicationRequest) && _instance.OnGroupsRemoveGroupApplicationRequestEvent != null)
				{
					_instance.OnGroupsRemoveGroupApplicationRequestEvent((RemoveGroupApplicationRequest)e.Request);
				}
				else if (type == typeof(RemoveGroupInvitationRequest) && _instance.OnGroupsRemoveGroupInvitationRequestEvent != null)
				{
					_instance.OnGroupsRemoveGroupInvitationRequestEvent((RemoveGroupInvitationRequest)e.Request);
				}
				else if (type == typeof(RemoveMembersRequest) && _instance.OnGroupsRemoveMembersRequestEvent != null)
				{
					_instance.OnGroupsRemoveMembersRequestEvent((RemoveMembersRequest)e.Request);
				}
				else if (type == typeof(UnblockEntityRequest) && _instance.OnGroupsUnblockEntityRequestEvent != null)
				{
					_instance.OnGroupsUnblockEntityRequestEvent((UnblockEntityRequest)e.Request);
				}
				else if (type == typeof(UpdateGroupRequest) && _instance.OnGroupsUpdateGroupRequestEvent != null)
				{
					_instance.OnGroupsUpdateGroupRequestEvent((UpdateGroupRequest)e.Request);
				}
				else if (type == typeof(UpdateGroupRoleRequest) && _instance.OnGroupsUpdateRoleRequestEvent != null)
				{
					_instance.OnGroupsUpdateRoleRequestEvent((UpdateGroupRoleRequest)e.Request);
				}
				else if (type == typeof(GetLanguageListRequest) && _instance.OnLocalizationGetLanguageListRequestEvent != null)
				{
					_instance.OnLocalizationGetLanguageListRequestEvent((GetLanguageListRequest)e.Request);
				}
				else if (type == typeof(CancelAllMatchmakingTicketsForPlayerRequest) && _instance.OnMultiplayerCancelAllMatchmakingTicketsForPlayerRequestEvent != null)
				{
					_instance.OnMultiplayerCancelAllMatchmakingTicketsForPlayerRequestEvent((CancelAllMatchmakingTicketsForPlayerRequest)e.Request);
				}
				else if (type == typeof(CancelMatchmakingTicketRequest) && _instance.OnMultiplayerCancelMatchmakingTicketRequestEvent != null)
				{
					_instance.OnMultiplayerCancelMatchmakingTicketRequestEvent((CancelMatchmakingTicketRequest)e.Request);
				}
				else if (type == typeof(CreateBuildWithCustomContainerRequest) && _instance.OnMultiplayerCreateBuildWithCustomContainerRequestEvent != null)
				{
					_instance.OnMultiplayerCreateBuildWithCustomContainerRequestEvent((CreateBuildWithCustomContainerRequest)e.Request);
				}
				else if (type == typeof(CreateBuildWithManagedContainerRequest) && _instance.OnMultiplayerCreateBuildWithManagedContainerRequestEvent != null)
				{
					_instance.OnMultiplayerCreateBuildWithManagedContainerRequestEvent((CreateBuildWithManagedContainerRequest)e.Request);
				}
				else if (type == typeof(CreateMatchmakingTicketRequest) && _instance.OnMultiplayerCreateMatchmakingTicketRequestEvent != null)
				{
					_instance.OnMultiplayerCreateMatchmakingTicketRequestEvent((CreateMatchmakingTicketRequest)e.Request);
				}
				else if (type == typeof(CreateRemoteUserRequest) && _instance.OnMultiplayerCreateRemoteUserRequestEvent != null)
				{
					_instance.OnMultiplayerCreateRemoteUserRequestEvent((CreateRemoteUserRequest)e.Request);
				}
				else if (type == typeof(CreateServerMatchmakingTicketRequest) && _instance.OnMultiplayerCreateServerMatchmakingTicketRequestEvent != null)
				{
					_instance.OnMultiplayerCreateServerMatchmakingTicketRequestEvent((CreateServerMatchmakingTicketRequest)e.Request);
				}
				else if (type == typeof(DeleteAssetRequest) && _instance.OnMultiplayerDeleteAssetRequestEvent != null)
				{
					_instance.OnMultiplayerDeleteAssetRequestEvent((DeleteAssetRequest)e.Request);
				}
				else if (type == typeof(DeleteBuildRequest) && _instance.OnMultiplayerDeleteBuildRequestEvent != null)
				{
					_instance.OnMultiplayerDeleteBuildRequestEvent((DeleteBuildRequest)e.Request);
				}
				else if (type == typeof(DeleteCertificateRequest) && _instance.OnMultiplayerDeleteCertificateRequestEvent != null)
				{
					_instance.OnMultiplayerDeleteCertificateRequestEvent((DeleteCertificateRequest)e.Request);
				}
				else if (type == typeof(DeleteRemoteUserRequest) && _instance.OnMultiplayerDeleteRemoteUserRequestEvent != null)
				{
					_instance.OnMultiplayerDeleteRemoteUserRequestEvent((DeleteRemoteUserRequest)e.Request);
				}
				else if (type == typeof(EnableMultiplayerServersForTitleRequest) && _instance.OnMultiplayerEnableMultiplayerServersForTitleRequestEvent != null)
				{
					_instance.OnMultiplayerEnableMultiplayerServersForTitleRequestEvent((EnableMultiplayerServersForTitleRequest)e.Request);
				}
				else if (type == typeof(GetAssetUploadUrlRequest) && _instance.OnMultiplayerGetAssetUploadUrlRequestEvent != null)
				{
					_instance.OnMultiplayerGetAssetUploadUrlRequestEvent((GetAssetUploadUrlRequest)e.Request);
				}
				else if (type == typeof(GetBuildRequest) && _instance.OnMultiplayerGetBuildRequestEvent != null)
				{
					_instance.OnMultiplayerGetBuildRequestEvent((GetBuildRequest)e.Request);
				}
				else if (type == typeof(GetContainerRegistryCredentialsRequest) && _instance.OnMultiplayerGetContainerRegistryCredentialsRequestEvent != null)
				{
					_instance.OnMultiplayerGetContainerRegistryCredentialsRequestEvent((GetContainerRegistryCredentialsRequest)e.Request);
				}
				else if (type == typeof(GetMatchRequest) && _instance.OnMultiplayerGetMatchRequestEvent != null)
				{
					_instance.OnMultiplayerGetMatchRequestEvent((GetMatchRequest)e.Request);
				}
				else if (type == typeof(GetMatchmakingQueueRequest) && _instance.OnMultiplayerGetMatchmakingQueueRequestEvent != null)
				{
					_instance.OnMultiplayerGetMatchmakingQueueRequestEvent((GetMatchmakingQueueRequest)e.Request);
				}
				else if (type == typeof(GetMatchmakingTicketRequest) && _instance.OnMultiplayerGetMatchmakingTicketRequestEvent != null)
				{
					_instance.OnMultiplayerGetMatchmakingTicketRequestEvent((GetMatchmakingTicketRequest)e.Request);
				}
				else if (type == typeof(GetMultiplayerServerDetailsRequest) && _instance.OnMultiplayerGetMultiplayerServerDetailsRequestEvent != null)
				{
					_instance.OnMultiplayerGetMultiplayerServerDetailsRequestEvent((GetMultiplayerServerDetailsRequest)e.Request);
				}
				else if (type == typeof(GetQueueStatisticsRequest) && _instance.OnMultiplayerGetQueueStatisticsRequestEvent != null)
				{
					_instance.OnMultiplayerGetQueueStatisticsRequestEvent((GetQueueStatisticsRequest)e.Request);
				}
				else if (type == typeof(GetRemoteLoginEndpointRequest) && _instance.OnMultiplayerGetRemoteLoginEndpointRequestEvent != null)
				{
					_instance.OnMultiplayerGetRemoteLoginEndpointRequestEvent((GetRemoteLoginEndpointRequest)e.Request);
				}
				else if (type == typeof(GetTitleEnabledForMultiplayerServersStatusRequest) && _instance.OnMultiplayerGetTitleEnabledForMultiplayerServersStatusRequestEvent != null)
				{
					_instance.OnMultiplayerGetTitleEnabledForMultiplayerServersStatusRequestEvent((GetTitleEnabledForMultiplayerServersStatusRequest)e.Request);
				}
				else if (type == typeof(JoinMatchmakingTicketRequest) && _instance.OnMultiplayerJoinMatchmakingTicketRequestEvent != null)
				{
					_instance.OnMultiplayerJoinMatchmakingTicketRequestEvent((JoinMatchmakingTicketRequest)e.Request);
				}
				else if (type == typeof(ListMultiplayerServersRequest) && _instance.OnMultiplayerListArchivedMultiplayerServersRequestEvent != null)
				{
					_instance.OnMultiplayerListArchivedMultiplayerServersRequestEvent((ListMultiplayerServersRequest)e.Request);
				}
				else if (type == typeof(ListAssetSummariesRequest) && _instance.OnMultiplayerListAssetSummariesRequestEvent != null)
				{
					_instance.OnMultiplayerListAssetSummariesRequestEvent((ListAssetSummariesRequest)e.Request);
				}
				else if (type == typeof(ListBuildSummariesRequest) && _instance.OnMultiplayerListBuildSummariesRequestEvent != null)
				{
					_instance.OnMultiplayerListBuildSummariesRequestEvent((ListBuildSummariesRequest)e.Request);
				}
				else if (type == typeof(ListCertificateSummariesRequest) && _instance.OnMultiplayerListCertificateSummariesRequestEvent != null)
				{
					_instance.OnMultiplayerListCertificateSummariesRequestEvent((ListCertificateSummariesRequest)e.Request);
				}
				else if (type == typeof(ListContainerImagesRequest) && _instance.OnMultiplayerListContainerImagesRequestEvent != null)
				{
					_instance.OnMultiplayerListContainerImagesRequestEvent((ListContainerImagesRequest)e.Request);
				}
				else if (type == typeof(ListContainerImageTagsRequest) && _instance.OnMultiplayerListContainerImageTagsRequestEvent != null)
				{
					_instance.OnMultiplayerListContainerImageTagsRequestEvent((ListContainerImageTagsRequest)e.Request);
				}
				else if (type == typeof(ListMatchmakingQueuesRequest) && _instance.OnMultiplayerListMatchmakingQueuesRequestEvent != null)
				{
					_instance.OnMultiplayerListMatchmakingQueuesRequestEvent((ListMatchmakingQueuesRequest)e.Request);
				}
				else if (type == typeof(ListMatchmakingTicketsForPlayerRequest) && _instance.OnMultiplayerListMatchmakingTicketsForPlayerRequestEvent != null)
				{
					_instance.OnMultiplayerListMatchmakingTicketsForPlayerRequestEvent((ListMatchmakingTicketsForPlayerRequest)e.Request);
				}
				else if (type == typeof(ListMultiplayerServersRequest) && _instance.OnMultiplayerListMultiplayerServersRequestEvent != null)
				{
					_instance.OnMultiplayerListMultiplayerServersRequestEvent((ListMultiplayerServersRequest)e.Request);
				}
				else if (type == typeof(ListQosServersRequest) && _instance.OnMultiplayerListQosServersRequestEvent != null)
				{
					_instance.OnMultiplayerListQosServersRequestEvent((ListQosServersRequest)e.Request);
				}
				else if (type == typeof(ListVirtualMachineSummariesRequest) && _instance.OnMultiplayerListVirtualMachineSummariesRequestEvent != null)
				{
					_instance.OnMultiplayerListVirtualMachineSummariesRequestEvent((ListVirtualMachineSummariesRequest)e.Request);
				}
				else if (type == typeof(RemoveMatchmakingQueueRequest) && _instance.OnMultiplayerRemoveMatchmakingQueueRequestEvent != null)
				{
					_instance.OnMultiplayerRemoveMatchmakingQueueRequestEvent((RemoveMatchmakingQueueRequest)e.Request);
				}
				else if (type == typeof(RequestMultiplayerServerRequest) && _instance.OnMultiplayerRequestMultiplayerServerRequestEvent != null)
				{
					_instance.OnMultiplayerRequestMultiplayerServerRequestEvent((RequestMultiplayerServerRequest)e.Request);
				}
				else if (type == typeof(RolloverContainerRegistryCredentialsRequest) && _instance.OnMultiplayerRolloverContainerRegistryCredentialsRequestEvent != null)
				{
					_instance.OnMultiplayerRolloverContainerRegistryCredentialsRequestEvent((RolloverContainerRegistryCredentialsRequest)e.Request);
				}
				else if (type == typeof(SetMatchmakingQueueRequest) && _instance.OnMultiplayerSetMatchmakingQueueRequestEvent != null)
				{
					_instance.OnMultiplayerSetMatchmakingQueueRequestEvent((SetMatchmakingQueueRequest)e.Request);
				}
				else if (type == typeof(ShutdownMultiplayerServerRequest) && _instance.OnMultiplayerShutdownMultiplayerServerRequestEvent != null)
				{
					_instance.OnMultiplayerShutdownMultiplayerServerRequestEvent((ShutdownMultiplayerServerRequest)e.Request);
				}
				else if (type == typeof(UpdateBuildRegionsRequest) && _instance.OnMultiplayerUpdateBuildRegionsRequestEvent != null)
				{
					_instance.OnMultiplayerUpdateBuildRegionsRequestEvent((UpdateBuildRegionsRequest)e.Request);
				}
				else if (type == typeof(UploadCertificateRequest) && _instance.OnMultiplayerUploadCertificateRequestEvent != null)
				{
					_instance.OnMultiplayerUploadCertificateRequestEvent((UploadCertificateRequest)e.Request);
				}
				else if (type == typeof(GetGlobalPolicyRequest) && _instance.OnProfilesGetGlobalPolicyRequestEvent != null)
				{
					_instance.OnProfilesGetGlobalPolicyRequestEvent((GetGlobalPolicyRequest)e.Request);
				}
				else if (type == typeof(GetEntityProfileRequest) && _instance.OnProfilesGetProfileRequestEvent != null)
				{
					_instance.OnProfilesGetProfileRequestEvent((GetEntityProfileRequest)e.Request);
				}
				else if (type == typeof(GetEntityProfilesRequest) && _instance.OnProfilesGetProfilesRequestEvent != null)
				{
					_instance.OnProfilesGetProfilesRequestEvent((GetEntityProfilesRequest)e.Request);
				}
				else if (type == typeof(SetGlobalPolicyRequest) && _instance.OnProfilesSetGlobalPolicyRequestEvent != null)
				{
					_instance.OnProfilesSetGlobalPolicyRequestEvent((SetGlobalPolicyRequest)e.Request);
				}
				else if (type == typeof(SetProfileLanguageRequest) && _instance.OnProfilesSetProfileLanguageRequestEvent != null)
				{
					_instance.OnProfilesSetProfileLanguageRequestEvent((SetProfileLanguageRequest)e.Request);
				}
				else if (type == typeof(SetEntityProfilePolicyRequest) && _instance.OnProfilesSetProfilePolicyRequestEvent != null)
				{
					_instance.OnProfilesSetProfilePolicyRequestEvent((SetEntityProfilePolicyRequest)e.Request);
				}
			}
			else
			{
				Type type2 = e.Result.GetType();
				if (type2 == typeof(LoginResult) && _instance.OnLoginResultEvent != null)
				{
					_instance.OnLoginResultEvent((LoginResult)e.Result);
				}
				else if (type2 == typeof(AcceptTradeResponse) && _instance.OnAcceptTradeResultEvent != null)
				{
					_instance.OnAcceptTradeResultEvent((AcceptTradeResponse)e.Result);
				}
				else if (type2 == typeof(AddFriendResult) && _instance.OnAddFriendResultEvent != null)
				{
					_instance.OnAddFriendResultEvent((AddFriendResult)e.Result);
				}
				else if (type2 == typeof(AddGenericIDResult) && _instance.OnAddGenericIDResultEvent != null)
				{
					_instance.OnAddGenericIDResultEvent((AddGenericIDResult)e.Result);
				}
				else if (type2 == typeof(AddOrUpdateContactEmailResult) && _instance.OnAddOrUpdateContactEmailResultEvent != null)
				{
					_instance.OnAddOrUpdateContactEmailResultEvent((AddOrUpdateContactEmailResult)e.Result);
				}
				else if (type2 == typeof(AddSharedGroupMembersResult) && _instance.OnAddSharedGroupMembersResultEvent != null)
				{
					_instance.OnAddSharedGroupMembersResultEvent((AddSharedGroupMembersResult)e.Result);
				}
				else if (type2 == typeof(AddUsernamePasswordResult) && _instance.OnAddUsernamePasswordResultEvent != null)
				{
					_instance.OnAddUsernamePasswordResultEvent((AddUsernamePasswordResult)e.Result);
				}
				else if (type2 == typeof(ModifyUserVirtualCurrencyResult) && _instance.OnAddUserVirtualCurrencyResultEvent != null)
				{
					_instance.OnAddUserVirtualCurrencyResultEvent((ModifyUserVirtualCurrencyResult)e.Result);
				}
				else if (type2 == typeof(AndroidDevicePushNotificationRegistrationResult) && _instance.OnAndroidDevicePushNotificationRegistrationResultEvent != null)
				{
					_instance.OnAndroidDevicePushNotificationRegistrationResultEvent((AndroidDevicePushNotificationRegistrationResult)e.Result);
				}
				else if (type2 == typeof(AttributeInstallResult) && _instance.OnAttributeInstallResultEvent != null)
				{
					_instance.OnAttributeInstallResultEvent((AttributeInstallResult)e.Result);
				}
				else if (type2 == typeof(CancelTradeResponse) && _instance.OnCancelTradeResultEvent != null)
				{
					_instance.OnCancelTradeResultEvent((CancelTradeResponse)e.Result);
				}
				else if (type2 == typeof(ConfirmPurchaseResult) && _instance.OnConfirmPurchaseResultEvent != null)
				{
					_instance.OnConfirmPurchaseResultEvent((ConfirmPurchaseResult)e.Result);
				}
				else if (type2 == typeof(ConsumeItemResult) && _instance.OnConsumeItemResultEvent != null)
				{
					_instance.OnConsumeItemResultEvent((ConsumeItemResult)e.Result);
				}
				else if (type2 == typeof(ConsumePSNEntitlementsResult) && _instance.OnConsumePSNEntitlementsResultEvent != null)
				{
					_instance.OnConsumePSNEntitlementsResultEvent((ConsumePSNEntitlementsResult)e.Result);
				}
				else if (type2 == typeof(ConsumeXboxEntitlementsResult) && _instance.OnConsumeXboxEntitlementsResultEvent != null)
				{
					_instance.OnConsumeXboxEntitlementsResultEvent((ConsumeXboxEntitlementsResult)e.Result);
				}
				else if (type2 == typeof(CreateSharedGroupResult) && _instance.OnCreateSharedGroupResultEvent != null)
				{
					_instance.OnCreateSharedGroupResultEvent((CreateSharedGroupResult)e.Result);
				}
				else if (type2 == typeof(PlayFab.ClientModels.ExecuteCloudScriptResult) && _instance.OnExecuteCloudScriptResultEvent != null)
				{
					_instance.OnExecuteCloudScriptResultEvent((PlayFab.ClientModels.ExecuteCloudScriptResult)e.Result);
				}
				else if (type2 == typeof(GetAccountInfoResult) && _instance.OnGetAccountInfoResultEvent != null)
				{
					_instance.OnGetAccountInfoResultEvent((GetAccountInfoResult)e.Result);
				}
				else if (type2 == typeof(ListUsersCharactersResult) && _instance.OnGetAllUsersCharactersResultEvent != null)
				{
					_instance.OnGetAllUsersCharactersResultEvent((ListUsersCharactersResult)e.Result);
				}
				else if (type2 == typeof(GetCatalogItemsResult) && _instance.OnGetCatalogItemsResultEvent != null)
				{
					_instance.OnGetCatalogItemsResultEvent((GetCatalogItemsResult)e.Result);
				}
				else if (type2 == typeof(GetCharacterDataResult) && _instance.OnGetCharacterDataResultEvent != null)
				{
					_instance.OnGetCharacterDataResultEvent((GetCharacterDataResult)e.Result);
				}
				else if (type2 == typeof(GetCharacterInventoryResult) && _instance.OnGetCharacterInventoryResultEvent != null)
				{
					_instance.OnGetCharacterInventoryResultEvent((GetCharacterInventoryResult)e.Result);
				}
				else if (type2 == typeof(GetCharacterLeaderboardResult) && _instance.OnGetCharacterLeaderboardResultEvent != null)
				{
					_instance.OnGetCharacterLeaderboardResultEvent((GetCharacterLeaderboardResult)e.Result);
				}
				else if (type2 == typeof(GetCharacterDataResult) && _instance.OnGetCharacterReadOnlyDataResultEvent != null)
				{
					_instance.OnGetCharacterReadOnlyDataResultEvent((GetCharacterDataResult)e.Result);
				}
				else if (type2 == typeof(GetCharacterStatisticsResult) && _instance.OnGetCharacterStatisticsResultEvent != null)
				{
					_instance.OnGetCharacterStatisticsResultEvent((GetCharacterStatisticsResult)e.Result);
				}
				else if (type2 == typeof(GetContentDownloadUrlResult) && _instance.OnGetContentDownloadUrlResultEvent != null)
				{
					_instance.OnGetContentDownloadUrlResultEvent((GetContentDownloadUrlResult)e.Result);
				}
				else if (type2 == typeof(CurrentGamesResult) && _instance.OnGetCurrentGamesResultEvent != null)
				{
					_instance.OnGetCurrentGamesResultEvent((CurrentGamesResult)e.Result);
				}
				else if (type2 == typeof(GetLeaderboardResult) && _instance.OnGetFriendLeaderboardResultEvent != null)
				{
					_instance.OnGetFriendLeaderboardResultEvent((GetLeaderboardResult)e.Result);
				}
				else if (type2 == typeof(GetFriendLeaderboardAroundPlayerResult) && _instance.OnGetFriendLeaderboardAroundPlayerResultEvent != null)
				{
					_instance.OnGetFriendLeaderboardAroundPlayerResultEvent((GetFriendLeaderboardAroundPlayerResult)e.Result);
				}
				else if (type2 == typeof(GetFriendsListResult) && _instance.OnGetFriendsListResultEvent != null)
				{
					_instance.OnGetFriendsListResultEvent((GetFriendsListResult)e.Result);
				}
				else if (type2 == typeof(GameServerRegionsResult) && _instance.OnGetGameServerRegionsResultEvent != null)
				{
					_instance.OnGetGameServerRegionsResultEvent((GameServerRegionsResult)e.Result);
				}
				else if (type2 == typeof(GetLeaderboardResult) && _instance.OnGetLeaderboardResultEvent != null)
				{
					_instance.OnGetLeaderboardResultEvent((GetLeaderboardResult)e.Result);
				}
				else if (type2 == typeof(GetLeaderboardAroundCharacterResult) && _instance.OnGetLeaderboardAroundCharacterResultEvent != null)
				{
					_instance.OnGetLeaderboardAroundCharacterResultEvent((GetLeaderboardAroundCharacterResult)e.Result);
				}
				else if (type2 == typeof(GetLeaderboardAroundPlayerResult) && _instance.OnGetLeaderboardAroundPlayerResultEvent != null)
				{
					_instance.OnGetLeaderboardAroundPlayerResultEvent((GetLeaderboardAroundPlayerResult)e.Result);
				}
				else if (type2 == typeof(GetLeaderboardForUsersCharactersResult) && _instance.OnGetLeaderboardForUserCharactersResultEvent != null)
				{
					_instance.OnGetLeaderboardForUserCharactersResultEvent((GetLeaderboardForUsersCharactersResult)e.Result);
				}
				else if (type2 == typeof(GetPaymentTokenResult) && _instance.OnGetPaymentTokenResultEvent != null)
				{
					_instance.OnGetPaymentTokenResultEvent((GetPaymentTokenResult)e.Result);
				}
				else if (type2 == typeof(GetPhotonAuthenticationTokenResult) && _instance.OnGetPhotonAuthenticationTokenResultEvent != null)
				{
					_instance.OnGetPhotonAuthenticationTokenResultEvent((GetPhotonAuthenticationTokenResult)e.Result);
				}
				else if (type2 == typeof(GetPlayerCombinedInfoResult) && _instance.OnGetPlayerCombinedInfoResultEvent != null)
				{
					_instance.OnGetPlayerCombinedInfoResultEvent((GetPlayerCombinedInfoResult)e.Result);
				}
				else if (type2 == typeof(GetPlayerProfileResult) && _instance.OnGetPlayerProfileResultEvent != null)
				{
					_instance.OnGetPlayerProfileResultEvent((GetPlayerProfileResult)e.Result);
				}
				else if (type2 == typeof(GetPlayerSegmentsResult) && _instance.OnGetPlayerSegmentsResultEvent != null)
				{
					_instance.OnGetPlayerSegmentsResultEvent((GetPlayerSegmentsResult)e.Result);
				}
				else if (type2 == typeof(GetPlayerStatisticsResult) && _instance.OnGetPlayerStatisticsResultEvent != null)
				{
					_instance.OnGetPlayerStatisticsResultEvent((GetPlayerStatisticsResult)e.Result);
				}
				else if (type2 == typeof(GetPlayerStatisticVersionsResult) && _instance.OnGetPlayerStatisticVersionsResultEvent != null)
				{
					_instance.OnGetPlayerStatisticVersionsResultEvent((GetPlayerStatisticVersionsResult)e.Result);
				}
				else if (type2 == typeof(GetPlayerTagsResult) && _instance.OnGetPlayerTagsResultEvent != null)
				{
					_instance.OnGetPlayerTagsResultEvent((GetPlayerTagsResult)e.Result);
				}
				else if (type2 == typeof(GetPlayerTradesResponse) && _instance.OnGetPlayerTradesResultEvent != null)
				{
					_instance.OnGetPlayerTradesResultEvent((GetPlayerTradesResponse)e.Result);
				}
				else if (type2 == typeof(GetPlayFabIDsFromFacebookIDsResult) && _instance.OnGetPlayFabIDsFromFacebookIDsResultEvent != null)
				{
					_instance.OnGetPlayFabIDsFromFacebookIDsResultEvent((GetPlayFabIDsFromFacebookIDsResult)e.Result);
				}
				else if (type2 == typeof(GetPlayFabIDsFromFacebookInstantGamesIdsResult) && _instance.OnGetPlayFabIDsFromFacebookInstantGamesIdsResultEvent != null)
				{
					_instance.OnGetPlayFabIDsFromFacebookInstantGamesIdsResultEvent((GetPlayFabIDsFromFacebookInstantGamesIdsResult)e.Result);
				}
				else if (type2 == typeof(GetPlayFabIDsFromGameCenterIDsResult) && _instance.OnGetPlayFabIDsFromGameCenterIDsResultEvent != null)
				{
					_instance.OnGetPlayFabIDsFromGameCenterIDsResultEvent((GetPlayFabIDsFromGameCenterIDsResult)e.Result);
				}
				else if (type2 == typeof(GetPlayFabIDsFromGenericIDsResult) && _instance.OnGetPlayFabIDsFromGenericIDsResultEvent != null)
				{
					_instance.OnGetPlayFabIDsFromGenericIDsResultEvent((GetPlayFabIDsFromGenericIDsResult)e.Result);
				}
				else if (type2 == typeof(GetPlayFabIDsFromGoogleIDsResult) && _instance.OnGetPlayFabIDsFromGoogleIDsResultEvent != null)
				{
					_instance.OnGetPlayFabIDsFromGoogleIDsResultEvent((GetPlayFabIDsFromGoogleIDsResult)e.Result);
				}
				else if (type2 == typeof(GetPlayFabIDsFromKongregateIDsResult) && _instance.OnGetPlayFabIDsFromKongregateIDsResultEvent != null)
				{
					_instance.OnGetPlayFabIDsFromKongregateIDsResultEvent((GetPlayFabIDsFromKongregateIDsResult)e.Result);
				}
				else if (type2 == typeof(GetPlayFabIDsFromNintendoSwitchDeviceIdsResult) && _instance.OnGetPlayFabIDsFromNintendoSwitchDeviceIdsResultEvent != null)
				{
					_instance.OnGetPlayFabIDsFromNintendoSwitchDeviceIdsResultEvent((GetPlayFabIDsFromNintendoSwitchDeviceIdsResult)e.Result);
				}
				else if (type2 == typeof(GetPlayFabIDsFromPSNAccountIDsResult) && _instance.OnGetPlayFabIDsFromPSNAccountIDsResultEvent != null)
				{
					_instance.OnGetPlayFabIDsFromPSNAccountIDsResultEvent((GetPlayFabIDsFromPSNAccountIDsResult)e.Result);
				}
				else if (type2 == typeof(GetPlayFabIDsFromSteamIDsResult) && _instance.OnGetPlayFabIDsFromSteamIDsResultEvent != null)
				{
					_instance.OnGetPlayFabIDsFromSteamIDsResultEvent((GetPlayFabIDsFromSteamIDsResult)e.Result);
				}
				else if (type2 == typeof(GetPlayFabIDsFromTwitchIDsResult) && _instance.OnGetPlayFabIDsFromTwitchIDsResultEvent != null)
				{
					_instance.OnGetPlayFabIDsFromTwitchIDsResultEvent((GetPlayFabIDsFromTwitchIDsResult)e.Result);
				}
				else if (type2 == typeof(GetPlayFabIDsFromXboxLiveIDsResult) && _instance.OnGetPlayFabIDsFromXboxLiveIDsResultEvent != null)
				{
					_instance.OnGetPlayFabIDsFromXboxLiveIDsResultEvent((GetPlayFabIDsFromXboxLiveIDsResult)e.Result);
				}
				else if (type2 == typeof(GetPublisherDataResult) && _instance.OnGetPublisherDataResultEvent != null)
				{
					_instance.OnGetPublisherDataResultEvent((GetPublisherDataResult)e.Result);
				}
				else if (type2 == typeof(GetPurchaseResult) && _instance.OnGetPurchaseResultEvent != null)
				{
					_instance.OnGetPurchaseResultEvent((GetPurchaseResult)e.Result);
				}
				else if (type2 == typeof(GetSharedGroupDataResult) && _instance.OnGetSharedGroupDataResultEvent != null)
				{
					_instance.OnGetSharedGroupDataResultEvent((GetSharedGroupDataResult)e.Result);
				}
				else if (type2 == typeof(GetStoreItemsResult) && _instance.OnGetStoreItemsResultEvent != null)
				{
					_instance.OnGetStoreItemsResultEvent((GetStoreItemsResult)e.Result);
				}
				else if (type2 == typeof(GetTimeResult) && _instance.OnGetTimeResultEvent != null)
				{
					_instance.OnGetTimeResultEvent((GetTimeResult)e.Result);
				}
				else if (type2 == typeof(GetTitleDataResult) && _instance.OnGetTitleDataResultEvent != null)
				{
					_instance.OnGetTitleDataResultEvent((GetTitleDataResult)e.Result);
				}
				else if (type2 == typeof(GetTitleNewsResult) && _instance.OnGetTitleNewsResultEvent != null)
				{
					_instance.OnGetTitleNewsResultEvent((GetTitleNewsResult)e.Result);
				}
				else if (type2 == typeof(GetTitlePublicKeyResult) && _instance.OnGetTitlePublicKeyResultEvent != null)
				{
					_instance.OnGetTitlePublicKeyResultEvent((GetTitlePublicKeyResult)e.Result);
				}
				else if (type2 == typeof(GetTradeStatusResponse) && _instance.OnGetTradeStatusResultEvent != null)
				{
					_instance.OnGetTradeStatusResultEvent((GetTradeStatusResponse)e.Result);
				}
				else if (type2 == typeof(GetUserDataResult) && _instance.OnGetUserDataResultEvent != null)
				{
					_instance.OnGetUserDataResultEvent((GetUserDataResult)e.Result);
				}
				else if (type2 == typeof(GetUserInventoryResult) && _instance.OnGetUserInventoryResultEvent != null)
				{
					_instance.OnGetUserInventoryResultEvent((GetUserInventoryResult)e.Result);
				}
				else if (type2 == typeof(GetUserDataResult) && _instance.OnGetUserPublisherDataResultEvent != null)
				{
					_instance.OnGetUserPublisherDataResultEvent((GetUserDataResult)e.Result);
				}
				else if (type2 == typeof(GetUserDataResult) && _instance.OnGetUserPublisherReadOnlyDataResultEvent != null)
				{
					_instance.OnGetUserPublisherReadOnlyDataResultEvent((GetUserDataResult)e.Result);
				}
				else if (type2 == typeof(GetUserDataResult) && _instance.OnGetUserReadOnlyDataResultEvent != null)
				{
					_instance.OnGetUserReadOnlyDataResultEvent((GetUserDataResult)e.Result);
				}
				else if (type2 == typeof(GetWindowsHelloChallengeResponse) && _instance.OnGetWindowsHelloChallengeResultEvent != null)
				{
					_instance.OnGetWindowsHelloChallengeResultEvent((GetWindowsHelloChallengeResponse)e.Result);
				}
				else if (type2 == typeof(GrantCharacterToUserResult) && _instance.OnGrantCharacterToUserResultEvent != null)
				{
					_instance.OnGrantCharacterToUserResultEvent((GrantCharacterToUserResult)e.Result);
				}
				else if (type2 == typeof(LinkAndroidDeviceIDResult) && _instance.OnLinkAndroidDeviceIDResultEvent != null)
				{
					_instance.OnLinkAndroidDeviceIDResultEvent((LinkAndroidDeviceIDResult)e.Result);
				}
				else if (type2 == typeof(LinkCustomIDResult) && _instance.OnLinkCustomIDResultEvent != null)
				{
					_instance.OnLinkCustomIDResultEvent((LinkCustomIDResult)e.Result);
				}
				else if (type2 == typeof(LinkFacebookAccountResult) && _instance.OnLinkFacebookAccountResultEvent != null)
				{
					_instance.OnLinkFacebookAccountResultEvent((LinkFacebookAccountResult)e.Result);
				}
				else if (type2 == typeof(LinkFacebookInstantGamesIdResult) && _instance.OnLinkFacebookInstantGamesIdResultEvent != null)
				{
					_instance.OnLinkFacebookInstantGamesIdResultEvent((LinkFacebookInstantGamesIdResult)e.Result);
				}
				else if (type2 == typeof(LinkGameCenterAccountResult) && _instance.OnLinkGameCenterAccountResultEvent != null)
				{
					_instance.OnLinkGameCenterAccountResultEvent((LinkGameCenterAccountResult)e.Result);
				}
				else if (type2 == typeof(LinkGoogleAccountResult) && _instance.OnLinkGoogleAccountResultEvent != null)
				{
					_instance.OnLinkGoogleAccountResultEvent((LinkGoogleAccountResult)e.Result);
				}
				else if (type2 == typeof(LinkIOSDeviceIDResult) && _instance.OnLinkIOSDeviceIDResultEvent != null)
				{
					_instance.OnLinkIOSDeviceIDResultEvent((LinkIOSDeviceIDResult)e.Result);
				}
				else if (type2 == typeof(LinkKongregateAccountResult) && _instance.OnLinkKongregateResultEvent != null)
				{
					_instance.OnLinkKongregateResultEvent((LinkKongregateAccountResult)e.Result);
				}
				else if (type2 == typeof(LinkNintendoSwitchDeviceIdResult) && _instance.OnLinkNintendoSwitchDeviceIdResultEvent != null)
				{
					_instance.OnLinkNintendoSwitchDeviceIdResultEvent((LinkNintendoSwitchDeviceIdResult)e.Result);
				}
				else if (type2 == typeof(EmptyResult) && _instance.OnLinkOpenIdConnectResultEvent != null)
				{
					_instance.OnLinkOpenIdConnectResultEvent((EmptyResult)e.Result);
				}
				else if (type2 == typeof(LinkPSNAccountResult) && _instance.OnLinkPSNAccountResultEvent != null)
				{
					_instance.OnLinkPSNAccountResultEvent((LinkPSNAccountResult)e.Result);
				}
				else if (type2 == typeof(LinkSteamAccountResult) && _instance.OnLinkSteamAccountResultEvent != null)
				{
					_instance.OnLinkSteamAccountResultEvent((LinkSteamAccountResult)e.Result);
				}
				else if (type2 == typeof(LinkTwitchAccountResult) && _instance.OnLinkTwitchResultEvent != null)
				{
					_instance.OnLinkTwitchResultEvent((LinkTwitchAccountResult)e.Result);
				}
				else if (type2 == typeof(LinkWindowsHelloAccountResponse) && _instance.OnLinkWindowsHelloResultEvent != null)
				{
					_instance.OnLinkWindowsHelloResultEvent((LinkWindowsHelloAccountResponse)e.Result);
				}
				else if (type2 == typeof(LinkXboxAccountResult) && _instance.OnLinkXboxAccountResultEvent != null)
				{
					_instance.OnLinkXboxAccountResultEvent((LinkXboxAccountResult)e.Result);
				}
				else if (type2 == typeof(MatchmakeResult) && _instance.OnMatchmakeResultEvent != null)
				{
					_instance.OnMatchmakeResultEvent((MatchmakeResult)e.Result);
				}
				else if (type2 == typeof(OpenTradeResponse) && _instance.OnOpenTradeResultEvent != null)
				{
					_instance.OnOpenTradeResultEvent((OpenTradeResponse)e.Result);
				}
				else if (type2 == typeof(PayForPurchaseResult) && _instance.OnPayForPurchaseResultEvent != null)
				{
					_instance.OnPayForPurchaseResultEvent((PayForPurchaseResult)e.Result);
				}
				else if (type2 == typeof(PurchaseItemResult) && _instance.OnPurchaseItemResultEvent != null)
				{
					_instance.OnPurchaseItemResultEvent((PurchaseItemResult)e.Result);
				}
				else if (type2 == typeof(RedeemCouponResult) && _instance.OnRedeemCouponResultEvent != null)
				{
					_instance.OnRedeemCouponResultEvent((RedeemCouponResult)e.Result);
				}
				else if (type2 == typeof(PlayFab.ClientModels.EmptyResponse) && _instance.OnRefreshPSNAuthTokenResultEvent != null)
				{
					_instance.OnRefreshPSNAuthTokenResultEvent((PlayFab.ClientModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(RegisterForIOSPushNotificationResult) && _instance.OnRegisterForIOSPushNotificationResultEvent != null)
				{
					_instance.OnRegisterForIOSPushNotificationResultEvent((RegisterForIOSPushNotificationResult)e.Result);
				}
				else if (type2 == typeof(RegisterPlayFabUserResult) && _instance.OnRegisterPlayFabUserResultEvent != null)
				{
					_instance.OnRegisterPlayFabUserResultEvent((RegisterPlayFabUserResult)e.Result);
				}
				else if (type2 == typeof(RemoveContactEmailResult) && _instance.OnRemoveContactEmailResultEvent != null)
				{
					_instance.OnRemoveContactEmailResultEvent((RemoveContactEmailResult)e.Result);
				}
				else if (type2 == typeof(RemoveFriendResult) && _instance.OnRemoveFriendResultEvent != null)
				{
					_instance.OnRemoveFriendResultEvent((RemoveFriendResult)e.Result);
				}
				else if (type2 == typeof(RemoveGenericIDResult) && _instance.OnRemoveGenericIDResultEvent != null)
				{
					_instance.OnRemoveGenericIDResultEvent((RemoveGenericIDResult)e.Result);
				}
				else if (type2 == typeof(RemoveSharedGroupMembersResult) && _instance.OnRemoveSharedGroupMembersResultEvent != null)
				{
					_instance.OnRemoveSharedGroupMembersResultEvent((RemoveSharedGroupMembersResult)e.Result);
				}
				else if (type2 == typeof(PlayFab.ClientModels.EmptyResponse) && _instance.OnReportDeviceInfoResultEvent != null)
				{
					_instance.OnReportDeviceInfoResultEvent((PlayFab.ClientModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(ReportPlayerClientResult) && _instance.OnReportPlayerResultEvent != null)
				{
					_instance.OnReportPlayerResultEvent((ReportPlayerClientResult)e.Result);
				}
				else if (type2 == typeof(RestoreIOSPurchasesResult) && _instance.OnRestoreIOSPurchasesResultEvent != null)
				{
					_instance.OnRestoreIOSPurchasesResultEvent((RestoreIOSPurchasesResult)e.Result);
				}
				else if (type2 == typeof(SendAccountRecoveryEmailResult) && _instance.OnSendAccountRecoveryEmailResultEvent != null)
				{
					_instance.OnSendAccountRecoveryEmailResultEvent((SendAccountRecoveryEmailResult)e.Result);
				}
				else if (type2 == typeof(SetFriendTagsResult) && _instance.OnSetFriendTagsResultEvent != null)
				{
					_instance.OnSetFriendTagsResultEvent((SetFriendTagsResult)e.Result);
				}
				else if (type2 == typeof(SetPlayerSecretResult) && _instance.OnSetPlayerSecretResultEvent != null)
				{
					_instance.OnSetPlayerSecretResultEvent((SetPlayerSecretResult)e.Result);
				}
				else if (type2 == typeof(StartGameResult) && _instance.OnStartGameResultEvent != null)
				{
					_instance.OnStartGameResultEvent((StartGameResult)e.Result);
				}
				else if (type2 == typeof(StartPurchaseResult) && _instance.OnStartPurchaseResultEvent != null)
				{
					_instance.OnStartPurchaseResultEvent((StartPurchaseResult)e.Result);
				}
				else if (type2 == typeof(ModifyUserVirtualCurrencyResult) && _instance.OnSubtractUserVirtualCurrencyResultEvent != null)
				{
					_instance.OnSubtractUserVirtualCurrencyResultEvent((ModifyUserVirtualCurrencyResult)e.Result);
				}
				else if (type2 == typeof(UnlinkAndroidDeviceIDResult) && _instance.OnUnlinkAndroidDeviceIDResultEvent != null)
				{
					_instance.OnUnlinkAndroidDeviceIDResultEvent((UnlinkAndroidDeviceIDResult)e.Result);
				}
				else if (type2 == typeof(UnlinkCustomIDResult) && _instance.OnUnlinkCustomIDResultEvent != null)
				{
					_instance.OnUnlinkCustomIDResultEvent((UnlinkCustomIDResult)e.Result);
				}
				else if (type2 == typeof(UnlinkFacebookAccountResult) && _instance.OnUnlinkFacebookAccountResultEvent != null)
				{
					_instance.OnUnlinkFacebookAccountResultEvent((UnlinkFacebookAccountResult)e.Result);
				}
				else if (type2 == typeof(UnlinkFacebookInstantGamesIdResult) && _instance.OnUnlinkFacebookInstantGamesIdResultEvent != null)
				{
					_instance.OnUnlinkFacebookInstantGamesIdResultEvent((UnlinkFacebookInstantGamesIdResult)e.Result);
				}
				else if (type2 == typeof(UnlinkGameCenterAccountResult) && _instance.OnUnlinkGameCenterAccountResultEvent != null)
				{
					_instance.OnUnlinkGameCenterAccountResultEvent((UnlinkGameCenterAccountResult)e.Result);
				}
				else if (type2 == typeof(UnlinkGoogleAccountResult) && _instance.OnUnlinkGoogleAccountResultEvent != null)
				{
					_instance.OnUnlinkGoogleAccountResultEvent((UnlinkGoogleAccountResult)e.Result);
				}
				else if (type2 == typeof(UnlinkIOSDeviceIDResult) && _instance.OnUnlinkIOSDeviceIDResultEvent != null)
				{
					_instance.OnUnlinkIOSDeviceIDResultEvent((UnlinkIOSDeviceIDResult)e.Result);
				}
				else if (type2 == typeof(UnlinkKongregateAccountResult) && _instance.OnUnlinkKongregateResultEvent != null)
				{
					_instance.OnUnlinkKongregateResultEvent((UnlinkKongregateAccountResult)e.Result);
				}
				else if (type2 == typeof(UnlinkNintendoSwitchDeviceIdResult) && _instance.OnUnlinkNintendoSwitchDeviceIdResultEvent != null)
				{
					_instance.OnUnlinkNintendoSwitchDeviceIdResultEvent((UnlinkNintendoSwitchDeviceIdResult)e.Result);
				}
				else if (type2 == typeof(PlayFab.ClientModels.EmptyResponse) && _instance.OnUnlinkOpenIdConnectResultEvent != null)
				{
					_instance.OnUnlinkOpenIdConnectResultEvent((PlayFab.ClientModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(UnlinkPSNAccountResult) && _instance.OnUnlinkPSNAccountResultEvent != null)
				{
					_instance.OnUnlinkPSNAccountResultEvent((UnlinkPSNAccountResult)e.Result);
				}
				else if (type2 == typeof(UnlinkSteamAccountResult) && _instance.OnUnlinkSteamAccountResultEvent != null)
				{
					_instance.OnUnlinkSteamAccountResultEvent((UnlinkSteamAccountResult)e.Result);
				}
				else if (type2 == typeof(UnlinkTwitchAccountResult) && _instance.OnUnlinkTwitchResultEvent != null)
				{
					_instance.OnUnlinkTwitchResultEvent((UnlinkTwitchAccountResult)e.Result);
				}
				else if (type2 == typeof(UnlinkWindowsHelloAccountResponse) && _instance.OnUnlinkWindowsHelloResultEvent != null)
				{
					_instance.OnUnlinkWindowsHelloResultEvent((UnlinkWindowsHelloAccountResponse)e.Result);
				}
				else if (type2 == typeof(UnlinkXboxAccountResult) && _instance.OnUnlinkXboxAccountResultEvent != null)
				{
					_instance.OnUnlinkXboxAccountResultEvent((UnlinkXboxAccountResult)e.Result);
				}
				else if (type2 == typeof(UnlockContainerItemResult) && _instance.OnUnlockContainerInstanceResultEvent != null)
				{
					_instance.OnUnlockContainerInstanceResultEvent((UnlockContainerItemResult)e.Result);
				}
				else if (type2 == typeof(UnlockContainerItemResult) && _instance.OnUnlockContainerItemResultEvent != null)
				{
					_instance.OnUnlockContainerItemResultEvent((UnlockContainerItemResult)e.Result);
				}
				else if (type2 == typeof(PlayFab.ClientModels.EmptyResponse) && _instance.OnUpdateAvatarUrlResultEvent != null)
				{
					_instance.OnUpdateAvatarUrlResultEvent((PlayFab.ClientModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(UpdateCharacterDataResult) && _instance.OnUpdateCharacterDataResultEvent != null)
				{
					_instance.OnUpdateCharacterDataResultEvent((UpdateCharacterDataResult)e.Result);
				}
				else if (type2 == typeof(UpdateCharacterStatisticsResult) && _instance.OnUpdateCharacterStatisticsResultEvent != null)
				{
					_instance.OnUpdateCharacterStatisticsResultEvent((UpdateCharacterStatisticsResult)e.Result);
				}
				else if (type2 == typeof(UpdatePlayerStatisticsResult) && _instance.OnUpdatePlayerStatisticsResultEvent != null)
				{
					_instance.OnUpdatePlayerStatisticsResultEvent((UpdatePlayerStatisticsResult)e.Result);
				}
				else if (type2 == typeof(UpdateSharedGroupDataResult) && _instance.OnUpdateSharedGroupDataResultEvent != null)
				{
					_instance.OnUpdateSharedGroupDataResultEvent((UpdateSharedGroupDataResult)e.Result);
				}
				else if (type2 == typeof(UpdateUserDataResult) && _instance.OnUpdateUserDataResultEvent != null)
				{
					_instance.OnUpdateUserDataResultEvent((UpdateUserDataResult)e.Result);
				}
				else if (type2 == typeof(UpdateUserDataResult) && _instance.OnUpdateUserPublisherDataResultEvent != null)
				{
					_instance.OnUpdateUserPublisherDataResultEvent((UpdateUserDataResult)e.Result);
				}
				else if (type2 == typeof(UpdateUserTitleDisplayNameResult) && _instance.OnUpdateUserTitleDisplayNameResultEvent != null)
				{
					_instance.OnUpdateUserTitleDisplayNameResultEvent((UpdateUserTitleDisplayNameResult)e.Result);
				}
				else if (type2 == typeof(ValidateAmazonReceiptResult) && _instance.OnValidateAmazonIAPReceiptResultEvent != null)
				{
					_instance.OnValidateAmazonIAPReceiptResultEvent((ValidateAmazonReceiptResult)e.Result);
				}
				else if (type2 == typeof(ValidateGooglePlayPurchaseResult) && _instance.OnValidateGooglePlayPurchaseResultEvent != null)
				{
					_instance.OnValidateGooglePlayPurchaseResultEvent((ValidateGooglePlayPurchaseResult)e.Result);
				}
				else if (type2 == typeof(ValidateIOSReceiptResult) && _instance.OnValidateIOSReceiptResultEvent != null)
				{
					_instance.OnValidateIOSReceiptResultEvent((ValidateIOSReceiptResult)e.Result);
				}
				else if (type2 == typeof(ValidateWindowsReceiptResult) && _instance.OnValidateWindowsStoreReceiptResultEvent != null)
				{
					_instance.OnValidateWindowsStoreReceiptResultEvent((ValidateWindowsReceiptResult)e.Result);
				}
				else if (type2 == typeof(WriteEventResponse) && _instance.OnWriteCharacterEventResultEvent != null)
				{
					_instance.OnWriteCharacterEventResultEvent((WriteEventResponse)e.Result);
				}
				else if (type2 == typeof(WriteEventResponse) && _instance.OnWritePlayerEventResultEvent != null)
				{
					_instance.OnWritePlayerEventResultEvent((WriteEventResponse)e.Result);
				}
				else if (type2 == typeof(WriteEventResponse) && _instance.OnWriteTitleEventResultEvent != null)
				{
					_instance.OnWriteTitleEventResultEvent((WriteEventResponse)e.Result);
				}
				else if (type2 == typeof(GetEntityTokenResponse) && _instance.OnAuthenticationGetEntityTokenResultEvent != null)
				{
					_instance.OnAuthenticationGetEntityTokenResultEvent((GetEntityTokenResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.CloudScriptModels.ExecuteCloudScriptResult) && _instance.OnCloudScriptExecuteEntityCloudScriptResultEvent != null)
				{
					_instance.OnCloudScriptExecuteEntityCloudScriptResultEvent((PlayFab.CloudScriptModels.ExecuteCloudScriptResult)e.Result);
				}
				else if (type2 == typeof(AbortFileUploadsResponse) && _instance.OnDataAbortFileUploadsResultEvent != null)
				{
					_instance.OnDataAbortFileUploadsResultEvent((AbortFileUploadsResponse)e.Result);
				}
				else if (type2 == typeof(DeleteFilesResponse) && _instance.OnDataDeleteFilesResultEvent != null)
				{
					_instance.OnDataDeleteFilesResultEvent((DeleteFilesResponse)e.Result);
				}
				else if (type2 == typeof(FinalizeFileUploadsResponse) && _instance.OnDataFinalizeFileUploadsResultEvent != null)
				{
					_instance.OnDataFinalizeFileUploadsResultEvent((FinalizeFileUploadsResponse)e.Result);
				}
				else if (type2 == typeof(GetFilesResponse) && _instance.OnDataGetFilesResultEvent != null)
				{
					_instance.OnDataGetFilesResultEvent((GetFilesResponse)e.Result);
				}
				else if (type2 == typeof(GetObjectsResponse) && _instance.OnDataGetObjectsResultEvent != null)
				{
					_instance.OnDataGetObjectsResultEvent((GetObjectsResponse)e.Result);
				}
				else if (type2 == typeof(InitiateFileUploadsResponse) && _instance.OnDataInitiateFileUploadsResultEvent != null)
				{
					_instance.OnDataInitiateFileUploadsResultEvent((InitiateFileUploadsResponse)e.Result);
				}
				else if (type2 == typeof(SetObjectsResponse) && _instance.OnDataSetObjectsResultEvent != null)
				{
					_instance.OnDataSetObjectsResultEvent((SetObjectsResponse)e.Result);
				}
				else if (type2 == typeof(WriteEventsResponse) && _instance.OnEventsWriteEventsResultEvent != null)
				{
					_instance.OnEventsWriteEventsResultEvent((WriteEventsResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.GroupsModels.EmptyResponse) && _instance.OnGroupsAcceptGroupApplicationResultEvent != null)
				{
					_instance.OnGroupsAcceptGroupApplicationResultEvent((PlayFab.GroupsModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.GroupsModels.EmptyResponse) && _instance.OnGroupsAcceptGroupInvitationResultEvent != null)
				{
					_instance.OnGroupsAcceptGroupInvitationResultEvent((PlayFab.GroupsModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.GroupsModels.EmptyResponse) && _instance.OnGroupsAddMembersResultEvent != null)
				{
					_instance.OnGroupsAddMembersResultEvent((PlayFab.GroupsModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(ApplyToGroupResponse) && _instance.OnGroupsApplyToGroupResultEvent != null)
				{
					_instance.OnGroupsApplyToGroupResultEvent((ApplyToGroupResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.GroupsModels.EmptyResponse) && _instance.OnGroupsBlockEntityResultEvent != null)
				{
					_instance.OnGroupsBlockEntityResultEvent((PlayFab.GroupsModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.GroupsModels.EmptyResponse) && _instance.OnGroupsChangeMemberRoleResultEvent != null)
				{
					_instance.OnGroupsChangeMemberRoleResultEvent((PlayFab.GroupsModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(CreateGroupResponse) && _instance.OnGroupsCreateGroupResultEvent != null)
				{
					_instance.OnGroupsCreateGroupResultEvent((CreateGroupResponse)e.Result);
				}
				else if (type2 == typeof(CreateGroupRoleResponse) && _instance.OnGroupsCreateRoleResultEvent != null)
				{
					_instance.OnGroupsCreateRoleResultEvent((CreateGroupRoleResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.GroupsModels.EmptyResponse) && _instance.OnGroupsDeleteGroupResultEvent != null)
				{
					_instance.OnGroupsDeleteGroupResultEvent((PlayFab.GroupsModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.GroupsModels.EmptyResponse) && _instance.OnGroupsDeleteRoleResultEvent != null)
				{
					_instance.OnGroupsDeleteRoleResultEvent((PlayFab.GroupsModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(GetGroupResponse) && _instance.OnGroupsGetGroupResultEvent != null)
				{
					_instance.OnGroupsGetGroupResultEvent((GetGroupResponse)e.Result);
				}
				else if (type2 == typeof(InviteToGroupResponse) && _instance.OnGroupsInviteToGroupResultEvent != null)
				{
					_instance.OnGroupsInviteToGroupResultEvent((InviteToGroupResponse)e.Result);
				}
				else if (type2 == typeof(IsMemberResponse) && _instance.OnGroupsIsMemberResultEvent != null)
				{
					_instance.OnGroupsIsMemberResultEvent((IsMemberResponse)e.Result);
				}
				else if (type2 == typeof(ListGroupApplicationsResponse) && _instance.OnGroupsListGroupApplicationsResultEvent != null)
				{
					_instance.OnGroupsListGroupApplicationsResultEvent((ListGroupApplicationsResponse)e.Result);
				}
				else if (type2 == typeof(ListGroupBlocksResponse) && _instance.OnGroupsListGroupBlocksResultEvent != null)
				{
					_instance.OnGroupsListGroupBlocksResultEvent((ListGroupBlocksResponse)e.Result);
				}
				else if (type2 == typeof(ListGroupInvitationsResponse) && _instance.OnGroupsListGroupInvitationsResultEvent != null)
				{
					_instance.OnGroupsListGroupInvitationsResultEvent((ListGroupInvitationsResponse)e.Result);
				}
				else if (type2 == typeof(ListGroupMembersResponse) && _instance.OnGroupsListGroupMembersResultEvent != null)
				{
					_instance.OnGroupsListGroupMembersResultEvent((ListGroupMembersResponse)e.Result);
				}
				else if (type2 == typeof(ListMembershipResponse) && _instance.OnGroupsListMembershipResultEvent != null)
				{
					_instance.OnGroupsListMembershipResultEvent((ListMembershipResponse)e.Result);
				}
				else if (type2 == typeof(ListMembershipOpportunitiesResponse) && _instance.OnGroupsListMembershipOpportunitiesResultEvent != null)
				{
					_instance.OnGroupsListMembershipOpportunitiesResultEvent((ListMembershipOpportunitiesResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.GroupsModels.EmptyResponse) && _instance.OnGroupsRemoveGroupApplicationResultEvent != null)
				{
					_instance.OnGroupsRemoveGroupApplicationResultEvent((PlayFab.GroupsModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.GroupsModels.EmptyResponse) && _instance.OnGroupsRemoveGroupInvitationResultEvent != null)
				{
					_instance.OnGroupsRemoveGroupInvitationResultEvent((PlayFab.GroupsModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.GroupsModels.EmptyResponse) && _instance.OnGroupsRemoveMembersResultEvent != null)
				{
					_instance.OnGroupsRemoveMembersResultEvent((PlayFab.GroupsModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.GroupsModels.EmptyResponse) && _instance.OnGroupsUnblockEntityResultEvent != null)
				{
					_instance.OnGroupsUnblockEntityResultEvent((PlayFab.GroupsModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(UpdateGroupResponse) && _instance.OnGroupsUpdateGroupResultEvent != null)
				{
					_instance.OnGroupsUpdateGroupResultEvent((UpdateGroupResponse)e.Result);
				}
				else if (type2 == typeof(UpdateGroupRoleResponse) && _instance.OnGroupsUpdateRoleResultEvent != null)
				{
					_instance.OnGroupsUpdateRoleResultEvent((UpdateGroupRoleResponse)e.Result);
				}
				else if (type2 == typeof(GetLanguageListResponse) && _instance.OnLocalizationGetLanguageListResultEvent != null)
				{
					_instance.OnLocalizationGetLanguageListResultEvent((GetLanguageListResponse)e.Result);
				}
				else if (type2 == typeof(CancelAllMatchmakingTicketsForPlayerResult) && _instance.OnMultiplayerCancelAllMatchmakingTicketsForPlayerResultEvent != null)
				{
					_instance.OnMultiplayerCancelAllMatchmakingTicketsForPlayerResultEvent((CancelAllMatchmakingTicketsForPlayerResult)e.Result);
				}
				else if (type2 == typeof(CancelMatchmakingTicketResult) && _instance.OnMultiplayerCancelMatchmakingTicketResultEvent != null)
				{
					_instance.OnMultiplayerCancelMatchmakingTicketResultEvent((CancelMatchmakingTicketResult)e.Result);
				}
				else if (type2 == typeof(CreateBuildWithCustomContainerResponse) && _instance.OnMultiplayerCreateBuildWithCustomContainerResultEvent != null)
				{
					_instance.OnMultiplayerCreateBuildWithCustomContainerResultEvent((CreateBuildWithCustomContainerResponse)e.Result);
				}
				else if (type2 == typeof(CreateBuildWithManagedContainerResponse) && _instance.OnMultiplayerCreateBuildWithManagedContainerResultEvent != null)
				{
					_instance.OnMultiplayerCreateBuildWithManagedContainerResultEvent((CreateBuildWithManagedContainerResponse)e.Result);
				}
				else if (type2 == typeof(CreateMatchmakingTicketResult) && _instance.OnMultiplayerCreateMatchmakingTicketResultEvent != null)
				{
					_instance.OnMultiplayerCreateMatchmakingTicketResultEvent((CreateMatchmakingTicketResult)e.Result);
				}
				else if (type2 == typeof(CreateRemoteUserResponse) && _instance.OnMultiplayerCreateRemoteUserResultEvent != null)
				{
					_instance.OnMultiplayerCreateRemoteUserResultEvent((CreateRemoteUserResponse)e.Result);
				}
				else if (type2 == typeof(CreateMatchmakingTicketResult) && _instance.OnMultiplayerCreateServerMatchmakingTicketResultEvent != null)
				{
					_instance.OnMultiplayerCreateServerMatchmakingTicketResultEvent((CreateMatchmakingTicketResult)e.Result);
				}
				else if (type2 == typeof(PlayFab.MultiplayerModels.EmptyResponse) && _instance.OnMultiplayerDeleteAssetResultEvent != null)
				{
					_instance.OnMultiplayerDeleteAssetResultEvent((PlayFab.MultiplayerModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.MultiplayerModels.EmptyResponse) && _instance.OnMultiplayerDeleteBuildResultEvent != null)
				{
					_instance.OnMultiplayerDeleteBuildResultEvent((PlayFab.MultiplayerModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.MultiplayerModels.EmptyResponse) && _instance.OnMultiplayerDeleteCertificateResultEvent != null)
				{
					_instance.OnMultiplayerDeleteCertificateResultEvent((PlayFab.MultiplayerModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.MultiplayerModels.EmptyResponse) && _instance.OnMultiplayerDeleteRemoteUserResultEvent != null)
				{
					_instance.OnMultiplayerDeleteRemoteUserResultEvent((PlayFab.MultiplayerModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(EnableMultiplayerServersForTitleResponse) && _instance.OnMultiplayerEnableMultiplayerServersForTitleResultEvent != null)
				{
					_instance.OnMultiplayerEnableMultiplayerServersForTitleResultEvent((EnableMultiplayerServersForTitleResponse)e.Result);
				}
				else if (type2 == typeof(GetAssetUploadUrlResponse) && _instance.OnMultiplayerGetAssetUploadUrlResultEvent != null)
				{
					_instance.OnMultiplayerGetAssetUploadUrlResultEvent((GetAssetUploadUrlResponse)e.Result);
				}
				else if (type2 == typeof(GetBuildResponse) && _instance.OnMultiplayerGetBuildResultEvent != null)
				{
					_instance.OnMultiplayerGetBuildResultEvent((GetBuildResponse)e.Result);
				}
				else if (type2 == typeof(GetContainerRegistryCredentialsResponse) && _instance.OnMultiplayerGetContainerRegistryCredentialsResultEvent != null)
				{
					_instance.OnMultiplayerGetContainerRegistryCredentialsResultEvent((GetContainerRegistryCredentialsResponse)e.Result);
				}
				else if (type2 == typeof(GetMatchResult) && _instance.OnMultiplayerGetMatchResultEvent != null)
				{
					_instance.OnMultiplayerGetMatchResultEvent((GetMatchResult)e.Result);
				}
				else if (type2 == typeof(GetMatchmakingQueueResult) && _instance.OnMultiplayerGetMatchmakingQueueResultEvent != null)
				{
					_instance.OnMultiplayerGetMatchmakingQueueResultEvent((GetMatchmakingQueueResult)e.Result);
				}
				else if (type2 == typeof(GetMatchmakingTicketResult) && _instance.OnMultiplayerGetMatchmakingTicketResultEvent != null)
				{
					_instance.OnMultiplayerGetMatchmakingTicketResultEvent((GetMatchmakingTicketResult)e.Result);
				}
				else if (type2 == typeof(GetMultiplayerServerDetailsResponse) && _instance.OnMultiplayerGetMultiplayerServerDetailsResultEvent != null)
				{
					_instance.OnMultiplayerGetMultiplayerServerDetailsResultEvent((GetMultiplayerServerDetailsResponse)e.Result);
				}
				else if (type2 == typeof(GetQueueStatisticsResult) && _instance.OnMultiplayerGetQueueStatisticsResultEvent != null)
				{
					_instance.OnMultiplayerGetQueueStatisticsResultEvent((GetQueueStatisticsResult)e.Result);
				}
				else if (type2 == typeof(GetRemoteLoginEndpointResponse) && _instance.OnMultiplayerGetRemoteLoginEndpointResultEvent != null)
				{
					_instance.OnMultiplayerGetRemoteLoginEndpointResultEvent((GetRemoteLoginEndpointResponse)e.Result);
				}
				else if (type2 == typeof(GetTitleEnabledForMultiplayerServersStatusResponse) && _instance.OnMultiplayerGetTitleEnabledForMultiplayerServersStatusResultEvent != null)
				{
					_instance.OnMultiplayerGetTitleEnabledForMultiplayerServersStatusResultEvent((GetTitleEnabledForMultiplayerServersStatusResponse)e.Result);
				}
				else if (type2 == typeof(JoinMatchmakingTicketResult) && _instance.OnMultiplayerJoinMatchmakingTicketResultEvent != null)
				{
					_instance.OnMultiplayerJoinMatchmakingTicketResultEvent((JoinMatchmakingTicketResult)e.Result);
				}
				else if (type2 == typeof(ListMultiplayerServersResponse) && _instance.OnMultiplayerListArchivedMultiplayerServersResultEvent != null)
				{
					_instance.OnMultiplayerListArchivedMultiplayerServersResultEvent((ListMultiplayerServersResponse)e.Result);
				}
				else if (type2 == typeof(ListAssetSummariesResponse) && _instance.OnMultiplayerListAssetSummariesResultEvent != null)
				{
					_instance.OnMultiplayerListAssetSummariesResultEvent((ListAssetSummariesResponse)e.Result);
				}
				else if (type2 == typeof(ListBuildSummariesResponse) && _instance.OnMultiplayerListBuildSummariesResultEvent != null)
				{
					_instance.OnMultiplayerListBuildSummariesResultEvent((ListBuildSummariesResponse)e.Result);
				}
				else if (type2 == typeof(ListCertificateSummariesResponse) && _instance.OnMultiplayerListCertificateSummariesResultEvent != null)
				{
					_instance.OnMultiplayerListCertificateSummariesResultEvent((ListCertificateSummariesResponse)e.Result);
				}
				else if (type2 == typeof(ListContainerImagesResponse) && _instance.OnMultiplayerListContainerImagesResultEvent != null)
				{
					_instance.OnMultiplayerListContainerImagesResultEvent((ListContainerImagesResponse)e.Result);
				}
				else if (type2 == typeof(ListContainerImageTagsResponse) && _instance.OnMultiplayerListContainerImageTagsResultEvent != null)
				{
					_instance.OnMultiplayerListContainerImageTagsResultEvent((ListContainerImageTagsResponse)e.Result);
				}
				else if (type2 == typeof(ListMatchmakingQueuesResult) && _instance.OnMultiplayerListMatchmakingQueuesResultEvent != null)
				{
					_instance.OnMultiplayerListMatchmakingQueuesResultEvent((ListMatchmakingQueuesResult)e.Result);
				}
				else if (type2 == typeof(ListMatchmakingTicketsForPlayerResult) && _instance.OnMultiplayerListMatchmakingTicketsForPlayerResultEvent != null)
				{
					_instance.OnMultiplayerListMatchmakingTicketsForPlayerResultEvent((ListMatchmakingTicketsForPlayerResult)e.Result);
				}
				else if (type2 == typeof(ListMultiplayerServersResponse) && _instance.OnMultiplayerListMultiplayerServersResultEvent != null)
				{
					_instance.OnMultiplayerListMultiplayerServersResultEvent((ListMultiplayerServersResponse)e.Result);
				}
				else if (type2 == typeof(ListQosServersResponse) && _instance.OnMultiplayerListQosServersResultEvent != null)
				{
					_instance.OnMultiplayerListQosServersResultEvent((ListQosServersResponse)e.Result);
				}
				else if (type2 == typeof(ListVirtualMachineSummariesResponse) && _instance.OnMultiplayerListVirtualMachineSummariesResultEvent != null)
				{
					_instance.OnMultiplayerListVirtualMachineSummariesResultEvent((ListVirtualMachineSummariesResponse)e.Result);
				}
				else if (type2 == typeof(RemoveMatchmakingQueueResult) && _instance.OnMultiplayerRemoveMatchmakingQueueResultEvent != null)
				{
					_instance.OnMultiplayerRemoveMatchmakingQueueResultEvent((RemoveMatchmakingQueueResult)e.Result);
				}
				else if (type2 == typeof(RequestMultiplayerServerResponse) && _instance.OnMultiplayerRequestMultiplayerServerResultEvent != null)
				{
					_instance.OnMultiplayerRequestMultiplayerServerResultEvent((RequestMultiplayerServerResponse)e.Result);
				}
				else if (type2 == typeof(RolloverContainerRegistryCredentialsResponse) && _instance.OnMultiplayerRolloverContainerRegistryCredentialsResultEvent != null)
				{
					_instance.OnMultiplayerRolloverContainerRegistryCredentialsResultEvent((RolloverContainerRegistryCredentialsResponse)e.Result);
				}
				else if (type2 == typeof(SetMatchmakingQueueResult) && _instance.OnMultiplayerSetMatchmakingQueueResultEvent != null)
				{
					_instance.OnMultiplayerSetMatchmakingQueueResultEvent((SetMatchmakingQueueResult)e.Result);
				}
				else if (type2 == typeof(PlayFab.MultiplayerModels.EmptyResponse) && _instance.OnMultiplayerShutdownMultiplayerServerResultEvent != null)
				{
					_instance.OnMultiplayerShutdownMultiplayerServerResultEvent((PlayFab.MultiplayerModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.MultiplayerModels.EmptyResponse) && _instance.OnMultiplayerUpdateBuildRegionsResultEvent != null)
				{
					_instance.OnMultiplayerUpdateBuildRegionsResultEvent((PlayFab.MultiplayerModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(PlayFab.MultiplayerModels.EmptyResponse) && _instance.OnMultiplayerUploadCertificateResultEvent != null)
				{
					_instance.OnMultiplayerUploadCertificateResultEvent((PlayFab.MultiplayerModels.EmptyResponse)e.Result);
				}
				else if (type2 == typeof(GetGlobalPolicyResponse) && _instance.OnProfilesGetGlobalPolicyResultEvent != null)
				{
					_instance.OnProfilesGetGlobalPolicyResultEvent((GetGlobalPolicyResponse)e.Result);
				}
				else if (type2 == typeof(GetEntityProfileResponse) && _instance.OnProfilesGetProfileResultEvent != null)
				{
					_instance.OnProfilesGetProfileResultEvent((GetEntityProfileResponse)e.Result);
				}
				else if (type2 == typeof(GetEntityProfilesResponse) && _instance.OnProfilesGetProfilesResultEvent != null)
				{
					_instance.OnProfilesGetProfilesResultEvent((GetEntityProfilesResponse)e.Result);
				}
				else if (type2 == typeof(SetGlobalPolicyResponse) && _instance.OnProfilesSetGlobalPolicyResultEvent != null)
				{
					_instance.OnProfilesSetGlobalPolicyResultEvent((SetGlobalPolicyResponse)e.Result);
				}
				else if (type2 == typeof(SetProfileLanguageResponse) && _instance.OnProfilesSetProfileLanguageResultEvent != null)
				{
					_instance.OnProfilesSetProfileLanguageResultEvent((SetProfileLanguageResponse)e.Result);
				}
				else if (type2 == typeof(SetEntityProfilePolicyResponse) && _instance.OnProfilesSetProfilePolicyResultEvent != null)
				{
					_instance.OnProfilesSetProfilePolicyResultEvent((SetEntityProfilePolicyResponse)e.Result);
				}
			}
		}
	}
}
