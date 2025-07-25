using BSCore;
using BSCore.Chat;
using NodeClient;
using PlayFab;
using Zenject;

public class BB2ClientServiceInstaller : ClientServiceInstaller
{
	public override void InstallBindings()
	{
		base.Container.DefaultParent = null;
		PlayFabSettings.TitleId = _gameConfig.GetTitleId();
		BoltNetwork.SetPrefabPool(new PrefabInstantiator(base.Container));
		base.Container.Bind<BaseProfileFactory>().To<ProfileFactory>().WhenInjectedInto<ClientPlayFabGameItemService>();
		base.Container.Bind<MenuLoadoutManager>().AsSingle();
		base.Container.Bind<MatchMakerApi>().AsSingle();
		base.Container.Bind<SkyVuMatchmaker>().AsSingle();
		base.Container.Bind<TeamDeathMatchHelper>().AsSingle();
		base.Container.DeclareSignal<CameraBlockingEffectUpdatedSignal>();
		base.Container.DeclareSignal<TrippingEffectUpdatedSignal>();
		base.Container.DeclareSignal<DeployableCreatedDestroyedSignal>();
		base.Container.DeclareSignal<LocalPlayerRespawnDataUpdatedSignal>();
		base.Container.DeclareSignal<ClientRequestRespawnSignal>();
		base.Container.DeclareSignal<ClientUpdateRespawnSelectionsSignal>();
		base.Container.DeclareSignal<LocalPlayerDiedSignal>();
		base.Container.DeclareSignal<LocalPlayerRespawnedSignal>();
		base.Container.DeclareSignal<RequestEmoticonSignal>();
		base.Container.DeclareSignal<ShowEmoticonSignal>();
		BindUISignals();
		base.InstallBindings();
		Installer<SocketInstaller>.Install(base.Container);
		Installer<ChatInstaller>.Install(base.Container);
	}

	private void BindUISignals()
	{
		base.Container.DeclareSignal<InRangeOfPickupSignal>();
		base.Container.DeclareSignal<OutOfRangeOfPickupSignal>();
		base.Container.DeclareSignal<TryPickupItemSignal>();
		base.Container.DeclareSignal<PlayerHealthChangedSignal>();
		base.Container.DeclareSignal<WeaponStateUpdatedSignal>();
		base.Container.DeclareSignal<ChargeValueUpdatedSignal>().OptionalSubscriber();
		base.Container.DeclareSignal<ShotPathData>().OptionalSubscriber();
		base.Container.DeclareSignal<LoadoutUpdatedSignal>().OptionalSubscriber();
		base.Container.DeclareSignal<AmmoClipsUpdatedSignal>().OptionalSubscriber();
		base.Container.DeclareSignal<WeaponSlotSelectedSignal>();
		base.Container.DeclareSignal<ActiveWeaponSlotUpdatedSignal>();
		base.Container.DeclareSignal<WeaponCurrentSpreadUpdated>();
		base.Container.DeclareSignal<SlideOutMenuUISignal>();
		base.Container.DeclareSignal<SecondLifeButtonClickedSignal>();
		base.Container.DeclareSignal<BattleRoyaleRemainingPlayersUpdatedSignal>();
		base.Container.DeclareSignal<MatchStateUpdatedSignal>();
		base.Container.DeclareSignal<DamagableDamagedSignal>();
		base.Container.DeclareSignal<ClientPlayerDiedSignal>();
		base.Container.DeclareSignal<PlayerEliminatedSignal>();
		base.Container.DeclareSignal<NotificationSignal>();
		base.Container.DeclareSignal<EnterExitClosedZoneSignal>();
		base.Container.DeclareSignal<MatchTimesUpdatedSignal>();
		base.Container.DeclareSignal<TDMScoresUpdatedSignal>();
		base.Container.DeclareSignal<TDMPlayersUpdatedSignal>();
		base.Container.DeclareSignal<MatchExpectedPlayerCountUpdatedSignal>();
		base.Container.DeclareSignal<MatchPlayersLoadedUpdatedSignal>();
		base.Container.DeclareSignal<StatusFlagsupdatedSignal>();
		base.Container.DeclareSignal<SpecialAbilityStateUpdatedSignal>().OptionalSubscriber();
		base.Container.DeclareSignal<TryRequestRespawn>();
		base.Container.DeclareSignal<SurvivalStateUpdatedSignal>();
		base.Container.DeclareSignal<ClientPlayerLoadedSignal>();
		base.Container.DeclareSignal<LeaderboardUpdatedSignal>().OptionalSubscriber();
		base.Container.DeclareSignal<LeaderboardInstantiatedSignal>().OptionalSubscriber();
		base.Container.DeclareSignal<ReloadStateUpdatedSignal>().OptionalSubscriber();
		base.Container.DeclareSignal<SpawnPointPickerToggledUISignal>();
	}

	private void OnDestroy()
	{
		SocketInstaller.Destroy(base.Container);
	}
}
