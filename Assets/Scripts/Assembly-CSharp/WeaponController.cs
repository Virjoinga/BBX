using System;
using System.Collections;
using System.Collections.Generic;
using BSCore;
using Bolt;
using Constants;
using UnityEngine;
using Zenject;

public class WeaponController : BaseEntityEventListener<IPlayerState, PlayerInputCommand, PlayerHitCommand>, IWeaponController
{
	private class WeaponSwitchRequest
	{
		public bool requested;

		public int index = -1;
	}

	private const float WEAPON_SWITCH_COOLDOWN = 0.5f;

	[Inject]
	private SignalBus _signalBus;

	[Inject]
	private ProfileManager _profileManager;

	[SerializeField]
	private LayerMask _localClientHurtboxLayer;

	private PlayerController _playerController;

	private HealthController _healthController;

	private MatchStateHelper _matchStateHelper;

	private LoadoutController _loadoutController;

	private StatusEffectController _statusEffectController;

	private DeployableController _deployableController = new DeployableController();

	private readonly WeaponSwitchRequest _weaponSwitchRequest = new WeaponSwitchRequest();

	private bool _reloadPressed;

	private bool _fireHeld;

	private bool _meleeHeld;

	private bool _zoomHeld;

	private readonly List<HitInfo> _hitCache = new List<HitInfo>();

	private BoltHitboxBody _hitBoxBody;

	private Coroutine _reloadRoutine;

	private FiringWeapon _reloadingWeapon;

	private WeaponValidator _weaponValidator;

	private float _weaponsSwitchCooldownTimer;

	private int _fireHeldDuration;

	private int _fireHeldGraceFrames;

	private Coroutine _channelingStatusEffectRoutine;

	private IPlayerMotor _playerMotor => _playerController.DefaultPlayerMotor;

	private bool _hasActiveWeapon
	{
		get
		{
			if (WeaponIndexIsValid(base.state.Loadouts[0].ActiveWeapon))
			{
				return _activeWeaponProfile != null;
			}
			return false;
		}
	}

	private WeaponProfile _activeWeaponProfile => WeaponHandler.ActiveWeaponProfile;

	private Weapon _activeWeapon
	{
		get
		{
			if (!_hasActiveWeapon)
			{
				return null;
			}
			return base.state.Loadouts[0].Weapons[base.state.Loadouts[0].ActiveWeapon];
		}
	}

	public int RemainingAmmo
	{
		get
		{
			if (base.state.Loadouts[0].ActiveWeapon < 0 || !_hasActiveWeapon)
			{
				return 0;
			}
			return _activeWeapon.RemainingAmmo;
		}
	}

	public WeaponHandler WeaponHandler { get; private set; }

	protected override void Awake()
	{
		_playerController = GetComponent<PlayerController>();
		_healthController = GetComponent<HealthController>();
		_matchStateHelper = GetComponent<MatchStateHelper>();
		WeaponHandler = GetComponent<WeaponHandler>();
		_loadoutController = GetComponent<LoadoutController>();
		_loadoutController.Hit += OnWeaponHit;
		_statusEffectController = GetComponent<StatusEffectController>();
		_hitBoxBody = GetComponent<BoltHitboxBody>();
	}

	private void Update()
	{
		if (!base.entity.isControlled || base.entity.isOwner)
		{
			return;
		}
		if (_weaponsSwitchCooldownTimer > 0f)
		{
			_weaponsSwitchCooldownTimer -= Time.deltaTime;
		}
		if (BSCoreInput.GetButtonDown(Option.SwitchWeapon) && _weaponsSwitchCooldownTimer <= 0f)
		{
			_weaponSwitchRequest.requested = true;
			_weaponSwitchRequest.index = ((base.state.Loadouts[0].ActiveWeapon == 0) ? 1 : 0);
			_weaponsSwitchCooldownTimer = 0.5f;
		}
		if (BSCoreInput.GetButtonDown(Option.Reload))
		{
			_reloadPressed = true;
		}
		if (_loadoutController.Outfit != null && _activeWeaponProfile != null && !_activeWeaponProfile.IsMelee && !_activeWeaponProfile.Spread.IsSpread)
		{
			Vector3 aimPosition = _loadoutController.Outfit.AimPointHandler.AimPosition;
			Vector3 hitPosition = aimPosition;
			if (WeaponHandler.HasActiveWeapon && _reloadRoutine == null)
			{
				hitPosition = WeaponHandler.ActiveWeapon.HitPosition;
			}
			_signalBus.Fire(new ShotPathData(aimPosition, hitPosition, WeaponHandler.ActiveWeaponProfile.HideShotPathBlockedIcon));
		}
		UpdateReloadIndicator(base.state.Loadouts[0].ActiveWeapon);
	}

	private void FixedUpdate()
	{
		if (base.entity.isOwner || !_loadoutController.HasOutfit)
		{
			return;
		}
		int activeWeapon = base.state.Loadouts[0].ActiveWeapon;
		if (_hasActiveWeapon && _loadoutController.Weapons[activeWeapon] != null)
		{
			if (_loadoutController.HasOutfit)
			{
				_loadoutController.Weapons[activeWeapon].AnimationController.IsReloading = base.state.Loadouts[0].Weapons[activeWeapon].IsReloading;
			}
			if (base.entity.isControlled && _loadoutController.WeaponProfiles[activeWeapon].Spread.IsVariableSpread)
			{
				_signalBus.Fire(new WeaponCurrentSpreadUpdated(_loadoutController.Weapons[activeWeapon].SpreadPercentToMax));
			}
		}
		if (!base.entity.isControlled)
		{
			return;
		}
		for (int i = 0; i < base.state.Loadouts[0].Weapons.Length; i++)
		{
			Weapon weapon = base.state.Loadouts[0].Weapons[i];
			if (!string.IsNullOrEmpty(weapon.Id) && _loadoutController.WeaponProfiles[i] != null && _loadoutController.WeaponProfiles[i].Reload.InBackground)
			{
				int num = weapon.NextFireFrame - BoltNetwork.ServerFrame;
				float reloadPercent = 0f;
				if (num > 0)
				{
					reloadPercent = (float)num / (float)_loadoutController.WeaponProfiles[i].Reload.TimeFrames;
				}
				_signalBus.Fire(new WeaponStateUpdatedSignal(i, reloadPercent));
			}
		}
		_playerController.AnimationController.SetBool(PlayerAnimationController.Parameter.MeleeInCooldown, base.state.NextMeleeTime > BoltNetwork.ServerFrame);
	}

	public void InitializeCharacter(Outfit outfit)
	{
		WeaponHandler.OnOutfitSet();
	}

	public void SetMeleeWeapon(HeroClass heroClass, WeaponProfile profile)
	{
		_playerController.AnimationController.SetMeleeWeapon(heroClass, profile);
		if (_loadoutController.MeleeWeapon != null)
		{
			_loadoutController.MeleeWeapon.MeleeAnimationTriggered += OnMeleeAnimationTriggered;
		}
	}

	private void OnMeleeAnimationTriggered(WeaponProfile.MeleeOptionData meleeOption)
	{
		if (base.entity.isControlled && !base.entity.isOwner)
		{
			_playerController.AnimationController.SetTrigger(meleeOption.AnimParam);
		}
		else if (base.entity.isOwner)
		{
			GetMeleeTriggerMethod(meleeOption.AnimParam)?.Invoke();
		}
	}

	private Action GetMeleeTriggerMethod(PlayerAnimationController.Parameter animParam)
	{
		return base.state.UseMeleeStanding;
	}

