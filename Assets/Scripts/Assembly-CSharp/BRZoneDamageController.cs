using BSCore;
using BSCore.Constants.Config;
using UnityEngine;
using Zenject;

public class BRZoneDamageController : MonoBehaviour
{
	private const string BRZONE_TAG = "BRZone";

	[Inject]
	private ConfigManager _configManager;

	[Inject]
	private SignalBus _signalBus;

	private PlayerController _playerController;

	private HealthController _healthController;

	private float _damagePerSecond = 1f;

	private int _zonesOccupiedCount;

	private MatchState _matchState;

	[Inject]
	private void Construct()
	{
		BattleRoyaleConfigData battleRoyaleConfigData = _configManager.Get<BattleRoyaleConfigData>(DataKeys.BattleRoyale);
		_damagePerSecond = battleRoyaleConfigData.ZoneCloseConfig.DamagePerSecond;
		_signalBus.Subscribe<MatchStateUpdatedSignal>(OnBRMatchStateUpdated);
	}

	private void Awake()
	{
		_playerController = GetComponent<PlayerController>();
		_healthController = GetComponent<HealthController>();
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<MatchStateUpdatedSignal>(OnBRMatchStateUpdated);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "BRZone")
		{
			_zonesOccupiedCount++;
			CancelInvoke();
			InvokeRepeating("DealDamage", 0f, 1f);
			if (_playerController.IsLocal)
			{
				_signalBus.Fire(new EnterExitClosedZoneSignal
				{
					InClosedZone = true
				});
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!(other.tag == "BRZone"))
		{
			return;
		}
		_zonesOccupiedCount--;
		if (_zonesOccupiedCount <= 0)
		{
			_zonesOccupiedCount = 0;
			CancelInvoke();
			if (_playerController.IsLocal)
			{
				_signalBus.Fire(new EnterExitClosedZoneSignal
				{
					InClosedZone = false
				});
			}
		}
	}

	private void DealDamage()
	{
		if (_matchState == MatchState.Active)
		{
			_healthController.TakeSelfDamage(_damagePerSecond);
		}
	}

	private void OnBRMatchStateUpdated(MatchStateUpdatedSignal signal)
	{
		_matchState = signal.MatchState;
	}
}
