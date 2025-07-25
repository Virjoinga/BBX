using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KinematicCharacterController.Examples
{
	public class PlanetManager : MonoBehaviour, IMoverController
	{
		public PhysicsMover PlanetMover;

		public SphereCollider GravityField;

		public float GravityStrength = 10f;

		public Vector3 OrbitAxis = Vector3.forward;

		public float OrbitSpeed = 10f;

		public Teleporter OnPlaygroundTeleportingZone;

		public Teleporter OnPlanetTeleportingZone;

		private List<ExampleCharacterController> _characterControllersOnPlanet = new List<ExampleCharacterController>();

		private Vector3 _savedGravity;

		private Quaternion _lastRotation;

		private void Start()
		{
			Teleporter onPlaygroundTeleportingZone = OnPlaygroundTeleportingZone;
			onPlaygroundTeleportingZone.OnCharacterTeleport = (UnityAction<ExampleCharacterController>)Delegate.Remove(onPlaygroundTeleportingZone.OnCharacterTeleport, new UnityAction<ExampleCharacterController>(ControlGravity));
			Teleporter onPlaygroundTeleportingZone2 = OnPlaygroundTeleportingZone;
			onPlaygroundTeleportingZone2.OnCharacterTeleport = (UnityAction<ExampleCharacterController>)Delegate.Combine(onPlaygroundTeleportingZone2.OnCharacterTeleport, new UnityAction<ExampleCharacterController>(ControlGravity));
			Teleporter onPlanetTeleportingZone = OnPlanetTeleportingZone;
			onPlanetTeleportingZone.OnCharacterTeleport = (UnityAction<ExampleCharacterController>)Delegate.Remove(onPlanetTeleportingZone.OnCharacterTeleport, new UnityAction<ExampleCharacterController>(UnControlGravity));
			Teleporter onPlanetTeleportingZone2 = OnPlanetTeleportingZone;
			onPlanetTeleportingZone2.OnCharacterTeleport = (UnityAction<ExampleCharacterController>)Delegate.Combine(onPlanetTeleportingZone2.OnCharacterTeleport, new UnityAction<ExampleCharacterController>(UnControlGravity));
			_lastRotation = PlanetMover.transform.rotation;
			PlanetMover.MoverController = this;
		}

		public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
		{
			goalPosition = PlanetMover.Rigidbody.position;
			_lastRotation = (goalRotation = Quaternion.Euler(OrbitAxis * OrbitSpeed * deltaTime) * _lastRotation);
			foreach (ExampleCharacterController item in _characterControllersOnPlanet)
			{
				item.Gravity = (PlanetMover.transform.position - item.transform.position).normalized * GravityStrength;
			}
		}

		private void ControlGravity(ExampleCharacterController cc)
		{
			_savedGravity = cc.Gravity;
			_characterControllersOnPlanet.Add(cc);
		}

		private void UnControlGravity(ExampleCharacterController cc)
		{
			cc.Gravity = _savedGravity;
			_characterControllersOnPlanet.Remove(cc);
		}
	}
}