	private void PollForInput()
	{
		if (BSCoreInput.GetButton(Option.Fire))
		{
			_fireHeld = true;
		}
		if (BSCoreInput.GetButton(Option.Zoom))
		{
			_zoomHeld = true;
		}
	}

	private void ClearPolledInput()
	{
		_weaponSwitchRequest.requested = false;
		_reloadPressed = false;
		_fireHeld = false;
		_meleeHeld = false;
		_zoomHeld = false;
	}

	private bool WeaponIndexIsValid(int index)
	{
		if (index >= 0 && index < base.state.Loadouts[0].Weapons.Length)
		{
			return !string.IsNullOrEmpty(base.state.Loadouts[0].Weapons[index].Id);
		}
		return false;
	}

	protected override void OnAnyAttached()
	{
		base.state.AddCallback("Loadouts[].ActiveWeapon", OnActiveWeaponChanged);
	}

	protected override void OnOwnerOnlyAttached()
	{
		_weaponValidator = new WeaponValidator(base.entity, _hitBoxBody, _profileManager, DealDamageToHit, SpawnEntity);
		_healthController.Damaged += OnDamaged;
	}

	private void OnDamaged(BoltEntity attacker, float amount)
	{
		if (WeaponHandler.HasActiveWeapon && WeaponHandler.ActiveWeapon.IsChannelingWeapon && _channelingStatusEffectRoutine != null)
		{
			_activeWeapon.InCooldownUntil = BoltNetwork.ServerFrame;
			_activeWeapon.NextFireFrame = BoltNetwork.ServerFrame + _activeWeaponProfile.Reload.TimeFrames;
			base.state.CancelFire();
			TryCancelChannelRoutine();
		}
	}

	protected override void OnOwnerOrControllerAttached()
	{
		base.state.AddCallback("Loadouts[].Weapons[].Id", OnWeaponEquipped);
	}

	protected override void OnControllerOnlyAttached()
	{
		_signalBus.Subscribe<WeaponSlotSelectedSignal>(OnWeaponSlotSelected);
		base.state.AddCallback("Loadouts[].Weapons[].RemainingAmmo", OnAmmoUpdated);
		base.state.AddCallback("Loadouts[].Weapons[].MaxAmmo", OnAmmoUpdated);
		_playerController.HurtCollider.gameObject.layer = LayerMask.NameToLayer("LocalHurtbox");
	}

	protected override void OnRemoteOnlyAttached()
	{
		base.state.AddCallback("Loadouts[].Weapons[].ChargeValue", OnChargeValueUpdated);
		base.state.AddCallback("CameraOffset", OnCameraOffsetUpdated);
		IPlayerState playerState = base.state;
		playerState.OnUseMeleeForward = (Action)Delegate.Combine(playerState.OnUseMeleeForward, new Action(OnUseMeleeForward));
		IPlayerState playerState2 = base.state;
		playerState2.OnUseMeleeBackward = (Action)Delegate.Combine(playerState2.OnUseMeleeBackward, new Action(OnUseMeleeBackward));
		IPlayerState playerState3 = base.state;
		playerState3.OnUseMeleeLeft = (Action)Delegate.Combine(playerState3.OnUseMeleeLeft, new Action(OnUseMeleeLeft));
		IPlayerState playerState4 = base.state;
		playerState4.OnUseMeleeRight = (Action)Delegate.Combine(playerState4.OnUseMeleeRight, new Action(OnUseMeleeRight));
		IPlayerState playerState5 = base.state;
		playerState5.OnUseMeleeStanding = (Action)Delegate.Combine(playerState5.OnUseMeleeStanding, new Action(OnUseMeleeStanding));
	}

	protected override void OnControllerOnlyDetached()
	{
		_signalBus.Unsubscribe<WeaponSlotSelectedSignal>(OnWeaponSlotSelected);
	}

	protected override void OnControllerOrRemoteAttached()
	{
		IPlayerState playerState = base.state;
		playerState.OnCancelFire = (Action)Delegate.Combine(playerState.OnCancelFire, new Action(OnCancelFire));
	}

	public override void SimulateOwner()
	{
		int serverFrame = BoltNetwork.ServerFrame;
		int num = serverFrame - 600 - 1;
		int ammoRemaining = 0;
		int nextFireTime = 0;
		bool isReloading = false;
		if (_hasActiveWeapon)
		{
			Weapon activeWeapon = _activeWeapon;
			ammoRemaining = activeWeapon.RemainingAmmo;
			nextFireTime = activeWeapon.NextFireFrame;
			isReloading = activeWeapon.IsReloading;
		}
		WeaponHandler.TrackWeaponState(serverFrame, num, ammoRemaining, nextFireTime, isReloading);
		_weaponValidator.ForgetFireHistory(num);
	}

	public override void SimulateController()
	{
		_ = BoltNetwork.ServerFrame;
		foreach (HitInfo item in _hitCache)
		{
			IPlayerHitCommandInput data = item.ToHitCommandInput();
			base.entity.QueueInput(data);
		}
		_hitCache.Clear();
	}

	public void SimulateWeaponController(ref IPlayerInputCommandInput input)
	{
		PollForInput();
		if (!_playerController.LocalInputBlocked)
		{
			int serverFrame = BoltNetwork.ServerFrame;
			Weapon activeWeapon = _activeWeapon;
			ClientTryCopyValuesToInput(ref input);
			ClientTryFireWeapon(ref input, serverFrame);
			HandleZooming(ref input, activeWeapon);
			if (_weaponSwitchRequest.requested)
			{
				ClientTrySwitchWeapon(ref input, serverFrame);
			}
			ClientTryReloadWeapon(ref input, activeWeapon, serverFrame);
			ClientTryUseMelee(ref input, serverFrame);
			WeaponHandler.TrySendChargeValueUpdatedSignal();
		}
		if (_loadoutController.HasOutfit)
		{
			input.CameraOffset = _loadoutController.Outfit.AimPointHandler.Offset.x;
		}
		ClearPolledInput();
	}

	private void ClientTryCopyValuesToInput(ref IPlayerInputCommandInput input)
	{
		if (_hasActiveWeapon && WeaponHandler.ActiveWeapon is ChargingWeapon)
		{
			input.ChargeValue = (WeaponHandler.ActiveWeapon as ChargingWeapon).ChargeValue;
		}
	}

	private void ClientTryFireWeapon(ref IPlayerInputCommandInput input, int serverFrame)
	{
		int activeWeapon = base.state.Loadouts[0].ActiveWeapon;
		input.WeaponIndex = activeWeapon;
		input.FireHeld = _fireHeld;
		input.ShouldFire = false;
		if (_hasActiveWeapon)
		{
			bool fireHeld = _fireHeld && base.state.InputEnabled && !base.state.WeaponsDisabled && CanFireWeapon(input.WeaponIndex, serverFrame);
			Weapon weapon = base.state.Loadouts[0].Weapons[activeWeapon];
			FireResults fireResults = WeaponHandler.TryFire(fireHeld, serverFrame, weapon.MaxAmmo - weapon.RemainingAmmo);
			if (fireResults.Fired)
			{
				input.ShouldFire = true;
				input.Position = fireResults.Position;
				input.Forward = fireResults.Forward;
				input.HitType = (int)WeaponHandler.ActiveWeapon.HitType;
				input.AmmoUsed = fireResults.AmmoUsed;
				input.ChargeTime = fireResults.ChargeTime;
			}
		}
	}

