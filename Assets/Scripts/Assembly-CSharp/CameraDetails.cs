using System;
using UnityEngine;

[Serializable]
public class CameraDetails
{
	[Tooltip("How quickly the player spins")]
	public float HorizontalRotationSensitivity = 5f;

	[Tooltip("How quickly the player aims up/down")]
	public float VerticalRotationSensitivity = 5f;

	[Tooltip("How far below the horizon the player can aim")]
	public float MaxDownPitch = 45f;

	[Tooltip("How far above the horizon the player can aim")]
	public float MaxUpPitch = 60f;
}
