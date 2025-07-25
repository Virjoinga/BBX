using UnityEngine;

public class TestLoadoutLoader : MonoBehaviour
{
	private OfflineWeaponController _weaponController;

	private WeaponHandler _weaponHandler;

	public LoadoutController LoadoutController { get; private set; }

	protected virtual void Awake()
	{
		LoadoutController = GetComponent<LoadoutController>();
		_weaponController = GetComponent<OfflineWeaponController>();
		_weaponHandler = GetComponent<WeaponHandler>();
	}

	public void LoadLoadout(LoadoutData loadout)
	{
		LoadoutController.SetLoadout(loadout, LoadoutSetCompleted);
	}

	private void LoadoutSetCompleted(bool didSet)
	{
		LoadoutController.Outfit.AimPointHandler.IsLocalPlayer = true;
		_weaponController.OnLoadoutSet();
	}

	public void EquipWeapon(int index, string id)
	{
		if (_weaponHandler.HasActiveWeapon)
		{
			_weaponHandler.StowWeapon(delegate
			{
				EquipWeapon(index, id);
				_weaponController.OnLoadoutSet();
			});
		}
		else
		{
			LoadoutController.EquipWeapon(index, id);
			_weaponController.OnLoadoutSet();
		}
	}

	public void EquipMeleeMelee(string id)
	{
		LoadoutController.EquipMeleeWeapon(id);
	}

	public void LoadBackpack(string id)
	{
		LoadoutController.EquipBackpack(id);
	}

	public void LoadHat(string id)
	{
		LoadoutController.EquipHat(id);
	}
}
