public struct WeaponStateUpdatedSignal
{
	public int index;

	public int remainingAmmo;

	public int maxAmmo;

	public bool reloadsInBackground;

	public float reloadPercent;

	public WeaponStateUpdatedSignal(int index, int remainingAmmo, int maxAmmo)
	{
		this.index = index;
		this.remainingAmmo = remainingAmmo;
		this.maxAmmo = maxAmmo;
		reloadsInBackground = false;
		reloadPercent = 0f;
	}

	public WeaponStateUpdatedSignal(int index, float reloadPercent)
	{
		this.index = index;
		remainingAmmo = 0;
		maxAmmo = 0;
		reloadsInBackground = true;
		this.reloadPercent = reloadPercent;
	}
}
