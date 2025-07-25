using System.Collections.Generic;

public class AssistTracker : BaseEntityBehaviour<IPlayerState>
{
	private class AssistInfo
	{
		public float damage;

		public float timestamp;
	}

	private HealthController _healthController;

	private readonly Dictionary<BoltEntity, AssistInfo> _assistInfoByAttacker = new Dictionary<BoltEntity, AssistInfo>();

	private readonly float _timeLimit = 30f;

	protected override void OnOwnerOnlyAttached()
	{
		_healthController = GetComponent<HealthController>();
		_healthController.Changed += OnHealthChanged;
		_healthController.Damaged += OnDamaged;
	}

	public bool DidPlayerAssist(BoltEntity player)
	{
		if (_assistInfoByAttacker.TryGetValue(player, out var value) && value.damage * 10f >= base.state.Damageable.MaxHealth)
		{
			return value.timestamp + _timeLimit >= BoltNetwork.Time;
		}
		return false;
	}

	private void OnHealthChanged()
	{
		if (!(base.state.Damageable.Health >= base.state.Damageable.MaxHealth))
		{
			return;
		}
		foreach (AssistInfo value in _assistInfoByAttacker.Values)
		{
			value.damage = 0f;
			value.timestamp = 0f;
		}
	}

	private void OnDamaged(BoltEntity attacker, float damage)
	{
		if (!(base.entity == attacker))
		{
			if (!_assistInfoByAttacker.TryGetValue(attacker, out var value))
			{
				value = new AssistInfo();
				_assistInfoByAttacker.Add(attacker, value);
			}
			value.damage += damage;
			value.timestamp = BoltNetwork.Time;
		}
	}
}
