using System;
using System.Collections;
using System.Collections.Generic;
using BSCore;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class UIPrefabManager : MonoBehaviour
{
	public struct UIInstanceHandleMapping
	{
		public GameObject UIInstance;

		public AsyncOperationHandle<GameObject> Handle;

		public UIInstanceHandleMapping(GameObject uiInstance, AsyncOperationHandle<GameObject> handle)
		{
			UIInstance = uiInstance;
			Handle = handle;
		}
	}

	[Inject]
	private ZenjectInstantiater _zenjectInstantiater;

	private static Dictionary<UIPrefabIds, UIInstanceHandleMapping> _spawnedUIHandlesByPrefabId = new Dictionary<UIPrefabIds, UIInstanceHandleMapping>();

	public static string sceneLoad = string.Empty;

	private static string _onGuiLog;

	private static UIPrefabManager _instance;

	[SerializeField]
	private Canvas _interactiveCanvas;

	[SerializeField]
	private Canvas _noninteractiveCanvas;

	[SerializeField]
	private CanvasGroup _interactiveCanvasGroup;

	[SerializeField]
	private CanvasGroup _nonInteractiveCanvasGroup;

	public static void log(string text)
	{
		_onGuiLog = text + "\n" + _onGuiLog;
	}

	private void OnGUI()
	{
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.normal.textColor = Color.green;
		gUIStyle.fontSize = Screen.width / 100;
		GUI.Label(new Rect(20f, 20f, Screen.width - 10, Screen.height - 10), _onGuiLog, gUIStyle);
	}

	public static bool IsMainMenuScene()
	{
		return SceneManager.GetActiveScene().name == "MainMenu";
	}

	public static void Instantiate(UIPrefabIds prefabId, bool interactive = true, int layer = 0, bool useParent = true)
	{
		Instantiate(prefabId, null, interactive, layer, useParent);
	}

	public static void Instantiate(UIPrefabIds prefabId, Action<GameObject> operationCompleted, bool interactive = true, int layer = 0, bool useParent = true)
	{
		if (_spawnedUIHandlesByPrefabId.ContainsKey(prefabId))
		{
			Debug.LogError($"[UIPrefabManager] Tried to spawn UI {prefabId} that already exists!");
			operationCompleted?.Invoke(_spawnedUIHandlesByPrefabId[prefabId].UIInstance);
		}
		else
		{
			_instance.StartCoroutine(_instance.InstantiateUIRoutine(prefabId, operationCompleted, interactive, layer, useParent));
		}
	}

	private IEnumerator InstantiateUIRoutine(UIPrefabIds prefabId, Action<GameObject> operationCompleted, bool interactive = true, int layer = 0, bool useParent = true)
	{
		AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(prefabId.ToString());
		yield return handle;
		if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
		{
			Transform parentTransform = null;
			if (useParent)
			{
				parentTransform = (interactive ? _instance._interactiveCanvas.transform : _instance._noninteractiveCanvas.transform);
			}
			GameObject gameObject = _instance._zenjectInstantiater.Instantiate(handle.Result, parentTransform);
			if (!_spawnedUIHandlesByPrefabId.ContainsKey(prefabId))
			{
				_spawnedUIHandlesByPrefabId.Add(prefabId, new UIInstanceHandleMapping(gameObject, handle));
			}
			else
			{
				Debug.LogError($"[UIPrefabManager] Spawned a duplicate UI element! {prefabId} This shouldn't happen pls fix");
			}
			if (layer != 0)
			{
				Canvas canvas = gameObject.AddComponent<Canvas>();
				canvas.overrideSorting = true;
				canvas.sortingOrder = layer;
				if (interactive)
				{
					gameObject.AddComponent<GraphicRaycaster>();
				}
			}
			operationCompleted?.Invoke(gameObject);
		}
		else
		{
			operationCompleted?.Invoke(null);
		}
	}

	public static void Destroy(UIPrefabIds prefabId, float delay = 0f)
	{
		DelayedAction.RunAfterSeconds(delay, delegate
		{
			if (_spawnedUIHandlesByPrefabId.ContainsKey(prefabId))
			{
				UIInstanceHandleMapping uIInstanceHandleMapping = _spawnedUIHandlesByPrefabId[prefabId];
				UnityEngine.Object.Destroy(uIInstanceHandleMapping.UIInstance);
				Addressables.Release(uIInstanceHandleMapping.Handle);
				_spawnedUIHandlesByPrefabId.Remove(prefabId);
			}
		});
	}

	public static void TryDestroyAllBut(UIPrefabIds excludedUI)
	{
		List<UIPrefabIds> list = new List<UIPrefabIds>();
		foreach (KeyValuePair<UIPrefabIds, UIInstanceHandleMapping> item in _spawnedUIHandlesByPrefabId)
		{
			if (item.Key != excludedUI)
			{
				UIInstanceHandleMapping uIInstanceHandleMapping = _spawnedUIHandlesByPrefabId[item.Key];
				UnityEngine.Object.Destroy(uIInstanceHandleMapping.UIInstance);
				Addressables.Release(uIInstanceHandleMapping.Handle);
				list.Add(item.Key);
			}
		}
		foreach (UIPrefabIds item2 in list)
		{
			_spawnedUIHandlesByPrefabId.Remove(item2);
		}
	}

	public static void ShowHideInteractiveCanvas(bool shouldShow)
	{
		if (shouldShow && _instance._interactiveCanvasGroup.alpha != 1f)
		{
			_instance._interactiveCanvasGroup.alpha = 1f;
		}
		else if (!shouldShow && _instance._interactiveCanvasGroup.alpha != 0f)
		{
			_instance._interactiveCanvasGroup.alpha = 0f;
		}
	}

	public static void ShowHideNonInteractiveCanvas(bool shouldShow)
	{
		if (shouldShow && _instance._nonInteractiveCanvasGroup.alpha != 1f)
		{
			_instance._nonInteractiveCanvasGroup.alpha = 1f;
		}
		else if (!shouldShow && _instance._nonInteractiveCanvasGroup.alpha != 0f)
		{
			_instance._nonInteractiveCanvasGroup.alpha = 0f;
		}
	}

	private void Awake()
	{
		_instance = this;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Debug.Log("OnSceneLoaded: " + scene.name + " mode: " + mode);
		sceneLoad = scene.name;
	}
}