	private void HandleZooming(ref IPlayerInputCommandInput input, Weapon activeWeapon)
	{
		if (!MouseLockToggle.MouseLockReleased)
		{
			float cameraVertical = input.CameraVertical;
			float cameraHorizontal = input.CameraHorizontal;
			bool hasActiveWeapon = _hasActiveWeapon;
			input.IsZooming = _zoomHeld && hasActiveWeapon;
			bool enableIronSight = WeaponHandler.HandleIronSightMode(input.IsZooming, ref cameraVertical, ref cameraHorizontal);
			if (WeaponHandler.ActiveWeaponProfile != null && MonoBehaviourSingleton<OTSCamera>.IsInstantiated)
			{
				MonoBehaviourSingleton<OTSCamera>.Instance.SetIronSightMode(enableIronSight, WeaponHandler.ActiveWeaponProfile);
			}
			input.CameraVertical = cameraVertical;
			input.CameraHorizontal = cameraHorizontal;
		}
	}

	private void ClientTrySwitchWeapon(ref IPlayerInputCommandInput input, int serverFrame)
	{
		if (base.state.InputEnabled && !base.state.Stunned && !base.state.WeaponsDisabled && !WeaponIsLocked(input.WeaponIndex, serverFrame))
		{
			int index = _weaponSwitchRequest.index;
			if (CanSwitchTo(index) || (index < 0 && _hasActiveWeapon))
			{
				input.ShouldSwitchWeapon = true;
				input.WeaponSlot = index;
			}
		}
	}

	private void ClientTryReloadWeapon(ref IPlayerInputCommandInput input, Weapon activeWeapon, int serverFrame)
	{
		if (base.state.InputEnabled && _reloadPressed && _hasActiveWeapon && !WeaponIsLocked(input.WeaponIndex, serverFrame) && !activeWeapon.IsReloading && RemainingAmmo < activeWeapon.MaxAmmo)
		{
			input.ShouldReload = true;
		}
	}

	private void ClientTryUseMelee(ref IPlayerInputCommandInput input, int serverFrame)
	{
		int activeWeapon = base.state.Loadouts[0].ActiveWeapon;
		bool flag = base.state.InputEnabled && (_meleeHeld || (!_hasActiveWeapon && _fireHeld));
		if (_hasActiveWeapon && _loadoutController.HasOutfit && _loadoutController.Weapons != null)
		{
			FiringWeapon firingWeapon = _loadoutController.Weapons[activeWeapon];
			if (firingWeapon != null)
			{
				flag &= firingWeapon.CanMeleeOutOfLock || !firingWeapon.LocksWeaponsWhileFiring || !WeaponIsLocked(input.WeaponIndex, serverFrame);
			}
		}
		bool shouldCancelFire = _hasActiveWeapon && flag && WeaponHandler.ActiveWeapon.CanMeleeOutOfLock && _activeWeapon.InCooldownUntil > BoltNetwork.ServerFrame;
		if (flag && base.state.NextMeleeTime <= serverFrame)
		{
			Vector2 movementInput = new Vector2(input.Horizontal, input.Vertical);
			WeaponProfile.MeleeOptionData meleeOptionFromInput = WeaponHandler.GetMeleeOptionFromInput(movementInput, _playerMotor.State.Velocity, _playerMotor.State.IsGroundedOrWithinGrace);
			if (WeaponHandler.TryUseMelee(shouldCancelFire, meleeOptionFromInput, OnMeleeComplete))
			{
				input.ShouldMelee = true;
				input.MeleeDirection = (int)meleeOptionFromInput.MovementDirection;
			}
		}
	}

	protected override void ExecuteCommand(PlayerInputCommand cmd, bool resetState)
	{
		if (_loadoutController.HasOutfit)
		{
			int weaponIndex = cmd.Input.WeaponIndex;
			bool inputAllowed = base.state.InputEnabled && !base.state.WeaponsDisabled;
			Weapon weapon = null;
			bool hasWeapon = false;
			WeaponProfile weaponProfile = null;
			if (WeaponIndexIsValid(weaponIndex))
			{
				weapon = base.state.Loadouts[0].Weapons[weaponIndex];
				weaponProfile = WeaponHandler.ActiveWeaponProfile;
				hasWeapon = weapon != null && weaponProfile != null;
			}
			if (base.entity.isOwner)
			{
				CopyInputParametersToState(cmd, inputAllowed, weaponIndex, hasWeapon, weapon);
				HandleWeaponSwitchInput(cmd, inputAllowed);
			}
			HandleFireInput(cmd, resetState, inputAllowed, weaponIndex, hasWeapon, weapon, weaponProfile, BoltNetwork.FrameDeltaTime);
			HandleMeleeInput(cmd, resetState, inputAllowed);
			HandleReloadInput(cmd, resetState, inputAllowed, weaponIndex, hasWeapon, weapon);
		}
	}

	protected override void ExecuteCommand(PlayerHitCommand cmd, bool resetState)
	{
		if (base.entity.isOwner && _loadoutController.HasOutfit)
		{
			if (BoltNetwork.ServerFrame - cmd.Input.HitServerFrame > 600)
			{
				cmd.Input.HitServerFrame = BoltNetwork.ServerFrame - 600;
			}
			int serverFrame = cmd.ServerFrame;
			WeaponStateAtFrame weaponStateAtFrame = WeaponHandler.WeaponStateAtFrame(cmd.Input.LaunchServerFrame);
			WeaponProfile.MeleeOptionData optionByDirection = _loadoutController.MeleeWeaponProfile.Melee.GetOptionByDirection((MeleeWeapon.Direction)cmd.Input.MeleeDirection);
			int num = optionByDirection.MovementDurationFrames + optionByDirection.CooldownFrames;
			if (_weaponValidator.IsValidHitCommandInput(serverFrame, cmd.Input, weaponStateAtFrame, base.state.NextMeleeTime, num))
			{
				_weaponValidator.ProcessValidInput(serverFrame, cmd.Input, out var _, weaponStateAtFrame, base.state.NextMeleeTime, num);
			}
		}
	}

	private void HandleMeleeInput(PlayerInputCommand cmd, bool resetState, bool inputAllowed)
	{
		if (resetState)
		{
			base.state.NextMeleeTime = cmd.Result.NextMeleeTime;
			return;
		}
		if (cmd.IsFirstExecution)
		{
			bool flag = false;
			bool flag2 = false;
			if (WeaponHandler.HasActiveWeapon && WeaponHandler.ActiveWeapon.LocksWeaponsWhileFiring)
			{
				flag2 = base.state.Loadouts[0].Weapons[base.state.Loadouts[0].ActiveWeapon].InCooldownUntil > cmd.ServerFrame;
				flag = flag2 && WeaponHandler.ActiveWeapon.LocksWeaponsWhileFiring && !WeaponHandler.ActiveWeapon.CanMeleeOutOfLock;
			}
			if (inputAllowed && cmd.Input.ShouldMelee && !flag && base.state.NextMeleeTime <= cmd.ServerFrame)
			{
				if (flag2 && WeaponHandler.ActiveWeapon.CanMeleeOutOfLock)
				{
					IPlayerInputCommandResult result = cmd.Result;
					int inCooldownUntil = (_activeWeapon.InCooldownUntil = BoltNetwork.ServerFrame);
					result.InCooldownUntil = inCooldownUntil;
					if (base.entity.isOwner)
					{
						_activeWeapon.NextFireFrame = BoltNetwork.ServerFrame + _activeWeaponProfile.Reload.TimeFrames;
						base.state.CancelFire();
						string id = WeaponHandler.ActiveWeaponProfile.Id;
						_statusEffectController.TryCancelActiveEffectsFromWeapon(id, base.entity);
						TryCancelChannelRoutine();
					}
				}
				WeaponProfile.MeleeOptionData optionByDirection = _loadoutController.MeleeWeaponProfile.Melee.GetOptionByDirection((MeleeWeapon.Direction)cmd.Input.MeleeDirection);
				base.state.NextMeleeTime = cmd.ServerFrame + optionByDirection.MovementDurationFrames + optionByDirection.CooldownFrames;
				if (base.entity.isOwner)
				{
					base.state.UseMeleeStanding();
				}
				_playerMotor.OverrideInput(cmd.Input.Horizontal, cmd.Input.Vertical, optionByDirection);
			}
		}
		cmd.Result.NextMeleeTime = base.state.NextMeleeTime;
	}

