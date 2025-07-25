using System;
using System.Collections.Generic;
using BSCore;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CustomizableHUDController : MonoBehaviour
{
	[Inject(Id = DataStoreKeys.HUDCustomization)]
	private DataStoreString _hudCustomizationDataStore;

	[SerializeField]
	private Button _closeButton;

	[SerializeField]
	private Button _resetButton;

	[SerializeField]
	private CustomizableHUDElement[] _elements;

	public static bool HasInstance => Instance != null;

	public static CustomizableHUDController Instance { get; private set; }

	private event Action _closed;

	public event Action Closed
	{
		add
		{
			_closed += value;
		}
		remove
		{
			_closed -= value;
		}
	}

	[ContextMenu("Collect Elements")]
	private void CollectElements()
	{
		_elements = GetComponentsInChildren<CustomizableHUDElement>();
	}

	private void Awake()
	{
		Instance = this;
		CustomizableHUDElement[] elements = _elements;
		for (int i = 0; i < elements.Length; i++)
		{
			elements[i].ToggleCustomization(isOn: false);
		}
		_closeButton.gameObject.SetActive(value: false);
		_resetButton.gameObject.SetActive(value: false);
	}

	private void Start()
	{
		Dictionary<string, ElementCustomization> dictionary = JsonConvert.DeserializeObject<Dictionary<string, ElementCustomization>>(_hudCustomizationDataStore.Value);
		CustomizableHUDElement[] elements = _elements;
		foreach (CustomizableHUDElement customizableHUDElement in elements)
		{
			if (dictionary.TryGetValue(customizableHUDElement.Id, out var value))
			{
				customizableHUDElement.ApplyCustomization(value);
			}
		}
		_closeButton.onClick.AddListener(OnCloseClicked);
		_resetButton.onClick.AddListener(OnResetClicked);
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public void ToggleCustomization(bool isOn)
	{
		_closeButton.gameObject.SetActive(isOn);
		_resetButton.gameObject.SetActive(isOn);
		CustomizableHUDElement[] elements = _elements;
		for (int i = 0; i < elements.Length; i++)
		{
			elements[i].ToggleCustomization(isOn);
		}
	}

	private void OnResetClicked()
	{
		CustomizableHUDElement[] elements = _elements;
		for (int i = 0; i < elements.Length; i++)
		{
			elements[i].ResetCustomization();
		}
		_hudCustomizationDataStore.Value = _hudCustomizationDataStore.Default;
	}

	private void OnCloseClicked()
	{
		Dictionary<string, ElementCustomization> dictionary = new Dictionary<string, ElementCustomization>();
		CustomizableHUDElement[] elements = _elements;
		foreach (CustomizableHUDElement customizableHUDElement in elements)
		{
			dictionary.Add(customizableHUDElement.Id, customizableHUDElement.GetCustomization());
		}
		_hudCustomizationDataStore.Value = JsonConvert.SerializeObject(dictionary);
		this._closed?.Invoke();
	}
}
