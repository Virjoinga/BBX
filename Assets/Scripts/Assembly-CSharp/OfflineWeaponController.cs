using System.Collections;
using System.Linq;
using BSCore;
using Constants;
using UnityEngine;
using Zenject;

public class OfflineWeaponController : MonoBehaviour, IWeaponController
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private LayerMask _localClientHurtboxLayer;

	private LoadoutController _loadoutController;

	private IPlayerMotor _playerMotor;

	private OfflinePlayerController _playerController;

	private readonly int[] _remainingAmmoBySlot = new int[4];

	private bool _fire;

	private bool _useMelee;

	private float _lastFireTime;

	private float _isInCooldownUntil;

	private float _nextMeleeTime;

	private Coroutine _reloadRoutine;

	private FiringWeapon _reloadingWeapon;

	private float _fireHeldDuration;

	private int _fireHeldGraceFrames;

	private bool _hasEnoughAmmo
	{
		get
		{
			if (WeaponHandler.HasActiveWeapon)
			{
				return RemainingAmmo > 0;
			}
			return false;
		}
	}

	private WeaponProfile _activeWeaponProfile => WeaponHandler.ActiveWeaponProfile;

	private int _activeWeaponIndex => WeaponHandler.ActiveWeaponIndex;

	public int RemainingAmmo => _remainingAmmoBySlot[_activeWeaponIndex];

	public WeaponHandler WeaponHandler { get; private set; }

	public bool IsZooming { get; private set; }

	private void Awake()
	{
		_loadoutController = GetComponent<LoadoutController>();
		WeaponHandler = GetComponent<WeaponHandler>();
		_playerMotor = GetComponent<IPlayerMotor>();
		WeaponHandler.GetServerTime = () => Time.realtimeSinceStartup;
		_playerController = GetComponent<OfflinePlayerController>();
		_playerController.HurtCollider.gameObject.layer = LayerMask.NameToLayer("LocalHurtbox");
	}

	private void Update()
	{
		if (BSCoreInput.GetButtonDown(Option.Reload) && RemainingAmmo < _activeWeaponProfile.ClipSize)
		{
			Reload();
		}
		if (BSCoreInput.GetButtonDown(Option.WeaponSlot1))
		{
			if (_activeWeaponIndex >= 0 && !WeaponIsLocked(_activeWeaponIndex, Time.realtimeSinceStartup))
			{
				StowWeapon();
			}
		}
		else if (BSCoreInput.GetButtonDown(Option.WeaponSlot2))
		{
			TryDeployWeapon(0);
		}
		else if (BSCoreInput.GetButtonDown(Option.WeaponSlot3))
		{
			TryDeployWeapon(1);
		}
		else if (BSCoreInput.GetButtonDown(Option.WeaponSlot4))
		{
			TryDeployWeapon(2);
		}
		else if (BSCoreInput.GetButtonDown(Option.WeaponSlot5))
		{
			TryDeployWeapon(3);
		}
		if (WeaponHandler.HasActiveWeapon && WeaponHandler.ActiveWeaponProfile.Spread.IsVariableSpread)
		{
			_signalBus.Fire(new WeaponCurrentSpreadUpdated(WeaponHandler.ActiveWeapon.SpreadPercentToMax));
		}
	}

	private void FixedUpdate()
	{
		_fire = BSCoreInput.GetButton(Option.Fire);
		_useMelee = BSCoreInput.GetButtonDown(Option.Melee);
		IsZooming = BSCoreInput.GetButton(Option.Zoom);
		_playerController.AnimationController.SetBool(PlayerAnimationController.Parameter.MeleeInCooldown, _nextMeleeTime > Time.realtimeSinceStartup);
	}

	public int GetServerFrame()
	{
		return (int)(Time.realtimeSinceStartup / Time.fixedDeltaTime);
	}

	public void OnLoadoutSet()
	{
		for (int i = 0; i < _loadoutController.WeaponProfiles.Length; i++)
		{
			_remainingAmmoBySlot[i] = _loadoutController.WeaponProfiles[i]?.ClipSize ?? 0;
		}
	}

	public void SetMeleeWeapon(HeroClass heroClass, WeaponProfile profile)
	{
		_playerController.AnimationController.SetMeleeWeapon(heroClass, profile);
		_loadoutController.MeleeWeapon.MeleeAnimationTriggered += OnMeleeAnimationTriggered;
	}

	private void OnMeleeAnimationTriggered(WeaponProfile.MeleeOptionData meleeOption)
	{
		_playerController.AnimationController.SetTrigger(meleeOption.AnimParam);
	}

	public void HandlePlayerInput(bool inputEnabled, ref Vector2 movementInput, ref float cameraVertical, ref float cameraHorizontal)
	{
		if (!_loadoutController.HasOutfit)
		{
			return;
		}
		int serverFrame = Mathf.FloorToInt(Time.realtimeSinceStartup / Time.fixedDeltaTime);
		bool flag = WeaponHandler.HasActiveWeapon && _isInCooldownUntil > Time.realtimeSinceStartup;
		if (!WeaponHandler.HasActiveWeapon)
		{
			_useMelee |= _fire;
			_fire = false;
		}
		_fire &= inputEnabled && WeaponHandler.HasActiveWeapon && !WeaponIsFireLocked(_activeWeaponIndex, Time.realtimeSinceStartup) && _hasEnoughAmmo && _reloadingWeapon == null;
		bool fireHeld = _fire && !flag;
		_useMelee &= inputEnabled && _nextMeleeTime <= Time.realtimeSinceStartup;
		if (WeaponHandler.HasActiveWeapon)
		{
			FiringWeapon firingWeapon = _loadoutController.Weapons[_activeWeaponIndex];
			_useMelee &= !firingWeapon.LocksWeaponsWhileFiring || firingWeapon.CanMeleeOutOfLock || !WeaponIsLocked(_activeWeaponIndex, Time.realtimeSinceStartup);
		}
		IsZooming &= WeaponHandler.HasActiveWeapon;
		bool enableIronSight = WeaponHandler.HandleIronSightMode(IsZooming, ref cameraVertical, ref cameraHorizontal);
		MonoBehaviourSingleton<OTSCamera>.Instance.SetIronSightMode(enableIronSight, WeaponHandler.ActiveWeaponProfile);
		int shotCount = (WeaponHandler.HasActiveWeapon ? (WeaponHandler.ActiveWeaponProfile.ClipSize - _remainingAmmoBySlot[_activeWeaponIndex]) : 0);
		FireResults fireResults = WeaponHandler.TryFire(fireHeld, serverFrame, shotCount);
		bool flag2 = WeaponHandler.HasActiveWeapon && WeaponHandler.ActiveWeapon.LocksWeaponsWhileFiring;
		bool flag3 = false;
		WeaponProfile.MeleeOptionData meleeOptionData = null;
		if (_useMelee)
		{
			meleeOptionData = WeaponHandler.GetMeleeOptionFromInput(movementInput, _playerMotor.State.Velocity, _playerMotor.State.IsGroundedOrWithinGrace);
			flag3 = WeaponHandler.TryUseMelee(flag && flag2, meleeOptionData, OnMeleeComplete);
		}
		if (fireResults.Fired)
		{
			_lastFireTime = Time.realtimeSinceStartup;
			float num = ((!WeaponHandler.ActiveWeaponProfile.Spinup.SpinsUp) ? WeaponHandler.ActiveWeaponProfile.Cooldown : Mathf.Lerp(WeaponHandler.ActiveWeaponProfile.Spinup.Min, WeaponHandler.ActiveWeaponProfile.Cooldown, Mathf.InverseLerp(0f, WeaponHandler.ActiveWeaponProfile.Spinup.TimeToFull, _fireHeldDuration)));
			_isInCooldownUntil = Time.realtimeSinceStartup + num;
			_remainingAmmoBySlot[_activeWeaponIndex] -= fireResults.AmmoUsed;
			if (_remainingAmmoBySlot[_activeWeaponIndex] <= 0)
			{
				Reload(WeaponHandler.ActiveWeaponProfile.Reload.Delay);
			}
		}
		if (flag3)
		{
			_nextMeleeTime = Time.realtimeSinceStartup + meleeOptionData.MovementDuration + meleeOptionData.Recovery + meleeOptionData.Cooldown;
			_playerMotor.OverrideInput(movementInput.x, movementInput.y, meleeOptionData);
			if (flag2)
			{
				_isInCooldownUntil = Time.realtimeSinceStartup;
				flag = false;
			}
		}
		if (WeaponHandler.HasActiveWeapon)
		{
			WeaponState animationStates = new WeaponState
			{
				FireResults = fireResults,
				IsFiring = (fireResults.Fired || flag),
				IsReloading = (_reloadingWeapon != null),
				UsedMelee = flag3
			};
			WeaponHandler.ActiveWeapon.AnimationController.SetAnimationStates(animationStates);
		}
		if (_fire)
		{
			_fireHeldDuration += Time.deltaTime;
			if (WeaponHandler.ActiveWeaponProfile.Spinup.SpinsUp && _fireHeldDuration > WeaponHandler.ActiveWeaponProfile.Spinup.TimeToFull)
			{
				_fireHeldDuration = WeaponHandler.ActiveWeaponProfile.Spinup.TimeToFull;
			}
		}
		else if (_fireHeldGraceFrames >= 3)
		{
			_fireHeldDuration -= Time.deltaTime;
			if (_fireHeldDuration < 0f)
			{
				_fireHeldDuration = 0f;
				_fireHeldGraceFrames = 0;
			}
		}
		else
		{
			_fireHeldGraceFrames++;
		}
		if (WeaponHandler.HasActiveWeapon && WeaponHandler.ActiveWeaponProfile.SelfEffects.Any((WeaponProfile.EffectData e) => e.EffectType == Match.EffectType.ForcedMovement && e.Duration > 0f) && _lastFireTime + WeaponHandler.ActiveWeaponProfile.Cooldown >= Time.realtimeSinceStartup)
		{
			movementInput.y = 1f;
		}
		_fire = false;
		_useMelee = false;
	}

	private void OnMeleeComplete()
	{
	}

	public void TryDeployWeapon(int index)
	{
		if ((WeaponIndexIsValid(_activeWeaponIndex) && WeaponIsLocked(_activeWeaponIndex, Time.realtimeSinceStartup)) || !WeaponHandler.CanDeployIndex(index))
		{
			return;
		}
		WeaponHandler.DeployWeapon(index, delegate
		{
			_fireHeldDuration = 0f;
			_fireHeldGraceFrames = 0;
			_playerMotor.SetProfileProperties(_loadoutController.OutfitProfile.HeroClassProfile, _activeWeaponProfile);
			if (_remainingAmmoBySlot[index] <= 0)
			{
				Reload();
			}
		});
	}

	public void StowWeapon()
	{
		if (WeaponHandler.CanStow(_activeWeaponIndex))
		{
			WeaponHandler.StowWeapon(delegate
			{
				_playerMotor.SetProfileProperties(_loadoutController.OutfitProfile.HeroClassProfile, _activeWeaponProfile);
			});
		}
	}

	public void Reload(float delay = 0f)
	{
		if (WeaponHandler.HasActiveWeapon)
		{
			_reloadRoutine = StartCoroutine(ReloadRoutine(delay));
		}
	}

	private IEnumerator ReloadRoutine(float delay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		int index = WeaponHandler.ActiveWeaponIndex;
		_reloadingWeapon = _loadoutController.Weapons[index];
		if (_reloadingWeapon.AnimationController.BackpackOperation == WeaponAnimationController.BackpackOp.OpenForReload)
		{
			_loadoutController.Backpack.AnimationController.Open(playAudio: false);
		}
		if (_reloadingWeapon.AudioPlayer != null)
		{
			_reloadingWeapon.AudioPlayer.PlayReload();
		}
		_reloadingWeapon.AnimationController.IsReloading = true;
		Debug.Log("[OfflineWeaponController] Starting reload...");
		yield return new WaitForSeconds(_reloadingWeapon.Profile.Reload.Time);
		Debug.Log("[OfflineWeaponController] Reload complete");
		if (_reloadingWeapon.AudioPlayer != null)
		{
			_reloadingWeapon.AudioPlayer.StopReload();
		}
		if (_reloadingWeapon.AnimationController.BackpackOperation == WeaponAnimationController.BackpackOp.OpenForReload)
		{
			_loadoutController.Backpack.AnimationController.Close();
		}
		_reloadingWeapon.AnimationController.IsReloading = false;
		_remainingAmmoBySlot[index] = _reloadingWeapon.Profile.ClipSize;
		_reloadingWeapon = null;
		_reloadRoutine = null;
	}

	private void OnGUI()
	{
		if (_loadoutController.HasOutfit && _activeWeaponProfile != null)
		{
			float y = Screen.height - 30;
			GUI.Label(new Rect(10f, y, 50f, 20f), "Ammo");
			GUI.Label(new Rect(50f, y, 50f, 20f), _remainingAmmoBySlot[_activeWeaponIndex].ToString());
		}
	}

	public bool WeaponIsLocked(int index, float serverFrame)
	{
		if (!WeaponIndexIsValid(index))
		{
			return true;
		}
		ChargingWeapon chargingWeapon = _loadoutController.Weapons[index] as ChargingWeapon;
		if (chargingWeapon != null && chargingWeapon.LocksWeaponsWhileFiring && chargingWeapon.IsCharging)
		{
			return true;
		}
		if (_loadoutController.Weapons[index].LocksWeaponsWhileFiring)
		{
			return _lastFireTime + _activeWeaponProfile.Cooldown > serverFrame;
		}
		return false;
	}

	public bool WeaponIsFireLocked(int index, float serverFrame)
	{
		if (!WeaponIndexIsValid(index))
		{
			return true;
		}
		if (_loadoutController.Weapons[index].LocksWeaponsWhileFiring)
		{
			return _lastFireTime + _activeWeaponProfile.Cooldown > serverFrame;
		}
		return false;
	}

	private bool WeaponIndexIsValid(int index)
	{
		if (index >= 0 && index < _loadoutController.Weapons.Length)
		{
			return _loadoutController.Weapons[index] != null;
		}
		return false;
	}
}
