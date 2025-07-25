using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using BSCore.Constants.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SurvivalMatchStateDisplay : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[Inject]
	protected ConfigManager _configManager;

	[SerializeField]
	private Transform _container;

	[SerializeField]
	private SurvivalPlayerPlate _playerPlatePrefab;

	[SerializeField]
	private TextMeshProUGUI _enemiesToKill;

	[SerializeField]
	private Image _intermissionIndicator;

	private readonly Dictionary<int, SurvivalPlayerPlate> _platesByIndex = new Dictionary<int, SurvivalPlayerPlate>();

	private int _enemiesToKillCount;

	private SurvivalConfigData _config;

	private void Start()
	{
		_config = _configManager.Get<SurvivalConfigData>(DataKeys.Survival);
		_signalBus.Subscribe<LeaderboardUpdatedSignal>(OnPlayersUpdated);
		_signalBus.Subscribe<SurvivalStateUpdatedSignal>(OnGameModeStateUpdated);
		_signalBus.Fire(default(LeaderboardInstantiatedSignal));
		_intermissionIndicator.fillAmount = 0f;
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<LeaderboardUpdatedSignal>(OnPlayersUpdated);
		_signalBus.Unsubscribe<SurvivalStateUpdatedSignal>(OnGameModeStateUpdated);
	}

	private void OnPlayersUpdated(LeaderboardUpdatedSignal signal)
	{
		SurvivalPlayer player = signal.player;
		int index = signal.index;
		bool flag = player.EntityId != null;
		SurvivalPlayerPlate value;
		bool flag2 = _platesByIndex.TryGetValue(index, out value);
		if (flag && !flag2)
		{
			value = Object.Instantiate(_playerPlatePrefab, _container);
			value.Setup(player.Name, player.Points);
			_platesByIndex.Add(index, value);
		}
		else if (!flag && flag2)
		{
			_platesByIndex.Remove(index);
			Object.Destroy(value.gameObject);
		}
		else if (flag && flag2)
		{
			value.Points = player.Points;
		}
		List<SurvivalPlayerPlate> list = _platesByIndex.Values.ToList();
		if (list.Count > 1)
		{
			list = list.OrderByDescending((SurvivalPlayerPlate p) => p.Points).ToList();
			for (int num = 0; num < list.Count; num++)
			{
				list[num].transform.SetSiblingIndex(num);
			}
		}
	}

	private void OnGameModeStateUpdated(SurvivalStateUpdatedSignal signal)
	{
		_enemiesToKill.text = signal.enemiesToKill.ToString();
		if (_enemiesToKillCount != 0 && signal.enemiesToKill == 0)
		{
			StartCoroutine(CountdownIntermission());
		}
		_enemiesToKillCount = signal.enemiesToKill;
	}

	private IEnumerator CountdownIntermission()
	{
		float step = 1f / _config.IntermissionLength;
		for (float t = 1f; t >= 0f; t -= Time.deltaTime * step)
		{
			_intermissionIndicator.fillAmount = t;
			yield return null;
		}
		_intermissionIndicator.fillAmount = 0f;
	}
}
