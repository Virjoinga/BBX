using System;
using System.Collections;
using BSCore;
using BSCore.Constants.Config;
using Bolt;
using Constants;
using TMPro;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(HealthController))]
[RequireComponent(typeof(WeaponController))]
public class PlayerController : BaseEntityBehaviour<IPlayerState, PlayerInputCommand, PlayerLoadedCommand>, IPlayerController, IMovable
{
	public class AttachToken : IProtocolToken
	{
		public int gameModeType;

		public GameModeType GameModeType => (GameModeType)gameModeType;

		public AttachToken()
		{
		}

		public AttachToken(GameModeType gameModeType)
		{
			this.gameModeType = (int)gameModeType;
		}

		public void Read(UdpPacket packet)
		{
			gameModeType = packet.ReadInt();
		}

		public void Write(UdpPacket packet)
		{
			packet.WriteInt(gameModeType);
		}
	}

	[Serializable]
	public class SpeedHackDetectionConfigData
	{
		public int Threshold = 15;

		public int Frequency = 60;

		public float PenaltyDuration = 30f;

		public string WarningMessage;

		public string KickMessage;
	}

	[Serializable]
	private class CharacterModelInfo
	{
		public string Id;

		public Animator Model;
	}

	public const float ACTIVE_ZOOM_SPEED_MODIFIER = -0.4f;

	[Inject]
	private SignalBus _signalBus;

	[Inject]
	private ZenjectInstantiater _zenjectInstantiator;

	[Inject]
	private DataStoreManager _dataStoreManager;

	[Inject]
	private ConfigManager _configManager;

	[Inject]
	protected GameConfigData _gameConfigData;

	[Inject]
	private EmoticonData _emoticonData;

	[SerializeField]
	private CapsuleCollider _hurtCollider;

	[SerializeField]
	private HealthDisplay _healthDisplay;

	[SerializeField]
	private GameObject _playerInfoDisplay;

	[SerializeField]
	private GameObject _healthBar;

	[SerializeField]
	private PlayerAudioStateController _playerAudioStateController;

	[SerializeField]
	private TextMeshProUGUI _displayNameText;

	[SerializeField]
	private SimplePlayerMotor _defaultPlayerMotor;

	[SerializeField]
	private BoltKinematicPlayerController _kinematicPlayerMotor;

	[SerializeField]
	private MinimapArrow _miniMapArrow;

	[SerializeField]
	private ParticleSystem _particleSystem;

	[SerializeField]
	private GameObject _cameraPrefab;

	[SerializeField]
	private EmoteCameraController _emoteCameraController;

	[SerializeField]
	private FadeableUI _emoticonIconFadeable;

	[SerializeField]
	private Image _emoticonIcon;

	private Outfit _outfit;

	private WeaponController _weaponController;

	private HealthController _healthController;

	private bool _playerReadyCommandSent;

	private DataStoreFloat _mouseSensitivity;

	private PlayerColliderInitializer _playerColliderInitializer;

	private DataStoreFloat _cameraOffsetX;

	private DataStoreFloat _cameraOffsetZ;

	private SpeedHackDetectionConfigData _speedHackDetectionConfig;

	private int _frameCount;

	private int _commandCount;

	private bool _hackDetected;

	private bool _inTimeout;

	private bool _showingEmoticon;

	private int _requestedEmoticon = -1;

	private Coroutine _ownerEmoteRoutine;

	public static PlayerController LocalPlayer { get; private set; }

	public static bool HasLocalPlayer => LocalPlayer != null;

	public AimPointHandler AimPointHandler { get; private set; }

	public IPlayerMotor Motor { get; private set; }

	public Vector3 Velocity
	{
		get
		{
			if (!base.entity.isControlled)
			{
				return base.state.Movable.Velocity;
			}
			return _kinematicPlayerMotor.State.Velocity;
		}
	}

	public PlayerAnimationController AnimationController { get; private set; }

	public IPlayerMotor DefaultPlayerMotor => _kinematicPlayerMotor;

	public LoadoutController LoadoutController { get; private set; }

	public WeaponHandler WeaponHandler => _weaponController.WeaponHandler;

