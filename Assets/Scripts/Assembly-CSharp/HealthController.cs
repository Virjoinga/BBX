using System;
using System.Linq;
using Bolt;
using UnityEngine;
using Zenject;

public class HealthController : BaseEntityBehaviour<IPlayerState>, IDamageable
{
	[Inject]
	private SignalBus _signalBus;

	private float _health;

	private PlayerController _playerController;

	private StatusEffectController _statusEffectController;

	private MatchStateHelper _matchStateHelper;

	public int Team => base.state.Team;

	public Collider HurtCollider => _playerController.HurtCollider;

	public Transform DamageNumberSpawn => _playerController.LoadoutController.Outfit.HatContainer;

	private event Action _died;

	public event Action Died
	{
		add
		{
			_died += value;
		}
		remove
		{
			_died -= value;
		}
	}

	private event Action _respawned;

	public event Action Respawned
	{
		add
		{
			_respawned += value;
		}
		remove
		{
			_respawned -= value;
		}
	}

	private event Action _changed;

	public event Action Changed
	{
		add
		{
			_changed += value;
		}
		remove
		{
			_changed -= value;
		}
	}

	private event Action<BoltEntity, float> _damaged;

	public event Action<BoltEntity, float> Damaged
	{
		add
		{
			_damaged += value;
		}
		remove
		{
			_damaged -= value;
		}
	}

	private event Action _maxChanged;

	public event Action MaxChanged
	{
		add
		{
			_maxChanged += value;
		}
		remove
		{
			_maxChanged -= value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_statusEffectController = GetComponent<StatusEffectController>();
		_playerController = GetComponent<PlayerController>();
		_matchStateHelper = GetComponent<MatchStateHelper>();
	}

	protected override void OnAnyAttached()
	{
		base.state.AddCallback("Damageable.Health", OnHealthChanged);
		base.state.AddCallback("Damageable.MaxHealth", OnMaxHealthChanged);
		_health = base.state.Damageable.Health;
	}

	protected override void OnAnyDetached()
	{
		base.state.RemoveCallback("Damageable.Health", OnHealthChanged);
		base.state.RemoveCallback("Damageable.MaxHealth", OnMaxHealthChanged);
	}

	private void OnHealthChanged()
	{
		float health = base.state.Damageable.Health;
		if (_health > 0f && health <= 0f)
		{
			this._died?.Invoke();
		}
		else if (_health <= 0f && health > 0f)
		{
			this._respawned?.Invoke();
		}
		this._changed?.Invoke();
		_health = health;
	}

	private void OnMaxHealthChanged()
	{
		this._maxChanged?.Invoke();
	}

	public void TakeSelfDamage(float damage)
	{
		if (base.entity.isOwner)
		{
			TakeDamage(default(HitInfo), damage, base.entity);
		}
	}

	public void TakeDamage(HitInfo hitInfo, float damage, BoltEntity attacker)
	{
		TakeDamage(hitInfo, damage, attacker, null);
	}

	public void TakeDamage(HitInfo hitInfo, float damage, BoltEntity attacker, WeaponProfile.EffectData[] effects)
	{
		if (base.state.Damageable.Health <= 0f || base.state.IsShielded || _matchStateHelper.MatchStateCached != MatchState.Active)
		{
			return;
		}
		if (effects != null && effects.Any((WeaponProfile.EffectData e) => e.InverseForAlly) && attacker.TryFindState<IPlayerState>(out var playerState) && playerState.Team == base.state.Team)
		{
			damage *= -1f;
		}
		Debug.Log($"[HealthController] {base.name} has taken {damage} damage from {attacker.name}");
		float num = base.state.Damageable.Health + base.state.Damageable.Shield;
		if (damage > 0f && damage > num)
		{
			damage = num;
		}
		else if (damage < 0f && base.state.Damageable.Health + Mathf.Abs(damage) > base.state.Damageable.MaxHealth)
		{
			damage = 0f - (base.state.Damageable.MaxHealth - base.state.Damageable.Health);
		}
		float num2 = 0f;
		if (base.state.Damageable.Shield > 0f && damage > 0f)
		{
			num2 = Mathf.Min(base.state.Damageable.Shield, damage);
			base.state.Damageable.Shield -= num2;
			damage -= num2;
		}
		base.state.Damageable.Health = Mathf.Clamp(base.state.Damageable.Health - damage, 0f, base.state.Damageable.MaxHealth);
		if (base.state.Damageable.Health > 0f && effects != null && effects.Length != 0 && !hitInfo.weaponProfile.SpawnedEntity.SpawnsEntity)
		{
			foreach (WeaponProfile.EffectData effectData in effects)
			{
				_statusEffectController.TryApplyEffect(hitInfo.weaponId, effectData, attacker);
			}
		}
		if (damage > 0f || num2 > 0f)
		{
			float num4 = damage + num2;
			this._damaged?.Invoke(attacker, num4);
			DamagableDamaged damagableDamaged = DamagableDamaged.Create(GlobalTargets.AllClients, ReliabilityModes.ReliableOrdered);
			damagableDamaged.IsPlayer = true;
			damagableDamaged.Attacker = attacker;
			damagableDamaged.Victim = base.entity;
			damagableDamaged.Damage = num4;
			damagableDamaged.Died = false;
			damagableDamaged.WeaponId = hitInfo.weaponId;
			if (base.state.Damageable.Health == 0f)
			{
				damagableDamaged.Died = true;
				base.state.InputEnabled = false;
				_signalBus.Fire(new PlayerDiedSignal(_playerController, attacker));
				_playerController.StatusEffectController.KillAllEffects();
			}
			OnHealthChanged();
			damagableDamaged.Send();
		}
	}

	public bool TryHeal(float amount)
	{
		if (base.state.Damageable.Health >= base.state.Damageable.MaxHealth || base.state.Damageable.Health <= 0f)
		{
			return false;
		}
		base.state.Damageable.Health = Mathf.Clamp(base.state.Damageable.Health + amount, 0f, base.state.Damageable.MaxHealth);
		return true;
	}
}
