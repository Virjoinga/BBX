using Bolt;
using Constants;
using UnityEngine;
using Zenject;

public class DeployableEntity<TState> : SpawnedEntity<IDeployableState>, IDamageable
{
	[Inject]
	private SignalBus _signalBus;

	[SerializeField]
	private Collider _hurtCollider;

	[SerializeField]
	private Renderer _renderer;

	[SerializeField]
	private GameObject _friendlyArrow;

	[SerializeField]
	private Transform _damageNumbersSpawn;

	private NetworkId _ownerNetworkId;

	public Collider HurtCollider => _hurtCollider;

	public int Team => base.state.Team;

	public Transform DamageNumberSpawn => _damageNumbersSpawn;

	public void TakeDamage(HitInfo hitInfo, float damage, BoltEntity attacker)
	{
		if (base.state.Damageable.Health <= 0f)
		{
			return;
		}
		IPlayerState playerState = attacker.GetState<IPlayerState>();
		if (playerState == null || playerState.Team == Team)
		{
			return;
		}
		float health = base.state.Damageable.Health;
		if (damage > 0f && damage > health)
		{
			damage = health;
		}
		else if (damage < 0f && base.state.Damageable.Health + Mathf.Abs(damage) > base.state.Damageable.MaxHealth)
		{
			damage = 0f - (base.state.Damageable.MaxHealth - base.state.Damageable.Health);
		}
		base.state.Damageable.Health = Mathf.Clamp(base.state.Damageable.Health - damage, 0f, base.state.Damageable.MaxHealth);
		if (damage > 0f)
		{
			DamagableDamaged damagableDamaged = DamagableDamaged.Create(GlobalTargets.AllClients, ReliabilityModes.ReliableOrdered);
			damagableDamaged.IsPlayer = false;
			damagableDamaged.Attacker = attacker;
			damagableDamaged.Victim = base.entity;
			damagableDamaged.Damage = damage;
			damagableDamaged.Died = base.state.Damageable.Health == 0f;
			damagableDamaged.WeaponId = hitInfo.weaponId;
			damagableDamaged.Send();
			if (base.state.Damageable.Health == 0f)
			{
				BoltNetwork.Destroy(base.gameObject);
			}
		}
	}

	public void TakeDamage(HitInfo hitInfo, float damage, BoltEntity attacker, WeaponProfile.EffectData[] effects)
	{
		TakeDamage(hitInfo, damage, attacker);
	}

	protected override void OnRemoteOnlyAttached()
	{
		base.state.AddCallback("Owner", OnOwnerUpdated);
	}

	private void OnOwnerUpdated()
	{
		_ownerNetworkId = base.state.Owner.networkId;
		if (_ownerNetworkId == PlayerController.LocalPlayer.entity.networkId)
		{
			_signalBus.Fire(new DeployableCreatedDestroyedSignal
			{
				WeaponProfileId = base.state.WeaponId,
				IsActive = true
			});
		}
		if (PlayerController.LocalPlayer.Team == Team)
		{
			_renderer.material.SetColor("_TeamColor", Match.FriendlyTeamColor);
			_renderer.material.SetFloat("_EnableGlow", 1f);
			_friendlyArrow.SetActive(value: true);
		}
	}

	protected override void OnRemoteOnlyDetached()
	{
		if (PlayerController.HasLocalPlayer && !_ownerNetworkId.IsZero && _ownerNetworkId == PlayerController.LocalPlayer.entity.networkId)
		{
			_signalBus.Fire(new DeployableCreatedDestroyedSignal
			{
				WeaponProfileId = base.state.WeaponId,
				IsActive = false
			});
		}
	}
}