	public StatusEffectController StatusEffectController { get; private set; }

	public bool IsAlive => base.state.Damageable.Health > 0f;

	public CapsuleCollider HurtCollider => _hurtCollider;

	public bool CanEmote { get; set; } = true;

	public bool IsLocal => base.entity.isControlled;

	public int Team => base.state.Team;

	public bool HasTeamSet { get; private set; }

	public bool IsLocalPlayerTeammate
	{
		get
		{
			if (LocalPlayer == null)
			{
				return false;
			}
			if (!base.entity.isControlled)
			{
				return Team == LocalPlayer.Team;
			}
			return false;
		}
	}

	public Transform AimPoint => AimPointHandler.AimPoint;

	public bool LocalInputBlocked { get; set; }

	IPlayerState IPlayerController.state => base.state;

	Transform IPlayerController.transform => base.transform;

	GameObject IPlayerController.gameObject => base.gameObject;

	protected override void Awake()
	{
		_playerInfoDisplay.SetActive(value: false);
		Motor = _kinematicPlayerMotor;
		LoadoutController = GetComponent<LoadoutController>();
		_weaponController = GetComponent<WeaponController>();
		_healthController = GetComponent<HealthController>();
		StatusEffectController = GetComponent<StatusEffectController>();
		AnimationController = GetComponent<PlayerAnimationController>();
		_playerColliderInitializer = GetComponent<PlayerColliderInitializer>();
	}

	private void Update()
	{
		if (base.entity.isControlled && !base.entity.isOwner && !BoltNetwork.IsServer)
		{
			_playerAudioStateController.SetAudioState(base.state.Speed, base.state.StrafeSpeed, base.state.Movable.IsGrounded);
		}
	}

	private void FixedUpdate()
	{
		_frameCount++;
		_commandCount--;
		AnimationController.IsDead = base.state.Damageable.Health <= 0f;
	}

	private void CheckForSpeedHack()
	{
		_commandCount++;
		if (_frameCount < _speedHackDetectionConfig.Frequency)
		{
			return;
		}
		if (_commandCount > _speedHackDetectionConfig.Threshold)
		{
			base.state.InputEnabled = false;
			ServerMessageEvent serverMessageEvent = ServerMessageEvent.Create(base.entity.controller, ReliabilityModes.ReliableOrdered);
			if (_hackDetected && !_inTimeout)
			{
				serverMessageEvent.MessageId = 3;
				Invoke("KickFromServer", 0.25f);
			}
			else
			{
				serverMessageEvent.MessageId = 2;
				Invoke("ReenableInput", _speedHackDetectionConfig.PenaltyDuration);
				_hackDetected = true;
				_inTimeout = true;
			}
			serverMessageEvent.Send();
		}
		_frameCount = 0;
	}

	private void ReenableInput()
	{
		base.state.InputEnabled = true;
		_inTimeout = false;
	}

	private void KickFromServer()
	{
		base.entity.controller.Disconnect();
	}

	protected override void OnAnyAttached()
	{
		base.name = $"Player-{base.entity.networkId}";
		base.state.SetTransforms(base.state.Transform, base.transform);
		_healthController.Respawned += OnRespawn;
		IPlayerState playerState = base.state;
		playerState.OnEmote = (Action)Delegate.Combine(playerState.OnEmote, new Action(PlayEmote));
		IPlayerState playerState2 = base.state;
		playerState2.OnBreakAway = (Action)Delegate.Combine(playerState2.OnBreakAway, new Action(OnBreakAway));
		_displayNameText.text = base.state.DisplayName;
		base.state.AddCallback("DisplayName", (PropertyCallbackSimple)delegate
		{
			_displayNameText.text = base.state.DisplayName;
		});
	}

	protected override void OnControllerOnlyAttached()
	{
		MonoBehaviourSingleton<MouseLockToggle>.Instance.MouseCanLock = true;
		MonoBehaviourSingleton<MouseLockToggle>.Instance.TryLockCursor();
		_mouseSensitivity = _dataStoreManager.GetStore<DataStoreFloat, float>(DataStoreKeys.MouseSensitivity);
		_weaponController.WeaponHandler.SetZoomSensitivity(_dataStoreManager.GetStore<DataStoreFloat, float>(DataStoreKeys.ZoomSensitivity));
		_signalBus.Subscribe<RequestEmoticonSignal>(OnEmoticonRequested);
	}

