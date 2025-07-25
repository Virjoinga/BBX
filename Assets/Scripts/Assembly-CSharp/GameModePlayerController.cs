using Bolt;

public abstract class GameModePlayerController : EntityBehaviour<IPlayerState>
{
	private HealthController _healthController;

	protected abstract UIPrefabIds _matchHudPrefabId { get; }

	protected abstract UIPrefabIds _deathScreenPrefabId { get; }

	protected virtual void Awake()
	{
		_healthController = GetComponent<HealthController>();
	}

	public override void Attached()
	{
		_healthController.Died += OnDeath;
		_healthController.Respawned += OnRespawn;
		if (base.entity.isControlled)
		{
			InstantiateMatchUI();
		}
	}

	public override void Detached()
	{
		_healthController.Died += OnDeath;
		_healthController.Respawned += OnRespawn;
		if (base.entity.isControlled)
		{
			UIPrefabManager.Destroy(_matchHudPrefabId);
		}
	}

	protected virtual void InstantiateMatchUI()
	{
	}

	protected virtual void OnDeath()
	{
	}

	protected virtual void OnRespawn()
	{
	}
}
