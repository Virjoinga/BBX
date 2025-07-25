using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFillBar : MonoBehaviour
{
	protected const float MAX_ALPHA_VALUE = 1f;

	[SerializeField]
	private Image _barSprite;

	private bool _isInterpolating;

	public Color Color
	{
		get
		{
			return _barSprite.color;
		}
		set
		{
			_barSprite.color = value;
		}
	}

	public void SetToPercent(float percentValue)
	{
		if (_barSprite != null)
		{
			if (_barSprite.type != Image.Type.Filled)
			{
				throw new InvalidOperationException("Trying to fill an image whose Image.Type is not Image.Type.Filled");
			}
			_barSprite.fillAmount = percentValue;
		}
	}

	public void SetToValue(int current, int max)
	{
		SetToPercent(current / max);
	}

	public void SetToValue(float current, float max)
	{
		SetToPercent(current / max);
	}

	public void SetToValue(float current, float min, float max)
	{
		SetToPercent((current - min) / (max - min));
	}

	public void FillBar(float duration)
	{
		InterpolateBar(0f, 1f, duration);
	}

	public void DrainBar(float duration)
	{
		InterpolateBar(1f, 0f, duration);
	}

	public void DrainBar(float start, float duration)
	{
		InterpolateBar(start, 0f, duration);
	}

	public void InterpolateBar(float start, float end, float duration)
	{
		if (base.gameObject.activeInHierarchy)
		{
			StopAllCoroutines();
			_isInterpolating = true;
			StartCoroutine(InterpolateBarCoroutine(start, end, duration));
		}
	}

	private IEnumerator InterpolateBarCoroutine(float start, float end, float duration)
	{
		start = Mathf.Clamp01(start);
		end = Mathf.Clamp01(end);
		SetToPercent(start);
		for (float currentDuration = 0f; currentDuration < duration; currentDuration += Time.deltaTime)
		{
			if (!_isInterpolating)
			{
				break;
			}
			SetToPercent(Mathf.Lerp(start, end, currentDuration / duration));
			yield return null;
		}
		SetToPercent(end);
	}

	public void StopInterpolation()
	{
		_isInterpolating = false;
	}
}