	protected override void OnControllerOrRemoteAttached()
	{
		_healthController.Changed += OnHealthChanged;
		_healthController.MaxChanged += OnMaxHealthChanged;
		_healthController.Died += OnDeath;
		base.state.AddCallback("StatusFlags", OnStatusFlagsUpdated);
		base.state.AddCallback("Team", OnTeamSet);
		base.state.AddCallback("Emoticon", OnEmoticonUpdated);
		StartCoroutine(WaitForDisplayNameRoutine());
	}

	private IEnumerator WaitForDisplayNameRoutine()
	{
		yield return new WaitUntil(() => !string.IsNullOrEmpty(base.state.DisplayName));
		_signalBus.Fire(new ClientPlayerLoadedSignal(base.state.DisplayName));
	}

	protected override void OnRemoteOnlyAttached()
	{
		_miniMapArrow.gameObject.SetActive(value: false);
	}

	protected override void OnOwnerOnlyAttached()
	{
		base.state.Emoticon = -1;
		_speedHackDetectionConfig = _configManager.Get<SpeedHackDetectionConfigData>(DataKeys.speedHackDetection);
		base.state.AddCallback("Team", (PropertyCallbackSimple)delegate
		{
			MonoBehaviourSingleton<SpawnManager>.Instance.RegisterPlayerTransfrom(base.transform, (TeamId)base.state.Team);
		});
	}

	protected override void OnControllerOnlyDetached()
	{
		MonoBehaviourSingleton<MouseLockToggle>.Instance.MouseCanLock = false;
		MonoBehaviourSingleton<MouseLockToggle>.Instance.ReleaseCursor();
	}

	public override void Detached()
	{
		if (base.entity.isControlled && !base.entity.isOwner)
		{
			if (LocalPlayer == this)
			{
				LocalPlayer = null;
			}
			_signalBus.TryUnsubscribe<RequestEmoticonSignal>(OnEmoticonRequested);
		}
		if (!base.entity.isOwner)
		{
			_healthController.Changed -= OnHealthChanged;
			_healthController.MaxChanged -= OnMaxHealthChanged;
			_healthController.Died -= OnDeath;
		}
		_healthController.Respawned -= OnRespawn;
	}

	protected override void OnOwnerOnlyDetached()
	{
		MonoBehaviourSingleton<SpawnManager>.Instance.DeRegisterPlayerTransform(base.transform, (TeamId)base.state.Team);
	}

	public override void ControlGained()
	{
		LocalPlayer = this;
		Debug.Log("[PlayerController] Local player attached");
	}

	public override void ControlLost()
	{
		if (LocalPlayer == this)
		{
			LocalPlayer = null;
		}
		Debug.Log("[PlayerController] Local player lost");
	}

	private void OnEmoticonRequested(RequestEmoticonSignal signal)
	{
		_requestedEmoticon = signal.EmoticonId;
	}

	public override void SimulateController()
	{
		IPlayerInputCommandInput input = PlayerInputCommand.Create();
		if (!LocalInputBlocked)
		{
			Vector2 vector = new Vector2(DeadZoneMaxed(BSCoreInput.GetAxis(Option.Horizontal), 0.3f), DeadZoneMaxed(BSCoreInput.GetAxis(Option.Vertical), 0.3f));
			if (StatusEffectController.HasStatus(Match.StatusType.Tripping))
			{
				vector.x *= -1f;
				vector.y *= -1f;
			}
			if (base.state.Movable.ForcedMovement)
			{
				vector.y = 1f;
			}
			input.Horizontal = vector.x;
			input.Vertical = vector.y;
			if (!MouseLockToggle.MouseLockReleased)
			{
				float value = _mouseSensitivity.Value;
				float num = BSCoreInput.GetAxis(Option.CameraHorizontal);
				float num2 = BSCoreInput.GetAxis(Option.CameraVertical);
				if (BSCoreInput.UsingJoystick)
				{
					num *= 20f;
					num2 *= 20f;
				}
				input.CameraHorizontal = num * value;
				input.CameraVertical = num2 * value;
			}
			bool flag = base.state.InputEnabled && !base.state.Stunned;
			input.Jump = flag && BSCoreInput.GetButtonDown(Option.Jump);
			input.Emote = flag && BSCoreInput.GetButtonDown(Option.Emote);
			input.Emoticon = _requestedEmoticon;
		}
		else
		{
			input.Emoticon = -1;
		}
		_requestedEmoticon = -1;
		_weaponController.SimulateWeaponController(ref input);
		base.entity.QueueInput(input);
		if (MatchMap.Loaded && !_playerReadyCommandSent)
		{
			base.entity.QueueInput(PlayerLoadedCommand.Create());
		}
	}

