using Zenject;

namespace BSCore.Chat
{
	public class ChatInstaller : Installer<ChatInstaller>
	{
		public override void InstallBindings()
		{
			base.Container.Bind<ChatClient>().AsSingle().NonLazy();
			base.Container.Bind<ChatCommandController>().AsSingle();
		}
	}
}
