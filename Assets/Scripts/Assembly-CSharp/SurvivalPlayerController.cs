using System.Collections;
using BSCore;
using Bolt;
using Constants;
using UnityEngine;
using Zenject;

public class SurvivalPlayerController : GameModePlayerController
{
	[Inject]
	private SignalBus _signalBus;

	private StatusEffectController _statusEffectController;

	private float _breakawayRadius = 5f;

	private int _breakawayUseCooldown = 600;

	private bool _breakawayPressed;

	private int _lastBreakawayUse;

	private bool _respawnPressed;

	private bool _hasRequestedRespawn;

	protected override UIPrefabIds _matchHudPrefabId => UIPrefabIds.SurvivalMatchHud;

	protected override UIPrefabIds _deathScreenPrefabId => UIPrefabIds.SurvivalDeathScreen;

	private bool _canUseBreakaway
	{
		get
		{
			if (base.state.InputEnabled && ((Match.StatusType)base.state.StatusFlags).Has(Match.StatusType.Hugged))
			{
				return _lastBreakawayUse + _breakawayUseCooldown <= BoltNetwork.ServerFrame;
			}
			return false;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_statusEffectController = GetComponent<StatusEffectController>();
	}

	private void Update()
	{
		if (base.entity.isControlled && !base.entity.isOwner)
		{
			_breakawayPressed |= BSCoreInput.GetButtonDown(Option.UseSpecial);
		}
	}

	public override void Attached()
	{
		if (((PlayerController.AttachToken)base.entity.attachToken).GameModeType == GameModeType.Survival)
		{
			base.Attached();
			if (base.entity.isControlled)
			{
				base.state.AddCallback("LastSpecialUse", UpdateLastBreakawayUse);
				UpdateLastBreakawayUse();
				_signalBus.Subscribe<TryRequestRespawn>(OnRespawnRequested);
			}
		}
	}

	public override void Detached()
	{
		if (((PlayerController.AttachToken)base.entity.attachToken).GameModeType == GameModeType.Survival)
		{
			base.Detached();
			if (base.entity.isControlled)
			{
				base.state.RemoveCallback("LastSpecialUse", UpdateLastBreakawayUse);
				_signalBus.Unsubscribe<TryRequestRespawn>(OnRespawnRequested);
			}
		}
	}

	public override void SimulateController()
	{
		if (base.state.GameModeType == 2)
		{
			_breakawayPressed |= BSCoreInput.GetButtonDown(Option.UseSpecial);
			if (_breakawayPressed && _canUseBreakaway)
			{
				base.entity.QueueInput(UseBreakawayCommand.Create());
				_lastBreakawayUse = BoltNetwork.ServerFrame;
			}
			if (_respawnPressed && !_hasRequestedRespawn && base.state.Damageable.Health == 0f)
			{
				base.entity.QueueInput(RequestRespawnCommand.Create());
				_hasRequestedRespawn = true;
				Invoke("ClearHasRequestedRespawn", 1f);
			}
			_breakawayPressed = false;
			_respawnPressed = false;
			int num = _lastBreakawayUse + _breakawayUseCooldown - BoltNetwork.ServerFrame;
			float chargePercent = 1f;
			if (num > 0)
			{
				chargePercent = 1f - (float)num / (float)_breakawayUseCooldown;
			}
			_signalBus.TryFire(new SpecialAbilityStateUpdatedSignal(_canUseBreakaway, chargePercent));
		}
	}

	private void ClearHasRequestedRespawn()
	{
		_hasRequestedRespawn = false;
	}

	public override void ExecuteCommand(Command command, bool resetState)
	{
		if (base.state.GameModeType == 2)
		{
			if (base.entity.isOwner && command is UseBreakawayCommand)
			{
				UseBreakawayAttack(command as UseBreakawayCommand);
			}
			else if (base.entity.isOwner && command is RequestRespawnCommand)
			{
				Respawn(command as RequestRespawnCommand);
			}
		}
	}

	private void UseBreakawayAttack(UseBreakawayCommand command)
	{
		if (base.entity.isOwner && _canUseBreakaway)
		{
			base.state.LastSpecialUse = BoltNetwork.ServerFrame;
			StartCoroutine(DisableInputForBreakaway());
			WeaponProfile.EffectData effectData = new WeaponProfile.EffectData(new WeaponProfile.WeaponProfileData.EffectProfileData
			{
				effectType = Match.EffectType.ForcedMelee.ToString(),
				duration = 1.4f
			});
			_statusEffectController.TryApplyEffect("breakaway", effectData, base.entity);
			DebugExtension.DebugWireSphere(base.transform.position, Color.red, _breakawayRadius, 2f);
			Collider[] array = Physics.OverlapSphere(base.transform.position, _breakawayRadius, LayerMaskConfig.SurvivalEnemyLayers, QueryTriggerInteraction.Collide);
			foreach (Collider obj in array)
			{
				Vector3 vector = obj.transform.position - base.transform.position;
				vector.y = 0f;
				obj.GetComponent<SurvivalEnemy>().Knockback(vector.normalized);
			}
		}
	}

	private IEnumerator DisableInputForBreakaway()
	{
		base.state.BreakAway();
		base.state.InputEnabled = false;
		yield return new WaitForSeconds(1.4f);
		base.state.InputEnabled = true;
	}

	private void OnRespawnRequested()
	{
		if (base.entity.isControlled)
		{
			_respawnPressed = true;
		}
	}

	private void Respawn(RequestRespawnCommand command)
	{
		if (base.entity.isOwner)
		{
			_signalBus.Fire(new PlayerRequestedRespawnSignal(base.entity));
		}
	}

	private void UpdateLastBreakawayUse()
	{
		_lastBreakawayUse = base.state.LastSpecialUse;
	}
}
