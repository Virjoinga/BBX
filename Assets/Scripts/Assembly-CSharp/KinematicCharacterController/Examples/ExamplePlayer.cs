using UnityEngine;

namespace KinematicCharacterController.Examples
{
	public class ExamplePlayer : MonoBehaviour
	{
		public ExampleCharacterController Character;

		public ExampleCharacterCamera CharacterCamera;

		private const string MouseXInput = "Mouse X";

		private const string MouseYInput = "Mouse Y";

		private const string MouseScrollInput = "Mouse ScrollWheel";

		private const string HorizontalInput = "Horizontal";

		private const string VerticalInput = "Vertical";

		private void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
			CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);
			CharacterCamera.IgnoredColliders.Clear();
			CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
			HandleCameraInput();
			HandleCharacterInput();
		}

		private void HandleCameraInput()
		{
			float axisRaw = Input.GetAxisRaw("Mouse Y");
			float axisRaw2 = Input.GetAxisRaw("Mouse X");
			Vector3 rotationInput = new Vector3(axisRaw2, axisRaw, 0f);
			if (Cursor.lockState != CursorLockMode.Locked)
			{
				rotationInput = Vector3.zero;
			}
			float zoomInput = 0f - Input.GetAxis("Mouse ScrollWheel");
			CharacterCamera.UpdateWithInput(Time.deltaTime, zoomInput, rotationInput);
			if (Input.GetMouseButtonDown(1))
			{
				CharacterCamera.TargetDistance = ((CharacterCamera.TargetDistance == 0f) ? CharacterCamera.DefaultDistance : 0f);
			}
		}

		private void HandleCharacterInput()
		{
			PlayerCharacterInputs inputs = new PlayerCharacterInputs
			{
				MoveAxisForward = Input.GetAxisRaw("Vertical"),
				MoveAxisRight = Input.GetAxisRaw("Horizontal"),
				CameraRotation = CharacterCamera.Transform.rotation,
				JumpDown = Input.GetKeyDown(KeyCode.Space),
				CrouchDown = Input.GetKeyDown(KeyCode.C),
				CrouchUp = Input.GetKeyUp(KeyCode.C)
			};
			Character.SetInputs(ref inputs);
		}
	}
}