	private void HandleFireInput(PlayerInputCommand cmd, bool resetState, bool inputAllowed, int index, bool hasWeapon, Weapon weapon, WeaponProfile weaponProfile, float deltaTime)
	{
		if (!hasWeapon)
		{
			return;
		}
		if (resetState)
		{
			weapon.NextFireFrame = cmd.Result.NextFireTime;
			weapon.RemainingAmmo = cmd.Result.RemainingAmmo;
			weapon.InCooldownUntil = cmd.Result.InCooldownUntil;
			base.state.WeaponFiring = cmd.Result.WeaponFiring;
			base.state.WeaponCharging = cmd.Result.WeaponCharging;
			base.state.WeaponReloading = cmd.Result.WeaponReloading;
			base.state.WeaponExtended = cmd.Result.WeaponExtended;
			base.state.FullBodyPrimaryWeaponCharging = cmd.Result.FullBodyPrimaryWeaponCharging;
			base.state.FullBodyPrimaryWeaponFiring = cmd.Result.FullBodyPrimaryWeaponFiring;
			base.state.FullBodySecondaryWeaponCharging = cmd.Result.FullBodySecondaryWeaponCharging;
			base.state.FullBodySecondaryWeaponFiring = cmd.Result.FullBodySecondaryWeaponFiring;
			base.state.UpperBodyPrimaryWeaponCharging = cmd.Result.UpperBodyPrimaryWeaponCharging;
			base.state.UpperBodyPrimaryWeaponFiring = cmd.Result.UpperBodyPrimaryWeaponFiring;
			base.state.UpperBodySecondaryWeaponCharging = cmd.Result.UpperBodySecondaryWeaponCharging;
			base.state.UpperBodySecondaryWeaponFiring = cmd.Result.UpperBodySecondaryWeaponFiring;
			_fireHeldDuration = cmd.Result.FireHeldDuration;
			_fireHeldGraceFrames = cmd.Result.FireHeldGraceFrames;
			return;
		}
		if (cmd.Input.ShouldFire && inputAllowed && CanFireWeapon(index, cmd.ServerFrame))
		{
			int num = ((!weaponProfile.Spinup.SpinsUp) ? weaponProfile.CooldownFrames : ((int)Mathf.Lerp(weaponProfile.Spinup.MinFrames, weaponProfile.CooldownFrames, Mathf.InverseLerp(0f, weaponProfile.Spinup.FramesToFull, _fireHeldDuration))));
			weapon.NextFireFrame = cmd.ServerFrame + num;
			weapon.InCooldownUntil = weapon.NextFireFrame;
			int remainingAmmo = Mathf.Max(0, weapon.RemainingAmmo - cmd.Input.AmmoUsed);
			weapon.RemainingAmmo = remainingAmmo;
			TryStartAmmoRecharge(weapon.RemainingAmmo);
			if (weapon.RemainingAmmo <= 0)
			{
				if (weaponProfile.Reload.InBackground)
				{
					weapon.NextFireFrame += weaponProfile.Reload.TimeFrames;
					weapon.RemainingAmmo = weapon.MaxAmmo;
				}
				else
				{
					Reload(index, cmd.ServerFrame, weaponProfile.Reload.Delay);
				}
			}
			if (base.entity.isOwner)
			{
				ServerHandleFireInput(cmd, weapon);
			}
		}
		if (_playerController.IsAlive && _loadoutController.Weapons[index] != null && _loadoutController.Weapons[index].AnimationController != null && _loadoutController.Weapons[index].AnimationController.HasBodyAnimation)
		{
			FiringWeaponBodyAnimationData bodyAnimation = _loadoutController.Weapons[index].AnimationController.BodyAnimation;
			int serverFrame = cmd.ServerFrame;
			bool flag = weaponProfile.ItemType == ItemType.primaryWeapon;
			bool flag2 = weapon.ReloadStartFrame + weaponProfile.Reload.TimeFrames > serverFrame;
			bool flag3 = weapon.NextFireFrame >= serverFrame && !flag2;
			bool flag4 = weapon.InCooldownUntil >= serverFrame;
			FiringWeapon firingWeapon = _loadoutController.Weapons[index];
			bool flag5 = false;
			if (base.entity.isOwner)
			{
				flag5 = !flag3 && !flag2 && weapon.ChargeValue > 0f;
			}
			else
			{
				ChargingWeapon chargingWeapon = firingWeapon as ChargingWeapon;
				if (chargingWeapon != null)
				{
					flag5 = chargingWeapon.IsCharging;
				}
			}
			switch (bodyAnimation.Section)
			{
			case PlayerAnimationController.SectionLayer.FullBody:
				cmd.Result.FullBodyPrimaryWeaponCharging = flag5 && flag;
				cmd.Result.FullBodyPrimaryWeaponFiring = flag4 && flag;
				cmd.Result.FullBodySecondaryWeaponCharging = flag5 && !flag;
				cmd.Result.FullBodySecondaryWeaponFiring = flag4 && !flag;
				break;
			case PlayerAnimationController.SectionLayer.UpperBody:
				cmd.Result.UpperBodyPrimaryWeaponCharging = flag5 && flag;
				cmd.Result.UpperBodyPrimaryWeaponFiring = flag4 && flag;
				cmd.Result.UpperBodySecondaryWeaponCharging = flag5 && !flag;
				cmd.Result.UpperBodySecondaryWeaponFiring = flag4 && !flag;
				break;
			default:
				cmd.Result.WeaponFiring = flag3;
				cmd.Result.WeaponCharging = flag5;
				cmd.Result.WeaponReloading = flag2;
				break;
			}
		}
		if (_hasActiveWeapon && _loadoutController.Weapons[index] != null)
		{
			cmd.Result.WeaponExtended = _loadoutController.Weapons[index].AnimationController.IsExtended;
		}
		else
		{
			cmd.Result.WeaponExtended = false;
		}
		if (_loadoutController.MeleeWeapon != null)
		{
			cmd.Result.MeleeExtended = _loadoutController.MeleeWeapon.IsTriggerMeleeExtended;
		}
		if (cmd.Result.MeleeExtended)
		{
			base.state.MeleeExtended();
		}
		base.state.WeaponFiring = cmd.Result.WeaponFiring;
		base.state.WeaponCharging = cmd.Result.WeaponCharging;
		base.state.WeaponReloading = cmd.Result.WeaponReloading;
		base.state.WeaponExtended = cmd.Result.WeaponExtended;
		base.state.FullBodyPrimaryWeaponCharging = cmd.Result.FullBodyPrimaryWeaponCharging;
		base.state.FullBodyPrimaryWeaponFiring = cmd.Result.FullBodyPrimaryWeaponFiring;
		base.state.FullBodySecondaryWeaponCharging = cmd.Result.FullBodySecondaryWeaponCharging;
		base.state.FullBodySecondaryWeaponFiring = cmd.Result.FullBodySecondaryWeaponFiring;
		base.state.UpperBodyPrimaryWeaponCharging = cmd.Result.UpperBodyPrimaryWeaponCharging;
		base.state.UpperBodyPrimaryWeaponFiring = cmd.Result.UpperBodyPrimaryWeaponFiring;
		base.state.UpperBodySecondaryWeaponCharging = cmd.Result.UpperBodySecondaryWeaponCharging;
		base.state.UpperBodySecondaryWeaponFiring = cmd.Result.UpperBodySecondaryWeaponFiring;
		cmd.Result.InCooldownUntil = weapon.InCooldownUntil;
		cmd.Result.NextFireTime = weapon.NextFireFrame;
		cmd.Result.RemainingAmmo = weapon.RemainingAmmo;
		if (!inputAllowed || !hasWeapon || weapon.IsReloading || !_playerController.IsAlive)
		{
			_fireHeldDuration = 0;
			_fireHeldGraceFrames = 0;
		}
		else if (cmd.Input.FireHeld)
		{
			_fireHeldDuration++;
			if (weaponProfile.Spinup.SpinsUp && _fireHeldDuration > weaponProfile.Spinup.FramesToFull)
			{
				_fireHeldDuration = weaponProfile.Spinup.FramesToFull;
			}
		}
		else if (_fireHeldGraceFrames >= 3)
		{
			_fireHeldDuration--;
			if (_fireHeldDuration < 0)
			{
				_fireHeldDuration = 0;
				_fireHeldGraceFrames = 0;
			}
		}
		else
		{
			_fireHeldGraceFrames++;
		}
		cmd.Result.FireHeldDuration = _fireHeldDuration;
		cmd.Result.FireHeldGraceFrames = _fireHeldGraceFrames;
	}

