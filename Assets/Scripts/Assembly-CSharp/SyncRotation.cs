using UnityEngine;

public class SyncRotation : MonoBehaviour
{
	[SerializeField]
	private Transform _syncTo;

	private void Update()
	{
		base.transform.rotation = _syncTo.rotation;
	}
}
