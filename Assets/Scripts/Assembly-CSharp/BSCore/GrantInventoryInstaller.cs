using Zenject;

namespace BSCore
{
	public class GrantInventoryInstaller : Installer<GrantInventoryInstaller>
	{
		public override void InstallBindings()
		{
			base.Container.Bind<GrantInventoryManager>().AsSingle();
		}
	}
}
