using Zenject;

namespace BSCore
{
	public class RemoteFileInstaller : Installer<RemoteFileInstaller>
	{
		public override void InstallBindings()
		{
			base.Container.Bind<RemoteFileManager>().AsSingle();
		}
	}
}
