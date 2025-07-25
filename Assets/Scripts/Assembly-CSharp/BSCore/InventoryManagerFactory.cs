using Zenject;

namespace BSCore
{
	public class InventoryManagerFactory : IFactory<string, InventoryManager>, IFactory
	{
		private DiContainer _container;

		public InventoryManagerFactory(DiContainer container)
		{
			_container = container;
		}

		public InventoryManager Create(string serviceId)
		{
			return _container.Instantiate<InventoryManager>(new object[1] { serviceId });
		}
	}
}
