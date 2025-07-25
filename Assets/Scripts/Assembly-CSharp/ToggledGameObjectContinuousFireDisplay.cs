using UnityEngine;

public class ToggledGameObjectContinuousFireDisplay : BaseContinuousFireDisplay
{
	[SerializeField]
	private GameObject _gameObject;

	private void Start()
	{
		_gameObject.SetActive(value: false);
	}

	private void OnDisable()
	{
		_gameObject.SetActive(value: false);
	}

	public override void Toggle(bool isOn)
	{
		_gameObject.SetActive(isOn);
	}
}
