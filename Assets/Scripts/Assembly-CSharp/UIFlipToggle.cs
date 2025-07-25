using UnityEngine;
using UnityEngine.UI;

public class UIFlipToggle : MonoBehaviour
{
	[SerializeField]
	private Toggle _toggle;

	[SerializeField]
	private GameObject _onDisplay;

	[SerializeField]
	private GameObject _offDisplay;

	private void Awake()
	{
		_toggle.onValueChanged.AddListener(OnValueChanged);
		OnValueChanged(_toggle.isOn);
	}

	private void OnValueChanged(bool isOn)
	{
		_onDisplay.SetActive(isOn);
		_offDisplay.SetActive(!isOn);
	}

	private void Reset()
	{
		_toggle = GetComponent<Toggle>();
	}
}
