using System.Collections;
using BSCore;
using UnityEngine;
using Zenject;

public class OfflinePlayerController : MonoBehaviour, IPlayerController, IMovable
{
	[Inject]
	private DataStoreManager _dataStoreManager;

	[SerializeField]
	private CapsuleCollider _hurtCollider;

	[SerializeField]
	private bool _inputEnabled = true;

	[SerializeField]
	private PlayerAudioStateController _playerAudioStateController;

	[SerializeField]
	private OTSCamera _otsCamera;

	[SerializeField]
	private EmoteCameraController _emoteCameraController;

	private WeaponHandler _weaponHandler;

	private OfflineWeaponController _weaponController;

	private Outfit _outfit;

	private bool _jump;

	private bool _emote;

	private DataStoreFloat _cameraOffsetX;

	private DataStoreFloat _cameraOffsetZ;

	private DataStoreFloat _mouseSensitivity;

	public BoltEntity entity => null;

	public IPlayerState state => null;

	public CapsuleCollider HurtCollider => _hurtCollider;

	public Transform AimPoint => AimPointHandler.AimPoint;

	public PlayerAnimationController AnimationController { get; private set; }

	public int ServerFrame { get; private set; }

	public IPlayerMotor Motor { get; private set; }

	public AimPointHandler AimPointHandler { get; private set; }

	public Vector3 Velocity => Motor.State.Velocity;

	public bool LocalInputBlocked
	{
		get
		{
			return !_inputEnabled;
		}
		set
		{
			_inputEnabled = !value;
		}
	}

	public bool IsLocal => true;

	public bool IsLocalPlayerTeammate => true;

	Transform IPlayerController.transform => base.transform;

	GameObject IPlayerController.gameObject => base.gameObject;

	private void Awake()
	{
		Motor = GetComponent<IPlayerMotor>();
		_weaponHandler = GetComponent<WeaponHandler>();
		_weaponController = GetComponent<OfflineWeaponController>();
		AnimationController = GetComponent<PlayerAnimationController>();
	}

	private IEnumerator Start()
	{
		yield return null;
		MonoBehaviourSingleton<MouseLockToggle>.Instance.MouseCanLock = true;
		MonoBehaviourSingleton<MouseLockToggle>.Instance.TryLockCursor();
		_weaponController.WeaponHandler.SetZoomSensitivity(_dataStoreManager.GetStore<DataStoreFloat, float>(DataStoreKeys.ZoomSensitivity));
		_mouseSensitivity = _dataStoreManager.GetStore<DataStoreFloat, float>(DataStoreKeys.MouseSensitivity);
	}

	private void FixedUpdate()
	{
		_jump = BSCoreInput.GetButtonDown(Option.Jump);
		_emote = BSCoreInput.GetButtonDown(Option.Emote);
		if (_outfit == null)
		{
			return;
		}
		Vector2 movementInput = new Vector2(DeadZoneMaxed(BSCoreInput.GetAxis(Option.Horizontal), 0.3f), DeadZoneMaxed(BSCoreInput.GetAxis(Option.Vertical), 0.3f));
		float cameraVertical = 0f;
		float cameraHorizontal = 0f;
		if (!MouseLockToggle.MouseLockReleased)
		{
			_ = _mouseSensitivity.Value;
			cameraHorizontal = BSCoreInput.GetAxis(Option.CameraHorizontal);
			cameraVertical = BSCoreInput.GetAxis(Option.CameraVertical);
			if (BSCoreInput.UsingJoystick)
			{
				cameraHorizontal *= 20f;
				cameraVertical *= 20f;
			}
			cameraVertical *= _mouseSensitivity.Value;
			cameraHorizontal *= _mouseSensitivity.Value;
		}
		_weaponController.HandlePlayerInput(_inputEnabled, ref movementInput, ref cameraVertical, ref cameraHorizontal);
		Vector2 camera = new Vector2(cameraVertical, cameraHorizontal);
		float num = 1f;
		if (!_weaponHandler.HasActiveWeapon)
		{
			num += _outfit.Profile.HeroClassProfile.Speed.NoWeaponMultiplier;
		}
		else if (_weaponController.IsZooming)
		{
			num += -0.4f;
		}
		PlayerMotorState animationStates = Motor.Move(movementInput, camera, _jump, num, _inputEnabled, Time.fixedDeltaTime);
		AnimationController.SetAnimationStates(animationStates);
		_playerAudioStateController.SetAudioState(animationStates.Speed, animationStates.StrafeSpeed, animationStates.IsGrounded);
		if (_inputEnabled && _emote && _weaponController.WeaponIsLocked(_weaponHandler.ActiveWeaponIndex, ServerFrame))
		{
			StartCoroutine(DisableInputForEmote());
			_playerAudioStateController.PlayEmoteMusic();
		}
		_jump = false;
		_emote = false;
		ServerFrame++;
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

	private IEnumerator DisableInputForEmote()
	{
		AnimationController.SetTrigger(PlayerAnimationController.Parameter.Emote);
		_inputEnabled = false;
		int activeWeaponIndex = _weaponController.WeaponHandler.ActiveWeaponIndex;
		if (activeWeaponIndex >= 0)
		{
			_weaponController.WeaponHandler.StowWeapon();
		}
		yield return _emoteCameraController.EmoteRoutine(AnimationController.GetEmoteLength());
		_inputEnabled = true;
		if (activeWeaponIndex >= 0)
		{
			_weaponController.WeaponHandler.DeployWeapon(activeWeaponIndex);
		}
	}

	public void InitializeCharacter(Outfit outfit)
	{
		_outfit = outfit;
		AnimationController.SetAnimationStates(Motor.State);
		Motor.InitializeCharacter(_outfit);
		Motor.SetProfileProperties(_outfit.Profile.HeroClassProfile, null);
		_playerAudioStateController.Init(_outfit);
		AimPointHandler = _outfit.AimPointHandler;
		_otsCamera.SetTarget(AimPointHandler);
		_weaponController.WeaponHandler.OnOutfitSet();
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

	public void SetPosition(Vector3 position, Quaternion rotation, bool resetVelocity = false)
	{
		PlayerMotorState playerMotorState = new PlayerMotorState
		{
			Position = position,
			Rotation = rotation,
			Velocity = (resetVelocity ? Vector3.zero : Motor.State.Velocity),
			Aim = Vector3.zero,
			IsGrounded = false
		};
		Motor.SetState(playerMotorState);
	}

	private void OnGUI()
	{
		float num = Mathf.Max(0f, Motor.State.DoubleJumpCooldown);
		GUI.Label(new Rect(10f, 110f, 200f, 20f), "Double Jump cooldown: " + num.ToString("n2") + "s");
	}

	public void ForceMove(Vector3 force, bool breaksMelee)
	{
	}
}
