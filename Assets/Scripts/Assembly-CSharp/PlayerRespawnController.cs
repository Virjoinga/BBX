using System.Linq;
using UnityEngine;
using Zenject;

public class PlayerRespawnController : BaseEntityBehaviour<IPlayerState, RequestRespawnCommand, UpdateRespawnSelectionsCommand>
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private GameObject _minimapArrow;

	[SerializeField]
	private GameObject _deathIcon;

	private HealthController _healthController;

	private string _cachedNextRespawnPointId = string.Empty;

	private float _cachedCanRespawnTime;

	private float _cachedForceRespawnTime;

	private bool _sendRequestRespawnCommand;

	private bool _sendUpdateRespawnSelectionCommand;

	private string _updatedSpawnPointId = string.Empty;

	private string _updatedLoadoutHeroClass = string.Empty;

	protected override void Awake()
	{
		_healthController = GetComponent<HealthController>();
	}

	protected override void OnAnyAttached()
	{
		if (!base.entity.isOwner)
		{
			_healthController.Died += OnDeath;
			_healthController.Respawned += OnRespawn;
			if (base.entity.isControlled)
			{
				_signalBus.Subscribe<TDMPlayersUpdatedSignal>(OnTDMPlayersUpdated);
				_signalBus.Subscribe<ClientUpdateRespawnSelectionsSignal>(SendUpdateRespawnSelectionsCommand);
				_signalBus.Subscribe<ClientRequestRespawnSignal>(SendRespawnRequestCommand);
			}
		}
	}

	protected override void OnAnyDetached()
	{
		if (!base.entity.isOwner)
		{
			_healthController.Died -= OnDeath;
			_healthController.Respawned -= OnRespawn;
			if (base.entity.isControlled)
			{
				_signalBus.Unsubscribe<TDMPlayersUpdatedSignal>(OnTDMPlayersUpdated);
				_signalBus.Unsubscribe<ClientUpdateRespawnSelectionsSignal>(SendUpdateRespawnSelectionsCommand);
				_signalBus.Unsubscribe<ClientRequestRespawnSignal>(SendRespawnRequestCommand);
			}
		}
	}

	public override void SimulateController()
	{
		if (_sendUpdateRespawnSelectionCommand)
		{
			IUpdateRespawnSelectionsCommandInput updateRespawnSelectionsCommandInput = UpdateRespawnSelectionsCommand.Create();
			updateRespawnSelectionsCommandInput.SpawnPointId = _updatedSpawnPointId;
			updateRespawnSelectionsCommandInput.Loadout = _updatedLoadoutHeroClass;
			base.entity.QueueInput(updateRespawnSelectionsCommandInput);
			_updatedSpawnPointId = string.Empty;
			_updatedLoadoutHeroClass = string.Empty;
			_sendUpdateRespawnSelectionCommand = false;
		}
		if (_sendRequestRespawnCommand)
		{
			IRequestRespawnCommandInput data = RequestRespawnCommand.Create();
			base.entity.QueueInput(data);
			_sendRequestRespawnCommand = false;
		}
	}

	private void OnDeath()
	{
		if (base.entity.isControlled)
		{
			_minimapArrow.SetActive(value: false);
			_deathIcon.SetActive(value: true);
			_signalBus.Fire(default(LocalPlayerDiedSignal));
		}
	}

	private void OnRespawn()
	{
		if (base.entity.isControlled)
		{
			_minimapArrow.SetActive(value: true);
			_deathIcon.SetActive(value: false);
			_signalBus.Fire(default(LocalPlayerRespawnedSignal));
		}
	}

	private void OnTDMPlayersUpdated(TDMPlayersUpdatedSignal signal)
	{
		if (PlayerController.HasLocalPlayer)
		{
			TDMPlayerState tDMPlayerState = signal.Players.FirstOrDefault((TDMPlayerState x) => x.NetworkId == PlayerController.LocalPlayer.entity.networkId);
			if (tDMPlayerState != null && (tDMPlayerState.NextRespawnPointId != _cachedNextRespawnPointId || tDMPlayerState.CanRespawnTime != _cachedCanRespawnTime || tDMPlayerState.ForceRespawnTime != _cachedForceRespawnTime))
			{
				_cachedNextRespawnPointId = tDMPlayerState.NextRespawnPointId;
				_cachedCanRespawnTime = tDMPlayerState.CanRespawnTime;
				_cachedForceRespawnTime = tDMPlayerState.ForceRespawnTime;
				_signalBus.Fire(new LocalPlayerRespawnDataUpdatedSignal
				{
					SelectedSpawnId = _cachedNextRespawnPointId,
					CanRespawnTime = _cachedCanRespawnTime,
					ForceRespawnTime = _cachedForceRespawnTime
				});
			}
		}
	}

	private void SendRespawnRequestCommand()
	{
		_sendRequestRespawnCommand = true;
	}

	private void SendUpdateRespawnSelectionsCommand(ClientUpdateRespawnSelectionsSignal signal)
	{
		_updatedSpawnPointId = signal.SpawnPointId;
		_updatedLoadoutHeroClass = signal.Loadout;
		_sendUpdateRespawnSelectionCommand = true;
	}

	protected override void ExecuteCommand(RequestRespawnCommand cmd, bool resetState)
	{
		if (base.entity.isOwner)
		{
			_signalBus.Fire(new PlayerRequestedRespawnSignal(base.entity));
		}
	}

	protected override void ExecuteCommand(UpdateRespawnSelectionsCommand cmd, bool resetState)
	{
		if (base.entity.isOwner)
		{
			_signalBus.Fire(new UpdateRespawnPointSignal(base.entity, cmd.Input.SpawnPointId, cmd.Input.Loadout));
		}
	}
}
