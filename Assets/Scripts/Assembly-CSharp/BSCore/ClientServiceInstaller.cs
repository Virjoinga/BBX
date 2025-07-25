using UnityEngine;
using Zenject;

namespace BSCore
{
	public class ClientServiceInstaller : MonoInstaller<ClientServiceInstaller>
	{
		[SerializeField]
		protected GameConfigData _gameConfig;

		[SerializeField]
		protected EmoticonData _emoticonData;

		[SerializeField]
		private BackgroundMusicManager _backgroundMusicManagerPrefab;

		public override void InstallBindings()
		{
			Installer<SignalBusInstaller>.Install(base.Container);
			Installer<UserInstaller>.Install(base.Container);
			Installer<ProfileInstaller>.Install(base.Container);
			Installer<ConfigInstaller>.Install(base.Container);
			Installer<InventoryInstaller>.Install(base.Container);
			Installer<CloudCodeInstaller>.Install(base.Container);
			Installer<DataStoreInstaller>.Install(base.Container);
			base.Container.Bind<ILoginService>().To<ClientPlayFabLoginService>().WhenInjectedInto<LoginManager>();
			base.Container.Bind<IUserService>().To<ClientPlayFabUserService>().WhenInjectedInto<UserManager>();
			base.Container.BindInstance(_gameConfig).AsSingle();
			base.Container.BindInstance(_emoticonData).AsSingle();
			base.Container.Bind<IGameItemService>().To<ClientPlayFabGameItemService>().WhenInjectedInto<ProfileManager>();
			base.Container.Bind<IInventoryService>().To<ClientPlayFabInventoryService>().WhenInjectedInto<InventoryManager>();
			base.Container.Bind<IConfigService>().To<ClientPlayFabConfigService>().WhenInjectedInto<ConfigManager>();
			base.Container.Bind<IPurchasingService>().To<PlayFabPurchasingService>().WhenInjectedInto<PurchasingManager>();
			base.Container.Bind<ICloudCodeService>().To<ClientPlayFabCloudCodeService>().WhenInjectedInto<CloudCodeManager>();
			base.Container.Bind<IStoreService>().To<PlayFabStoreService>().WhenInjectedInto<StoreManager>();
			base.Container.Bind<IRemoteFileService>().To<PlayFabRemoteFileService>().WhenInjectedInto<RemoteFileManager>();
			base.Container.Bind<IStatisticsService>().To<ClientPlayFabStatisticsService>().WhenInjectedInto<StatisticsManager>();
			base.Container.Bind<ILeaderboardService>().To<ClientPlayFabLeaderboardService>().WhenInjectedInto<LeaderboardManager>();
			base.Container.Bind<PlayFabFriendsService>().WhenInjectedInto<ClientFriendsManager>();
			base.Container.BindInterfacesTo<ActiveUI.Manager>().AsSingle();
			base.Container.Bind<ZenjectInstantiater>().AsSingle();
			Installer<RemoteFileInstaller>.Install(base.Container);
			base.Container.DeclareSignal<LoginManager.LoggedInSignal>();
			base.Container.Bind<BackgroundMusicManager>().FromComponentInNewPrefab(_backgroundMusicManagerPrefab).AsSingle();
			base.Container.BindInterfacesAndSelfTo<AudioMixerManager>().AsSingle();
			base.Container.Bind<StatisticsManager>().AsSingle();
			base.Container.Bind<LeaderboardManager>().AsSingle();
			base.Container.Bind<LoginManager>().AsSingle();
			base.Container.Bind<InventoryManager>().AsSingle();
			base.Container.Bind<PurchasingManager>().AsSingle();
			base.Container.Bind<StoreManager>().AsSingle();
			base.Container.Bind<ClientFriendsManager>().AsSingle().NonLazy();
			base.Container.Bind<PlayfabMatchmaker>().AsSingle();
			base.Container.BindInterfacesAndSelfTo<ExperienceManager>().AsSingle();
			base.Container.BindInterfacesAndSelfTo<GraphicSettingsManager>().AsSingle();
			base.Container.Bind<GroupManager>().AsSingle().NonLazy();
			base.Container.Bind<SteamAbstractionLayer>().AsSingle();
			base.Container.Bind<SteamIAPManager>().AsSingle();
			base.Container.DeclareSignal<SteamAbstractionLayer.OverlayStateChangedSignal>();
		}
	}
}
