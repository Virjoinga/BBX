public interface IWeaponController
{
	int RemainingAmmo { get; }

	int GetServerFrame();

	void SetMeleeWeapon(HeroClass heroClass, WeaponProfile profile);
}
