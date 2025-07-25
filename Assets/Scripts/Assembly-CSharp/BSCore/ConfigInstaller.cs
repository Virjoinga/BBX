using Zenject;

namespace BSCore
{
	public class ConfigInstaller : Installer<ConfigInstaller>
	{
		public override void InstallBindings()
		{
			base.Container.Bind<ConfigManager>().AsSingle();
			base.Container.Bind<PickupConfig>().AsSingle();
		}
	}
}
