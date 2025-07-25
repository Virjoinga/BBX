using UnityEngine;

public class AfterIKPositionSyncer : MonoBehaviour
{
	[SerializeField]
	private Transform _toSync;

	private void Start()
	{
		LateUpdate();
	}

	private void LateUpdate()
	{
		base.transform.position = _toSync.position;
		base.transform.rotation = _toSync.rotation;
	}
}
