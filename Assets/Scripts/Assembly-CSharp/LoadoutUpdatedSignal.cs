public struct LoadoutUpdatedSignal
{
	public int index;

	public WeaponProfile profile;

	public LoadoutUpdatedSignal(int index, WeaponProfile profile)
	{
		this.index = index;
		this.profile = profile;
	}
}
