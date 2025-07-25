using System.Collections.Generic;
using UnityEngine;

public class SurvivalSpawnManager : SpawnManager
{
	[SerializeField]
	private List<PlayerSpawnPoint> _enemySpawnPoints = new List<PlayerSpawnPoint>();

	private List<PlayerSpawnPoint> _originalEnemySpawnPoints;

	protected override void Awake()
	{
		base.Awake();
		_originalEnemySpawnPoints = new List<PlayerSpawnPoint>(_enemySpawnPoints);
	}

	public static PlayerSpawnPoint GetEnemySpawnPoint()
	{
		SurvivalSpawnManager survivalSpawnManager = MonoBehaviourSingleton<SpawnManager>.Instance as SurvivalSpawnManager;
		if (survivalSpawnManager._enemySpawnPoints.Count <= 0)
		{
			survivalSpawnManager._enemySpawnPoints = new List<PlayerSpawnPoint>(survivalSpawnManager._originalEnemySpawnPoints);
		}
		PlayerSpawnPoint playerSpawnPoint = survivalSpawnManager._enemySpawnPoints.Random();
		survivalSpawnManager._enemySpawnPoints.Remove(playerSpawnPoint);
		return playerSpawnPoint;
	}
}
