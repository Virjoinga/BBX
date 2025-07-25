using UnityEngine;

public class MainMenuWeaponController : MonoBehaviour, IWeaponController
{
	private MainMenuPlayerController _playerController;

	private LoadoutController _loadoutController;

	public int RemainingAmmo => 0;

	private void Awake()
	{
		_playerController = GetComponent<MainMenuPlayerController>();
		_loadoutController = GetComponent<LoadoutController>();
		_loadoutController.OutfitEquipped += OnOutfitEquipped;
	}

	public int GetServerFrame()
	{
		return 0;
	}

	public void SetMeleeWeapon(HeroClass heroClass, WeaponProfile profile)
	{
		_playerController.AnimationController.SetMeleeWeapon(heroClass, profile);
	}

	private void OnOutfitEquipped()
	{
		AimedHandHeldWeaponOutfitHelper component = _loadoutController.Outfit.GetComponent<AimedHandHeldWeaponOutfitHelper>();
		if (component != null)
		{
			component.DisableAiming();
		}
	}
}
