using System.Collections;
using UnityEngine;

public abstract class BaseContinuousFireDisplay : MonoBehaviour, IContinuousFireDisplay
{
	private const float RESET_DELAY = 0.1f;

	[SerializeField]
	protected float _displayDelay;

	protected Coroutine _displayCoroutine;

	public abstract void Toggle(bool isOn);

	public void DisplayFor(float duration)
	{
		if (_displayCoroutine != null)
		{
			StopCoroutine(_displayCoroutine);
		}
		_displayCoroutine = StartCoroutine(DisplayForRoutine(duration));
	}

	public void CancelDisplayFor()
	{
		if (_displayCoroutine != null)
		{
			StopCoroutine(_displayCoroutine);
			Toggle(isOn: false);
			_displayCoroutine = null;
		}
	}

	private IEnumerator DisplayForRoutine(float duration)
	{
		if (_displayDelay > 0f)
		{
			yield return new WaitForSeconds(_displayDelay);
		}
		Toggle(isOn: true);
		yield return new WaitForSeconds(duration - _displayDelay + 0.1f);
		Toggle(isOn: false);
		_displayCoroutine = null;
	}
}