	private void ServerHandleFireInput(PlayerInputCommand cmd, Weapon weapon)
	{
		if (WeaponHandler.ActiveWeaponProfile.Charge.CanCharge)
		{
			if (cmd.Input.AmmoUsed > weapon.RemainingAmmo)
			{
				cmd.Input.AmmoUsed = weapon.RemainingAmmo;
			}
		}
		else
		{
			cmd.Input.AmmoUsed = WeaponHandler.ActiveWeaponProfile.VolleySize;
		}
		BoltEntity hitVictim = null;
		_weaponValidator.TrackFireHistory(cmd, WeaponHandler.ActiveWeaponProfile);
		if (WeaponHandler.ActiveWeapon.HitType.IsRaycast())
		{
			IPlayerHitCommandInput playerHitCommandInput = PlayerHitCommand.Create();
			playerHitCommandInput.HitType = 0;
			playerHitCommandInput.LaunchServerFrame = cmd.ServerFrame;
			playerHitCommandInput.Origin = cmd.Input.Position;
			playerHitCommandInput.Forward = cmd.Input.Forward;
			if (WeaponHandler.ActiveWeaponProfile.Spread.IsAoE)
			{
				playerHitCommandInput.Point = playerHitCommandInput.Origin;
			}
			else
			{
				if (WeaponHandler.ActiveWeaponProfile.Spread.IsSpread)
				{
					WeaponStateAtFrame weaponStateAtFrame = WeaponHandler.WeaponStateAtFrame(cmd.ServerFrame);
					int shotCount = weapon.MaxAmmo - weapon.RemainingAmmo;
					playerHitCommandInput.Forward = Match.GetSpreadForward(playerHitCommandInput.Forward, weaponStateAtFrame.WeaponUp, weaponStateAtFrame.WeaponRight, WeaponHandler.ActiveWeaponProfile.Spread.Amount, shotCount);
					cmd.Input.Forward = playerHitCommandInput.Forward;
				}
				if (!Physics.Raycast(playerHitCommandInput.Origin, playerHitCommandInput.Forward, out var hitInfo, 500f, LayerMaskConfig.GroundLayers))
				{
					hitInfo.point = playerHitCommandInput.Origin + playerHitCommandInput.Forward * 200f;
				}
				playerHitCommandInput.Point = hitInfo.point;
			}
			playerHitCommandInput.HitServerFrame = cmd.ServerFrame;
			playerHitCommandInput.WeaponId = WeaponHandler.ActiveWeaponProfile.Id;
			playerHitCommandInput.ChargeTime = cmd.Input.ChargeTime;
			int serverFrame = cmd.ServerFrame;
			WeaponStateAtFrame weaponStateAtFrame2 = WeaponHandler.WeaponStateAtFrame(playerHitCommandInput.LaunchServerFrame);
			int cooldownFrames = _loadoutController.MeleeWeaponProfile.CooldownFrames;
			if (_weaponValidator.IsValidHitCommandInput(serverFrame, playerHitCommandInput, weaponStateAtFrame2, base.state.NextMeleeTime, cooldownFrames))
			{
				_weaponValidator.ProcessValidInput(serverFrame, playerHitCommandInput, out hitVictim, weaponStateAtFrame2, base.state.NextMeleeTime, cooldownFrames);
			}
		}
		if (WeaponHandler.ActiveWeaponProfile.SelfEffects.Length != 0)
		{
			if (WeaponHandler.ActiveWeapon.IsChannelingWeapon)
			{
				TryCancelChannelRoutine();
				_channelingStatusEffectRoutine = StartCoroutine(ApplyStatusEffectAfterChannel(WeaponHandler.ActiveWeaponProfile.Id, WeaponHandler.ActiveWeaponProfile.Cooldown));
			}
			else
			{
				for (int i = 0; i < WeaponHandler.ActiveWeaponProfile.SelfEffects.Length; i++)
				{
					_statusEffectController.TryApplyEffect(weapon.Id, WeaponHandler.ActiveWeaponProfile.SelfEffects[i], base.entity);
				}
			}
		}
		SendFiredEvent(cmd, hitVictim);
	}

	private IEnumerator ApplyStatusEffectAfterChannel(string startingWeaponId, float channelTime)
	{
		yield return new WaitForSeconds(channelTime);
		if (_playerController.IsAlive && WeaponHandler.HasActiveWeapon && WeaponHandler.ActiveWeaponProfile.Id == startingWeaponId && WeaponHandler.ActiveWeaponProfile.SelfEffects.Length != 0)
		{
			for (int i = 0; i < WeaponHandler.ActiveWeaponProfile.SelfEffects.Length; i++)
			{
				_statusEffectController.TryApplyEffect(WeaponHandler.ActiveWeaponProfile.Id, WeaponHandler.ActiveWeaponProfile.SelfEffects[i], base.entity);
			}
		}
		_channelingStatusEffectRoutine = null;
	}

	private void TryCancelChannelRoutine()
	{
		if (_channelingStatusEffectRoutine != null)
		{
			StopCoroutine(_channelingStatusEffectRoutine);
			_channelingStatusEffectRoutine = null;
		}
	}

