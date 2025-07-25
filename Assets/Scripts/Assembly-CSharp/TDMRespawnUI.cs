using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BSCore;
using Constants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TDMRespawnUI : MonoBehaviour
{
	private const float FADE_TIME = 0.25f;

	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private SpawnPointUI _spawnPointPrefab;

	[SerializeField]
	private Color _teamSpawnColor;

	[SerializeField]
	private Color _neutralSpawnColor;

	[SerializeField]
	private Button _respawnButton;

	[SerializeField]
	private TextMeshProUGUI _timerText;

	[SerializeField]
	private TDMClassSelector _tdmClassSelector;

	[SerializeField]
	private FadeableUI _container;

	[SerializeField]
	private Transform _spawnPointUIContainer;

	private Dictionary<string, SpawnPointUI> _spawnPointOptionsById = new Dictionary<string, SpawnPointUI>();

	private SpawnPointUI _selectedSpawnPoint;

	private float _canRespawnTime = float.MaxValue;

	private float _forceRespawnTime;

	private string _updatedSpawnId = string.Empty;

	private string _updatedHeroClass = string.Empty;

	private bool _matchCompleted;

	private IEnumerator Start()
	{
		_container.FadeOut(0f);
		_signalBus.Subscribe<LocalPlayerDiedSignal>(OnLocalPlayerDied);
		_signalBus.Subscribe<LocalPlayerRespawnedSignal>(OnLocalPlayerRespawned);
		_signalBus.Subscribe<LocalPlayerRespawnDataUpdatedSignal>(OnRespawnDataUpdated);
		_signalBus.Subscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
		_signalBus.Subscribe<SpawnPointPickerToggledUISignal>(OnSpawnPointPickerToggled);
		_respawnButton.onClick.AddListener(OnRespawnClicked);
		_tdmClassSelector.gameObject.SetActive(value: false);
		yield return new WaitUntil(() => MonoBehaviourSingleton<SpawnManager>.IsInstantiated && PlayerController.HasLocalPlayer && PlayerController.LocalPlayer.HasTeamSet && MonoBehaviourSingleton<RespawnCamera>.IsInstantiated);
		yield return new WaitForSeconds(1f);
		SetupSpawnPointUILocations();
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<LocalPlayerDiedSignal>(OnLocalPlayerDied);
		_signalBus.Unsubscribe<LocalPlayerRespawnedSignal>(OnLocalPlayerRespawned);
		_signalBus.Unsubscribe<LocalPlayerRespawnDataUpdatedSignal>(OnRespawnDataUpdated);
		_signalBus.Unsubscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
		_signalBus.Unsubscribe<SpawnPointPickerToggledUISignal>(OnSpawnPointPickerToggled);
	}

	private void OnLocalPlayerDied()
	{
		StartCoroutine(EnableUIAfterDelay());
	}

	private void OnSpawnPointPickerToggled(SpawnPointPickerToggledUISignal signal)
	{
		_spawnPointUIContainer.gameObject.SetActive(signal.IsOpen);
	}

	private IEnumerator EnableUIAfterDelay()
	{
		yield return new WaitForSeconds(Match.RESPAWNUI_DELAY);
		if (!_matchCompleted)
		{
			if (!PlayerController.HasLocalPlayer || !PlayerController.LocalPlayer.LoadoutController.HasOutfit)
			{
				Debug.LogError("[TDMRespawnUI] Unable to select hero class something went wrong");
			}
			MonoBehaviourSingleton<BetaInfoDisplay>.Instance.ToggleVisibleState(isVisible: false);
			_container.FadeIn(0.25f);
			MonoBehaviourSingleton<MouseLockToggle>.Instance.ReleaseCursor();
			MonoBehaviourSingleton<MouseLockToggle>.Instance.MouseCanLock = false;
		}
	}

	private void OnLocalPlayerRespawned()
	{
		_container.FadeOut(0.25f);
		MonoBehaviourSingleton<BetaInfoDisplay>.Instance.ToggleVisibleState(isVisible: true);
		_updatedSpawnId = string.Empty;
		_updatedHeroClass = string.Empty;
		MonoBehaviourSingleton<MouseLockToggle>.Instance.MouseCanLock = true;
		MonoBehaviourSingleton<MouseLockToggle>.Instance.TryLockCursor();
	}

	private void SetupSpawnPointUILocations()
	{
		int localPlayerTeam = PlayerController.LocalPlayer.Team;
		Camera respawnCam = MonoBehaviourSingleton<RespawnCamera>.Instance.RespawnCam;
		foreach (PlayerSpawnPoint item in MonoBehaviourSingleton<SpawnManager>.Instance.PlayerSpawnPoints.Where((PlayerSpawnPoint x) => x.Team == (TeamId)localPlayerTeam || x.Team == TeamId.Neutral))
		{
			SpawnPointUI spawnPointUI = Object.Instantiate(_spawnPointPrefab, _spawnPointUIContainer);
			Vector3 position = respawnCam.WorldToScreenPoint(item.Position);
			spawnPointUI.transform.position = position;
			Color color = ((item.Team == (TeamId)localPlayerTeam) ? _teamSpawnColor : _neutralSpawnColor);
			spawnPointUI.Populate(item.UniqueId, color);
			spawnPointUI.SpawnPointSelected += OnSpawnPointSelected;
			_spawnPointOptionsById.Add(item.UniqueId, spawnPointUI);
		}
	}

	private void OnRespawnDataUpdated(LocalPlayerRespawnDataUpdatedSignal signal)
	{
		if (!string.IsNullOrEmpty(signal.SelectedSpawnId) && _spawnPointOptionsById.ContainsKey(signal.SelectedSpawnId))
		{
			SelectSpawnUI(_spawnPointOptionsById[signal.SelectedSpawnId]);
			_updatedSpawnId = signal.SelectedSpawnId;
		}
		_canRespawnTime = signal.CanRespawnTime;
		_forceRespawnTime = signal.ForceRespawnTime;
	}

	private void OnSpawnPointSelected(SpawnPointUI spawnPointUI)
	{
		SelectSpawnUI(spawnPointUI);
		_updatedSpawnId = spawnPointUI.SpawnID;
		SendSelectionsUpdatedSignal();
	}

	private void OnClassSelected(HeroClass updatedClass)
	{
		_updatedHeroClass = updatedClass.ToString();
		SendSelectionsUpdatedSignal();
	}

	private void SendSelectionsUpdatedSignal()
	{
		_signalBus.Fire(new ClientUpdateRespawnSelectionsSignal
		{
			SpawnPointId = _updatedSpawnId,
			Loadout = _updatedHeroClass
		});
	}

	private void SelectSpawnUI(SpawnPointUI spawnPointUI)
	{
		if (_selectedSpawnPoint != null)
		{
			_selectedSpawnPoint.SetHighlight(isHighlighted: false);
		}
		_selectedSpawnPoint = spawnPointUI;
		_selectedSpawnPoint.SetHighlight(isHighlighted: true);
	}

	private void OnRespawnClicked()
	{
		if (BoltNetwork.ServerTime > _canRespawnTime)
		{
			_signalBus.Fire(default(ClientRequestRespawnSignal));
		}
	}

	private void FixedUpdate()
	{
		if (BoltNetwork.Server == null)
		{
			return;
		}
		float num = _forceRespawnTime - BoltNetwork.ServerTime;
		if (num > 0f)
		{
			if (num <= 3f)
			{
				_timerText.color = Color.red;
			}
			else
			{
				_timerText.color = Color.white;
			}
			_timerText.text = $"{num:0.0}";
		}
		else
		{
			_timerText.text = string.Empty;
		}
		_respawnButton.interactable = BoltNetwork.ServerTime > _canRespawnTime;
	}

	private void OnMatchStateUpdated(MatchStateUpdatedSignal signal)
	{
		if (signal.MatchState == MatchState.Complete)
		{
			_matchCompleted = true;
			MonoBehaviourSingleton<BetaInfoDisplay>.Instance.ToggleVisibleState(isVisible: true);
			_container.FadeOut(0f);
		}
	}
}
