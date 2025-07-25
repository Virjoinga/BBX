using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviourSingleton<SpawnManager>
{
	[SerializeField]
	protected List<PlayerSpawnPoint> _playerSpawnPoints = new List<PlayerSpawnPoint>();

	[SerializeField]
	protected List<PickupSpawnPoint> _pickupSpawnPoints = new List<PickupSpawnPoint>();

	private List<PickupSpawnPoint> _remainingPickupSpawnPoints;

	protected Dictionary<TeamId, List<Transform>> _playerTransformsByTeam = new Dictionary<TeamId, List<Transform>>();

	public List<PickupSpawnPoint> PickupSpawnPoints => _pickupSpawnPoints;

	public List<PlayerSpawnPoint> PlayerSpawnPoints => _playerSpawnPoints;

	protected override void Awake()
	{
		base.Awake();
		_remainingPickupSpawnPoints = new List<PickupSpawnPoint>(_pickupSpawnPoints);
	}

	public void RegisterPlayerTransfrom(Transform playerTransform, TeamId teamId)
	{
		Debug.Log($"[SpawnManager] Registering Player {playerTransform.name} on team {teamId}");
		if (!_playerTransformsByTeam.ContainsKey(teamId))
		{
			_playerTransformsByTeam[teamId] = new List<Transform>();
		}
		_playerTransformsByTeam[teamId].Add(playerTransform);
	}

	public void DeRegisterPlayerTransform(Transform playerTransform, TeamId teamId)
	{
		if (_playerTransformsByTeam.ContainsKey(teamId))
		{
			_playerTransformsByTeam[teamId].RemoveAll((Transform x) => x == playerTransform);
		}
	}

	public void ResetSpawnPointCooldowns()
	{
		foreach (PlayerSpawnPoint playerSpawnPoint in _playerSpawnPoints)
		{
			playerSpawnPoint.ResetCooldown();
		}
	}

	public virtual PlayerSpawnPoint GetRandomPlayerSpawnPoint()
	{
		return _playerSpawnPoints[Random.Range(0, _playerSpawnPoints.Count)];
	}

	public virtual PlayerSpawnPoint GetRandomPlayerSpawnPointForTeam(TeamId team, bool forceTeamSpawn = false)
	{
		List<PlayerSpawnPoint> list = _playerSpawnPoints.Where((PlayerSpawnPoint x) => x.Team == team).ToList();
		return _playerSpawnPoints[Random.Range(0, list.Count)];
	}

	public virtual PlayerSpawnPoint GetSpawnPointById(string uniqueId, TeamId team)
	{
		Debug.Log("[SpawnManager] Fetching spawn point with Id " + uniqueId);
		PlayerSpawnPoint playerSpawnPoint = _playerSpawnPoints.FirstOrDefault((PlayerSpawnPoint x) => x.UniqueId == uniqueId);
		if (playerSpawnPoint == null)
		{
			Debug.LogError("[SpawnManager] Failed to get spawnpoint with Id " + uniqueId + ". Getting Random Team Spawn");
			return GetRandomPlayerSpawnPointForTeam(team);
		}
		return playerSpawnPoint;
	}

	public virtual PickupSpawnPoint GetRandomPickupSpawnPoint()
	{
		return _remainingPickupSpawnPoints[Random.Range(0, _remainingPickupSpawnPoints.Count)];
	}

	public virtual PickupSpawnPoint GetNextPickupSpawnPoint()
	{
		if (_remainingPickupSpawnPoints.Count < 1)
		{
			_remainingPickupSpawnPoints = new List<PickupSpawnPoint>(_pickupSpawnPoints);
		}
		PickupSpawnPoint pickupSpawnPoint = _remainingPickupSpawnPoints[Random.Range(0, _remainingPickupSpawnPoints.Count)];
		_remainingPickupSpawnPoints.Remove(pickupSpawnPoint);
		return pickupSpawnPoint;
	}
}
