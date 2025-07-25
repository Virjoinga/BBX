using Zenject;

namespace NodeClient
{
	public class SocketInstaller : Installer<SocketInstaller>
	{
		public override void InstallBindings()
		{
			base.Container.Bind<SocketClient>().AsSingle().NonLazy();
			base.Container.DeclareSignal<SocketErrorSignal>().OptionalSubscriber();
			base.Container.DeclareSignal<SocketConnectedSignal>().OptionalSubscriber();
			base.Container.DeclareSignal<SocketReconnectedSignal>().OptionalSubscriber();
			base.Container.DeclareSignal<SocketAuthenticatedSignal>().OptionalSubscriber();
			base.Container.DeclareSignal<SocketReconnectingSignal>().OptionalSubscriber();
			base.Container.DeclareSignal<SocketDisconnectedSignal>().OptionalSubscriber();
			base.Container.DeclareSignal<SocketChannelJoinedSignal>().OptionalSubscriber();
			base.Container.DeclareSignal<SocketChannelLeftSignal>().OptionalSubscriber();
			base.Container.DeclareSignal<StatusUpdatedSignal>().OptionalSubscriber();
		}

		public static void Destroy(DiContainer Container)
		{
			Container.Resolve<SocketClient>().Disconnect();
		}
	}
}
