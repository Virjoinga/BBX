using UnityEngine;

[CreateAssetMenu(fileName = "PickupHelper", menuName = "Pickup Helper")]
public class PickupSpawningConfigData : ScriptableObject
{
	[SerializeField]
	private LayerMask _groundLayer;

	[SerializeField]
	private float _distanceAboveGround = 0.5f;

	private static PickupSpawningConfigData _instanceCache;

	private static PickupSpawningConfigData _instance
	{
		get
		{
			if (_instanceCache == null)
			{
				_instanceCache = Resources.Load<PickupSpawningConfigData>("Config/PickupHelper");
			}
			return _instanceCache;
		}
	}

	public static void RepositionAboveGround(Transform transform)
	{
		if (Physics.SphereCast(transform.position + Vector3.up * 0.1f, 0.1f, Vector3.down, out var hitInfo, 100f, _instance._groundLayer))
		{
			float distance = hitInfo.distance;
			transform.position += Vector3.down * (distance - _instance._distanceAboveGround);
		}
	}
}
