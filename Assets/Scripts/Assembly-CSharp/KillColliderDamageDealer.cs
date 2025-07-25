using UnityEngine;

public class KillColliderDamageDealer : MonoBehaviour
{
	private const string KILL_COLLIDER_TAG = "KillCollider";

	private HealthController _healthController;

	private void Awake()
	{
		_healthController = GetComponentInParent<HealthController>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "KillCollider")
		{
			_healthController.TakeSelfDamage(9999999f);
		}
	}
}
