using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BRSpawnManager : SpawnManager
{
	[SerializeField]
	private List<SecondLifeSpawnPoint> _2ndLifeSpawnPoints;

	private List<PlayerSpawnPoint> _originalSpawnPoints;

	public static PlayerSpawnPoint Get2ndLifeSpawnPoint()
	{
		return (MonoBehaviourSingleton<SpawnManager>.Instance as BRSpawnManager).Get2ndLifeSpawnPointInternal();
	}

	protected override void Awake()
	{
		base.Awake();
		_originalSpawnPoints = new List<PlayerSpawnPoint>(_playerSpawnPoints);
	}

	public override PlayerSpawnPoint GetRandomPlayerSpawnPoint()
	{
		if (_playerSpawnPoints.Count < 1)
		{
			_playerSpawnPoints = new List<PlayerSpawnPoint>(_originalSpawnPoints);
		}
		PlayerSpawnPoint randomPlayerSpawnPoint = base.GetRandomPlayerSpawnPoint();
		_playerSpawnPoints.Remove(randomPlayerSpawnPoint);
		return randomPlayerSpawnPoint;
	}

	private PlayerSpawnPoint Get2ndLifeSpawnPointInternal()
	{
		List<BRZoneEntity> nonOpenZones = BRZonesController.GetNonOpenZones();
		if (nonOpenZones.Count == 0)
		{
			return _2ndLifeSpawnPoints[Random.Range(0, _2ndLifeSpawnPoints.Count)];
		}
		List<SecondLifeSpawnPoint> list = _2ndLifeSpawnPoints.Where((SecondLifeSpawnPoint sp) => nonOpenZones.Contains(sp.Zone) && !nonOpenZones.Contains(sp.OtherZone)).ToList();
		return list[Random.Range(0, list.Count)];
	}

	[ContextMenu("Arrange Spawn Points")]
	private void ArrangeSpawnPoints()
	{
		float num = 170f;
		float num2 = 360f / (float)_playerSpawnPoints.Count;
		for (int i = 0; i < _playerSpawnPoints.Count; i++)
		{
			_playerSpawnPoints[i].transform.localPosition = new Vector3(num * Mathf.Sin(num2 * (float)i), 0f, num * Mathf.Cos(num2 * (float)i));
			_playerSpawnPoints[i].transform.LookAt(base.transform);
		}
	}
}
