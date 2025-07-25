using BSCore;
using UnityEngine;
using Zenject;

public class WeaponPickupEntity : BasePickupEntity<IWeaponPickupState>
{
	[Inject]
	private ProfileManager _profileManager;

	[SerializeField]
	private Transform _effectSpawnPoint;

	public ProfileWithHeroClass PickupProfile { get; private set; }

	public static void SpawnPickup(string itemId, PickupSpawnPoint spawnPoint)
	{
		SpawnPickup(itemId, spawnPoint.Position);
	}

	public static void SpawnPickup(string itemId, Vector3 position)
	{
		BasePickupEntity<IWeaponPickupState>.SpawnPickup(BoltPrefabs.WeaponPickupEntity, position, Quaternion.identity, PickupType.Weapon).GetState<IWeaponPickupState>().ItemId = itemId;
	}

	protected override void SpawnModel(string itemId)
	{
		ProfileWithHeroClass byId = _profileManager.GetById<ProfileWithHeroClass>(itemId);
		if (byId == null)
		{
			Debug.LogErrorFormat("Unable to setup pickup. Unable to get itemProfile for Id {0}", itemId);
		}
		else
		{
			PickupProfile = byId;
			SpawnModel(PickupProfile);
			SpawnPickupEffect(PickupProfile);
		}
	}

	private void SpawnPickupEffect(ProfileWithHeroClass profile)
	{
		GameObject gameObject = Resources.Load<GameObject>(string.Concat(profile.Rarity, "PickupEffect"));
		if (gameObject != null)
		{
			Object.Instantiate(gameObject, _effectSpawnPoint);
		}
	}
}