	private float DeadZoneClamped(float value, float deadZone)
	{
		return Mathf.Clamp(DeadZoned(value, deadZone), -1f, 1f);
	}

	private float DeadZoned(float value, float deadZone)
	{
		if (!(Mathf.Abs(value) < deadZone))
		{
			return value;
		}
		return 0f;
	}

	private float DeadZoneMaxed(float value, float deadZone)
	{
		float num = DeadZoned(value, deadZone);
		if (num == 0f)
		{
			return 0f;
		}
		return (num > 0f) ? 1 : (-1);
	}

	public Vector2 SquareUpCircularInput(Vector2 input)
	{
		if (input.sqrMagnitude == 0f)
		{
			return Vector2.zero;
		}
		Vector2 normalized = input.normalized;
		float value = ((normalized.x == 0f || !(normalized.y >= -0.70710677f) || !(normalized.y <= 0.70710677f)) ? (input.x / Mathf.Abs(normalized.y)) : ((normalized.x >= 0f) ? (input.x / normalized.x) : ((0f - input.x) / normalized.x)));
		return new Vector2(y: Mathf.Clamp((normalized.y == 0f || !(normalized.x >= -0.70710677f) || !(normalized.x <= 0.70710677f)) ? (input.y / Mathf.Abs(normalized.x)) : ((normalized.y >= 0f) ? (input.y / normalized.y) : ((0f - input.y) / normalized.y)), -1f, 1f), x: Mathf.Clamp(value, -1f, 1f));
	}

	protected override void ExecuteCommand(PlayerInputCommand cmd, bool resetState)
	{
		if (_outfit == null)
		{
			return;
		}
		if (resetState)
		{
			_kinematicPlayerMotor.SetState(CommandResultToPlayerMotorState(cmd.Result));
			return;
		}
		if (base.entity.isOwner)
		{
			CheckForSpeedHack();
		}
		Vector2 movement = new Vector2(cmd.Input.Horizontal, cmd.Input.Vertical);
		if (base.state.Movable.ForcedMovement)
		{
			movement.y = 1f;
		}
		float cameraVertical = cmd.Input.CameraVertical;
		float cameraHorizontal = cmd.Input.CameraHorizontal;
		Vector2 camera = new Vector2(cameraVertical, cameraHorizontal);
		bool flag = base.state.InputEnabled && !base.state.Stunned;
		cmd.Input.Jump &= !base.state.Movable.PreventJump;
		float num = Mathf.Max(1f + base.state.Movable.SpeedIncrease + base.state.Movable.SpeedDecrease, 0f);
		if (!WeaponHandler.HasActiveWeapon && !WeaponHandler.IsTransitioningWeapon)
		{
			num += _outfit.Profile.HeroClassProfile.Speed.NoWeaponMultiplier;
		}
		else if (cmd.Input.IsZooming)
		{
			num += -0.4f;
		}
		PlayerMotorState animationStates = Motor.Move(movement, camera, cmd.Input.Jump, num, flag, BoltNetwork.FrameDeltaTime);
		AnimationController.SetAnimationStates(animationStates);
		SetPlayerMotorStateOnCommandResult(animationStates, cmd.Result);
		base.state.Movable.Velocity = cmd.Result.Velocity;
		base.state.Movable.IsGrounded = cmd.Result.IsGrounded;
		base.state.Speed = animationStates.Speed;
		base.state.StrafeSpeed = animationStates.StrafeSpeed;
		base.state.TurnSpeed = animationStates.TurnSpeed;
		base.state.VerticalSpeed = (animationStates.IsGrounded ? 0f : Mathf.Clamp(animationStates.Velocity.y, -1f, 1f));
		if (flag && cmd.IsFirstExecution && cmd.Input.Emote && CanEmote && !_weaponController.WeaponIsLocked(base.state.Loadouts[0].ActiveWeapon, cmd.ServerFrame) && base.entity.isOwner)
		{
			_ownerEmoteRoutine = StartCoroutine(DisableInputForEmote());
		}
		if (base.entity.isOwner && cmd.IsFirstExecution && cmd.Input.Emoticon != -1 && !_showingEmoticon)
		{
			StartCoroutine(ShowEmoticonRoutine(cmd.Input.Emoticon));
		}
	}