	private void HandleWeaponSwitchInput(PlayerInputCommand cmd, bool inputAllowed)
	{
		int activeWeapon = base.state.Loadouts[0].ActiveWeapon;
		if (!inputAllowed || !cmd.Input.ShouldSwitchWeapon || WeaponIsLocked(activeWeapon, cmd.ServerFrame))
		{
			return;
		}
		int weaponSlot = cmd.Input.WeaponSlot;
		if (weaponSlot < 0)
		{
			base.state.Loadouts[0].ActiveWeapon = -1;
		}
		else if (CanSwitchTo(weaponSlot))
		{
			base.state.Loadouts[0].ActiveWeapon = weaponSlot;
			int cooldownFrames = _loadoutController.WeaponProfiles[weaponSlot].CooldownFrames;
			Weapon weapon = base.state.Loadouts[0].Weapons[weaponSlot];
			if (!_loadoutController.Weapons[weaponSlot].IsAbilityWeapon && weapon.NextFireFrame - cmd.ServerFrame > cooldownFrames)
			{
				weapon.NextFireFrame = cmd.ServerFrame;
			}
		}
	}

	private void HandleReloadInput(PlayerInputCommand cmd, bool resetState, bool inputAllowed, int index, bool hasWeapon, Weapon weapon)
	{
		if (!hasWeapon)
		{
			return;
		}
		if (resetState)
		{
			weapon.IsReloading = cmd.Result.IsReloading;
			weapon.ReloadStartFrame = cmd.Result.ReloadStartFrame;
			return;
		}
		if (cmd.IsFirstExecution && inputAllowed && cmd.Input.ShouldReload)
		{
			Reload(index, cmd.ServerFrame);
		}
		cmd.Result.IsReloading = weapon.IsReloading;
		cmd.Result.ReloadStartFrame = weapon.ReloadStartFrame;
	}

	private void SendFiredEvent(PlayerInputCommand cmd, BoltEntity victim)
	{
		bool isRaycast = WeaponHandler.ActiveWeapon.HitType.IsRaycast();
		FiredEvent firedEvent = FiredEvent.Create(base.entity, EntityTargets.EveryoneExceptOwnerAndController);
		firedEvent.Position = cmd.Input.Position;
		firedEvent.Forward = cmd.Input.Forward;
		firedEvent.IsRaycast = isRaycast;
		firedEvent.TimeFired = cmd.ServerFrame;
		firedEvent.TimeRaised = BoltNetwork.ServerFrame;
		if (firedEvent.IsRaycast)
		{
			firedEvent.Victim = victim;
		}
		firedEvent.Send();
	}

	private void CopyInputParametersToState(PlayerInputCommand cmd, bool inputAllowed, int index, bool hasWeapon, Weapon weapon)
	{
		base.state.CameraOffset = cmd.Input.CameraOffset;
		if (index >= 0 && hasWeapon && _loadoutController.HasOutfit && _loadoutController.Weapons != null && !(_loadoutController.Weapons[index] == null) && weapon != null && _loadoutController.Weapons[index] is ChargingWeapon && weapon.ChargeValue != cmd.Input.ChargeValue)
		{
			if (!inputAllowed)
			{
				cmd.Input.ChargeValue = 0f;
			}
			base.state.Loadouts[0].Weapons[index].ChargeValue = cmd.Input.ChargeValue;
		}
	}

	private bool CanFireWeapon(int index, int serverFrame)
	{
		if (!_hasActiveWeapon || _activeWeapon.IsReloading || WeaponIsFireLocked(index, serverFrame))
		{
			return false;
		}
		Weapon weapon = base.state.Loadouts[0].Weapons[index];
		if (weapon.RemainingAmmo > 0)
		{
			return weapon.NextFireFrame <= serverFrame;
		}
		return false;
	}

	private void DealDamageToHit(HitInfo hit)
	{
		if (hit.collider == null)
		{
			return;
		}
		IDamageable componentInParent = hit.collider.GetComponentInParent<IDamageable>();
		bool flag = componentInParent.entity == base.entity;
		WeaponProfile weaponProfile = (hit.weaponProfile = _profileManager.GetById<WeaponProfile>(hit.weaponId));
		if (componentInParent.entity.gameObject.CompareTag("Player"))
		{
			IPlayerState playerState = componentInParent.entity.GetState<IPlayerState>();
			if (playerState.IsShielded || (playerState.Team == base.state.Team && (!flag || !weaponProfile.Explosion.Explodes)))
			{
				Debug.Log($"[WeaponController] Ignoring damage from hit (is teammate and not explosive):\n{hit}");
				return;
			}
		}
		Debug.Log($"[WeaponController] Dealing damage from hit:\n{hit}");
		hit.hitEntity.GetComponent<IDamageable>().TakeDamage(hit, weaponProfile.CalculateDamage(hit, _loadoutController, flag, base.state.DamageModifier, base.state.MeleeDamageModifier), base.entity, weaponProfile.Effects);
		WeaponProfile.KnockbackData knockback = weaponProfile.Knockback;
		if (weaponProfile.IsMelee)
		{
			knockback = weaponProfile.Melee.GetOptionByDirection(hit.meleeDirection).Knockback;
		}
		if (knockback.HasKnockback)
		{
			hit.hitEntity.GetComponent<IMovable>()?.ForceMove(hit.forward * knockback.Force, knockback.BreaksMelee);
		}
	}

	private void SpawnEntity(WeaponProfile weaponProfile, IPlayerHitCommandInput input)
	{
		if (_matchStateHelper.MatchStateCached != MatchState.Active)
		{
			return;
		}
		Vector3 direction = Vector3.Reflect(input.ForwardOnHit, input.Normal);
		BoltEntity boltEntity = BoltNetwork.Instantiate(weaponProfile.GetSpawnedEntityId(), input.Point, input.ProjectileRotation);
		boltEntity.GetComponent<ISpawnedEntityController>().Setup(weaponProfile, base.entity);
		BouncingProjectile component = boltEntity.GetComponent<BouncingProjectile>();
		if (component != null)
		{
			component.Launch(direction, DealDamageToHit);
		}
		ChainsawShotgunEntity component2 = boltEntity.GetComponent<ChainsawShotgunEntity>();
		if (component2 != null)
		{
			if (input.Victim != null)
			{
				component2.StickToPlayer(input.Victim);
			}
			component2.ListenForExplosion(DealDamageToHit);
		}
		TNTEntity component3 = boltEntity.GetComponent<TNTEntity>();
		if (component3 != null)
		{
			component3.ListenForExplosion(DealDamageToHit);
		}
		IDarkMatter darkMatter = boltEntity.GetState<IDarkMatter>();
		if (darkMatter != null)
		{
			darkMatter.OwnerTeam = base.state.Team;
			darkMatter.Owner = base.entity;
			darkMatter.Scale = weaponProfile.Explosion.Range;
			boltEntity.transform.localScale = Vector3.one * weaponProfile.Explosion.Range;
		}
		if (weaponProfile.SpawnedEntity.IsDeployable)
		{
			IDeployableState deployableState = boltEntity.GetState<IDeployableState>();
			Damageable damageable = deployableState.Damageable;
			float health = (deployableState.Damageable.MaxHealth = weaponProfile.SpawnedEntity.Health);
			damageable.Health = health;
			deployableState.Team = base.state.Team;
			deployableState.Owner = base.entity;
			deployableState.WeaponId = weaponProfile.Id;
			boltEntity.GetComponent<IDamageable>();
			_deployableController.TrackDeployable(weaponProfile, boltEntity);
		}
	}

