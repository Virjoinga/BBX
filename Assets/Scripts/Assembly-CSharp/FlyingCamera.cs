using UnityEngine;

public class FlyingCamera : MonoBehaviour
{
	private float _mult = 0.5f;

	private float _camMult = 2f;

	private void Update()
	{
		if (Input.GetMouseButton(1))
		{
			float axis = Input.GetAxis("Vertical");
			float axis2 = Input.GetAxis("Horizontal");
			float axis3 = Input.GetAxis("Mouse Y");
			float axis4 = Input.GetAxis("Mouse X");
			base.transform.Rotate(Vector3.up, axis4 * _camMult, Space.World);
			base.transform.Rotate(Vector3.right, (0f - axis3) * _camMult, Space.Self);
			base.transform.position += base.transform.forward * axis * _mult;
			base.transform.position += base.transform.right * axis2 * _mult;
			if (Input.GetKey(KeyCode.Q))
			{
				base.transform.position -= base.transform.up * _mult;
			}
			else if (Input.GetKey(KeyCode.E))
			{
				base.transform.position += base.transform.up * _mult;
			}
		}
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			_mult *= 2f;
		}
		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			_mult *= 0.5f;
		}
	}
}