	private PlayerMotorState CommandResultToPlayerMotorState(IPlayerInputCommandResult result)
	{
		return new PlayerMotorState
		{
			Position = result.Position,
			Rotation = result.Rotation,
			Velocity = result.Velocity,
			IsGrounded = result.IsGrounded,
			Aim = result.Aim,
			IsRotatingToForward = result.IsRotatingToForward,
			RotationSign = result.RotationSign,
			IsJumping = result.IsJumping,
			JumpedThisFrame = result.JumpedThisFrame,
			JumpRequested = result.JumpRequested,
			TimeSinceJumpRequested = result.TimeSinceJumpRequested,
			TimeSinceLastAbleToJump = result.TimeSinceLastAbleToJump,
			JumpConsumed = result.JumpConsumed,
			IsOnJumpPad = result.IsOnJumpPad,
			HasConsumedJumpPadJump = result.HasConsumedJumpPadJump,
			PrevFrameHadInput = result.PrevFrameHadInput,
			MustUnground = result.MustUnground,
			MustUngroundTime = result.MustUngroundTime,
			LastMovementIterationFoundAnyGround = result.LastMovementIterationFoundAnyGround,
			FoundAnyGround = result.FoundAnyGround,
			IsStableOnGround = result.IsStableOnGround,
			TimeSinceLastGrounded = result.TimeSinceLastGrounded,
			SnappingPrevented = result.SnappingPrevented,
			GroundNormal = result.GroundNormal,
			InnerGroundNormal = result.InnerGroundNormal,
			OuterGroundNormal = result.OuterGroundNormal,
			IsMeleeing = result.IsMeleeing,
			InputOverriden = result.InputOverriden,
			InputOverride = result.InputOverride,
			VelocityOverriden = result.VelocityOverriden,
			VelocityOverrideDurationRemaining = result.VelocityOverrideDurationRemaining,
			VelocityOverrideRecoveryRemaining = result.VelocityOverrideRecoveryRemaining,
			VelocityOverridenUntilGrounded = result.VelocityOverridenUntilGrounded,
			DoubleJumpCooldown = result.DoubleJumpCooldown,
			DoubleJumpConsumed = result.DoubleJumpConsumed,
			DoubleJumpedThisFrame = result.DoubleJumpedThisFrame
		};
	}