	public override void OnEvent(FiredEvent firedEvent)
	{
		if (!base.entity.isControllerOrOwner)
		{
			if (firedEvent.IsRaycast && firedEvent.Victim != null)
			{
				IDamageable componentInChildren = firedEvent.Victim.GetComponentInChildren<IDamageable>();
				firedEvent.Forward = (componentInChildren.HurtCollider.bounds.center - firedEvent.Position).normalized;
			}
			WeaponHandler.MockFire(firedEvent.Position, firedEvent.Forward, 0);
		}
	}

	public int GetServerFrame()
	{
		return BoltNetwork.ServerFrame;
	}

	private void OnWeaponSlotSelected(WeaponSlotSelectedSignal signal)
	{
		if (signal.index != base.state.Loadouts[0].ActiveWeapon)
		{
			_weaponSwitchRequest.requested = true;
			_weaponSwitchRequest.index = signal.index;
		}
	}

	private void OnWeaponHit(HitInfo hit)
	{
		if (!base.entity.isControlled || base.entity.isOwner || hit.hitType == HitType.Raycast)
		{
			Debug.Log($"[WeaponController] Not sending hit, !entity.isControlled = {!base.entity.isControlled}, entity.isOwner = {base.entity.isOwner}, hit.hitType = {hit.hitType}");
			return;
		}
		hit.hitServerFrame = BoltNetwork.ServerFrame;
		IDamageable damageable = null;
		if (hit.collider != null)
		{
			Debug.Log($"[WeaponController] {hit.collider.name} has been hit by {base.name}. Launched on {hit.launchServerFrame}, hit on {hit.hitServerFrame}\n{hit}");
			damageable = hit.collider.gameObject.GetComponentInParent<IDamageable>();
		}
		if (damageable == null && !hit.weaponProfile.ShouldSendGroundHitToServer(hit.hitType))
		{
			if (damageable == null)
			{
				Debug.Log("[WeaponController] Not sending hit, didn't hit anything");
			}
			else if (!hit.weaponProfile.ShouldSendGroundHitToServer(hit.hitType))
			{
				Debug.Log("[WeaponController] Not sending hit, weapon doesn't care about ground hits");
			}
		}
		else
		{
			Debug.Log("[WeaponController] Caching hit to send next simulate controller tick");
			_hitCache.Add(hit);
		}
	}

	private bool ShouldDealDamage(IDamageable victim, WeaponProfile.EffectData effectData)
	{
		GameModeType gameModeType = (GameModeType)base.state.GameModeType;
		if (gameModeType == GameModeType.TeamDeathMatch || gameModeType == GameModeType.Survival)
		{
			if (victim.entity.networkId != base.entity.networkId)
			{
				if (victim.Team == _playerController.Team)
				{
					return effectData.InverseForAlly;
				}
				return true;
			}
			return false;
		}
		return true;
	}

	private bool CanSwitchTo(int index)
	{
		if (index >= 0 && index < base.state.Loadouts[0].Weapons.Length)
		{
			return !string.IsNullOrEmpty(base.state.Loadouts[0].Weapons[index].Id);
		}
		return false;
	}

	private void OnMeleeComplete()
	{
	}

	private void TryStartAmmoRecharge(int ammo)
	{
		if (_hasActiveWeapon && base.entity.isControllerOrOwner && _activeWeaponProfile.AmmoRecharge.CanRechargeAmmo && ammo > 0 && ammo < _activeWeaponProfile.ClipSize)
		{
			CancelAmmoRoutines();
			InvokeRepeating("RechargeAmmo", _activeWeaponProfile.AmmoRecharge.Delay, 1f);
		}
	}

	private void RechargeAmmo()
	{
		if (!_hasActiveWeapon || !base.entity.isControllerOrOwner)
		{
			return;
		}
		if (RemainingAmmo < _activeWeaponProfile.ClipSize)
		{
			Weapon activeWeapon = _activeWeapon;
			int num = Mathf.Clamp(RemainingAmmo + _activeWeaponProfile.AmmoRecharge.RecoveryRate, 0, activeWeapon.MaxAmmo);
			if (GameModeEntityHelper.AmmoClipsAreLimited)
			{
				int num2 = num - activeWeapon.RemainingAmmo;
				float num3 = Mathf.Min(base.state.AmmoClips, (float)num2 / (float)activeWeapon.MaxAmmo);
				if (num3 > 0f)
				{
					base.state.AmmoClips -= num3;
					SetRemainingAmmo(base.state.Loadouts[0].ActiveWeapon, num);
				}
			}
			else
			{
				SetRemainingAmmo(base.state.Loadouts[0].ActiveWeapon, num);
			}
		}
		else
		{
			CancelInvoke();
		}
	}

	private void Reload(int index, int serverFrame, float delay = 0f)
	{
		if (base.entity.isControllerOrOwner && !base.state.Stunned && !base.state.WeaponsDisabled && _reloadRoutine == null && (!GameModeEntityHelper.AmmoClipsAreLimited || !(base.state.AmmoClips <= 0f)) && !_loadoutController.WeaponProfiles[index].Reload.InBackground)
		{
			CancelInvoke("RechargeAmmo");
			_reloadRoutine = StartCoroutine(ReloadRoutine(index, serverFrame, delay));
		}
	}

	protected IEnumerator ReloadRoutine(int index, int serverFrame, float delay)
	{
		if (delay > 0f)
		{
			int num = Mathf.RoundToInt(delay / BoltNetwork.FrameDeltaTime);
			serverFrame += num;
			yield return new WaitForSeconds(delay);
		}
		Weapon weapon = base.state.Loadouts[0].Weapons[index];
		_reloadingWeapon = _loadoutController.Weapons[index];
		if (_reloadingWeapon.AnimationController.BackpackOperation == WeaponAnimationController.BackpackOp.OpenForReload)
		{
			_loadoutController.Backpack.AnimationController.Open(playAudio: false);
		}
		weapon.ReloadStartFrame = serverFrame;
		weapon.IsReloading = true;
		weapon.NextFireFrame = serverFrame + _reloadingWeapon.Profile.Reload.TimeFrames;
		if (_reloadingWeapon.AudioPlayer != null)
		{
			_reloadingWeapon.AudioPlayer.PlayReload();
		}
		yield return new WaitForSeconds(_reloadingWeapon.Profile.Reload.Time);
		if (_reloadingWeapon.AudioPlayer != null)
		{
			_reloadingWeapon.AudioPlayer.StopReload();
		}
		int maxAmmo = weapon.MaxAmmo;
		if (GameModeEntityHelper.AmmoClipsAreLimited)
		{
			int remainingAmmo = weapon.RemainingAmmo;
			float b = (float)(maxAmmo - remainingAmmo) / (float)maxAmmo;
			float num2 = Mathf.Min(base.state.AmmoClips, b);
			if (num2 > 0f)
			{
				base.state.AmmoClips -= num2;
				SetRemainingAmmo(index, maxAmmo);
			}
		}
		else
		{
			SetRemainingAmmo(index, maxAmmo);
		}
		if (_reloadingWeapon.AnimationController.BackpackOperation == WeaponAnimationController.BackpackOp.OpenForReload)
		{
			_loadoutController.Backpack.AnimationController.Close();
		}
		base.state.Loadouts[0].Weapons[index].IsReloading = false;
		_reloadingWeapon = null;
		_reloadRoutine = null;
	}

	private void CancelReload()
	{
		if (_reloadRoutine == null)
		{
			return;
		}
		StopCoroutine(_reloadRoutine);
		_reloadRoutine = null;
		if (_reloadingWeapon != null)
		{
			if (_reloadingWeapon.AudioPlayer != null)
			{
				_reloadingWeapon.AudioPlayer.StopReload();
			}
			_reloadingWeapon = null;
		}
	}

