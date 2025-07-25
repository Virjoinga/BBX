using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace BSCore
{
	public class ProfileManager
	{
		private IGameItemService _gameItemService;

		private Dictionary<string, BaseProfile> _profileById = new Dictionary<string, BaseProfile>();

		public bool HasFetched { get; private set; }

		public List<BaseProfile> AllProfiles => new List<BaseProfile>(_profileById.Values);

		[Inject]
		public ProfileManager(IGameItemService gameItemService)
		{
			_gameItemService = gameItemService;
		}

		public void Fetch(string catalogName, Action<List<BaseProfile>> onSuccess, Action<FailureReasons> onFailure)
		{
			_gameItemService.Fetch(catalogName, onSuccessWrapper, onFailure);
			void onSuccessWrapper(Dictionary<string, BaseProfile> profiles)
			{
				OnProfilesFetched(profiles);
				onSuccess(profiles.Values.ToList());
			}
		}

		private void OnProfilesFetched(Dictionary<string, BaseProfile> profiles)
		{
			_profileById = profiles;
			HasFetched = true;
		}

		public BaseProfile GetById(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			BaseProfile value = null;
			_profileById.TryGetValue(id, out value);
			return value;
		}

		public T GetById<T>(string id) where T : BaseProfile
		{
			return GetById(id) as T;
		}

		public List<T> GetAllOfType<T>() where T : BaseProfile
		{
			return (from p in _profileById.Values
				where p is T
				select p as T).ToList();
		}
	}
}
