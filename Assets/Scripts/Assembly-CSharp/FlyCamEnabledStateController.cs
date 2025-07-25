using UnityEngine;

[RequireComponent(typeof(FlyCamController))]
public class FlyCamEnabledStateController : MonoBehaviour
{
	private FlyCamController _flyCam;

	private IPlayerController _playerController;

	private void Awake()
	{
		_flyCam = GetComponent<FlyCamController>();
		_playerController = Object.FindObjectOfType<OfflinePlayerController>();
	}

	private void Update()
	{
		if (_playerController == null)
		{
			_playerController = PlayerController.LocalPlayer;
		}
		else if (Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.RightShift))
		{
			_flyCam.enabled = !_flyCam.enabled;
			_playerController.LocalInputBlocked = _flyCam.enabled;
			if (_flyCam.enabled)
			{
				base.transform.position = Camera.main.transform.position;
				base.transform.rotation = Camera.main.transform.rotation;
			}
			UIPrefabManager.ShowHideInteractiveCanvas(!_flyCam.enabled);
			UIPrefabManager.ShowHideNonInteractiveCanvas(!_flyCam.enabled);
		}
	}
}
