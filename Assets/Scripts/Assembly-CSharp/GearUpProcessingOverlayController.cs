using UnityEngine;

public class GearUpProcessingOverlayController : MonoBehaviourSingleton<GearUpProcessingOverlayController>
{
	[SerializeField]
	private GameObject _overlay;

	public void SetOverlayState(bool isActive)
	{
		_overlay.SetActive(isActive);
	}
}
