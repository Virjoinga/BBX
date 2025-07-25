using Zenject;

namespace BSCore
{
	public class UserInstaller : Installer<UserInstaller>
	{
		public override void InstallBindings()
		{
			base.Container.Bind<UserManager>().AsSingle();
		}
	}
}
