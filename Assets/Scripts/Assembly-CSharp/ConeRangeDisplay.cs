using UnityEngine;

public class ConeRangeDisplay : MonoBehaviour
{
	[SerializeField]
	private float _spread = 10f;

	[SerializeField]
	private float _range = 2f;

	private void OnDrawGizmos()
	{
		DebugExtension.DrawCone(base.transform.position, base.transform.forward * _range, Color.red, _spread);
	}
}
