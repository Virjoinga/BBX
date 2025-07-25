using System.Collections;
using Bolt;
using UnityEngine;
using Zenject;

public class BRPlayerController : GameModePlayerController
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private BattleRoyaleSpawnMotor _brSpawnMotor;

	[SerializeField]
	private PlayerAudioStateController _audioStateController;

	[SerializeField]
	private AudioClip _secondLifeAudio;

	private PlayerController _playerController;

	private StatusEffectController _statusEffectController;

	private bool _secondLifeClicked;

	private PlayerAnimationController _animationController;

	protected override UIPrefabIds _matchHudPrefabId => UIPrefabIds.BRMatchHud;

	protected override UIPrefabIds _deathScreenPrefabId => UIPrefabIds.BRDeathScreen;

	private Outfit _outfit => _playerController.LoadoutController.Outfit;

	private LoadoutController _loadoutController => _playerController.LoadoutController;

	protected override void Awake()
	{
		base.Awake();
		_playerController = GetComponent<PlayerController>();
		_statusEffectController = GetComponent<StatusEffectController>();
		_animationController = GetComponent<PlayerAnimationController>();
	}

	public override void Attached()
	{
		if (((PlayerController.AttachToken)base.entity.attachToken).GameModeType == GameModeType.BattleRoyale)
		{
			base.Attached();
			if (base.entity.isOwner)
			{
				StartCoroutine(CheckForLanding());
			}
			else
			{
				StartCoroutine(CheckForLanding());
				base.state.AddCallback("IsSecondLife", OnIsSecondLifeUpdated);
			}
			if (base.entity.isControlled)
			{
				_signalBus.Subscribe<SecondLifeButtonClickedSignal>(OnSecondLifeButtonClicked);
				_signalBus.Subscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
			}
		}
	}

	public override void Detached()
	{
		if (base.state.GameModeType != 1)
		{
			return;
		}
		base.Detached();
		if (base.entity.isControlled)
		{
			if (!base.state.IsSecondLife)
			{
				_signalBus.Unsubscribe<SecondLifeButtonClickedSignal>(OnSecondLifeButtonClicked);
			}
			_signalBus.Unsubscribe<MatchStateUpdatedSignal>(OnMatchStateUpdated);
		}
		if (!base.entity.isOwner)
		{
			base.state.RemoveCallback("IsSecondLife", OnIsSecondLifeUpdated);
		}
	}

	private IEnumerator CheckForLanding()
	{
		if (base.entity.isControlled || base.entity.isOwner)
		{
			_playerController.CanEmote = false;
			_playerController.OverridePlayerMotor(_brSpawnMotor);
		}
		yield return new WaitUntil(() => _loadoutController.HasOutfit);
		yield return new WaitForSeconds(0.25f);
		_animationController.Dive();
		_loadoutController.MeleeWeapon.gameObject.SetActive(value: false);
		float landtransitionDuration = 0.8f;
		Ray ray = new Ray(base.transform.position, Vector3.down);
		while (true)
		{
			float num = Mathf.Abs(base.state.Movable.Velocity.y) * landtransitionDuration;
			ray.origin = base.transform.position;
			DebugExtension.DebugArrow(ray.origin, ray.direction * num, Color.blue);
			if (Physics.SphereCast(ray, _outfit.MovementColliderData.Radius, num, LayerMaskConfig.GroundLayers))
			{
				break;
			}
			yield return null;
		}
		_brSpawnMotor.InputEnabledOverride = false;
		_animationController.StartDiveLand();
		yield return new WaitForSeconds(0.8f);
		_loadoutController.MeleeWeapon.gameObject.SetActive(value: true);
		_animationController.FinishDiveLand();
		yield return new WaitForSeconds(1f);
		if (base.entity.isControlled || base.entity.isOwner)
		{
			_playerController.ReleasePlayerMotorOverride();
			_playerController.CanEmote = true;
		}
	}

	private void OnSecondLifeButtonClicked(SecondLifeButtonClickedSignal signal)
	{
		_secondLifeClicked = true;
	}

	public override void SimulateController()
	{
		if (base.state.GameModeType == 1 && base.state.CanSecondLife && base.entity.isControlled && _secondLifeClicked && base.state.Damageable.Health <= 0f)
		{
			base.entity.QueueInput(RequestRespawnCommand.Create());
			_secondLifeClicked = false;
		}
	}

	public override void ExecuteCommand(Command command, bool resetState)
	{
		if (base.state.GameModeType == 1 && base.state.CanSecondLife && base.entity.isOwner && command is RequestRespawnCommand)
		{
			HandleBRAfterDeathCommand(command as RequestRespawnCommand);
		}
	}

	private void HandleBRAfterDeathCommand(RequestRespawnCommand cmd)
	{
		_signalBus.Fire(new PlayerRequestedRespawnSignal(base.entity));
	}

	private void OnIsSecondLifeUpdated()
	{
		if (base.state.IsSecondLife)
		{
			_signalBus.Unsubscribe<SecondLifeButtonClickedSignal>(OnSecondLifeButtonClicked);
			OnStartSecondLife();
		}
	}

	private void OnStartSecondLife()
	{
		UIPrefabManager.Instantiate(UIPrefabIds.BRSecondLifeHud);
		UIPrefabManager.Destroy(UIPrefabIds.BRDeathScreen);
		ClientBattleRoyaleCallbacks.IsSpectating = false;
		ClientBattleRoyaleCallbacks.CancelSpectateDelay = true;
		StartCoroutine(OnStartSecondLifeRoutine());
	}

	private IEnumerator OnStartSecondLifeRoutine()
	{
		_playerController.CanEmote = false;
		yield return new WaitUntil(() => _loadoutController.HasOutfit && _outfit.Profile.HeroClass == HeroClass.secondLife);
		_outfit.SetFadeLevel(0f);
		yield return null;
		_audioStateController.PlayAudioOverride(_secondLifeAudio);
		yield return null;
		_outfit.SetFadeLevel(1f);
		yield return new WaitForSeconds(2.667f);
		_playerController.CanEmote = true;
	}

	private void OnMatchStateUpdated(MatchStateUpdatedSignal signal)
	{
		if (signal.MatchState == MatchState.Complete && base.entity.isOwner)
		{
			_statusEffectController.KillAllEffects();
		}
	}
}
