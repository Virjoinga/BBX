using UnityEngine;

public class ChargingWeapon : FireOnReleaseWeapon
{
	[SerializeField]
	private bool _loopChargeAudio = true;

	private ChargingLaunchPoint _chargingLaunchPoint;

	private IWeaponController _weaponController;

	public bool IsCharging { get; private set; }

	public float ChargeValue { get; private set; }

	public float ChargeTime { get; private set; }

	protected override void Start()
	{
		_weaponController = GetComponentInParent<IWeaponController>();
		_chargingLaunchPoint = _launchPoint as ChargingLaunchPoint;
		if (_chargingLaunchPoint == null)
		{
			Debug.LogError("Charging Weapons need to use the Charging Launch Point!");
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		IsCharging = false;
		base.AnimationController.IsCharging = false;
		TryStopChargeSFX();
		ChargeValue = 0f;
		ChargeTime = 0f;
	}

	protected override void FireHoldStarted()
	{
		base.FireHoldStarted();
		if (_weaponAudioPlayer != null)
		{
			_weaponAudioPlayer.PlayCharge(_loopChargeAudio);
		}
		float chargeTime = (ChargeValue = 0f);
		ChargeTime = chargeTime;
		_chargingLaunchPoint.SetChargePercent(0f, 0f);
		IsCharging = true;
		base.AnimationController.IsCharging = true;
	}

	public override int LaunchPointFire(Vector3 position, ref Vector3 forward, int serverFrame, int shotCount, bool isMock = false)
	{
		int num = 1;
		if (!_profile.Charge.SingleAmmoCharge)
		{
			int num2 = _profile.CalculateAmmoUsageAtChargeTime(ChargeTime);
			num += num2;
			if (num > _weaponController.RemainingAmmo)
			{
				num = _weaponController.RemainingAmmo;
			}
		}
		_launchPoint.Fire(position, ref forward, _fireDelay, shotCount, serverFrame, RaiseHit, isMock);
		return num;
	}

	public void SetChargeValue(float chargeValue)
	{
		ChargeValue = chargeValue;
		ChargeTime = chargeValue * _profile.Charge.Time;
	}

	protected override void FireHoldReleased()
	{
		base.FireHoldReleased();
		IsCharging = false;
		base.AnimationController.IsCharging = false;
		TryStopChargeSFX();
		float chargeTime = (ChargeValue = 0f);
		ChargeTime = chargeTime;
		_chargingLaunchPoint.SetChargePercent(0f, 0f);
	}

	protected override void Update()
	{
		if (_profile == null)
		{
			return;
		}
		if (IsCharging)
		{
			float num = _profile.Charge.AmmoIncrementTime * (float)(_weaponController.RemainingAmmo - 1);
			ChargeTime += Time.deltaTime;
			if (ChargeTime > num)
			{
				ChargeTime = num;
			}
		}
		float time = _profile.Charge.Time;
		ChargeValue = ChargeTime / time;
		_chargingLaunchPoint.SetChargePercent(ChargeTime, ChargeValue);
	}

	protected override FireResults GenerateFireResults(Vector3 position, Vector3 forward, int ammoUsed)
	{
		FireResults result = base.GenerateFireResults(position, forward, ammoUsed);
		result.IsCharging = IsCharging;
		result.ChargeTime = ChargeTime;
		return result;
	}

	private void TryStopChargeSFX()
	{
		if (_weaponAudioPlayer != null)
		{
			_weaponAudioPlayer.StopCharge();
		}
	}
}
