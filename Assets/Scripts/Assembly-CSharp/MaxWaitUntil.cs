using System;
using UnityEngine;

public class MaxWaitUntil : CustomYieldInstruction
{
	private Func<bool> _conditional;

	private float _maxWaitTime;

	private float _timer;

	public override bool keepWaiting
	{
		get
		{
			_timer += Time.deltaTime;
			if (!_conditional())
			{
				return _timer < _maxWaitTime;
			}
			return false;
		}
	}

	public MaxWaitUntil(Func<bool> conditional, float maxWaitTime)
	{
		_conditional = conditional;
		_maxWaitTime = maxWaitTime;
	}
}
