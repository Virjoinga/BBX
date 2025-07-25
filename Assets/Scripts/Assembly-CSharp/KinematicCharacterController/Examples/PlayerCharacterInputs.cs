using UnityEngine;

namespace KinematicCharacterController.Examples
{
	public struct PlayerCharacterInputs
	{
		public float MoveAxisForward;

		public float MoveAxisRight;

		public Quaternion CameraRotation;

		public bool JumpDown;

		public bool CrouchDown;

		public bool CrouchUp;
	}
}
