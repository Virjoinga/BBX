using UnityEngine;

public class BilboardToCamera : MonoBehaviour
{
	private void Update()
	{
		if (Camera.main != null)
		{
			base.transform.LookAt(Camera.main.transform);
		}
	}
}
