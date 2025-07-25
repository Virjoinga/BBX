using System.Collections.Generic;
using System.Linq;

public class DeployableController
{
	private struct SpawnedDeployableData
	{
		public WeaponProfile WeaponProfile;

		public LinkedList<BoltEntity> SpawnedEntities;

		public int SpawnedCount => SpawnedEntities.Count;

		public int MaxSpawnCount => WeaponProfile.SpawnedEntity.MaxDeployedCount;

		public SpawnedDeployableData(WeaponProfile weaponProfile)
		{
			WeaponProfile = weaponProfile;
			SpawnedEntities = new LinkedList<BoltEntity>();
		}
	}

	private Dictionary<string, SpawnedDeployableData> _deployableDataById = new Dictionary<string, SpawnedDeployableData>();

	public void TrackDeployable(WeaponProfile weaponProfile, BoltEntity entity)
	{
		if (!_deployableDataById.ContainsKey(weaponProfile.Id))
		{
			_deployableDataById.Add(weaponProfile.Id, new SpawnedDeployableData(weaponProfile));
		}
		ClearDestroyedEntities(weaponProfile.Id);
		_deployableDataById[weaponProfile.Id].SpawnedEntities.AddLast(entity);
		if (_deployableDataById[weaponProfile.Id].SpawnedCount > _deployableDataById[weaponProfile.Id].MaxSpawnCount)
		{
			BoltEntity value = _deployableDataById[weaponProfile.Id].SpawnedEntities.First.Value;
			_deployableDataById[weaponProfile.Id].SpawnedEntities.RemoveFirst();
			BoltNetwork.Destroy(value);
		}
	}

	private void ClearDestroyedEntities(string weaponId)
	{
		foreach (BoltEntity item in _deployableDataById[weaponId].SpawnedEntities.ToList())
		{
			if (item == null || item.gameObject == null)
			{
				_deployableDataById[weaponId].SpawnedEntities.Remove(item);
			}
		}
	}
}
