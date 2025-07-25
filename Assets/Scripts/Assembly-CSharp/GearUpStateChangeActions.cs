using UnityEngine;

public class GearUpStateChangeActions : ActiveUIStateChangedBase
{
	[SerializeField]
	private Transform _characterModelPosition;

	protected override void OnActiveUIShown()
	{
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.SwitchToGearUp(_characterModelPosition.position);
		}
	}

	protected override void OnActiveUIHidden()
	{
		if (MonoBehaviourSingleton<MenuCharacterDisplayController>.IsInstantiated)
		{
			MonoBehaviourSingleton<MenuCharacterDisplayController>.Instance.SwitchToHome();
		}
	}
}
