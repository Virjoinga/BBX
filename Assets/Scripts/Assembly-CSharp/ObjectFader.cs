using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFader : MonoBehaviour
{
	private const string DISSOLVE_VALUE = "_DissolveValue";

	[SerializeField]
	private bool _registerWithParent = true;

	private Renderer[] _renderers;

	private List<ObjectFader> _subFaders = new List<ObjectFader>();

	private List<int> _subFadersToRemove = new List<int>();

	private void Awake()
	{
		_renderers = GetComponentsInChildren<Renderer>();
	}

	private void Start()
	{
		if (base.transform.parent != null && _registerWithParent)
		{
			ObjectFader componentInParent = base.transform.parent.GetComponentInParent<ObjectFader>();
			if (componentInParent != null)
			{
				componentInParent.RegisterSubFader(this);
			}
		}
	}

	public void RegisterSubFader(ObjectFader objectFader)
	{
		_subFaders.Add(objectFader);
	}

	public void SetFadeLevel(float level)
	{
		Renderer[] renderers = _renderers;
		foreach (Renderer renderer in renderers)
		{
			if (!(renderer != null))
			{
				continue;
			}
			renderer.enabled = level >= 0.01f;
			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				if (material.HasProperty("_DissolveValue"))
				{
					material.SetFloat("_DissolveValue", level);
				}
			}
		}
		SetSubFaderLevel(level);
	}

	public void FadeIn(float duration = 0.5f, Action onComplete = null)
	{
		FadeFromTo(0f, 1f, duration, onComplete);
	}

	public void FadeOut(float duration = 0.5f, Action onComplete = null)
	{
		FadeFromTo(1f, 0f, duration, onComplete);
	}

	public void FadeFromTo(float from, float to, float duration = 0.5f, Action onComplete = null)
	{
		StartCoroutine(FadeFromToRoutine(from, to, duration, onComplete));
	}

	private IEnumerator FadeFromToRoutine(float from, float to, float duration, Action onComplete)
	{
		Debug.Log("[ObjectFader] Starting fade routine", base.gameObject);
		float step = 1f / duration;
		for (float t = 0f; t < 1f; t += Time.deltaTime * step)
		{
			SetFadeLevel(Mathf.Lerp(from, to, t));
			yield return null;
		}
		SetSubFaderLevel(to);
		onComplete?.Invoke();
		Debug.Log("[ObjectFader] Fade routine complete", base.gameObject);
	}

	private void SetSubFaderLevel(float level)
	{
		_subFadersToRemove.Clear();
		for (int i = 0; i < _subFaders.Count; i++)
		{
			if (_subFaders[i] != null)
			{
				_subFaders[i].SetFadeLevel(level);
			}
			else
			{
				_subFadersToRemove.Add(i);
			}
		}
		_subFadersToRemove.Reverse();
		foreach (int item in _subFadersToRemove)
		{
			_subFaders.RemoveAt(item);
		}
	}
}