	private void CancelAmmoRoutines()
	{
		if (base.entity.isControllerOrOwner)
		{
			CancelReload();
			for (int i = 0; i < base.state.Loadouts[0].Weapons.Length; i++)
			{
				base.state.Loadouts[0].Weapons[i].IsReloading = false;
			}
			CancelInvoke("RechargeAmmo");
		}
	}

	private void SetRemainingAmmo(int index, int remainingAmmo)
	{
		base.state.Loadouts[0].Weapons[index].RemainingAmmo = remainingAmmo;
	}

	private void OnWeaponEquipped(IState iState, string propertyPath, ArrayIndices arrayIndices)
	{
		int num = arrayIndices[1];
		if (num == base.state.Loadouts[0].ActiveWeapon)
		{
			CancelAmmoRoutines();
		}
		Weapon weapon = base.state.Loadouts[0].Weapons[num];
		if (!string.IsNullOrEmpty(weapon.Id))
		{
			WeaponProfile byId = _profileManager.GetById<WeaponProfile>(weapon.Id);
			weapon.RemainingAmmo = byId.ClipSize;
			if (base.entity.isOwner)
			{
				weapon.MaxAmmo = byId.ClipSize;
			}
		}
		else
		{
			weapon.RemainingAmmo = 0;
			if (base.entity.isOwner)
			{
				weapon.MaxAmmo = 0;
			}
		}
		if (!base.entity.isOwner)
		{
			WeaponHandler.ClearChargeValueCache();
			_signalBus.Fire(new ChargeValueUpdatedSignal(-1, 0f));
		}
	}

	private void OnActiveWeaponChanged()
	{
		int index = base.state.Loadouts[0].ActiveWeapon;
		CancelAmmoRoutines();
		if (index < 0)
		{
			WeaponHandler.StowWeapon(delegate
			{
				WeaponProfile byId = _profileManager.GetById<WeaponProfile>(base.state.Loadouts[0].MeleeWeapon.Id);
				_playerMotor.SetProfileProperties(_loadoutController.OutfitProfile.HeroClassProfile, byId);
				FireWeaponSlotUpdatedSignal(index, byId);
			});
			WeaponHandler.DrawMeleeWeapon();
		}
		else
		{
			WeaponHandler.SheatheMeleeWeapon();
			DeployWeapon(index);
		}
	}

	public void DeployWeapon(int index)
	{
		WeaponHandler.DeployWeapon(index, delegate
		{
			Weapon weapon = base.state.Loadouts[0].Weapons[index];
			WeaponProfile byId = _profileManager.GetById<WeaponProfile>(weapon.Id);
			_playerMotor.SetProfileProperties(_loadoutController.OutfitProfile.HeroClassProfile, byId);
			if (base.entity.isControllerOrOwner)
			{
				if (weapon.RemainingAmmo <= 0)
				{
					Reload(index, BoltNetwork.ServerFrame);
				}
				else
				{
					TryStartAmmoRecharge(RemainingAmmo);
				}
			}
			FireWeaponSlotUpdatedSignal(index, byId);
		});
	}

	private void FireWeaponSlotUpdatedSignal(int index, WeaponProfile weaponProfile)
	{
		if (base.entity.isControlled && !base.entity.isOwner && !base.state.IsSecondLife)
		{
			_signalBus.Fire(new ActiveWeaponSlotUpdatedSignal
			{
				ActiveWeaponSlotIndex = index,
				Profile = weaponProfile
			});
		}
	}

	private void OnAmmoUpdated(IState iState, string propertyPath, ArrayIndices arrayIndices)
	{
		int num = arrayIndices[1];
		Weapon weapon = base.state.Loadouts[0].Weapons[num];
		if (!string.IsNullOrEmpty(weapon.Id) && !_loadoutController.WeaponProfiles[num].Reload.InBackground)
		{
			_signalBus.Fire(new WeaponStateUpdatedSignal(num, weapon.RemainingAmmo, weapon.MaxAmmo));
		}
	}

	private void UpdateReloadIndicator(int index)
	{
		ReloadStateUpdatedSignal reloadStateUpdatedSignal;
		if (index < 0)
		{
			reloadStateUpdatedSignal = new ReloadStateUpdatedSignal(isReloading: false, null, 0f, 0f);
		}
		else
		{
			Weapon weapon = base.state.Loadouts[0].Weapons[index];
			reloadStateUpdatedSignal = new ReloadStateUpdatedSignal(weapon.IsReloading, _loadoutController.WeaponProfiles[index], (float)weapon.ReloadStartFrame * BoltNetwork.FrameDeltaTime, BoltNetwork.ServerTime);
		}
		_signalBus.Fire(reloadStateUpdatedSignal);
	}

	private void OnChargeValueUpdated(IState iState, string propertyPath, ArrayIndices arrayIndices)
	{
		int num = arrayIndices[1];
		if (_loadoutController.Weapons[num] != null && _loadoutController.Weapons[num] is ChargingWeapon)
		{
			(_loadoutController.Weapons[num] as ChargingWeapon).SetChargeValue(base.state.Loadouts[0].Weapons[num].ChargeValue);
		}
	}

	private void OnCameraOffsetUpdated()
	{
		if (_loadoutController.HasOutfit)
		{
			_loadoutController.Outfit.AimPointHandler.OnCameraOffsetXChanged(base.state.CameraOffset);
		}
	}

	private void OnCancelFire()
	{
		if (_hasActiveWeapon)
		{
			WeaponHandler.ActiveWeapon.CancelFire();
		}
	}

	public bool WeaponIsLocked(int index, int serverFrame)
	{
		if (!WeaponIndexIsValid(index))
		{
			return false;
		}
		Weapon weapon = base.state.Loadouts[0].Weapons[index];
		FiringWeapon firingWeapon = _loadoutController.Weapons[index];
		ChargingWeapon chargingWeapon = firingWeapon as ChargingWeapon;
		if (chargingWeapon != null && chargingWeapon.LocksWeaponsWhileFiring && chargingWeapon.IsCharging)
		{
			return true;
		}
		if (firingWeapon.LocksWeaponsWhileFiring)
		{
			return weapon.InCooldownUntil > serverFrame;
		}
		return false;
	}

	public bool WeaponIsFireLocked(int index, int serverFrame)
	{
		if (!WeaponIndexIsValid(index))
		{
			return false;
		}
		Weapon weapon = base.state.Loadouts[0].Weapons[index];
		if (_loadoutController.Weapons[index].LocksWeaponsWhileFiring)
		{
			return weapon.InCooldownUntil > serverFrame;
		}
		return false;
	}

	private void OnUseMeleeForward()
	{
		OnUseMelee(MeleeWeapon.Direction.Forward);
	}

	private void OnUseMeleeBackward()
	{
		OnUseMelee(MeleeWeapon.Direction.Backward);
	}

	private void OnUseMeleeLeft()
	{
		OnUseMelee(MeleeWeapon.Direction.Left);
	}

	private void OnUseMeleeRight()
	{
		OnUseMelee(MeleeWeapon.Direction.Right);
	}

	private void OnUseMeleeStanding()
	{
		OnUseMelee(MeleeWeapon.Direction.Standing);
	}

	private void OnUseMelee(MeleeWeapon.Direction direction)
	{
		_loadoutController.MeleeWeapon.MockSwing(direction);
	}
}