	private void SetPlayerMotorStateOnCommandResult(PlayerMotorState state, IPlayerInputCommandResult result)
	{
		result.Position = state.Position;
		result.Rotation = state.Rotation;
		result.Velocity = state.Velocity;
		result.IsGrounded = state.IsGrounded;
		result.Aim = state.Aim;
		result.IsRotatingToForward = state.IsRotatingToForward;
		result.RotationSign = state.RotationSign;
		result.IsJumping = state.IsJumping;
		result.JumpedThisFrame = state.JumpedThisFrame;
		result.JumpRequested = state.JumpRequested;
		result.TimeSinceJumpRequested = state.TimeSinceJumpRequested;
		result.TimeSinceLastAbleToJump = state.TimeSinceLastAbleToJump;
		result.JumpConsumed = state.JumpConsumed;
		result.IsOnJumpPad = state.IsOnJumpPad;
		result.HasConsumedJumpPadJump = state.HasConsumedJumpPadJump;
		result.PrevFrameHadInput = state.PrevFrameHadInput;
		result.MustUnground = state.MustUnground;
		result.MustUngroundTime = state.MustUngroundTime;
		result.LastMovementIterationFoundAnyGround = state.LastMovementIterationFoundAnyGround;
		result.FoundAnyGround = state.FoundAnyGround;
		result.IsStableOnGround = state.IsStableOnGround;
		result.TimeSinceLastGrounded = state.TimeSinceLastGrounded;
		result.SnappingPrevented = state.SnappingPrevented;
		result.GroundNormal = state.GroundNormal;
		result.InnerGroundNormal = state.InnerGroundNormal;
		result.OuterGroundNormal = state.OuterGroundNormal;
		result.IsMeleeing = state.IsMeleeing;
		result.InputOverriden = state.InputOverriden;
		result.InputOverride = state.InputOverride;
		result.VelocityOverriden = state.VelocityOverriden;
		result.VelocityOverrideDurationRemaining = state.VelocityOverrideDurationRemaining;
		result.VelocityOverrideRecoveryRemaining = state.VelocityOverrideRecoveryRemaining;
		result.VelocityOverridenUntilGrounded = state.VelocityOverridenUntilGrounded;
		result.DoubleJumpCooldown = state.DoubleJumpCooldown;
		result.DoubleJumpConsumed = state.DoubleJumpConsumed;
		result.DoubleJumpedThisFrame = state.DoubleJumpedThisFrame;
	}

	public void TryCancelEmote()
	{
		if (_ownerEmoteRoutine != null)
		{
			StopCoroutine(_ownerEmoteRoutine);
		}
	}

	private IEnumerator DisableInputForEmote()
	{
		base.state.Emote();
		base.state.InputEnabled = false;
		int activeWeaponIndex = _weaponController.WeaponHandler.ActiveWeaponIndex;
		if (activeWeaponIndex >= 0)
		{
			base.state.Loadouts[0].ActiveWeapon = -1;
		}
		yield return _emoteCameraController.EmoteRoutine(AnimationController.GetEmoteLength());
		if (base.state.Damageable.Health > 0f)
		{
			base.state.InputEnabled = true;
			if (activeWeaponIndex >= 0)
			{
				base.state.Loadouts[0].ActiveWeapon = activeWeaponIndex;
			}
		}
		_ownerEmoteRoutine = null;
	}

	private IEnumerator ShowEmoticonRoutine(int emoteId)
	{
		_showingEmoticon = true;
		base.state.Emoticon = emoteId;
		yield return new WaitForSeconds(EmoteWheel.EMOTICON_SHOW_DURATION);
		base.state.Emoticon = -1;
		_showingEmoticon = false;
	}

	protected override void ExecuteCommand(PlayerLoadedCommand cmd, bool resetState)
	{
		if (resetState)
		{
			_playerReadyCommandSent = cmd.Result.Received;
		}
		else if (base.entity.isOwner)
		{
			Debug.Log($"[PlayerController] Got Loaded Command from {base.entity.networkId} - {Time.realtimeSinceStartup}");
			_signalBus.Fire(new PlayerLoadedSignal(base.entity));
			cmd.Result.Received = true;
		}
	}

	public void InitializeCharacter(Outfit outfit)
	{
		_outfit = outfit;
		AnimationController.SetAnimationStates(Motor.State);
		_weaponController.InitializeCharacter(outfit);
		_playerAudioStateController.Init(outfit);
		AimPointHandler = outfit.AimPointHandler;
		Motor.InitializeCharacter(outfit);
		_playerColliderInitializer.InitializeCharacter(outfit);
		Motor.SetProfileProperties(_outfit.Profile.HeroClassProfile, null);
		if (base.entity.isControlled && !base.entity.isOwner)
		{
			if (MonoBehaviourSingleton<OTSCamera>.IsInstantiated)
			{
				MonoBehaviourSingleton<OTSCamera>.Instance.SetTarget(AimPointHandler);
			}
			else
			{
				_zenjectInstantiator.Instantiate(_cameraPrefab).GetComponentInChildren<OTSCamera>().SetTarget(AimPointHandler);
			}
			if (_cameraOffsetX == null)
			{
				_cameraOffsetX = _dataStoreManager.GetStore<DataStoreFloat, float>(DataStoreKeys.CameraOffsetX);
			}
			if (_cameraOffsetZ == null)
			{
				_cameraOffsetZ = _dataStoreManager.GetStore<DataStoreFloat, float>(DataStoreKeys.CameraOffsetZ);
			}
			_outfit.AimPointHandler.SetCameraOffsetDataStore(_cameraOffsetX, _cameraOffsetZ);
		}
		base.state.SetTransforms(base.state.AimPivotTransform, outfit.AimPointHandler.transform);
	}

