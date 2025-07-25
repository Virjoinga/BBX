using System;
using System.Collections;
using BSCore;
using Bolt;
using UnityEngine;
using Zenject;

public abstract class BasePickupEntity<TBasePickupState> : BaseEntityBehaviour<TBasePickupState> where TBasePickupState : IBasePickupState, IState, IDisposable
{
	protected enum State
	{
		Unbroken = 0,
		Revealed = 1,
		Consumed = 2
	}

	[Inject]
	protected PickupConfig _pickupConfig;

	[SerializeField]
	protected Transform _modelSpawnPoint;

	[SerializeField]
	protected GameObject _particleEffect;

	[SerializeField]
	protected bool _refills;

	[SerializeField]
	protected Collider _pickupTrigger;

	protected PickupData _pickupData;

	protected float _refillCooldown;

	public bool IsActive => base.state.ActiveState == 1;

	public static BoltEntity SpawnPickup(PrefabId prefabId, PickupSpawnPoint spawnPoint, PickupType pickupType)
	{
		return SpawnPickup(prefabId, spawnPoint.Position, spawnPoint.Rotation, pickupType);
	}

	public static BoltEntity SpawnPickup(PrefabId prefabId, Vector3 position, Quaternion rotation, PickupType pickupType)
	{
		BoltEntity result = BoltNetwork.Instantiate(prefabId, position, rotation);
		SetBasePickupProperties(result, pickupType);
		return result;
	}

	private static void SetBasePickupProperties(BoltEntity entity, PickupType pickupType)
	{
		TBasePickupState val = entity.GetState<TBasePickupState>();
		val.ActiveState = 1;
		val.Type = (int)pickupType;
	}

	protected virtual void Reset()
	{
	}

	protected override void OnAnyAttached()
	{
		base.state.SetTransforms(base.state.Transform, base.transform);
		base.state.AddCallback("ActiveState", OnActiveStateUpdated);
		OnActiveStateUpdated();
		base.state.AddCallback("Type", OnTypeChanged);
		OnTypeChanged();
	}

	protected override void OnAnyDetached()
	{
		base.state.RemoveCallback("ActiveState", OnActiveStateUpdated);
		base.state.RemoveCallback("Type", OnTypeChanged);
	}

	private void OnTypeChanged()
	{
		_pickupData = _pickupConfig.GetByType((PickupType)base.state.Type);
		_modelSpawnPoint.DestroyChildren();
		SetRefillTimer(_pickupData.Cooldown);
		if (!base.entity.isOwner)
		{
			SpawnModel(_pickupData.PrefabPath);
		}
	}

	public void SetRefillTimer(float refillCooldown)
	{
		_refillCooldown = refillCooldown;
	}

	public virtual bool TryClaimPickup()
	{
		if (IsActive)
		{
			TBasePickupState val = base.state;
			val.ActiveState = 2;
			return true;
		}
		return false;
	}

	protected virtual void OnActiveStateUpdated()
	{
		Debug.Log($"[BasePickupEntity] ActiveState changed to {(State)base.state.ActiveState}", base.gameObject);
		_modelSpawnPoint.gameObject.SetActive(IsActive);
		_particleEffect.gameObject.SetActive(IsActive);
	}

	protected virtual void SpawnModel(ProfileWithHeroClass profile)
	{
		OnPrefabLoaded(PrefabManager.Request(profile));
	}

	protected virtual void SpawnModel(string path)
	{
		Debug.Log("[BasePickupEntity] Spawning model at path: " + path, base.gameObject);
		OnPrefabLoaded(PrefabManager.RequestViaPath(path));
	}

	private void OnPrefabLoaded(GameObject prefab)
	{
		if (prefab != null)
		{
			GameObject obj = UnityEngine.Object.Instantiate(prefab, _modelSpawnPoint);
			UnityEngine.Object.Destroy(obj.GetComponentInChildren<BaseWeapon>());
			UnityEngine.Object.Destroy(obj.GetComponentInChildren<LaunchPoint>());
			base.name = "ItemPickup-" + prefab.name;
		}
	}

	protected virtual void StartRefill()
	{
		StartCoroutine(RefillRoutine());
	}

	private IEnumerator RefillRoutine()
	{
		yield return new WaitForSeconds(_refillCooldown);
		Refill();
	}

	protected virtual void Refill()
	{
		TBasePickupState val = base.state;
		val.ActiveState = 1;
	}
}
