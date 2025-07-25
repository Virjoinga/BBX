public struct ReloadStateUpdatedSignal
{
	public bool isReloading;

	public WeaponProfile profile;

	public float reloadStartTime;

	public float currentTime;

	public ReloadStateUpdatedSignal(bool isReloading, WeaponProfile profile, float reloadStartTime, float currentTime)
	{
		this.isReloading = isReloading;
		this.profile = profile;
		this.reloadStartTime = reloadStartTime;
		this.currentTime = currentTime;
	}
}
