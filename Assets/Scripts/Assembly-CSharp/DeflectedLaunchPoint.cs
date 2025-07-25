using System;
using System.Collections;
using UnityEngine;

public abstract class DeflectedLaunchPoint : LaunchPoint
{
	[SerializeField]
	protected float _currentSpread;

	protected bool _hasFiredRecently;

	private Coroutine _onCooldownCompleteRoutine;

	public override float SpreadPercentToMax => Mathf.InverseLerp(base.Profile.Spread.Min, base.Profile.Spread.Amount, _currentSpread);

	protected override void OnEnable()
	{
		base.OnEnable();
		if (base.Profile != null)
		{
			_currentSpread = (base.Profile.Spread.IsVariableSpread ? base.Profile.Spread.Min : base.Profile.Spread.Amount);
		}
	}

	private void FixedUpdate()
	{
		if (base.Profile != null && base.Profile.Spread.IsVariableSpread && !_hasFiredRecently)
		{
			_currentSpread = Mathf.MoveTowards(_currentSpread, base.Profile.Spread.Min, base.Profile.Spread.SpreadLossPerSecond * Time.fixedDeltaTime);
		}
	}

	public override void UpdateProperties(WeaponProfile profile)
	{
		base.UpdateProperties(profile);
		_currentSpread = (base.Profile.Spread.IsVariableSpread ? base.Profile.Spread.Min : base.Profile.Spread.Amount);
	}

	protected override void FireInternal(Vector3 position, ref Vector3 forward, float fireDelay, int projectileId, int serverFrame, Action<HitInfo> onHit, bool isMock = false)
	{
		Vector3 forward2 = forward;
		if (!isMock)
		{
			DeflectForward(ref forward2, projectileId);
		}
		base.FireInternal(position, ref forward2, fireDelay, projectileId, serverFrame, onHit, isMock);
		if (base.Profile.Spread.IsVariableSpread)
		{
			_currentSpread = Mathf.Min(_currentSpread + base.Profile.Spread.SpreadIncreasePerFire, base.Profile.Spread.Amount);
			if (_onCooldownCompleteRoutine != null)
			{
				StopCoroutine(_onCooldownCompleteRoutine);
			}
			_onCooldownCompleteRoutine = StartCoroutine(WaitForCooldownToComplete());
		}
	}

	protected abstract void DeflectForward(ref Vector3 forward, int shotCount);

	protected virtual IEnumerator WaitForCooldownToComplete()
	{
		_hasFiredRecently = true;
		yield return new WaitForSeconds(base.Profile.Spread.SpreadLossDelay);
		_hasFiredRecently = false;
	}
}
