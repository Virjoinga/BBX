using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHitDetector : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem _detectionParticleSystem;

	[SerializeField]
	private ParticleSystem[] _speedAffectedParticleSystems;

	private readonly List<ParticleCollisionEvent> _collisionEvents = new List<ParticleCollisionEvent>();

	private readonly List<ParticleSystem.Particle> _triggerEvents = new List<ParticleSystem.Particle>();

	private readonly List<GameObject> _detectedGameObjects = new List<GameObject>();

	private IMovable _playerMovable;

	private IPlayerController _playerController;

	private float _initialSpeed;

	[SerializeField]
	private Vector3 _velocity;

	[SerializeField]
	private Vector3 _localVelocity;

	[SerializeField]
	private float _localForwardVelocity;

	private event Action<GameObject, ParticleCollisionEvent> _collisionDetected;

	public event Action<GameObject, ParticleCollisionEvent> CollisionDetected
	{
		add
		{
			_collisionDetected += value;
		}
		remove
		{
			_collisionDetected -= value;
		}
	}

	private void Start()
	{
		_playerController = GetComponentInParent<IPlayerController>();
		if (_playerController != null)
		{
			_playerMovable = _playerController.gameObject.GetComponent<IMovable>();
		}
	}

	private void FixedUpdate()
	{
		if (_playerMovable != null)
		{
			_velocity = _playerMovable.Velocity;
			_localVelocity = _playerController.transform.InverseTransformDirection(_velocity);
			_localForwardVelocity = _localVelocity.z;
			ParticleSystem[] speedAffectedParticleSystems = _speedAffectedParticleSystems;
			for (int i = 0; i < speedAffectedParticleSystems.Length; i++)
			{
				ParticleSystem.MainModule main = speedAffectedParticleSystems[i].main;
				main.startSpeed = _initialSpeed + _localForwardVelocity;
			}
		}
	}

	public void UpdateProperties(WeaponProfile profile)
	{
		_initialSpeed = profile.Range;
		ParticleSystem[] speedAffectedParticleSystems = _speedAffectedParticleSystems;
		for (int i = 0; i < speedAffectedParticleSystems.Length; i++)
		{
			ParticleSystem.MainModule main = speedAffectedParticleSystems[i].main;
			main.startSpeed = _initialSpeed;
		}
		if (profile.Spread.Amount > 0f)
		{
			ParticleSystem.ShapeModule shape = _detectionParticleSystem.shape;
			shape.angle = profile.Spread.Amount;
		}
	}

	public void ClearHitDetectionCache()
	{
		_detectedGameObjects.Clear();
	}

	private void OnParticleCollision(GameObject other)
	{
		if (_detectedGameObjects.Contains(other))
		{
			return;
		}
		_detectedGameObjects.Add(other);
		if (other.GetComponentInParent<IPlayerController>() == _playerController)
		{
			return;
		}
		int collisionEvents = _detectionParticleSystem.GetCollisionEvents(other, _collisionEvents);
		for (int i = 0; i < collisionEvents; i++)
		{
			if (!LayerMaskConfig.GroundLayers.ContainsLayer(_collisionEvents[i].colliderComponent.gameObject.layer))
			{
				DebugExtension.DebugPoint(_collisionEvents[i].intersection, Color.red, 0.25f, 1f);
				this._collisionDetected?.Invoke(other, _collisionEvents[i]);
				break;
			}
		}
	}
}