	public void Spawn(PlayerSpawnPoint spawnPoint, bool resetState = false)
	{
		SetPosition(spawnPoint.transform.position, spawnPoint.transform.rotation, resetVelocity: true);
		if (resetState)
		{
			base.state.Loadouts[0].Weapons[1].IsReloading = false;
			base.state.Loadouts[0].Weapons[2].IsReloading = false;
			base.state.Movable.SpeedDecrease = 0f;
			base.state.Movable.SpeedIncrease = 0f;
			base.state.StatusFlags = 0;
			base.state.LastSpecialUse = 0;
			base.state.Stunned = false;
			base.state.WeaponsDisabled = false;
		}
	}

	public void SetPosition(Vector3 position, Quaternion rotation, bool resetVelocity = false)
	{
		base.state.InputEnabled = true;
		Vector3 velocity = (resetVelocity ? Vector3.zero : base.state.Movable.Velocity);
		PlayerMotorState playerMotorState = new PlayerMotorState
		{
			Position = position,
			Rotation = rotation,
			Velocity = velocity,
			Aim = Vector3.zero,
			IsGrounded = false
		};
		Motor.SetState(playerMotorState);
		base.state.Movable.Velocity = velocity;
		base.state.Movable.IsGrounded = false;
		base.state.Speed = 0f;
		base.state.StrafeSpeed = 0f;
	}

	protected void OnHealthChanged()
	{
		Debug.Log($"[PlayerController] Health changed to {base.state.Damageable.Health}");
		if (base.entity.isControlled)
		{
			_signalBus.Fire(new PlayerHealthChangedSignal
			{
				UpdatedHealth = base.state.Damageable.Health,
				MaxHealth = base.state.Damageable.MaxHealth
			});
		}
		if (IsLocalPlayerTeammate)
		{
			_healthDisplay.UpdateHealthBar(base.state.Damageable.Health, base.state.Damageable.MaxHealth);
		}
	}

	protected void OnMaxHealthChanged()
	{
		Debug.Log($"[PlayerController] Max Health changed to {base.state.Damageable.MaxHealth}");
		if (base.entity.isControlled)
		{
			_signalBus.Fire(new PlayerHealthChangedSignal
			{
				UpdatedHealth = base.state.Damageable.Health,
				MaxHealth = base.state.Damageable.MaxHealth
			});
		}
		if (IsLocalPlayerTeammate)
		{
			_healthDisplay.UpdateHealthBar(base.state.Damageable.Health, base.state.Damageable.MaxHealth);
		}
	}

	public void ForceMove(Vector3 force, bool breaksMelee)
	{
		base.state.Movable.Velocity += force;
		base.state.Movable.IsGrounded = false;
		Motor.ApplyForce(force, breaksMelee);
	}

	public void OverridePlayerMotor(IPlayerMotor newPlayerMotor)
	{
		Motor = newPlayerMotor;
		PlayerMotorState playerMotorState = new PlayerMotorState
		{
			Position = base.transform.position,
			Rotation = base.transform.rotation,
			Velocity = base.state.Movable.Velocity,
			Aim = base.state.AimPivotTransform.Rotation.eulerAngles,
			IsGrounded = base.state.Movable.IsGrounded
		};
		Motor.SetState(playerMotorState);
	}

