using System.Collections;
using BSCore;
using Constants;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CameraRespawnUI : MonoBehaviour
{
	[Inject]
	private SignalBus _signalBus;

	[Inject]
	protected DataStoreManager _dataStoreManager;

	[SerializeField]
	private GameObject _respawnUICanvas;

	[SerializeField]
	private Camera _mainCamera;

	[SerializeField]
	private Button _toggleSpawnMapButton;

	[SerializeField]
	private float _openCloseSpeed = 0.5f;

	[SerializeField]
	private TextMeshProUGUI _openCloseText;

	[SerializeField]
	private GameObject _openArrows;

	[SerializeField]
	private GameObject _closeArrows;

	private bool _spawnPickerMapOpen;

	private bool _matchCompleted;

	private Tweener _tweeningHandle;

	private DataStoreBool _spawnPickerOpenDataStore;

	private void Start()
	{
		_respawnUICanvas.SetActive(value: false);
		_signalBus.Subscribe<LocalPlayerDiedSignal>(OnLocalPlayerDied);
		_signalBus.Subscribe<LocalPlayerRespawnedSignal>(OnLocalPlayerRespawned);
		_signalBus.Subscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
		_spawnPickerOpenDataStore = _dataStoreManager.GetStore<DataStoreBool, bool>(DataStoreKeys.SpawnPickerOpen);
		_toggleSpawnMapButton.onClick.AddListener(ToggleSpawnPickerMap);
	}

	private void OnDestroy()
	{
		_signalBus.Unsubscribe<LocalPlayerDiedSignal>(OnLocalPlayerDied);
		_signalBus.Unsubscribe<LocalPlayerRespawnedSignal>(OnLocalPlayerRespawned);
		_signalBus.Unsubscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
	}

	private void OnLocalPlayerDied()
	{
		if (!_matchCompleted)
		{
			StartCoroutine(EnableUIAfterDelay());
		}
	}

	private void OnLocalPlayerRespawned()
	{
		if (!_matchCompleted)
		{
			_respawnUICanvas.SetActive(value: false);
			CloseSpawnPicker();
		}
	}

	private IEnumerator EnableUIAfterDelay()
	{
		yield return new WaitForSeconds(Match.RESPAWNUI_DELAY);
		if (!_matchCompleted)
		{
			if (_spawnPickerOpenDataStore.Value)
			{
				OpenSpawnPicker();
			}
			_respawnUICanvas.SetActive(value: true);
		}
	}

	private void ToggleSpawnPickerMap()
	{
		if (!_spawnPickerMapOpen)
		{
			OpenSpawnPicker();
			_spawnPickerOpenDataStore.Value = true;
		}
		else
		{
			CloseSpawnPicker();
			_spawnPickerOpenDataStore.Value = false;
		}
	}

	private void OpenSpawnPicker()
	{
		KillAllTweens();
		_tweeningHandle = _mainCamera.DORect(new Rect(0.5f, 0f, 1f, 1f), _openCloseSpeed).OnComplete(SendToggledSignal);
		_openCloseText.text = "Close Spawn Picker";
		_openArrows.SetActive(value: false);
		_closeArrows.SetActive(value: true);
		_spawnPickerMapOpen = true;
	}

	private void CloseSpawnPicker()
	{
		KillAllTweens();
		_tweeningHandle = _mainCamera.DORect(new Rect(0f, 0f, 1f, 1f), _openCloseSpeed);
		_openCloseText.text = "Open Spawn Picker";
		_openArrows.SetActive(value: true);
		_closeArrows.SetActive(value: false);
		_spawnPickerMapOpen = false;
		SendToggledSignal();
	}

	private void SendToggledSignal()
	{
		_signalBus.Fire(new SpawnPointPickerToggledUISignal
		{
			IsOpen = _spawnPickerMapOpen
		});
	}

	private void KillAllTweens()
	{
		if (_tweeningHandle != null && _tweeningHandle.IsPlaying())
		{
			_tweeningHandle.Kill();
		}
	}

	private void OnMatchStateUpdated(MatchStateUpdatedSignal signal)
	{
		if (signal.MatchState == MatchState.Complete)
		{
			_matchCompleted = true;
			_respawnUICanvas.SetActive(value: false);
			_mainCamera.DORect(new Rect(0f, 0f, 1f, 1f), _openCloseSpeed);
		}
	}
}
