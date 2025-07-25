using System;
using UnityEngine;

[RequireComponent(typeof(WeaponAnimationController))]
public class FiringWeapon : BaseWeapon
{
	[SerializeField]
	protected LaunchPoint _launchPoint;

	[SerializeField]
	protected float _projectileSpeed = 1f;

	[SerializeField]
	protected bool _locksWeaponsWhileFiring;

	[SerializeField]
	protected bool _canMeleeOutOfLock;

	[SerializeField]
	protected bool _isAbilityWeapon;

	[SerializeField]
	protected bool _isChannelingWeapon;

	[SerializeField]
	protected HitType _hitType;

	public WeaponAnimationController AnimationController { get; private set; }

	public virtual HitType HitType => _hitType;

	public Vector3 Origin => _launchPoint.transform.position;

	public Vector3 Forward => _launchPoint.transform.forward;

	public Vector3 HitPosition => _launchPoint.AimPosition;

	public float SpreadPercentToMax => _launchPoint.SpreadPercentToMax;

	public bool LocksWeaponsWhileFiring => _locksWeaponsWhileFiring;

	public bool CanMeleeOutOfLock => _canMeleeOutOfLock;

	public bool IsAbilityWeapon => _isAbilityWeapon;

	public bool IsChannelingWeapon => _isChannelingWeapon;

	public LaunchPoint LaunchPoint => _launchPoint;

	private event Action _fired;

	public event Action Fired
	{
		add
		{
			_fired += value;
		}
		remove
		{
			_fired -= value;
		}
	}

	private event Action _fireCanceled;

	public event Action FireCanceled
	{
		add
		{
			_fireCanceled += value;
		}
		remove
		{
			_fireCanceled -= value;
		}
	}

	protected override void Reset()
	{
		base.Reset();
		_launchPoint = GetComponentInChildren<LaunchPoint>();
	}

	protected override void Awake()
	{
		base.Awake();
		AnimationController = GetComponent<WeaponAnimationController>();
	}

	public virtual FireResults Fire(bool isFiring, Vector3 aimPoint, int serverFrame, int shotCount)
	{
		if (!isFiring)
		{
			return FireResults.Empty;
		}
		TryPlayFireSFX();
		Vector3 forward = (aimPoint - _launchPoint.transform.position).normalized;
		int ammoUsed = LaunchPointFire(_launchPoint.transform.position, ref forward, serverFrame, shotCount);
		return GenerateFireResults(_launchPoint.transform.position, forward, ammoUsed);
	}

	public virtual void CancelFire()
	{
		if (_launchPoint is ContinuousFireLaunchPoint)
		{
			(_launchPoint as ContinuousFireLaunchPoint).CancelFire();
			this._fireCanceled?.Invoke();
		}
		AnimationController.IsFiring = false;
	}

	public virtual int LaunchPointFire(Vector3 position, ref Vector3 forward, int serverFrame, int shotCount, bool isMock = false)
	{
		CancelInvoke("OnCooldownComplete");
		AnimationController.IsFiring = true;
		int num = 0;
		if (_profile.VolleySize > 1)
		{
			_launchPoint.FireVolley(_profile.VolleySize, _profile.VolleyDelay, position, forward, _fireDelay, shotCount, serverFrame, RaiseHit, isMock);
			num = _profile.VolleySize;
		}
		else
		{
			_launchPoint.Fire(position, ref forward, _fireDelay, shotCount, serverFrame, RaiseHit, isMock);
			num = 1;
		}
		Invoke("OnCooldownComplete", _profile.Cooldown);
		this._fired?.Invoke();
		return num;
	}

	private void OnCooldownComplete()
	{
		AnimationController.IsFiring = false;
	}

	protected override void UpdateProperties()
	{
		base.UpdateProperties();
		_launchPoint.UpdateProperties(_profile);
		_projectileSpeed = _profile.Projectile.Speed;
		if (AnimationController == null)
		{
			AnimationController = GetComponent<WeaponAnimationController>();
		}
		AnimationController.SetProperties(_profile);
	}

	protected virtual FireResults GenerateFireResults(Vector3 position, Vector3 forward, int ammoUsed)
	{
		return new FireResults
		{
			Fired = true,
			IsCharging = false,
			HitType = HitType,
			Position = position,
			Forward = forward,
			AmmoUsed = ammoUsed
		};
	}

	protected virtual void TryPlayFireSFX()
	{
		if (_weaponAudioPlayer != null)
		{
			_weaponAudioPlayer.PlayFire();
		}
	}

	protected override void RaiseHit(HitInfo hitInfo)
	{
		hitInfo.hitType = HitType;
		base.RaiseHit(hitInfo);
	}

	public EWeaponType GetWeaponType()
	{
		EWeaponType result = EWeaponType.Arm;
		try
		{
			Enum.TryParse<EWeaponType>(_profile.WeaponType, out result);
		}
		catch (Exception)
		{
			result = EWeaponType.Arm;
		}
		return result;
	}
}
