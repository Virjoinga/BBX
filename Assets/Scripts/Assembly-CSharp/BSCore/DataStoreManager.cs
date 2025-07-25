using Zenject;

namespace BSCore
{
	public class DataStoreManager
	{
		private DiContainer _container;

		[Inject]
		public DataStoreManager(DiContainer container)
		{
			_container = container;
		}

		public T1 GetStore<T1, T2>(DataStoreKeys key) where T1 : BaseDataStoreService<T2>
		{
			return _container.TryResolveId<T1>(key);
		}
	}
}
