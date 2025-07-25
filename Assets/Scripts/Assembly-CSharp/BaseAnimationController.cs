using System;
using UnityEngine;

public class BaseAnimationController : MonoBehaviour
{
	protected Animator _animator;

	protected bool _hasAnimator => _animator != null;

	protected virtual void Awake()
	{
		CacheAnimator();
	}

	public void SetFloat<T>(T parameter, float value) where T : struct, IComparable
	{
		if (_animator != null)
		{
			_animator.SetFloat(parameter.ToString(), value);
		}
	}

	public void SetBool<T>(T parameter, bool value) where T : struct, IComparable
	{
		if (_animator != null)
		{
			_animator.SetBool(parameter.ToString(), value);
		}
	}

	public void SetTrigger<T>(T parameter) where T : struct, IComparable
	{
		if (_animator != null)
		{
			_animator.SetTrigger(parameter.ToString());
		}
	}

	public void ResetTrigger<T>(T parameter) where T : struct, IComparable
	{
		if (_animator != null)
		{
			_animator.ResetTrigger(parameter.ToString());
		}
	}

	public void SetInteger<T>(T parameter, int value) where T : struct, IComparable
	{
		if (_animator != null)
		{
			_animator.SetInteger(parameter.ToString(), value);
		}
	}

	public float GetFloat<T>(T parameter) where T : struct, IComparable
	{
		if (!(_animator != null))
		{
			return 0f;
		}
		return _animator.GetFloat(parameter.ToString());
	}

	public bool GetBool<T>(T parameter) where T : struct, IComparable
	{
		if (!(_animator != null))
		{
			return false;
		}
		return _animator.GetBool(parameter.ToString());
	}

	public int GetInteger<T>(T parameter) where T : struct, IComparable
	{
		if (!(_animator != null))
		{
			return 0;
		}
		return _animator.GetInteger(parameter.ToString());
	}

	protected virtual void CacheAnimator()
	{
		_animator = GetComponent<Animator>();
		if (_animator == null)
		{
			Debug.LogError("[BaseAnimationController] Failed to get Animator Component on gameobject - " + base.gameObject.name);
		}
	}
}
