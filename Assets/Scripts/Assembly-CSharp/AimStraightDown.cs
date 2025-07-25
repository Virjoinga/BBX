using UnityEngine;

public class AimStraightDown : MonoBehaviour
{
	private void FixedUpdate()
	{
		base.transform.forward = Vector3.down;
	}
}
