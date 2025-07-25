using System.Collections;
using UnityEngine;

public class CratePickupEntity : CollectOnWalkOverPickupEntity<ICratePickupState>, IDamageable
{
	[SerializeField]
	private Collider _hurtCollider;

	[SerializeField]
	private Collider _groundCollider;

	[SerializeField]
	private GameObject _crateContainer;

	[SerializeField]
	private ObjectFader _crate;

	[SerializeField]
	private GameObject _pickupContainer;

	[SerializeField]
	private ObjectFader[] _pieces;

	[SerializeField]
	private float _throwStrength = 1f;

	[SerializeField]
	private ParticleSystem _respawnEffect;

	[SerializeField]
	private Transform _damageNumbersSpawn;

	private bool _isIndoors;

	public Collider HurtCollider => _hurtCollider;

	public int Team => -1;

	public Transform DamageNumberSpawn => _damageNumbersSpawn;

	public static BoltEntity SpawnPickup(PickupSpawnPoint spawnPoint, int hitsToBreak, PickupType pickupType)
	{
		BoltEntity boltEntity = BasePickupEntity<ICratePickupState>.SpawnPickup(BoltPrefabs.CratePickup, spawnPoint, pickupType);
		boltEntity.GetComponent<CratePickupEntity>()._isIndoors = spawnPoint.IsIndoors;
		ICratePickupState cratePickupState = boltEntity.GetState<ICratePickupState>();
		Damageable damageable = cratePickupState.Damageable;
		float health = (cratePickupState.Damageable.MaxHealth = hitsToBreak);
		damageable.Health = health;
		cratePickupState.ActiveState = 0;
		return boltEntity;
	}

	protected override void Start()
	{
		base.Start();
		_respawnEffect.Stop();
		ObjectFader[] pieces = _pieces;
		for (int i = 0; i < pieces.Length; i++)
		{
			pieces[i].gameObject.SetActive(value: false);
		}
	}

	public void TakeDamage(HitInfo hitInfo, float damage, BoltEntity attacker)
	{
		if (hitInfo.weaponProfile.ItemType == ItemType.meleeWeapon && base.state.Damageable.Health > 0f)
		{
			base.state.Damageable.Health = Mathf.RoundToInt(base.state.Damageable.Health - 1f);
			if (base.state.Damageable.Health <= 0f)
			{
				base.state.ActiveState = 1;
			}
		}
	}

	public void TakeDamage(HitInfo hitInfo, float damage, BoltEntity attacker, WeaponProfile.EffectData[] effects)
	{
		TakeDamage(hitInfo, damage, attacker);
	}

	protected override void OnActiveStateUpdated()
	{
		base.OnActiveStateUpdated();
		switch ((State)base.state.ActiveState)
		{
		case State.Revealed:
			Break();
			break;
		case State.Unbroken:
			Unbreak();
			break;
		}
	}

	private void Break()
	{
		_pickupContainer.SetActive(value: true);
		_groundCollider.enabled = false;
		if (base.entity.isOwner)
		{
			_crateContainer.SetActive(value: false);
			return;
		}
		ObjectFader[] pieces = _pieces;
		foreach (ObjectFader piece in pieces)
		{
			StartCoroutine(ThrowPiece(piece));
		}
		_crate.FadeOut(0.5f, delegate
		{
			_crateContainer.SetActive(value: false);
		});
	}

	private void Unbreak()
	{
		_pickupContainer.SetActive(value: false);
		_crateContainer.SetActive(value: true);
		_groundCollider.enabled = true;
		if (!base.entity.isOwner)
		{
			_crate.FadeIn();
			_respawnEffect.Play();
		}
	}

	private IEnumerator ThrowPiece(ObjectFader piece)
	{
		piece.gameObject.SetActive(value: true);
		piece.transform.localPosition = new Vector3(0f, 0.55f, 0f);
		piece.transform.rotation = Random.rotation;
		piece.SetFadeLevel(1f);
		piece.GetComponent<Rigidbody>().velocity = Random.onUnitSphere * _throwStrength;
		yield return new WaitForSeconds(0.25f);
		piece.FadeOut(0.25f, delegate
		{
			piece.gameObject.SetActive(value: false);
		});
	}

	protected override void StartRefill()
	{
		_pickupContainer.SetActive(value: false);
		base.StartRefill();
	}

	protected override void Refill()
	{
		base.state.ActiveState = 0;
		base.state.Damageable.Health = base.state.Damageable.MaxHealth;
		base.state.Type = (int)_pickupConfig.GetRandomType();
	}
}
