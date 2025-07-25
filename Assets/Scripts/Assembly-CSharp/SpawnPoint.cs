using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	public Vector3 Position => base.transform.position;

	public Quaternion Rotation => base.transform.rotation;

	[ContextMenu("Repositiong Above Ground")]
	private void RepositionAboveGround()
	{
		if (Physics.SphereCast(base.transform.position + Vector3.up * 0.1f, 0.1f, Vector3.down, out var hitInfo, 100f))
		{
			float distance = hitInfo.distance;
			base.transform.position = base.transform.position + Vector3.down * (distance - 0.2f);
		}
	}
}
