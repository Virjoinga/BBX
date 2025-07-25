using Zenject;

namespace BSCore
{
	public class ProfileInstaller : Installer<ProfileInstaller>
	{
		public override void InstallBindings()
		{
			base.Container.Bind<ProfileManager>().AsSingle();
		}
	}
}
