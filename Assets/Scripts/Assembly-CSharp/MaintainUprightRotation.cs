using UnityEngine;

public class MaintainUprightRotation : MonoBehaviour
{
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.identity;
	}
}
