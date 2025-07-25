using System;
using BSCore;
using BSCore.Constants.Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class ExperienceManager : IInitializable, IDisposable
{
	[Inject]
	private StatisticsManager _statisticsManager;

	[Inject]
	private ConfigManager _configManager;

	[Inject]
	private UserManager _userManager;

	private static int _baseExperience = 5000;

	private static float _experienceGrowthRate = 0.32f;

	public int PlayerExperience { get; private set; }

	public int LevelCap { get; private set; } = 30;

	public int PlayerCurrentLevel { get; private set; }

	public int MinimumExperience { get; private set; }

	public int StartingExperienceForCurrentLevel { get; private set; }

	public int ExperienceNeededForNextLevel { get; private set; }

	public void Initialize()
	{
		PlayerExperience = Mathf.Max(PlayerExperience, MinimumExperience);
		SceneManager.sceneLoaded += OnSceneLoaded;
		_configManager.Fetched += OnConfigDataFetched;
		if (_configManager.HasFetched)
		{
			OnConfigDataFetched();
		}
	}

	public void Dispose()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		_configManager.Fetched -= OnConfigDataFetched;
	}

	private void OnConfigDataFetched()
	{
		LevelCap = _configManager.Get(DataKeys.accountLevelCap, 30);
		_baseExperience = _configManager.Get(DataKeys.accountBaseXP, 5000);
		_experienceGrowthRate = _configManager.Get(DataKeys.accountXPGrowthRate, 0.32f);
		MinimumExperience = ExperienceForLevel(1) + 1;
	}

	private void OnStatisticFetchSuccess(StatisticKey key, int value)
	{
		PlayerExperience = Mathf.Max(value, MinimumExperience);
		PlayerCurrentLevel = LevelForExperience(PlayerExperience);
		StartingExperienceForCurrentLevel = ExperienceForLevel(LevelForExperience(PlayerExperience));
		ExperienceNeededForNextLevel = ExperienceForLevel(LevelForExperience(PlayerExperience) + 1);
		Debug.Log($"Updated Player EXP to {PlayerExperience}");
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "MainMenu")
		{
			_statisticsManager.FetchPlayerStatistic(_userManager.CurrentUser, StatisticKey.Experience, OnStatisticFetchSuccess, OnStatisticFetchFailed);
		}
	}

	private void OnStatisticFetchFailed(FailureReasons failureReason)
	{
		Debug.LogError($"[ExperienceManager] Failed to fetch experience statistics - {failureReason}");
	}

	private static int ExperienceForLevel(int level)
	{
		return Mathf.RoundToInt((float)_baseExperience * Mathf.Pow(1f + _experienceGrowthRate, level));
	}

	private int LevelForExperience(int experience)
	{
		return Mathf.Clamp(Mathf.FloorToInt(Mathf.Log((float)experience / (float)_baseExperience) / Mathf.Log(1f + _experienceGrowthRate)), 0, LevelCap);
	}
}
