using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TDMSpawnManager : SpawnManager
{
	private const int MAX_SPAWNS_TO_CHECK = 3;

	private Dictionary<TeamId, List<PlayerSpawnPoint>> _spawnPointsByTeam;

	protected override void Awake()
	{
		base.Awake();
		PopulateSpawnPointsByTeam();
	}

	private void PopulateSpawnPointsByTeam()
	{
		_spawnPointsByTeam = new Dictionary<TeamId, List<PlayerSpawnPoint>>();
		foreach (PlayerSpawnPoint playerSpawnPoint in _playerSpawnPoints)
		{
			if (!_spawnPointsByTeam.ContainsKey(playerSpawnPoint.Team))
			{
				_spawnPointsByTeam[playerSpawnPoint.Team] = new List<PlayerSpawnPoint>();
			}
			_spawnPointsByTeam[playerSpawnPoint.Team].Add(playerSpawnPoint);
		}
	}

	public override PlayerSpawnPoint GetRandomPlayerSpawnPointForTeam(TeamId team, bool forcePrimaryTeamSpawn = false)
	{
		if (forcePrimaryTeamSpawn)
		{
			if (_spawnPointsByTeam.ContainsKey(team) && _spawnPointsByTeam[team].Count > 0)
			{
				List<PlayerSpawnPoint> source = _spawnPointsByTeam[team];
				source = source.Where((PlayerSpawnPoint x) => x.PrimarySpawn).ToList();
				return TryGetClearSpawn(team, source);
			}
			Debug.LogError($"[TDMSpawnManager] No Team Spawns found for Team! {team}");
		}
		List<PlayerSpawnPoint> list = new List<PlayerSpawnPoint>();
		foreach (KeyValuePair<TeamId, List<PlayerSpawnPoint>> item in _spawnPointsByTeam)
		{
			if ((item.Key == team || item.Key == TeamId.Neutral) && item.Value.Count > 0)
			{
				list.AddRange(item.Value);
			}
		}
		if (list.Count <= 0)
		{
			Debug.LogError("[TDMSpawnManager] Error no spawn points setup!");
			return null;
		}
		return GetFurthestSpawnPoint(team, list);
	}

	private PlayerSpawnPoint TryGetClearSpawn(TeamId team, List<PlayerSpawnPoint> spawnPoints)
	{
		List<PlayerSpawnPoint> list = new List<PlayerSpawnPoint>();
		foreach (PlayerSpawnPoint spawnPoint in spawnPoints)
		{
			if (SpawnPointIsClear(team, spawnPoint))
			{
				list.Add(spawnPoint);
			}
		}
		if (list.Count == 0)
		{
			list = spawnPoints;
		}
		int index = Random.Range(0, list.Count);
		PlayerSpawnPoint playerSpawnPoint = list[index];
		playerSpawnPoint.StartSpawnCooldown();
		return playerSpawnPoint;
	}

	private bool SpawnPointIsClear(TeamId team, PlayerSpawnPoint spawnPoint)
	{
		if (spawnPoint.SpawnCooldownActive)
		{
			return false;
		}
		Collider[] array = Physics.OverlapSphere(spawnPoint.transform.position, spawnPoint.SphereOfInfluence, 9);
		if (array != null && array.Length != 0)
		{
			Collider[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				PlayerController component = array2[i].GetComponent<PlayerController>();
				if (component != null && component.Team != (int)team && component.IsAlive)
				{
					return false;
				}
			}
		}
		return true;
	}

	private PlayerSpawnPoint GetFurthestSpawnPoint(TeamId team, List<PlayerSpawnPoint> spawnPoints)
	{
		Dictionary<PlayerSpawnPoint, float> dictionary = new Dictionary<PlayerSpawnPoint, float>();
		foreach (PlayerSpawnPoint spawnPoint in spawnPoints)
		{
			List<Transform> list = new List<Transform>();
			foreach (KeyValuePair<TeamId, List<Transform>> item in _playerTransformsByTeam)
			{
				if (item.Key != team)
				{
					List<Transform> value = item.Value;
					value.RemoveAll((Transform x) => x == null);
					list.AddRange(value);
				}
			}
			if (list.Count <= 0)
			{
				Debug.LogError($"[TDMSpawnManager] No enemy transforms for team {team}");
				return TryGetClearSpawn(team, spawnPoints);
			}
			float value2 = list.Select((Transform e) => Vector3.Distance(spawnPoint.transform.position, e.position)).Min();
			if (!dictionary.ContainsKey(spawnPoint))
			{
				dictionary.Add(spawnPoint, value2);
			}
		}
		List<PlayerSpawnPoint> list2 = new List<PlayerSpawnPoint>();
		foreach (KeyValuePair<PlayerSpawnPoint, float> item2 in dictionary.OrderByDescending((KeyValuePair<PlayerSpawnPoint, float> i) => i.Value))
		{
			PlayerSpawnPoint key = item2.Key;
			if (!key.SpawnCooldownActive)
			{
				key.StartSpawnCooldown();
				return key;
			}
			list2.Add(key);
			if (list2.Count >= 3)
			{
				break;
			}
		}
		PlayerSpawnPoint playerSpawnPoint = list2.Random();
		playerSpawnPoint.StartSpawnCooldown();
		return playerSpawnPoint;
	}

	[ContextMenu("Validate Spawn Ids")]
	private void ValidateSpawnIds()
	{
		bool flag = false;
		foreach (string item in (from x in _playerSpawnPoints
			group x by x.UniqueId into y
			where y.Count() > 1
			select y.Key).ToList())
		{
			Debug.LogError("Got duplicate spawn ids for " + item);
			flag = true;
		}
		if (!flag)
		{
			Debug.Log("All Spawn Ids are Unique! Good Job!");
		}
	}
}
