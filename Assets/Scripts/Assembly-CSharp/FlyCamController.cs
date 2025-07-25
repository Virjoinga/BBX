using BSCore;
using Cinemachine;
using UnityEngine;

public class FlyCamController : MonoBehaviour
{
	[SerializeField]
	private CinemachineVirtualCamera _virtualCam;

	[SerializeField]
	private float _forwardSpeed = 10f;

	[SerializeField]
	private float _rotateSpeed = 5f;

	private void Reset()
	{
		_virtualCam = GetComponent<CinemachineVirtualCamera>();
	}

	private void Awake()
	{
		base.enabled = false;
	}

	private void OnEnable()
	{
		_virtualCam.Priority = 500;
	}

	private void OnDisable()
	{
		_virtualCam.Priority = 0;
	}

	private void Update()
	{
		if (!MouseLockToggle.MouseLockReleased)
		{
			float axis = BSCoreInput.GetAxis(Option.Vertical);
			float axis2 = BSCoreInput.GetAxis(Option.Horizontal);
			float num = (Input.GetKey(KeyCode.E) ? 1f : (Input.GetKey(KeyCode.Q) ? (-1f) : 0f));
			float num2 = 0f - BSCoreInput.GetAxis(Option.CameraVertical);
			float axis3 = BSCoreInput.GetAxis(Option.CameraHorizontal);
			float deltaTime = Time.deltaTime;
			base.transform.Rotate(Vector3.up, axis3 * _rotateSpeed * deltaTime, Space.World);
			base.transform.Rotate(Vector3.right, num2 * _rotateSpeed * deltaTime, Space.Self);
			base.transform.position = Vector3.MoveTowards(base.transform.position, base.transform.position + base.transform.forward, _forwardSpeed * axis * deltaTime);
			base.transform.position = Vector3.MoveTowards(base.transform.position, base.transform.position + base.transform.right, _forwardSpeed * axis2 * deltaTime);
			base.transform.position = Vector3.MoveTowards(base.transform.position, base.transform.position + base.transform.up, _forwardSpeed * num * deltaTime);
		}
	}
}
