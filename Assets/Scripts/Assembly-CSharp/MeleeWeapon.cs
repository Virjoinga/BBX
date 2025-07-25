using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : BaseWeapon
{
	private class WaitUntilMaxHeightAboveGround : CustomYieldInstruction
	{
		private IPlayerMotor _motor;

		private float _maxHeight;

		public override bool keepWaiting => _motor.State.HeightAboveGround > _maxHeight;

		public WaitUntilMaxHeightAboveGround(IPlayerMotor motor, float height)
		{
			_motor = motor;
			_maxHeight = height;
		}
	}

	public enum Direction
	{
		Standing = 0,
		Forward = 1,
		Backward = 2,
		Left = 3,
		Right = 4,
		Downward = 5
	}

	[SerializeField]
	private GameObject _impactEffect;

	[SerializeField]
	private Transform _hitOrigin;

	[SerializeField]
	private ParticleSystem[] _swingEffects;

	[SerializeField]
	private float _hitDetectionRange = 0.5f;

	[SerializeField]
	private Vector3 _standingHitDetectionOffset = Vector3.zero;

	private List<Collider> _collidersHit = new List<Collider>();

	private IPlayerController _wielder;

	private Collider _wielderCollider;

	private IPlayerMotor _motor;

	private bool _attacking;

	private WeaponProfile.MeleeOptionData _meleeOption;

	private bool _dealingDamage;

	private Collider[] _overlapColliders = new Collider[20];

	private RaycastHit[] _spherecastResults = new RaycastHit[20];

	private bool _isTriggerMeleeExtended;

	public bool IsTriggerMeleeExtended
	{
		get
		{
			bool isTriggerMeleeExtended = _isTriggerMeleeExtended;
			_isTriggerMeleeExtended = false;
			return isTriggerMeleeExtended;
		}
		set
		{
			_isTriggerMeleeExtended = value;
		}
	}

	private event Action<WeaponProfile.MeleeOptionData> _meleeAnimationTriggered;

	public event Action<WeaponProfile.MeleeOptionData> MeleeAnimationTriggered
	{
		add
		{
			_meleeAnimationTriggered += value;
		}
		remove
		{
			_meleeAnimationTriggered -= value;
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		base.transform.localPosition = Vector3.zero;
		base.transform.localEulerAngles = Vector3.zero;
	}

	protected override void Start()
	{
		base.Start();
		_wielder = GetComponentInParent<IPlayerController>();
		if (_wielder != null)
		{
			_wielderCollider = _wielder.HurtCollider;
			_motor = _wielder.Motor;
		}
		else
		{
			_motor = GetComponentInParent<IPlayerMotor>();
		}
		_swingEffects.Stop();
	}

	private void FixedUpdate()
	{
		if (!_dealingDamage)
		{
			return;
		}
		Vector3 normalized = _meleeOption.GetVelocity(_wielder.transform).normalized;
		float num = _motor.Height * 0.5f;
		Vector3 vector = Vector3.up * num;
		Vector3 vector2 = _wielder.transform.position + vector;
		DebugExtension.DebugArrow(vector2, normalized * _hitDetectionRange, Color.red);
		bool flag = _meleeOption.MovementDirection == Direction.Standing;
		int num2;
		if (flag)
		{
			num2 = Physics.OverlapSphereNonAlloc(vector2 + _wielder.transform.TransformDirection(_standingHitDetectionOffset), num, _overlapColliders, LayerMaskConfig.AffectableLayers);
			DebugExtension.DebugWireSphere(vector2 + _wielder.transform.TransformDirection(_standingHitDetectionOffset), Color.red, num);
		}
		else
		{
			num2 = Physics.SphereCastNonAlloc(vector2, num, normalized, _spherecastResults, _hitDetectionRange, LayerMaskConfig.AffectableLayers);
			DebugExtension.DebugWireSphere(vector2 + normalized, Color.red, num);
		}
		for (int i = 0; i < num2; i++)
		{
			Collider collider = ((!flag) ? _spherecastResults[i].collider : _overlapColliders[i]);
			if (collider != _wielderCollider && !_collidersHit.Contains(collider))
			{
				_collidersHit.Add(collider);
				OnHit(collider);
			}
		}
	}

	protected override void UpdateProperties()
	{
		base.UpdateProperties();
	}

	public IEnumerator Swing(WeaponProfile.MeleeOptionData meleeOption)
	{
		if (!_attacking)
		{
			_attacking = true;
			_meleeOption = meleeOption;
			_collidersHit.Clear();
			_weaponAudioPlayer.PlayFire();
			_swingEffects.Play();
			this._meleeAnimationTriggered?.Invoke(meleeOption);
			_dealingDamage = true;
			yield return new WaitForSeconds(meleeOption.DamageActive);
			_dealingDamage = false;
			_swingEffects.Stop();
			_attacking = false;
			_collidersHit.Clear();
		}
	}

	public void MockSwing(Direction direction)
	{
		this._meleeAnimationTriggered?.Invoke(_profile.Melee.GetOptionByDirection(direction));
	}

	protected virtual void OnHit(Collider collider)
	{
		Vector3 center = _wielderCollider.bounds.center;
		Vector3 vector = collider.ClosestPoint(center);
		if (!Physics.Linecast(center, vector, LayerMaskConfig.GroundLayers))
		{
			HitInfo hitInfo = new HitInfo
			{
				hitType = HitType.Melee,
				collider = collider,
				origin = center,
				forward = (vector - center).normalized,
				normal = -_hitOrigin.forward,
				point = vector,
				launchServerFrame = GetServerFrame(),
				weaponProfile = _profile,
				weaponId = _profile.Id,
				isMelee = true,
				meleeDirection = _meleeOption.MovementDirection
			};
			RaiseHit(hitInfo);
			if (_impactEffect != null && !BoltNetwork.IsServer)
			{
				SmartPool.Spawn(_impactEffect, collider.ClosestPointInside(hitInfo.point), Quaternion.LookRotation(hitInfo.normal));
			}
		}
	}
}
