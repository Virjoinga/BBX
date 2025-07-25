using UnityEngine;
using UnityEngine.UI;

public class DoubleJumpCooldownBar : MonoBehaviour
{
	[SerializeField]
	private Image _cooldownBar;

	private void Awake()
	{
		_cooldownBar.fillAmount = 0f;
	}

	private void Update()
	{
		if (!PlayerController.HasLocalPlayer)
		{
			_cooldownBar.fillAmount = 0f;
			return;
		}
		float doubleJumpCooldown = PlayerController.LocalPlayer.Motor.JumpDetails.DoubleJumpCooldown;
		float fillAmount = Mathf.Max(0f, PlayerController.LocalPlayer.Motor.State.DoubleJumpCooldown) / doubleJumpCooldown;
		_cooldownBar.fillAmount = fillAmount;
	}
}
