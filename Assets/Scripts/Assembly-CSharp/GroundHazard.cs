using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class GroundHazard : MonoBehaviour
{
	[SerializeField]
	private string _id;

	private Collider _collider;

	private BoltEntity _controllerEntity;

	private GroundHazardData _groundHazardData;

	private readonly List<Collider> _trackedColliders = new List<Collider>();

	private bool _detecting;

	public string Id => _id;

	private void Awake()
	{
		Debug.Log("[GroundHazard] Ground hazard Awake");
		_collider = GetComponent<Collider>();
	}

	private void OnEnable()
	{
		_collider.enabled = true;
	}

	private void OnDisable()
	{
		_collider.enabled = false;
	}

	public void RegisterHazardData(GroundHazardData groundHazardData, BoltEntity entity)
	{
		_groundHazardData = groundHazardData;
		_controllerEntity = entity;
		_detecting = true;
		Debug.Log($"[GroundHazard] Registered ground hazard {_groundHazardData.Id}, _controllerEntity: {_controllerEntity}");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_detecting)
		{
			Debug.Log($"[GroundHazard] {other.gameObject.name} entered hazard {_groundHazardData.Id}, _controllerEntity: {_controllerEntity}");
			_trackedColliders.Add(other);
			other.GetComponentInParent<StatusEffectController>().ApplyEffectWhile(_groundHazardData.Id, _groundHazardData.effect, _controllerEntity, () => _trackedColliders.Contains(other));
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (_detecting)
		{
			Debug.Log("[GroundHazard] " + other.gameObject.name + " exited hazard " + _groundHazardData.Id);
			_trackedColliders.Remove(other);
		}
	}
}
