using Zenject;

namespace BSCore
{
	public class CloudCodeInstaller : Installer<CloudCodeInstaller>
	{
		public override void InstallBindings()
		{
			base.Container.Bind<CloudCodeManager>().AsSingle();
		}
	}
}
