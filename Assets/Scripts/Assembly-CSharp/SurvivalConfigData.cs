using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SurvivalConfigData
{
	public BattleRoyalePickupConfigData Pickups;

	public int MaxEnemies;

	public int StartingEnemies;

	public float IntermissionLength;

	public WaveModifiers WaveModifiers;

	public PlayerCountModifiers PlayerCountModifiers;

	public float PointsPerKill;

	public float AssistBonusPoints;

	public int StartingAmmoClips;

	public SurvivalEnemyConfigData[] Enemies;

	public MapRotationData[] MapRotationList;

	public readonly Dictionary<string, SurvivalEnemyConfigData> EnemyConfigsByName = new Dictionary<string, SurvivalEnemyConfigData>();

	public readonly List<BasePickupConfigData> Dropables = new List<BasePickupConfigData>();

	private System.Random _rng;

	public void Setup()
	{
		SurvivalEnemyConfigData[] enemies = Enemies;
		for (int i = 0; i < enemies.Length; i++)
		{
			SurvivalEnemyConfigData value = enemies[i];
			EnemyConfigsByName.Add(value.Name, value);
		}
		Dropables.Add(Pickups.AmmoClip);
		Dropables.Sort(SortDropables);
	}

	private int SortDropables(BasePickupConfigData x, BasePickupConfigData y)
	{
		return x.DropChance.CompareTo(y.DropChance);
	}

	public string GetRandomMap()
	{
		int max = MapRotationList.Sum((MapRotationData x) => x.WeightedChance);
		int num = 0;
		int num2 = 0;
		int num3 = UnityEngine.Random.Range(0, max);
		for (num = 0; num < MapRotationList.Length; num++)
		{
			num2 += MapRotationList[num].WeightedChance;
			if (num2 > num3)
			{
				break;
			}
		}
		return MapRotationList[num].SceneName;
	}

	public bool ShouldDropPickup(out BasePickupConfigData pickupData)
	{
		if (_rng == null)
		{
			_rng = new System.Random();
		}
		int num = _rng.Next(0, 100);
		int num2 = 0;
		foreach (BasePickupConfigData dropable in Dropables)
		{
			num2 += dropable.DropChance;
			if (num <= num2)
			{
				pickupData = dropable;
				return true;
			}
		}
		pickupData = null;
		return false;
	}
}