	public void ReleasePlayerMotorOverride()
	{
		Motor = _defaultPlayerMotor;
		PlayerMotorState playerMotorState = new PlayerMotorState
		{
			Position = base.transform.position,
			Rotation = base.transform.rotation,
			Velocity = base.state.Movable.Velocity,
			Aim = base.state.AimPivotTransform.Rotation.eulerAngles,
			IsGrounded = base.state.Movable.IsGrounded
		};
		Motor.SetState(playerMotorState);
	}

	private void OnBreakAway()
	{
		if (!base.entity.isOwner)
		{
			_particleSystem.Play();
		}
	}

	private void PlayEmote()
	{
		if (!base.entity.isOwner)
		{
			_playerAudioStateController.PlayEmoteMusic();
			if (base.entity.isControlled)
			{
				_emoteCameraController.EmoteRoutine(AnimationController.GetEmoteLength());
			}
		}
	}

	private void OnEmoticonUpdated()
	{
		if (base.state.Emoticon != -1)
		{
			StartCoroutine(ShowEmoticonIconRoutine(base.state.Emoticon));
			_signalBus.Fire(new ShowEmoticonSignal
			{
				Name = base.state.DisplayName,
				EmoticonId = base.state.Emoticon
			});
		}
	}

	private IEnumerator ShowEmoticonIconRoutine(int emoteId)
	{
		Sprite spriteForEmoticon = _emoticonData.GetSpriteForEmoticon((EmoticonType)emoteId);
		if (spriteForEmoticon != null)
		{
			_emoticonIcon.sprite = spriteForEmoticon;
		}
		_emoticonIconFadeable.FadeIn(0.25f, isInteractable: false);
		yield return new WaitForSeconds(EmoteWheel.EMOTICON_SHOW_DURATION);
		_emoticonIconFadeable.FadeOut(0.25f);
	}

	private void OnDeath()
	{
		if (!base.entity.isOwner)
		{
			_playerColliderInitializer.SetCollidersActiveState(isEnabled: false);
			_outfit.OnDeath();
			_playerInfoDisplay.SetActive(value: false);
			LocalInputBlocked = true;
		}
	}

	private void OnRespawn()
	{
		if (!base.entity.isOwner)
		{
			_playerColliderInitializer.SetCollidersActiveState(isEnabled: true);
			_playerInfoDisplay.SetActive(!IsLocal);
			if (_outfit != null)
			{
				_outfit.OnRespawn();
			}
			LocalInputBlocked = false;
		}
	}

	private void OnStatusFlagsUpdated()
	{
		StatusEffectController.OnStatusFlagsUpdated((Match.StatusType)base.state.StatusFlags);
		if (base.entity.isControlled)
		{
			_signalBus.TryFire(new StatusFlagsupdatedSignal((Match.StatusType)base.state.StatusFlags));
		}
	}

	private void OnTeamSet()
	{
		HasTeamSet = true;
		Debug.Log($"Team set for player name: {base.state.DisplayName} ; team: {base.state.Team}");
		if (!base.entity.isControlled)
		{
			StartCoroutine(SetupTeamRoutine());
		}
	}

	private IEnumerator SetupTeamRoutine()
	{
		Debug.Log("[PlayerController] Starting SetupTeamRoutine");
		yield return new WaitUntil(() => HasLocalPlayer && LocalPlayer.HasTeamSet && LoadoutController.HasOutfit);
		Debug.Log($"[PlayerController] Player {base.state.DisplayName} is on Local Player Team? {IsLocalPlayerTeammate}");
		_healthBar.SetActive(IsLocalPlayerTeammate);
		_displayNameText.color = (IsLocalPlayerTeammate ? _gameConfigData.TeammateColor : _gameConfigData.EnemyColor);
		_playerInfoDisplay.SetActive(!IsLocal);
		_miniMapArrow.gameObject.SetActive(IsLocalPlayerTeammate);
		if (IsLocalPlayerTeammate)
		{
			_miniMapArrow.SetAsTeammate();
		}
		Renderer[] componentsInChildren = LoadoutController.Outfit.GetComponentsInChildren<Renderer>();
		for (int num = 0; num < componentsInChildren.Length; num++)
		{
			componentsInChildren[num].material.SetFloat("_EnableGlow", (!IsLocalPlayerTeammate) ? 1 : 0);
		}
	}
}
