using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TeamDeathMatchConfigData
{
	[Serializable]
	public struct TDMRewards
	{
		public int BaseCurrency;

		public int WinCurrency;

		public int KillCurrency;

		public int AssistCurrency;

		public int BaseEXP;

		public int WinEXP;

		public int KillEXP;

		public int AssistEXP;

		public KillStreakReward[] KillStreakRewards;
	}

	[Serializable]
	public struct KillStreakReward
	{
		public int StreakAmount;

		public int CurrencyBonus;

		public int EXPBonus;
	}

	public float TimeLimit;

	public int KillLimit;

	public float MinRespawnTime;

	public float ForcedRespawnTime;

	public float RespawnShieldTime;

	public float LoadingTimeout;

	public MapRotationData[] MapRotationList;

	public List<HealthPickupConfigData> HealthPickups;

	public TDMRewards Rewards;

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
}
